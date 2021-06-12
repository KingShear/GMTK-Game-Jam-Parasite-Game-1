using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    private Transform cameraLookAt;
    [SerializeField]
    private Transform cameraFollow;
    float adjustmentScale;
    float clipMoveSpeed;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        adjustmentScale = .9f;
        clipMoveSpeed = 5f;
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
            transform.position = cameraFollow.position;
        }
        this.transform.LookAt(cameraLookAt.position);
        RaycastHit hit;
        if (Physics.Raycast(cameraLookAt.transform.position, (this.transform.position - cameraLookAt.transform.position).normalized,out hit,Vector3.Distance(this.transform.position,cameraLookAt.transform.position)))
        {
            if (hit.transform.tag != "Player")
            {
                //Debug.Log("HELP");
                Vector3 newPosition = hit.point;
                newPosition = cameraLookAt.transform.position + (newPosition - cameraLookAt.transform.position) * adjustmentScale;
                this.transform.position = newPosition;
            }
        }
    }
}
