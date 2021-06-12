using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*yo zeldatechie: please ignore the crappy physics code for the parasite effects so far, thanks
    *I will fix it Saturday
    * Other notes: press T to test dash (I need to lerp so player doesn't clip through walls)
    * Press B to test block creation directly under player (fix buggy running jump off of platforms)
    * Press J to test high jump
    */ 

    [SerializeField]
    float speed;
    float rotationSpeed;
    float dashSpeed = 5000.0f;
    bool isDashing;
    float dashTimer = 0.0f;
    Rigidbody rb;
    [SerializeField]
    Transform cameraFollow;
    [SerializeField]
    Transform cameraRotate;
    [SerializeField]
    public bool isAttacking;
    bool isGrounded;
    float numJumps;
    float maxJumps;
    [SerializeField]
    Transform visuals;
    [SerializeField]
    float mouseSensitivity = 100f;
    float clampAngleMax = 40f;
    float clampAngleMin = -70f;
    float rotY = 0.0f;
    float rotX = 0.0f;
    float jumpForce = 200.0f;

    //activate bools when player collides with specific powerups
    bool speedParasite;
    bool jumpParasite;
    bool blockParasite;
    public GameObject parasiteBlockPrefab;

    GameObject playerForwardTransform;

    public string state;

    void Awake()
    {
        speed = 10f;
        rotationSpeed = 200f;
        rb = this.GetComponent<Rigidbody>();
        maxJumps = 1;
        numJumps = maxJumps;
        isAttacking = false;
        isDashing = false;

        speedParasite = false;
        jumpParasite = false;
        blockParasite = false;

        playerForwardTransform = GameObject.Find("Visuals");
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = cameraRotate.transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        Vector3 playerLocation = transform.position;
        Instantiate(parasiteBlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

        if (!isAttacking)
        {
            Move();
        }


        if (isDashing)
        {
            Dashing();
        }
        else
        {
            dashTimer = 0.0f;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ParasiteBlock();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ParasiteJump();
        }
    }

    void ParasiteDash()
    {

    }
    void ParasiteJump()
    {
        jumpForce = 300.0f; //adjust as needed
    }
    void ParasiteBlock()
    {
        Vector3 blockPosition = new Vector3(this.transform.position.x, this.transform.position.y - 0.25f, this.transform.position.z);
        Instantiate(parasiteBlockPrefab, blockPosition, Quaternion.identity);
        parasiteBlockPrefab.tag = "Ground";
    }

    void Move()
    {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float yVel = rb.velocity.y;
        Vector3 movement = new Vector3(horizontal,0,vertical);
        movement = cameraFollow.transform.TransformDirection(movement);
        movement = movement.normalized * speed;
        if (movement.magnitude > 0)
        {
            speed = 10f;
            RotateCharacter(movement);
            state = "walking";
        }
        else
        {
            state = "idle";
        }
        if (Input.GetButton("Fire3") && isGrounded)
        {
            speed = 20f;
            state = "running";
        }
        movement.y = yVel;
        rb.velocity = movement;
        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (numJumps > 0) )
            {
                if (numJumps != maxJumps)
                {
                    movement.y = 0;
                    rb.velocity = movement;
                    numJumps--;
                }
                isGrounded = false;
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                if(state == "running")
                {
                    rb.AddForce(movement.normalized * jumpForce, ForceMode.Impulse);
                }
                
            }
            
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            isDashing = true;
        }
        if (!isGrounded)
        {
            state = "jumping";
        }
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, clampAngleMin, clampAngleMax);
        Quaternion localRotation = Quaternion.Euler(-rotX,rotY,0.0f);
        cameraRotate.transform.rotation = localRotation;

    }



    void RotateCharacter(Vector3 lookAt)
    {
        lookAt += this.transform.position;
        lookAt.y = this.transform.position.y + 1f;
        //Debug.Log(lookAt);
        visuals.transform.LookAt(lookAt);
    }

    public IEnumerator Attack(float seconds)
    {
        isAttacking = true;
        yield return new WaitForSeconds(seconds);
        isAttacking = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isGrounded = true;
            numJumps = maxJumps;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isGrounded = false;
            numJumps--;
        }
    }
    private void Dashing()
    {
        rb.AddForce(playerForwardTransform.transform.forward * dashSpeed, ForceMode.Impulse);

        isDashing = false;
        /*Debug.Log("RBX: " + playerForwardTransform.transform.forward.x.ToString());
        Debug.Log("RBY: " + playerForwardTransform.transform.forward.y.ToString());
        Debug.Log("RBZ: " + playerForwardTransform.transform.forward.z.ToString());*/
    }
}
