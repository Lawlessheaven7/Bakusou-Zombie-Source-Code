using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponDisplay : MonoBehaviourPun
{
    public static WeaponDisplay instance;
    public GameObject[] icons;
    public TMP_Text WeaponName, WeaponSpecial;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
    

      
       //WeaponIconUpdate();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WeaponIconUpdate()
    {
     
            WeaponName.text = ThirdPersonCameraControl.instance.activeWeapon.weaponName;
            WeaponSpecial.text = ThirdPersonCameraControl.instance.activeWeapon.specialTrait;
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].SetActive(i == ThirdPersonCameraControl.instance.activeWeapon.Icon);
            }
        
        
    }

}
