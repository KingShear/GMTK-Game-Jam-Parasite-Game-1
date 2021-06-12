using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class HazardDetection : MonoBehaviour
{

    PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision Detected");
        DetectedCollision(collision.transform.tag);
    }

    void DetectedCollision(string colliderTag)
    {
        //Debug.Log("Checking collision for " + colliderTag);
        switch(colliderTag)
        {
            case "Spikes": FellOnSpikes();break;
            case "Acid": FellOnAcid();break;
            default:break;
        }
    }

    void FellOnSpikes()
    {
        Debug.Log("Ouch I fell on the spikes!");
    }
    void FellOnAcid()
    {
        Debug.Log("Ouch I fell on the acid!");
    }
}
