using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpSpawnPointManager : MonoBehaviour
{
    public static pickUpSpawnPointManager instance;

    public Transform[] pickUpSpawnPoint;

    public Transform[] healthPickUpPoint;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform spawn in pickUpSpawnPoint)
        {
            spawn.gameObject.SetActive(false);
        }

        foreach (Transform spawn2 in healthPickUpPoint)
        {
            spawn2.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetSpawnPoint()
    {
        return pickUpSpawnPoint[Random.Range(0, pickUpSpawnPoint.Length)];
    }

    public Transform GetSpawnPoint2()
    {
        return healthPickUpPoint[Random.Range(0, healthPickUpPoint.Length)];
    }
}
