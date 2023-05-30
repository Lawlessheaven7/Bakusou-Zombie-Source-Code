using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

using PhotonHastable = ExitGames.Client.Photon.Hashtable;

public class Customizer : MonoBehaviour
{
    public TMP_Dropdown colorDropdown;

    public List<Material> colorMaterials;

    private List<string> materialNames = new List<string>();

    public static List<Material> Materials;

    public int matIndex;

    public string currentMaterialName;

    private PhotonHastable hash = new PhotonHastable();
    // Start is called before the first frame update
    void Start()
    {
        Materials = colorMaterials;

        colorDropdown.ClearOptions();

        foreach(Material material in colorMaterials) 
        {
            materialNames.Add(material.name);
        }
        colorDropdown.AddOptions(materialNames);
        colorDropdown.RefreshShownValue();
        SetColor(matIndex);
    }

    // Update is called once per frame
    void Update()
    {
        SetColor(matIndex);
    }

    public void SetColor(int index)
    {
        matIndex = index;
        currentMaterialName = materialNames[index];
        if (PhotonNetwork.IsConnected)
        {
            setHash();
        }
       
    }

    public void setHash()
    {
        hash["Material"] = currentMaterialName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
