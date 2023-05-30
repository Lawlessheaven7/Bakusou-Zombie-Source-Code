using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;

    public float mouseSensitivity = 1f;
    public float moveSpeed = 5f, runSpeed = 8f;
    public float jumpForce = 7f;
    public float gravityModifier = 2.5f;
    //public float timeBetweenShots = .1f;
    public float maxMana = 10f, /*manaPerShot = 1f,*/ coolRate = 4f, manaCoolRate = 5f;

    public int maxHealth = 100;

    public bool invertLook;

    public Transform groundCheckPoint;

    public LayerMask groundLayer;

    public CharacterController charController;

    public GameObject bulletImpact;
    public GameObject playerHitImpact;
    public GameObject characterModel;

    public Weapon[] allWeapons;

    public Animator anim;

    private float verticalRotationMemory;
    private float activeSpeed;
    private float shotCounter;
    private float manaCounter;
    
    private Vector2 mouseInput;
    private Vector3 moveDirection, movement;
    
    private bool isGrounded;
    private bool manaDepleted;

    private int selectedWeapon;
    private int currentHealth;

    public int shotDamage;

    public Material[] allSkins;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        UIController.instance.manaBar.maxValue = maxMana;
        photonView.RPC("SetWeapon", RpcTarget.All, selectedWeapon);

        currentHealth = maxHealth;
        UIController.instance.healthBar.maxValue = maxHealth;
        UIController.instance.healthBar.value = currentHealth;

        //set up character skins and loop it 
        characterModel.GetComponent<Renderer>().material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }

    // Update is called once per frame
    void Update()
    {
        //Control only the local player 
        if (photonView.IsMine)
        {
            // Enable players to use mouse
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            //Using Eulerangles to only move the camera on the X axis based on the Honrizontal mouseInput
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            //Store rotation memory to prevent Clamp eulerangle instant shift.
            verticalRotationMemory += mouseInput.y;
            verticalRotationMemory = Mathf.Clamp(verticalRotationMemory, -20f, 30f);

            //Optional Invert Look
            if (invertLook)
            {
                //Using Eulerangles to move the camera Up and Down based on Vertical Mouse Input.
                viewPoint.rotation = Quaternion.Euler(verticalRotationMemory, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }
            else
            {
                viewPoint.rotation = Quaternion.Euler(-verticalRotationMemory, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }

            //Movement set up, making sure movement direction are based on players mouse Input and normalized so player cannot go faster diagonally.
            moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));


            //Running 
            if (Input.GetKey(KeyCode.LeftShift))
            {
                activeSpeed = runSpeed;
            }
            else
            {
                activeSpeed = moveSpeed;
            }

            //Gravity
            float yVelo = movement.y;
            movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized * activeSpeed;
            movement.y = yVelo;

            if (charController.isGrounded)
            {
                movement.y = 0f;
            }

            //Using raycast to check if the player is grounded
            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayer);

            if (Input.GetButtonDown("Jump") && charController.isGrounded)
            {
                movement.y = jumpForce;

            }

            movement.y += Physics.gravity.y * Time.deltaTime * gravityModifier;

            charController.Move(movement * Time.deltaTime);
            if (!manaDepleted)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }

                //Automatic Shooting
                if (Input.GetMouseButton(0) && allWeapons[selectedWeapon].isAutomatic)
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
                if(manaCounter <= 0)
                {
                    manaDepleted = false;
                    //UIController.instance.manadepletedMessage.gameObject.SetActive(false);
                    UIController.instance.image.gameObject.SetActive(false);
                }
            }

            if(manaCounter < 0)
            {
                manaCounter = 0f;
            }

            UIController.instance.manaBar.value = manaCounter;

            //Switch Weapon using mouse scroll wheel
            if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                selectedWeapon++;

                if(selectedWeapon >= allWeapons.Length)
                {
                    selectedWeapon = 0;
                }

                photonView.RPC("SetWeapon", RpcTarget.All, selectedWeapon);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                selectedWeapon--;

                if (selectedWeapon < 0)
                {
                    selectedWeapon = allWeapons.Length - 1;
                }
                photonView.RPC("SetWeapon", RpcTarget.All, selectedWeapon);
            }

            //Switch Weapon
            for (int i = 0; i < allWeapons.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    selectedWeapon = i;
                    photonView.RPC("SetWeapon", RpcTarget.All, selectedWeapon);
                }
            }

            anim.SetBool("grounded", isGrounded);
            anim.SetFloat("speed", moveDirection.magnitude);


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

    }

    private void Shoot()
    {
        //setting up the raycast point to the exact center of the screen.
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;

        //If raycast detect something store the information in the Raycasthit hit.
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("We hit " + hit.collider.gameObject.name);

            if(hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "Zombie")
            {
                Debug.Log("Hit" + hit.collider.gameObject.GetPhotonView().Owner.NickName);

                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);

                PhotonNetwork.Instantiate(bulletImpact.name, hit.point, Quaternion.identity);

                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 10f);
            }
         
        }

        shotCounter = allWeapons[selectedWeapon].timeBetweenShots;

        //Limiting player Shooting through a mana depletion system
        manaCounter += allWeapons[selectedWeapon].manaPerShot;

        if(manaCounter >= maxMana)
        {
            manaCounter = maxMana;

            manaDepleted = true;

            //UIController.instance.manadepletedMessage.gameObject.SetActive(true);
            UIController.instance.image.gameObject.SetActive(true);
        }

        allWeapons[selectedWeapon].weaponSound.Stop();
        allWeapons[selectedWeapon].weaponSound.Play();

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
            //Debug.Log(photonView.Owner.NickName + "I've been hit by" + damageDealer);

            currentHealth -= damageAmount;

            if(currentHealth <= 0)
            {
                currentHealth = 0;

                PlayerSpawner.instance.Death(damageDealer);

                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
            }

            UIController.instance.healthBar.value = currentHealth;

        }
   
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            if(MatchManager.instance.state == MatchManager.Gamestate.Playing)
            {
                cam.transform.position = viewPoint.position;
                cam.transform.rotation = viewPoint.rotation;
            }
            else
            {
                cam.transform.position = MatchManager.instance.mapCamPoint.position;
                cam.transform.rotation = MatchManager.instance.mapCamPoint.rotation;
            }

           
        }
      
    }

    void SwitchWeapon()
    {
        foreach(Weapon weapon in allWeapons)
        {
            weapon.gameObject.SetActive(false);

        }

        allWeapons[selectedWeapon].gameObject.SetActive(true);
       
    }

    //Switch weapon using RPC
    [PunRPC]
    public void SetWeapon(int weaponToSwitchTo)
    {
        if(weaponToSwitchTo < allWeapons.Length)
        {
            selectedWeapon = weaponToSwitchTo;
            SwitchWeapon();
        }
    }
}
        
