using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    GameObject objectToSpawn;

    GameObject objectSpawned;
    [SerializeField]
    float respawnTime;

    float currentTime;
    [SerializeField]
    bool hasObject;

    // Start is called before the first frame update
    void Start()
    {
        hasObject = true;
        objectSpawned =  Instantiate(objectToSpawn,this.transform.position,Quaternion.Euler(-90,0,0));
        objectSpawned.transform.SetParent(this.transform);
        currentTime = respawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.childCount == 0)
        {
            hasObject = false;
        }
        if(!hasObject)
        {
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
            {
                objectSpawned = Instantiate(objectToSpawn, this.transform.position, Quaternion.Euler(-90, 0, 0));
                objectSpawned.transform.SetParent(this.transform);
                currentTime = respawnTime;
                hasObject = true;
            }
        }

    }
}
