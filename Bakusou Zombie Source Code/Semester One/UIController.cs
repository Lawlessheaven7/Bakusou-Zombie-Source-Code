using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using MoreMountains.Tools;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text TimerText;

    public GameObject deathText;
    public GameObject image;
    public GameObject deathScreen;
    public GameObject leaderBoard;
    public GameObject endScreen;
    public GameObject optionsScreen;
    public GameObject settingScreen;
    public GameObject menuButtons, menuTitles;
    public GameObject SurvivorWins, ZombieWins;
    public GameObject playericon, zombieIcon;
    public Leaderboard leaderboardPlayerDisplay;

    public Slider manaBar;
    public Slider healthBar;
    public MMProgressBar Healthbar;
    public MMProgressBar ZombieHealthbar;
    public Image ManaBar;

    public Image damageEffect;
    public float damageAlpha = .25f, damageFadeSpeed = 2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openOptions();
        }

        if(optionsScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (damageEffect.color.a != 0)
        {
            damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, Mathf.MoveTowards(damageEffect.color.a, 0f, damageFadeSpeed * Time.deltaTime));
        }
    }

    public void openOptions()
    {
        if (!optionsScreen.activeInHierarchy)
        {
            optionsScreen.SetActive(true);
        }
        else if (optionsScreen.activeInHierarchy && !settingScreen.activeInHierarchy)
        {
            optionsScreen.SetActive(false);
        }
    }

    public void openSettings()
    {
        menuTitles.gameObject.SetActive(false);
        menuButtons.gameObject.SetActive(false);
        settingScreen.gameObject.SetActive(true);
    }

    public void closeSettings()
    {
        menuTitles.gameObject.SetActive(true);
        menuButtons.gameObject.SetActive(true);
        settingScreen.gameObject.SetActive(false);
    }

    public void ReturnToMain()
    {
        //making sure not to sync with other player when return to main menu
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void ShowDamage()
    {
        damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, .25f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
