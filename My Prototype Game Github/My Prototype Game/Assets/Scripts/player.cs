using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class player : MonoBehaviour
{
    //SceneManager.LoadScene();
    
    public Transform playerTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    public Transform cameraTransform;
    public Transform cameraSpot;
    public TextMeshProUGUI inNextRoom;
    private GameObject roomManager;
    private GameObject heldCameraManager;
    private GameObject mainCameraManager;

    public float playerSpeed;
    public float jumpSpeed;
    public float rotationSpeed;

    private float groundHeight;
    private int currentRoom;
    private bool isJumping;
    private bool isThrown;
    
    // Start is called before the first frame update
    void Start()
    {
        groundHeight = 0.5f;
        isJumping = false;
        isThrown = false;
        currentRoom = 1;
        playerTransform.position = new Vector3(1.5f, groundHeight, -47f);
        rb.freezeRotation = true;
        //roomManager = GameObject.FindWithTag("Room1Manager");
        heldCameraManager = GameObject.FindWithTag("Camera");
        mainCameraManager = GameObject.FindWithTag("MainCamera");
    }

    private void FixedUpdate()
    {
        if (!isThrown) //Camera is not thrown
        {
            firstPersonMovement();
        }
        else //Camera is thrown
        {
            thirdPersonMovement();
        }
    }

    /*
     * Old movement code
     * 
    movementVector = new Vector3(xMove, currentVelocity.y, zMove);

    //rb.velocity = new Vector3(xMove, rb.velocity.y, zMove) * playerSpeed;

    if (movementVector != Vector3.zero)
    {
        rb.velocity = movementVector;
    }
    */

    // Update is called once per frame
    void Update()
    {
        
        //The player wants to jump and they are on the ground
        if (Input.GetKey(KeyCode.Space) && !isJumping)
        {
            //Debug.Break();
            GoJump();
        }

        //The player wants to throw the camera
        if (Input.GetKey(KeyCode.T))
        {
            isThrown = true;
            
        }

        //The player wants to retreive the camera
        if (Input.GetKey(KeyCode.R))
        {
            isThrown = false;
        }

        //Animator control
        animator.SetBool("isThrown", isThrown);
        



    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isJumping && collision.gameObject.tag == "Ground") //Made contact with normal ground
        {
            isJumping = false;
        }

        if (collision.gameObject.tag == "Void") //Made contact with "death" void
        {
            playerTransform.position = new Vector3(5.996f, playerTransform.position.y, -18.672f);
            isThrown = false;
            mainCameraManager.GetComponent<camera>().playerFell();
            heldCameraManager.GetComponent<heldCamera>().returnToPlayer();
        }
        
        if (collision.gameObject.tag == "Laser")
        {
            playerTransform.position = new Vector3(14.895f, playerTransform.position.y, -6.88f);
            isThrown = false;
            mainCameraManager.GetComponent<camera>().playerFell();
            heldCameraManager.GetComponent<heldCamera>().returnToPlayer();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string currentRoomTag = other.tag;

        switch (currentRoomTag)
        {
            case "Room1":
                currentRoom = 1;
                roomManager = GameObject.FindWithTag("Room1Manager");                
                break;
            case "Room2":
                currentRoom = 2;
                roomManager = GameObject.FindWithTag("Room2Manager");               
                heldCameraManager.GetComponent<heldCamera>().roomChange();
                break;
            case "Room3":
                currentRoom = 3;
                roomManager = GameObject.FindWithTag("Room3Manager");
                heldCameraManager.GetComponent<heldCamera>().roomChange();
                break;
            case "Win":
                SceneManager.LoadScene("WinScreen");
                break;
            default:
                break;
        }
    }

    private void GoJump()
    {
        isJumping = true;
        rb.velocity += Vector3.up * jumpSpeed;
    }

    private void firstPersonMovement()
    {
        //Another try at moving the player
        Vector3 movementVector = Vector3.zero;
        Vector3 currentVelocity = rb.velocity;

        float xMove = Input.GetAxisRaw("Horizontal") * playerSpeed;
        float zMove = Input.GetAxisRaw("Vertical") * playerSpeed;

        movementVector = (playerTransform.forward * zMove) + (playerTransform.right * xMove);

        rb.AddForce(movementVector.normalized * playerSpeed * 10f, ForceMode.Force);

        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    private void thirdPersonMovement()
    {
        Vector3 movementVector = Vector3.zero;
        Vector3 currentVelocity = rb.velocity;
        Vector3 inputDirection;

        float h = Input.GetAxis("Horizontal") * playerSpeed;
        float v = Input.GetAxis("Vertical") * playerSpeed;

        inputDirection = new Vector3(h, 0, v) + playerTransform.position;

        if (h != 0 || v != 0)
        {
            Vector3 toFaceDirection = inputDirection - playerTransform.position;

            Quaternion targetRotation = Quaternion.LookRotation(toFaceDirection);
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        movementVector = new Vector3(h, currentVelocity.y, v);

        if (movementVector != Vector3.zero)
        {
            rb.velocity = movementVector;
        }

        animator.SetFloat("Speed", rb.velocity.magnitude);

        //rb.AddForce(movementVector.normalized * playerSpeed * 10f, ForceMode.Force);

    }

    public int currentPlayerRoom()
    {
        return currentRoom;
    }
}
