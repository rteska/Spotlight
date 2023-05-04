using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class switchesManager : MonoBehaviour
{
    
    public GameObject gameSwitch;
    public GameObject[] allSwitches;
    public GameObject[] allDoors; //Door left is 0, door right is 1
    public Material[] switchMaterials;
    public TextMeshProUGUI switchesText;
    public TextMeshProUGUI doorOpen;
    private int[] switchCheck;
    private int currentRoom;
    private Vector3[] allDoorsStart = {new Vector3(0, 0, 0), new Vector3(0, 0, 0)};
    private Vector3 goalForLeft;
    private Vector3 goalForRight;
    float currentDistanceLeft = 0f;
    float currentDistanceRight = 0f;

    // Start is called before the first frame update
    void Start()
    {
        doorOpen.enabled = false;
        switchCheck = new int[allSwitches.Length];
        for (int i = 0; i < allSwitches.Length; i++)
        {
            switchCheck[i] = 0;
        }

        //currentRoom = 1;
        for (int i = 0; i < allDoors.Length; i++)
        {
            Vector3 startPosition = allDoors[i].transform.position;
            allDoorsStart[i] = startPosition;
        }

        goalForLeft = new Vector3(0f, 0f, 0f);
        goalForRight = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        int tempCheck = 0;
        int numOfSwitches = allSwitches.Length;

        //Check if any switches were triggered
        foreach (int checking in switchCheck)
        {
            if (checking == 1)
            {
                tempCheck++;
            }
        }

        switchesText.text = "Switches Active: " + tempCheck;

        //If the switches aren't triggered, doors stay closed
        if (tempCheck < numOfSwitches)
        {
            closeAllDoors();
        }

        //If both switches are triggered
        if (tempCheck == numOfSwitches)
        {
            doorOpen.enabled = true;
            Vector3[] startingPositions = {allDoorsStart[0], allDoorsStart[1]};
            openDoors(allDoors[0], allDoors[1], startingPositions);

        }
        else
        {
            doorOpen.enabled = false;
        }

    }

    public void switchColorOn(GameObject foundSwitch)
    {
        ///GameObject tempFoundSwitch = GameObject.FindWithTag(foundSwitch.collider.tag);
        
        foundSwitch.GetComponent<MeshRenderer>().material = switchMaterials[1];

        //Hardcoded for the sake of prototype

        //foundSwitch.tag
        //Checking to see which switches were hit
        if (foundSwitch.tag == "Switch1")
        {
            switchCheck[0] = 1;
        }
        else //It was switch 2 that was hit
        {
            switchCheck[1] = 1;
        }
        
        //gameSwitch.GetComponent<MeshRenderer>().material = switchMaterials[1];

    }

    public void switchColorOff(GameObject foundSwitch)
    {
        //GameObject tempFoundSwitch = GameObject.FindWithTag(foundSwitch.collider.tag);

        foundSwitch.GetComponent<MeshRenderer>().material = switchMaterials[0];

        //Reset switchChecks
        switchCheck[0] = 0;
        switchCheck[1] = 0;


    }
    
    public void openDoors(GameObject doorLeft, GameObject doorRight, Vector3[] positions)
    {

        goalForLeft = new Vector3(positions[0].x - 1.176f, positions[0].y, positions[0].z); //13.76
        goalForRight = new Vector3(positions[1].x + 1.14f, positions[1].y, positions[1].z); //17.196
        currentDistanceLeft = Vector3.Distance(doorLeft.GetComponent<Transform>().position, goalForLeft);
        currentDistanceRight = Vector3.Distance(doorRight.GetComponent<Transform>().position, goalForRight);

        //Move the left door
        if (currentDistanceLeft > 0.1f)
        {
            doorLeft.GetComponent<Transform>().position = Vector3.Lerp(doorLeft.GetComponent<Transform>().position, goalForLeft, 5.0f * Time.deltaTime);
        }

        //Move the right door
        if (currentDistanceRight > 0.1f)
        {
            doorRight.GetComponent<Transform>().position = Vector3.Lerp(doorRight.GetComponent<Transform>().position, goalForRight, 5.0f * Time.deltaTime);
        }
        
        
       

    }

    //Method to call when the room changes
    public void currentRoomChange(int newRoom)
    {
        currentRoom = newRoom;
    }

    public void closeAllDoors()
    {
        for (int i = 0; i < allDoors.Length; i++)
        {
            allDoors[i].GetComponent<Transform>().position = allDoorsStart[i];
        }
        
    }
}
