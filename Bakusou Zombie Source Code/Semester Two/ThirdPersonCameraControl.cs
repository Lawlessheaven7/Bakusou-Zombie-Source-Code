using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

using UnityEngine.Events;
public class ThirdPersonCameraControl : MonoBehaviourPunCallbacks 
{
    public static ThirdPersonCameraControl instance;

    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfBulletProjectile;

    public float maxMana = 10f, currentMana = 10f, coolRate = 4f, manaCoolRate = 5f;
    public GameObject bulletImpact;
    public GameObject bullet;
    public GameObject playerHitImpact;
    public GameObject characterModel;
    public GameObject mainCamera;
    public GameObject recognizer;
    public GameObject fireAura;
    public GameObject cursedAura;

    public Transform firePoint;
    public Transform muzzlePoint;
    public Transform weaponHolder;
    public Transform debugTransform;

    public Weapon activeWeapon;
    public List<Weapon> allWeapons = new List<Weapon>();
    public List<Weapon> unlockableWeapons = new List<Weapon>();
    
    public float currentHealth;
    public int maxHealth = 100;
    public int currentWeapon;
    public int currentSkin;

    public Material[] allSkins;

    public Animator anim;

    private Camera cam;
    private Vector3 gunStartPos;
   

    private float activeSpeed;
    private float lastShootTime;
    public float shotCounter;
    private float manaCounter;

    private bool manaDepleted;
    public bool OnFire;
    public bool onCursed;
    public bool isSurvivor = true;
    public bool isInfected = false;
    

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    

    public PhotonView myPV;
    public int Player;

    [Header("Post Processing")]
    public PostProcessVolume PostProcessVolume;
    public Vignette vignette;


