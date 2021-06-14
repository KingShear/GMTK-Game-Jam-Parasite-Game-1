using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class HazardDetection : MonoBehaviour
{
    public Sprite heartSprite;
    public Image[] hearts;
    int num_hearts;

    PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.GetComponent<PlayerMovement>();
        num_hearts = 3;
    }

    // Update is called once per frame
    void Update()
    {
        switch(num_hearts)
        {
            case 3:
                hearts[0].sprite = heartSprite;
                hearts[1].sprite = heartSprite;
                hearts[2].sprite = heartSprite;
                break;
            case 2:
                hearts[0].sprite = heartSprite;
                hearts[1].sprite = heartSprite;
                break;
            case 1:
                hearts[0].sprite = heartSprite;
                break;
            case 0:
                hearts[0].sprite = heartSprite;
                hearts[1].sprite = heartSprite;
                hearts[2].sprite = heartSprite;
                num_hearts = 3;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision Detected");
        //Debug.Break();
        DetectedCollision(other.transform.tag);
    }

    void DetectedCollision(string colliderTag)
    {
        Debug.Log("Checking collision for " + colliderTag);
        switch(colliderTag)
        {
            case "Spikes": 
                FellOnSpikes();
                break;
            case "Acid": 
                FellOnAcid();
                break;
            default:break;
        }
    }

    void FellOnSpikes()
    {
        Debug.Log("Ouch I fell on the spikes!");
        num_hearts--;
        player.Respawn();
    }
    void FellOnAcid()
    {
        Debug.Log("Ouch I fell on the acid!");
        num_hearts--;
    }
}
