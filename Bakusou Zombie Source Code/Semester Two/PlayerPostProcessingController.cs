using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;


public class PlayerPostProcessingController : MonoBehaviourPun
{
    public PostProcessVolume PostProcessVolume;
    public Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ThirdPersonCameraControl.instance.photonView.IsMine)
        {
            if (PostProcessVolume.profile.TryGetSettings(out vignette))
            {
                vignette.intensity.value = ThirdPersonCameraControl.instance.maxHealth / (ThirdPersonCameraControl.instance.currentHealth * 4);
            }
        }
    }
}
