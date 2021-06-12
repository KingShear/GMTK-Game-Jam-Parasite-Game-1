using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    private Transform cameraLookAt;
    [SerializeField]
    private Transform cameraFollow;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
    }

    private void FixedUpdate()
    {
    }

    void CameraMove()
    {
        if (cameraFollow)
        {
            transform.position = Vector3.Lerp(transform.position, cameraFollow.position, 1.2f);
        }
        this.transform.LookAt(cameraLookAt.position);
        RaycastHit hit;
        if (Physics.Raycast(cameraLookAt.transform.position, (this.transform.position - cameraLookAt.transform.position).normalized,out hit,Vector3.Distance(this.transform.position,cameraLookAt.transform.position)))
        {
            if (hit.transform.tag != "Player")
            {
                //Debug.Log("HELP");
                Vector3 newPosition = hit.point;
                this.transform.position = newPosition;
            }
        }
    }
}
