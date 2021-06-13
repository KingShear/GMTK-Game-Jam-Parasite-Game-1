using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{

    [SerializeField]
    float pauseTime;
    float currentTime;
    bool isRotating;
    [SerializeField]
    float rotationSpeed;
    float xRotation;
    float prevRotation;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = pauseTime;
        isRotating = false;
        prevRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime < 0)
        {
            currentTime = pauseTime;
            isRotating = true;
        }
        if(!isRotating)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            xRotation += Time.deltaTime * rotationSpeed;
            if(xRotation - prevRotation > 180)
            {
                prevRotation = xRotation;
                isRotating = false;
            }
            transform.localRotation = Quaternion.Euler(xRotation,0,0);
        }
    }
}
