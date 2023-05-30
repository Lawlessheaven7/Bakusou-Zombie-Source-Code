using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Photon.Pun;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class zombieControl : MonoBehaviourPunCallbacks 
{
    public static zombieControl instance;

    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfBulletProjectile;

    public float maxMana = 10f, currentMana = 10f, coolRate = 4f, manaCoolRate = 5f;

    public float maxHealth = 100;

    public GameObject characterModel;
    public GameObject mainCamera;
    public GameObject recognizer;
    public GameObject damageBox;
    public GameObject ZombieAura;
    public GameObject executionAura;
    public GameObject fireAura;
    public GameObject executionExplosion;
    public GameObject damageAura;

    public Transform debugTransform;
    public Transform firePoint;
    public Transform DebuffSpawn;

    public float currentHealth;
    public int currentSkin;

    public bool shoot;
    public bool OnFire;

    public Material[] allSkins;

    public Animator anim;

    public AudioSource weaponSound;

    public Weapon activeWeapon;

    private Camera cam;
   

    private float activeSpeed;
    private float lastShootTime;
    public float shotCounter;
    private float manaCounter;

    private bool manaDepleted;
    public bool isZombie = true;
    

    private ZombieController zombieController;
    private ZombieInputs ZombieInputs;

    [Header("Post Processing")]
    public PostProcessVolume PostProcessVolume;
    public Vignette vignette;

    public PhotonView myPV;


    private void Awake()
    {
        instance = this;
  

        zombieController = GetComponent<ZombieController>();
        ZombieInputs = GetComponent<ZombieInputs>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        damageBox.SetActive(false);
        shoot = true;
    
        if (photonView.IsMine)
        {
            mainCamera.SetActive(true);
            recognizer.gameObject.SetActive(false);

            cam = Camera.main;
            UIController.instance.ManaBar.fillAmount = maxMana;

            currentHealth = maxHealth;
            UIController.instance.healthBar.maxValue = maxHealth;
            UIController.instance.healthBar.value = currentHealth;
            UIController.instance.Healthbar.SetBar(currentHealth, 0, maxHealth);

        }

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

        characterModel.GetComponent<Renderer>().material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }


    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (PostProcessVolume.profile.TryGetSettings(out vignette))
            {
                vignette.intensity.value = maxHealth / (currentHealth * 4);
            }

            if (shotCounter >= 0)
            {
                shotCounter -= Time.deltaTime;
                

            }
         
            if (isZombie)
            {
                UIController.instance.zombieIcon.gameObject.SetActive(true);
                UIController.instance.playericon.gameObject.SetActive(false);
            }
            Vector3 mouseWorldPosition = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                mouseWorldPosition = raycastHit.point;
                firePoint.LookAt(raycastHit.point);
                debugTransform.position = raycastHit.point;
            }


            if (!ZombieInputs.aim)
            {
                activeWeapon._animator.SetBool("isAiming", false);
            }

            if (ZombieInputs.aim)
            {
                activeWeapon._animator.SetBool("isAiming", true);
                aimCamera.gameObject.SetActive(true);
                zombieController.SetSensitivity(aimSensitivity);
                zombieController.setRotateOnMove(false);

                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }
            else
            {
           
                aimCamera.gameObject.SetActive(false);
                zombieController.setRotateOnMove(true);
                zombieController.SetSensitivity(normalSensitivity);
                
               
            }



            if (!manaDepleted)
            {
                if (ZombieInputs.shoot && shotCounter <= 0)
                {
                  
                    Shoot();
                    anim.SetTrigger("Shoot");
                    ZombieInputs.shoot = false;
                    shotCounter = activeWeapon.timeBetweenShots;
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
                    //UIController.instance.image.gameObject.SetActive(false);
                }
            }

            if (manaCounter < 0)
            {
                manaCounter = 0f;
            }

            UIController.instance.ManaBar.fillAmount = manaCounter / maxMana;


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

            if (OnFire)
            {
                currentHealth -= 3 * Time.deltaTime;
                if (photonView.IsMine)
                {
                    UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);
                }
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                string system = "system";

                PlayerSpawner.instance.Death(system);
            }

            //UIController.instance.healthBar.value = currentHealth;

        }

    }

   

    private void Shoot()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(damage());
            //setting up the raycast point to the exact center of the screen.
            Vector3 mouseWorldPosition = Vector3.zero;
            Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
            ray.origin = cam.transform.position;

            //If raycast detect something store the information in the Raycasthit hit.
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                mouseWorldPosition = hit.point;
                Vector3 aimDir = (mouseWorldPosition - firePoint.position).normalized;
                if (ZombieInputs.aim & shoot == true)
                {
                    activeWeapon.weaponSound.Stop();
                    activeWeapon.weaponSound.Play();
                    PhotonNetwork.Instantiate(activeWeapon.bullet.name, firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
                    


                    //Limiting player Shooting through a mana depletion system
                    manaCounter += activeWeapon.manaPerShot;

                    if (manaCounter >= maxMana)
                    {
                        manaCounter = maxMana;

                        manaDepleted = true;

                        //UIController.instance.manadepletedMessage.gameObject.SetActive(true);
                        //UIController.instance.image.gameObject.SetActive(true);
                    }

                    StartCoroutine(shoots());
                }

                
            }

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
    public void DealDamage(string damageDealer, int damageAmount, int actor)
    {
        TakeDamage(damageDealer, damageAmount, actor);
    }

    [PunRPC]
    public void killLayerDamageZombie(int damageAmount)
    {
        KillLayer(damageAmount);
    }

    [PunRPC]
    public void auraOn()
    {
        ZombieAura.SetActive(true);
    }


    [PunRPC]
    public void auraOff()
    {
        ZombieAura.SetActive(false);
    }

    [PunRPC]
    public void execution()
    {
        StartCoroutine(Execution());
    }

    [PunRPC]
    public void onFire()
    {
        StartCoroutine(catchFire());
    }

    [PunRPC]
    public void DamageAura()
    {
        StartCoroutine(DamageAuraOn());
    }


    public void TakeDamage(string damageDealer, int damageAmount, int actor)
    {
        if (photonView.IsMine)
        {
            //Debug.Log(photonView.Owner.NickName + "I've been hit by" + damageDealer);

            currentHealth -= damageAmount;

            UIController.instance.ShowDamage();

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                PlayerSpawner.instance.Death(damageDealer);

                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
            }

            UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);


        }

    }

    IEnumerator damage()
    {
        damageBox.SetActive(true);

        yield return new WaitForSeconds(1);

        damageBox.SetActive(false);

    }

    IEnumerator shoots()
    {
        shoot = false;
        myPV.RPC("auraOn", RpcTarget.All);
        yield return new WaitForSeconds(5);
        myPV.RPC("auraOff", RpcTarget.All);
        shoot = true;
    }

    IEnumerator Execution()
    {
        executionAura.SetActive(true);

        yield return new WaitForSeconds(5);

        PhotonNetwork.Instantiate(executionExplosion.name, DebuffSpawn.position, Quaternion.identity);

        executionAura.SetActive(false);

    }

    IEnumerator catchFire()
    {
        fireAura.SetActive(true);

        OnFire = true;

        yield return new WaitForSeconds(3);

        OnFire = false;

        if (photonView.IsMine)
        {
            UIController.instance.Healthbar.UpdateBar(currentHealth, 0, maxHealth);
        }

        fireAura.SetActive(false);
    }

    IEnumerator DamageAuraOn()
    {
        damageAura.SetActive(true);
        yield return new WaitForSeconds(30);
        damageAura.SetActive(false);
    }
}
