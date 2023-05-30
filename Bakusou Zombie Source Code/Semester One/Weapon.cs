using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Weapon : MonoBehaviourPun
{
    public static Weapon instance;

    public GameObject bullet;
    public GameObject muzzleFlash;
    public bool isAutomatic;
    public float timeBetweenShots = .1f, manaPerShot = 1f;
    public AudioSource weaponSound;
    public Transform firepoint;
    public Transform muzzlePoint;
    public Animator _animator;
    public string weaponName;
    public string specialTrait;
    public int Icon;
    public Color traitColor;
    
 

    private void Awake()
    {
       instance = this;
       
    }

    public void Start()
    {
       
   
    }

    private void Update()
    {

    }

}
