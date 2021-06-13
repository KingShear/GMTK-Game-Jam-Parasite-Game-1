using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    bool isGrounded;
    int layerMask = 1 << 3;
    float slerpSpeed = 10;
    [SerializeField]
    GameObject child;
    [SerializeField]
    GameObject gnome;
    [SerializeField]
    DiceMove dice;
    float resetTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isGrounded)
        {
            //draw ray from bottom of character
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(this.transform.position, Vector3.down * hit.distance, Color.yellow);
                this.transform.position = hit.point + (Vector3.up * 1.01f);
            }
            else
            {
                Debug.DrawRay(this.transform.position, Vector3.down * 1000, Color.red);
            }
        }
        //draw ray from bottom of character
        RaycastHit hit2;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit2, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(this.transform.position, Vector3.down * hit2.distance, Color.yellow);
            Vector3 normal = hit2.normal;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.FromToRotation(Vector3.up, normal),slerpSpeed*Time.deltaTime);
            child.transform.position = hit2.point;
        }
        else
        {
            Debug.DrawRay(this.transform.position, Vector3.down * 1000, Color.red);
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 rotation = new Vector3(-horizontal, 0, -vertical);
        rotation = rotation.normalized;
        child.transform.forward = rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Dice")
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Dice")
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("TRIGGERING");
        if (other.tag == "Holes")
        {
            //Debug.Log("InTheHole!");
            dice.canMoveUpdate(false);
            other.enabled = false;
            StartCoroutine(resetTimer(other));
        }
    }

    IEnumerator resetTimer(Collider other)
    {
        yield return new WaitForSeconds(resetTime);
        dice.canMoveUpdate(true);
        yield return new WaitForSeconds(resetTime);
        other.enabled = true;
    }

}
