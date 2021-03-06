using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    float rotationSpeed;
    Rigidbody rb;
    [SerializeField]
    Transform cameraFollow;
    [SerializeField]
    Transform cameraRotate;
    [SerializeField]
    public bool isAttacking;
    [SerializeField]
    bool isGrounded;
    [SerializeField]
    bool isOnPlatform;
    float groundDistance;
    Rigidbody platform;
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
    [SerializeField]
    float jumpForce = 400.0f;

    [SerializeField]
    Animator playerAnim;

    [SerializeField]
    GameObject shadow;


    Vector3 respawnPoint;

    //PARASITE VARIABLES
    bool dashParasiteActive;
    bool jumpParasiteActive;
    bool blockParasiteActive;

    const float parasiteTimer = 30.0f;
    float dashParasiteTimer;
    float jumpParasiteTimer;
    float blockParasiteTimer;

    string parasiteOld;
    public Image parasiteOldImage;
    string parasiteNew;
    public Image parasiteNewImage;
    public Sprite[] parasiteImages;
    public Sprite emptyImage;

    float dashSpeed = 10000.0f;
    float dashDuration = 1.5f;
    public GameObject parasiteBlockPrefab;
    GameObject prevSpawnedBlock;
    GameObject nextSpawnedBlock;
    bool isBlockSpawned = false;
    public GameObject[] parasiteParticleArray; //1 = dash, 2 = jump, 3 = block
    GameObject parasiteParticles;

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

        dashParasiteActive = false;
        dashParasiteTimer = parasiteTimer;
        jumpParasiteActive = false;
        jumpParasiteTimer = parasiteTimer;
        blockParasiteActive = false;
        blockParasiteTimer = parasiteTimer;
        parasiteOldImage.sprite = emptyImage;
        parasiteOld = "";
        parasiteNewImage.sprite = emptyImage;
        parasiteNew = "";

        playerForwardTransform = GameObject.Find("Visuals");
        isOnPlatform = false;
        groundDistance = 1f;
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

        ParasiteTimers();
        if (dashParasiteActive && (Input.GetMouseButtonDown(0)))
        {
            Dash();
        }
        if (blockParasiteActive && (Input.GetMouseButtonDown(0)))
        {
            isBlockSpawned = true;
            Block();
        }

        //shadow
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit, 10f))
        {
            shadow.SetActive(true);
            shadow.transform.position = hit.point + Vector3.up * 0.01f;
            shadow.transform.forward = -hit.normal;
        }
        else
        {
            shadow.SetActive(false);
        }

    }

    private void Dash()
    {
        //grounded dash
        if ((state == "running" || state == "walking") && (isGrounded || isOnPlatform))
        {
            if(blockParasiteActive)
            {
                rb.AddForce(playerForwardTransform.transform.up * 500, ForceMode.Impulse);
            }
            rb.AddForce(playerForwardTransform.transform.forward * dashSpeed, ForceMode.Impulse);
        }
            
    }
    private void Block()
    {
        Vector3 blockPosition;
        Vector3 loweredPos = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        parasiteBlockPrefab.tag = "Ground";
        if (!jumpParasiteActive)
        {
            blockPosition = loweredPos + (playerForwardTransform.transform.forward * 5);
        } else
        {
            blockPosition = loweredPos;
        }
        
        if (isBlockSpawned)
        {
            //set prevBlockSpawned to nextSpawnedBlock
            prevSpawnedBlock = nextSpawnedBlock;
            nextSpawnedBlock = (GameObject)Instantiate(parasiteBlockPrefab, blockPosition, Quaternion.identity);
            Destroy(prevSpawnedBlock);
            //set nextBlockSpawned to instantiated block
        } else {
            nextSpawnedBlock = (GameObject)Instantiate(parasiteBlockPrefab, blockPosition, Quaternion.identity);
        }
    }

    void Move()
    {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float yVel = rb.velocity.y;
        Vector3 movement = new Vector3(horizontal,0,vertical);
        movement = cameraFollow.transform.TransformDirection(movement);
        if(dashParasiteActive) { movement = movement.normalized * speed * 2; }
        else { movement = movement.normalized * speed; }
        
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
        /*if (Input.GetButton("Fire3") && isGrounded)
        {
            speed = 20f;
            state = "running";
        }*/
        movement.y = yVel;
        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded || isOnPlatform || (numJumps > 0) )
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
                playerAnim.SetBool("Jumping", true);
            }
            
        }

        if (!isGrounded && !isOnPlatform)
        {
            state = "jumping";
        }
        Vector3 flatMovement = new Vector3(movement.x, 0, movement.z);
        float speedAnim = flatMovement.sqrMagnitude;
        playerAnim.SetFloat("Speed", speedAnim);
        if (isOnPlatform)
        {
            //Debug.Log(platform.velocity);
            movement.x += platform.velocity.x;
            movement.z += platform.velocity.z;
            //Debug.Log(movement);
        }
        rb.velocity = movement;
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, clampAngleMin, clampAngleMax);
        Quaternion localRotation = Quaternion.Euler(-rotX,rotY,0.0f);
        cameraRotate.transform.rotation = localRotation;
    }

    private void ParasiteParticleEffect()
    {
        Vector3 particlePosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject particleEffect = Instantiate(parasiteParticles, particlePosition, Quaternion.identity);
        particleEffect.GetComponent<ParticleSystem>().Play();
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
        if(collision.transform.tag == "Ground" || collision.transform.tag == "Rotating Platform")
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit))
            {
                //  Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Ground" || hit.collider.tag == "Rotating Platform")
                {
                    isGrounded = true;
                    numJumps = maxJumps;
                    playerAnim.SetBool("Jumping", false);
                }
            }
        }

        if (collision.transform.tag == "DashJelly")
        {
            Debug.Log("DASH JELLY");
            dashParasiteActive = true;
            SwitchParasites("DASH");
            parasiteParticles = parasiteParticleArray[0];
            ParasiteParticleEffect();
        }
        if (collision.transform.tag == "JumpJelly")
        {
            Debug.Log("JUMP JELLY");
            jumpParasiteActive = true;
            SwitchParasites("JUMP");
            jumpForce = 750.0f; //adjust as needed
            parasiteParticles = parasiteParticleArray[1];
            ParasiteParticleEffect();
        }
        if (collision.transform.tag == "BlockJelly")
        {
            Debug.Log("BLOCK JELLY");
            blockParasiteActive = true;
            SwitchParasites("BLOCK");
            parasiteParticles = parasiteParticleArray[2];
            ParasiteParticleEffect();
        }

        if (collision.transform.tag == "Platform")
        {
            //Debug.Log(collision.transform.tag);
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position + Vector3.up,Vector3.down,out hit))
            {
                //  Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Platform")
                {
                    isGrounded = true;
                    isOnPlatform = true;
                    platform = hit.transform.GetComponent<Rigidbody>();
                    //  Debug.Log(isGrounded);
                    numJumps = maxJumps;
                    this.transform.SetParent(hit.transform);
                    playerAnim.SetBool("Jumping", false);
                }
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isGrounded = false;
            numJumps--;
        }
        if (collision.transform.tag == "Platform")
        {
            Debug.Log("Exited Collision of Platform");
            isGrounded = false;
            isOnPlatform = false;
            platform = null;
            numJumps--;
            this.transform.SetParent(null);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down,out hit,groundDistance))
            {
                if (hit.collider.tag == "Platform" || hit.collider.tag == "Ground")
                {
                    //Debug.Log(respawnPoint);
                    respawnPoint = hit.point +  (Vector3.up * .1f);
                    isGrounded = true;
                    numJumps = maxJumps;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DeathZone")
        {
            Respawn();
        }
    }
    public void Respawn()
    {
        this.transform.position = respawnPoint;
    }
    void ParasiteTimers()
    {
        if (dashParasiteActive)
        {
            dashParasiteTimer -= Time.deltaTime;
            if (dashParasiteTimer <= 0.0f)
            {
                dashParasiteActive = false;
                dashParasiteTimer = parasiteTimer;
                if (parasiteNew == "DASH") 
                { 
                    parasiteNew = "";
                    parasiteNewImage.sprite = emptyImage;
                } else if (parasiteOld == "DASH") { 
                    parasiteOld = "";
                    parasiteOldImage.sprite = emptyImage;
                }
            }
        }
        if (jumpParasiteActive)
        {
            jumpParasiteTimer -= Time.deltaTime;
            if (jumpParasiteTimer <= 0.0f)
            {
                jumpParasiteActive = false;
                jumpForce = 400.0f;
                jumpParasiteTimer = parasiteTimer;
                if (parasiteNew == "JUMP") 
                { 
                    parasiteNew = "";
                    parasiteNewImage.sprite = emptyImage;
                } else if (parasiteOld == "JUMP") { 
                    parasiteOld = "";
                    parasiteOldImage.sprite = emptyImage;
                }
            }
        }
        if (blockParasiteActive)
        {
            blockParasiteTimer -= Time.deltaTime;
            if (blockParasiteTimer <= 0.0f)
            {
                blockParasiteActive = false;
                blockParasiteTimer = parasiteTimer;
                if (parasiteNew == "BLOCK") 
                { 
                    parasiteNew = "";
                    parasiteNewImage.sprite = emptyImage;
                } else if (parasiteOld == "BLOCK") { 
                    parasiteOld = "";
                    parasiteOldImage.sprite = emptyImage;
                }
            }
        }
    }
    void SwitchParasites(string currentParasite)
    {
        if (currentParasite == "DASH" && dashParasiteActive) 
        { 
            dashParasiteTimer = parasiteTimer; 
            parasiteOld = parasiteNew;
            parasiteNew = currentParasite;
            parasiteOldImage.sprite = parasiteNewImage.sprite;
            parasiteNewImage.sprite = parasiteImages[0];
        } else if (currentParasite == "JUMP" && jumpParasiteActive) { 
            jumpParasiteTimer = parasiteTimer;
            parasiteOld = parasiteNew;
            parasiteNew = currentParasite;
            parasiteOldImage.sprite = parasiteNewImage.sprite;
            parasiteNewImage.sprite = parasiteImages[1];
        } else if (currentParasite == "BLOCK" && blockParasiteActive) { 
            blockParasiteTimer = parasiteTimer;
            parasiteOld = parasiteNew;
            parasiteNew = currentParasite;
            parasiteOldImage.sprite = parasiteNewImage.sprite;
            parasiteNewImage.sprite = parasiteImages[2];
        } else {
            parasiteOld = parasiteNew;
            parasiteNew = currentParasite;
        }

        Debug.Log("Old and new parasite(s): " + parasiteOld + " " + parasiteNew);
    }
}
