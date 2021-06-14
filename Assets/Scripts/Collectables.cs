using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectables : MonoBehaviour
{
    public Text partsText;
    int numCollectables;

    // Start is called before the first frame update
    void Start()
    {
        numCollectables = 0;
    }

    // Update is called once per frame
    void Update()
    {
        partsText.text = numCollectables.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Collectable")
        {
            Debug.Log("Collected");
            numCollectables++;
            Destroy(other.gameObject);
        }
        if(other.transform.tag == "EndLevel")
        {
            Debug.Log("Ending Level");
            if(numCollectables >=4)
            {
                Debug.Log("Level ended");
            }
        }
    }

    public float GetNumCollectables()
    {
        return numCollectables;
    }
}
