using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public GameObject[] points;
    int index;
    Rigidbody rb;
    float platformSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        rb = this.GetComponent<Rigidbody>();
        platformSpeed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(points != null && points.Length > 1)
        {

        }
    }
}
