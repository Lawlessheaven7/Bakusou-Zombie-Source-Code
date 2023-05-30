using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class optionMenu : MonoBehaviour
{
    public Toggle fullScreenTog;
    public Toggle vSyncTog;

    public ResolutionItem[] resolutions;

    public TMP_Text resolutionText;

    private int selectedResolution;

    public AudioMixer theMixer;

    public Slider masterSlider, musicSlider, sfxSlider;
    public TMP_Text masterLabel, musicLabel, sfxLabel;

    // Start is called before the first frame update
    void Start()
    {
        fullScreenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            vSyncTog.isOn = false;
        }
        else
        {
            vSyncTog.isOn = true;
        }

        //search for resolution in the list
        bool foundResolution = false;

        //loop through all the availbale resolution options to find and set the correct one 
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundResolution = true;

                selectedResolution = i;

                updateResolutionText();
            }

        }

        if (!foundResolution)
        {
            resolutionText.text = Screen.width.ToString() + " x " + Screen.height.ToString();
        }

        if (PlayerPrefs.HasKey("Master Vol"))
        {
            theMixer.SetFloat("Master Vol", PlayerPrefs.GetFloat("Master Vol"));
            masterSlider.value = PlayerPrefs.GetFloat("Master Vol");

        }

        if (PlayerPrefs.HasKey("Music Vol"))
        {
            theMixer.SetFloat("Music Vol", PlayerPrefs.GetFloat("Music Vol"));
            musicSlider.value = PlayerPrefs.GetFloat("Music Vol");

        }

        if (PlayerPrefs.HasKey("SFX Vol"))
        {
            theMixer.SetFloat("SFX Vol", PlayerPrefs.GetFloat("SFX Vol"));
            sfxSlider.value = PlayerPrefs.GetFloat("SFX Vol");

        }

        masterLabel.text = (masterSlider.value + 80).ToString();
        musicLabel.text = (musicSlider.value + 80).ToString();
        sfxLabel.text = (sfxSlider.value + 80).ToString();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResLeft()
    {
        selectedResolution--;
        //Do not go below the element 0
        if (selectedResolution < 0)
        {
            selectedResolution = 0;
        }

        updateResolutionText();
    }

    public void ResRight()
    {
        selectedResolution++;
        //Do not go above the maximum element
        if (selectedResolution > resolutions.Length - 1)
        {
            selectedResolution = resolutions.Length - 1;
        }

        updateResolutionText();
    }

    public void updateResolutionText()
    {
        resolutionText.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    public void ApplyGraphics()
    {
        //Apply Full Screen
        //Screen.fullScreen = fullScreenTog.isOn;

        //Apply Vsync
        if (vSyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        //set resolution
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullScreenTog.isOn);
    }

    public void SetMasterVolume()
    {
        masterLabel.text = (masterSlider.value + 80).ToString();

        theMixer.SetFloat("Master Vol", masterSlider.value);

        PlayerPrefs.SetFloat("Master Vol", masterSlider.value);
    }

    public void SetMusicVolume()
    {
        musicLabel.text = (musicSlider.value + 80).ToString();

        theMixer.SetFloat("Music Vol", musicSlider.value);

        PlayerPrefs.SetFloat("Music Vol", musicSlider.value);
    }

    public void SetSFXVolume()
    {
        sfxLabel.text = (sfxSlider.value + 80).ToString();

        theMixer.SetFloat("SFX Vol", sfxSlider.value);

        PlayerPrefs.SetFloat("SFX Vol", sfxSlider.value);
    }
}

[System.Serializable]
public class ResolutionItem
{
    public int horizontal, vertical;
}
