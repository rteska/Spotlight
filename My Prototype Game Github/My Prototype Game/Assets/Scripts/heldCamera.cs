using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class heldCamera : MonoBehaviour
{
    public Transform mainCameraTransform; //This is the camera the player can move with their mouse
    public Transform cameraTransform; //This is the transform of the held camera
    public Transform cameraSpot; //This is the point on the player where the held camera sits
    public Transform player; //This is the player transform itself

    public Camera mainCamera; //Camera that is on the player's head
    public Camera thrownCamera; //Camera that gets thrown

    private GameObject roomManager;
    private GameObject[] allSwitches;
    private GameObject playerObject;

    [SerializeField] private Rigidbody rb; //Thrown camera's rigidbody
    //private string[] switchTags = {"Switch1", "Switch2"};

    public TextMeshProUGUI inNextRoom;

    private int currentRoom;
    private bool isThrown;
    private bool onWall;
    //private bool foundHit;

    //private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        isThrown = false;
        onWall = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.useGravity = false;
        mainCamera.enabled = true;
        thrownCamera.enabled = false;
        currentRoom = 1;
        inNextRoom.enabled = false;
        playerObject = GameObject.FindWithTag("Player");
        //foundHit = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        
        switch (currentRoom)
        {
            case 1:
                roomManager = GameObject.FindWithTag("Room1Manager");
                roomManager.GetComponent<switchesManager>().enabled = true;
                allSwitches = roomManager.GetComponent<switchesManager>().allSwitches;              
                break;
            case 2:
                roomManager = GameObject.FindWithTag("Room2Manager");
                roomManager.GetComponent<switchesManager>().enabled = true;
                allSwitches = roomManager.GetComponent<switchesManager>().allSwitches;
                break;
            case 3:
                roomManager = GameObject.FindWithTag("Room3Manager");
                roomManager.GetComponent<switchesManager>().enabled = true;
                allSwitches = roomManager.GetComponent<switchesManager>().allSwitches;
                break;
            default:
                break;
        }
        

        //If the camera is in the air
        if (isThrown)
        {
            throwCamera();
        }

        //If the camera is on a wall
        if (onWall)
        {
            waitOnWall();
            
            //If the player wants the camera to return back
            if (Input.GetKeyDown(KeyCode.R))
            {
                inNextRoom.enabled = false;
                returnToPlayer();               
            }
            else
            {
                return;
            }
        }
        
        //If the camera is still attatched to the player
        if (cameraTransform.parent == cameraSpot)
        {
            rb.detectCollisions = false;
        }
        
        //If the player wants to throw the camera
        if (Input.GetKeyDown(KeyCode.T))
        {
            isThrown = true;
            cameraTransform.parent = null;
            rb.detectCollisions = true;
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        
    }

    private void FixedUpdate()
    {
        if (onWall)
        {
            waitOnWall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown && collision.gameObject.tag == "Wall")
        {
            isThrown = false;
            onWall = true;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
            cameraTransform.Rotate(0f, 180f, 0f);
            mainCamera.enabled = false;
            thrownCamera.enabled = true;
            
        }
    }

    private void throwCamera()
    {
        //Parts to work with: Camera's current x position, the player's current y rotation and the player's current y position
        
        Vector3 movementVector = Vector3.zero;

        movementVector = (mainCameraTransform.forward * 5f) + (mainCameraTransform.up * 1.1f);

        rb.AddForce(movementVector.normalized, ForceMode.Impulse);


    }

    public void roomChange()
    {
        for (int i = 0; i < 100; i++); //Delay


        if (currentRoom != playerObject.GetComponent<player>().currentPlayerRoom()) //If the player has moved out of the room that the camera is in
        {
            roomManager.GetComponent<switchesManager>().closeAllDoors();
            roomManager.GetComponent<switchesManager>().enabled = false;
            currentRoom++;
        }

        inNextRoom.enabled = true;
        
        
    }

    public void returnToPlayer()
    {
        isThrown = false;
        cameraTransform.parent = cameraSpot;
        cameraTransform.position = cameraSpot.position;
        cameraTransform.rotation = cameraSpot.rotation;
        rb.useGravity = false;
        rb.isKinematic = true;
        thrownCamera.enabled = false;
        mainCamera.enabled = true;
        onWall = false;

        foreach (GameObject returnSwitch in allSwitches)
        {
            roomManager.GetComponent<switchesManager>().switchColorOff(returnSwitch);
        }
    }


    /*
     * 
     * This section detects if the camera can "see" a switch
     * 
     * */

    private void waitOnWall()
    {
        //LayerMask canSee = LayerMask.GetMask("Switch");


        //Using Frustum planes
        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(thrownCamera);

        foreach (GameObject switchCheck in allSwitches)
        {
            Collider switchCollider = switchCheck.GetComponent<Collider>();

            if (GeometryUtility.TestPlanesAABB(cameraPlanes, switchCollider.bounds))
            {
                //Debug.Log("Made it here");
                roomManager.GetComponent<switchesManager>().switchColorOn(switchCheck);
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (GameObject returnSwitch in allSwitches)
            {
                roomManager.GetComponent<switchesManager>().switchColorOff(returnSwitch);
            }
            
        }


        //All rays

        /*

        //Center
        Vector3 cameraHitMid = new Vector3(600, 290, 0);
        Ray cameraRayMid = thrownCamera.ScreenPointToRay(cameraHitMid);
        RaycastHit[] allHitsMid = Physics.RaycastAll(cameraRayMid, 50f);
        Debug.DrawRay(cameraRayMid.origin, cameraRayMid.direction * 50f, Color.red);

        //Middle Left
        Vector3 cameraHitMidLeft = new Vector3(500, 290, 0);
        Ray cameraRayMidLeft = thrownCamera.ScreenPointToRay(cameraHitMidLeft);
        RaycastHit[] allHitsMidLeft = Physics.RaycastAll(cameraRayMidLeft, 50f);
        Debug.DrawRay(cameraRayMidLeft.origin, cameraRayMidLeft.direction * 50f, Color.red);

        //Middle Right
        Vector3 cameraHitMidRight = new Vector3(700, 290, 0);
        Ray cameraRayMidRight = thrownCamera.ScreenPointToRay(cameraHitMidRight);
        RaycastHit[] allHitsMidRight = Physics.RaycastAll(cameraRayMidRight, 50f);
        Debug.DrawRay(cameraRayMidRight.origin, cameraRayMidRight.direction * 50f, Color.red);

        //Low Left
        Vector3 cameraHitLowLeft = new Vector3(500, 190, 0);
        Ray cameraRayLowLeft = thrownCamera.ScreenPointToRay(cameraHitLowLeft);
        RaycastHit[] allHitsLowLeft = Physics.RaycastAll(cameraRayLowLeft, 50f);
        Debug.DrawRay(cameraRayLowLeft.origin, cameraRayLowLeft.direction * 50f, Color.red);

        //Low Middle
        Vector3 cameraHitLowMid = new Vector3(600, 190, 0);
        Ray cameraRayLowMiddle = thrownCamera.ScreenPointToRay(cameraHitLowMid);
        RaycastHit[] allHitsLowMiddle = Physics.RaycastAll(cameraRayLowMiddle, 50f);
        Debug.DrawRay(cameraRayLowMiddle.origin, cameraRayLowMiddle.direction * 50f, Color.red);

        //Low Right
        Vector3 cameraHitLowRight = new Vector3(700, 190, 0);
        Ray cameraRayLowRight = thrownCamera.ScreenPointToRay(cameraHitLowRight);
        RaycastHit[] allHitsLowRight = Physics.RaycastAll(cameraRayLowRight, 50f);
        Debug.DrawRay(cameraRayLowRight.origin, cameraRayLowRight.direction * 50f, Color.red);

        //High Left
        Vector3 cameraHitHighLeft = new Vector3(500, 390, 0);
        Ray cameraRayHighLeft = thrownCamera.ScreenPointToRay(cameraHitHighLeft);
        RaycastHit[] allHitsHighLeft = Physics.RaycastAll(cameraRayHighLeft);
        Debug.DrawRay(cameraRayHighLeft.origin, cameraRayHighLeft.direction * 50f, Color.red);

        //High Middle
        Vector3 cameraHitHighMid = new Vector3(600, 390, 0);
        Ray cameraRayHighMiddle = thrownCamera.ScreenPointToRay(cameraHitHighMid);
        RaycastHit[] allHitsHighMid = Physics.RaycastAll(cameraRayHighMiddle, 50f);
        Debug.DrawRay(cameraRayHighMiddle.origin, cameraRayHighMiddle.direction * 50f, Color.red);

        //High Right
        Vector3 cameraHitHighRight = new Vector3(700, 390, 0);
        Ray cameraRayHighRight = thrownCamera.ScreenPointToRay(cameraHitHighRight);
        RaycastHit[] allHitsHighRight = Physics.RaycastAll(cameraRayHighRight, 50f);
        Debug.DrawRay(cameraRayHighRight.origin, cameraRayHighRight.direction * 50f, Color.red);


        float offsetAmount = 0.9f;

        //Physics.RaycastAll
        Vector3 xzPositive = new Vector3(offsetAmount, 0, offsetAmount);
        Vector3 xzNegative = new Vector3(-offsetAmount, 0, -offsetAmount);
        Vector3 yPositive = new Vector3(0, offsetAmount, 0);
        Vector3 yNegative = new Vector3(0, -offsetAmount, 0);

        Vector3 xzyPositive = new Vector3(offsetAmount, offsetAmount, offsetAmount);
        Vector3 xzNegyPos = new Vector3(-offsetAmount, offsetAmount, -offsetAmount);
        Vector3 xzPosyNeg = new Vector3(offsetAmount, -offsetAmount, offsetAmount);
        Vector3 xzyNegative = new Vector3(-offsetAmount, -offsetAmount, -offsetAmount);
        */

        /*
         * Up, down, left, right and center rays
         * 
         */
        //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 50f, Color.red);
        /*
        Debug.DrawRay(cameraTransform.position + xzPositive, cameraTransform.forward * 50f, Color.red);
        Debug.DrawRay(cameraTransform.position + xzNegative, cameraTransform.forward * 50f, Color.red);
        Debug.DrawRay(cameraTransform.position + yPositive, cameraTransform.forward * 50f, Color.red);
        Debug.DrawRay(cameraTransform.position + yNegative, cameraTransform.forward * 50f, Color.red);
        

        /*
         * Corner raycasts
         * 
         
        Debug.DrawRay(cameraTransform.position + xzyPositive, cameraTransform.forward * 50f, Color.red);
        Debug.DrawRay(cameraTransform.position + xzNegyPos, cameraTransform.forward * 50f, Color.red);
        Debug.DrawRay(cameraTransform.position + xzPosyNeg, cameraTransform.forward * 50f, Color.red);
        Debug.DrawRay(cameraTransform.position + xzyNegative, cameraTransform.forward * 50f, Color.red);
        

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + xzPositive, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + xzNegative, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + yPositive, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + yNegative, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + xzyPositive, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + xzNegyPos, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + xzPosyNeg, cameraTransform.forward, out hit, 50f, canSee) ||
            Physics.Raycast(cameraTransform.position + xzyNegative, cameraTransform.forward, out hit, 50f, canSee)
            )
        {

            foundHit = true;
            for (int i = 0; i < switchTags.Length; i++)
            {
                if (hit.collider.tag == switchTags[i])
                {
                    //Debug.Log("Detect something");
                    roomManager.GetComponent<switchesManager>().switchColorOn(hit);
                    break;
                }
            }
            
            
            
        }
        */


    }


    

}