    private void Awake()
    {
        instance = this;
  

        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
    
        if (photonView.IsMine)
        {
            mainCamera.SetActive(true);
            firePoint.gameObject.SetActive(true);
            muzzlePoint.gameObject.SetActive(true);
            recognizer.gameObject.SetActive(false);


            gunStartPos = weaponHolder.position;
            cam = Camera.main;
            UIController.instance.ManaBar.fillAmount = maxMana;
            photonView.RPC("SwitchWeapon", RpcTarget.All);
            //photonView.RPC("IconSwitch", RpcTarget.All);



            firePoint.position = activeWeapon.firepoint.position;
            currentHealth = maxHealth;

            UIController.instance.Healthbar.SetBar(currentHealth, 0, maxHealth);
            UIController.instance.healthBar.maxValue = maxHealth;
            UIController.instance.healthBar.value = currentHealth;

        }


        
        characterModel.GetComponent<Renderer>().material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }



    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            UIController.instance.healthBar.value = currentHealth;
            UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);

            if (PostProcessVolume.profile.TryGetSettings(out vignette))
            {
                vignette.intensity.value = maxHealth / (currentHealth * 4);
            }


            if (isSurvivor)
            {
                UIController.instance.Healthbar.gameObject.SetActive(true);
                UIController.instance.ZombieHealthbar.gameObject.SetActive(false);
                UIController.instance.playericon.SetActive(true);
                UIController.instance.zombieIcon.SetActive(false);
            }

            if (shotCounter >= 0)
            {
                shotCounter -= Time.deltaTime;
            }

            //Regenerate();

            Vector3 mouseWorldPosition = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                mouseWorldPosition = raycastHit.point;
                firePoint.LookAt(raycastHit.point);
                muzzlePoint.LookAt(raycastHit.point);
                debugTransform.position = raycastHit.point;
            }


            if (!starterAssetsInputs.aim)
            {
                activeWeapon._animator.SetBool("isAiming", false);
            }

            if (starterAssetsInputs.aim)
            {
                 activeWeapon._animator.SetBool("isAiming", true);
                aimCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
                thirdPersonController.setRotateOnMove(false);

                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }
            else
            {
           
                aimCamera.gameObject.SetActive(false);
                thirdPersonController.setRotateOnMove(true);
                thirdPersonController.SetSensitivity(normalSensitivity);
                
               
            }



            if (!manaDepleted)
            {
                if (starterAssetsInputs.shoot)
                {

                    
                  
                    if(shotCounter <= 0)
                    {
                        Shoot();
                    }

                    

                    //activeWeapon._animator.SetTrigger("isShooting");
                    starterAssetsInputs.shoot = false;
                }

                if (Input.GetMouseButton(0) && activeWeapon.isAutomatic)
                {
                    shotCounter -= Time.deltaTime;

                    if (shotCounter <= 0)
                    {
                     
                        Shoot();
                        

                    }
                }

                //Mana Recharge
                manaCounter -= coolRate * Time.deltaTime;
            }
            else
            {
                manaCounter -= manaCoolRate * Time.deltaTime;
                if (manaCounter <= 0)
                {
                    manaDepleted = false;
                    //UIController.instance.manadepletedMessage.gameObject.SetActive(false);
                    UIController.instance.image.gameObject.SetActive(false);
                }
            }

            if (manaCounter < 0)
            {
                manaCounter = 0f;
            }

            UIController.instance.ManaBar.fillAmount = manaCounter / maxMana;

            //Switch Weapon using mouse scroll wheel
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {

                photonView.RPC("SwitchWeapon", RpcTarget.All);
                //photonView.RPC("IconSwitch", RpcTarget.All);
                firePoint.position = activeWeapon.firepoint.position;
                muzzlePoint.position = activeWeapon.muzzlePoint.position;
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                photonView.RPC("SwitchWeapon", RpcTarget.All);
                //photonView.RPC("IconSwitch", RpcTarget.All);
                firePoint.position = activeWeapon.firepoint.position;
                muzzlePoint.position = activeWeapon.muzzlePoint.position;
            }

            //Allow cursor to appear once escape is clicked in the build version.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;

            }
            //Allow cursor to go back to invisible if the player clikc their left mouse key.
            else if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButtonDown(0) && !UIController.instance.optionsScreen.activeInHierarchy)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

        }

        if (isInfected == true)
        {
         
            PlayerSpawner.instance.eventZombie();
            
            isSurvivor = false;
        }

        if (OnFire)
        {
            currentHealth -= 3 * Time.deltaTime;
            if (photonView.IsMine)
            {
                UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);
            }
        }

        if (onCursed)
        {
            currentHealth -= 2 * Time.deltaTime;
            if (photonView.IsMine)
            {
                UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);
            }
        }

        if(currentHealth <= 0)
        {
             if (photonView.IsMine)
            {
                currentHealth = 0;

                string system = "system";

                PlayerSpawner.instance.Death(system);

            }
        }

    }






    private void Shoot()
    {
        if (photonView.IsMine )
        {
            //setting up the raycast point to the exact center of the screen.
            Vector3 mouseWorldPosition = Vector3.zero;
            Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
            ray.origin = cam.transform.position;

            //If raycast detect something store the information in the Raycasthit hit.
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                mouseWorldPosition = hit.point;
                Vector3 aimDir = (mouseWorldPosition - firePoint.position).normalized;
                PhotonNetwork.Instantiate(activeWeapon.bullet.name, firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
                Instantiate(activeWeapon.muzzleFlash, muzzlePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

            }

            shotCounter = activeWeapon.timeBetweenShots;

         
                //Limiting player Shooting through a mana depletion system
                manaCounter += activeWeapon.manaPerShot;

                if (manaCounter >= maxMana)
                {
                    manaCounter = maxMana;

                    manaDepleted = true;


                    //UIController.instance.manadepletedMessage.gameObject.SetActive(true);
                    UIController.instance.image.gameObject.SetActive(true);
                }
            

            activeWeapon.weaponSound.Stop();
            activeWeapon.weaponSound.Play();

            //StartCoroutine(shoots());
        }
    }

    [PunRPC]
    public void SwitchWeapon()
    {
        if (activeWeapon != null)
        {
            activeWeapon.gameObject.SetActive(false);
        }

        currentWeapon++;

        if (currentWeapon >= allWeapons.Count)
        {
            currentWeapon = 0;
        }

        activeWeapon = allWeapons[currentWeapon];
        activeWeapon.gameObject.SetActive(true);

        if (photonView.IsMine)
        {
            WeaponDisplay.instance.WeaponName.text = activeWeapon.weaponName;
            WeaponDisplay.instance.WeaponSpecial.text = activeWeapon.specialTrait;
            WeaponDisplay.instance.WeaponSpecial.color = activeWeapon.traitColor;

            for (int i = 0; i < WeaponDisplay.instance.icons.Length; i++)
            {
                WeaponDisplay.instance.icons[i].SetActive(i == activeWeapon.Icon);
            }
        }

    }

    [PunRPC]
    public void AddWeapon(string weaponToAdd)
    {
        bool weaponUnlocked = false;

        if(unlockableWeapons.Count > 0)
        {
            for (int i = 0; i < unlockableWeapons.Count; i++)
            {
                if(unlockableWeapons[i].weaponName == weaponToAdd)
                {
                    weaponUnlocked = true;

                    allWeapons.Add(unlockableWeapons[i]);
                    unlockableWeapons.RemoveAt(i);
                    i = unlockableWeapons.Count;
                }
            
            }
        }

       if (weaponUnlocked)
        {
            currentWeapon = allWeapons.Count - 2;
            photonView.RPC("SwitchWeapon", RpcTarget.All);
            //photonView.RPC("IconSwitch", RpcTarget.All);
        }
    }

    [PunRPC]
    public void KillLayer(int damageAmount)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;

                string system = "system";

                PlayerSpawner.instance.Death(system);
            }

            UIController.instance.healthBar.value = currentHealth;
        }
      
    }

    [PunRPC]
    public void Heal()
    {
        currentHealth += 40;

        if(currentHealth > maxHealth)
        {
            currentHealth = 100;
        }
    }

    [PunRPC]
    public void onFire()
    {
        StartCoroutine(catchFire());
    }


    [PunRPC]
    public void KillLayerDamage(int damageAmount)
    {
        KillLayer(damageAmount);
    }

    [PunRPC]
    public void cursedOn()
    {
        StartCoroutine(cursed());
    }

    [PunRPC]
    public void DealDamage(string damageDealer, int damageAmount, int actor)
    {
        TakeDamage(damageDealer, damageAmount, actor);
    }


    public void TakeDamage(string damageDealer, int damageAmount, int actor)
    {
        if (photonView.IsMine)
        {
            Debug.Log(photonView.Owner.NickName + "I've been hit by" + damageDealer);

            currentHealth -= damageAmount;

            UIController.instance.ShowDamage();

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                PlayerSpawner.instance.Death(damageDealer);

                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
            }

            UIController.instance.healthBar.value = currentHealth;
        
        }

    }

    IEnumerator shoots()
    {
      
        yield return new WaitForSeconds(5);
   
    }

    IEnumerator catchFire()
    {
        fireAura.SetActive(true);

        OnFire = true;

        yield return new WaitForSeconds(5);

        OnFire = false;

        if (photonView.IsMine)
        {
            UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);
        }

        fireAura.SetActive(false);
    }

    IEnumerator cursed()
    {
        cursedAura.SetActive(true);
        onCursed = true;
        yield return new WaitForSeconds(15);
        onCursed = false;
        if (photonView.IsMine)
        {
            UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);
        }
        cursedAura.SetActive(false);
    }


}
