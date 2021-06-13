using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMove : MonoBehaviour
{

    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    bool canMove;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(canMove);
        if(canMove)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 rotation = new Vector3(vertical,0,horizontal);
            rotation = rotation.normalized;
            this.transform.rotation = Quaternion.AngleAxis(rotation.z * rotationSpeed * Time.deltaTime, Vector3.forward) * this.transform.rotation;
            this.transform.rotation = Quaternion.AngleAxis(rotation.x * rotationSpeed * Time.deltaTime, Vector3.left) * this.transform.rotation;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.tag == "Ground")
        {
            Debug.Log("Landed on the ground");
            canMoveUpdate(false);
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void canMoveUpdate(bool canMoveUpdated)
    {
        Debug.Log("updated can move");
        canMove = canMoveUpdated;
    }
}
