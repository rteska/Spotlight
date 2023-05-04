using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserMover : MonoBehaviour
{
    public GameObject laserHolder;

    private Vector3 startingPosition = new Vector3(11.02f, 0.2f, -14.151f);
    private Vector3 desiredPosition = new Vector3(0.4f, 0.2f, -14.151f);
    private float currentDistance;
    private bool switchDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        laserHolder.transform.position = startingPosition;
        switchDirection = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Switch direction: if false, go towards back wall. If true, go towards door side
        if (!switchDirection) //Head towards back wall
        {
            currentDistance = Vector3.Distance(laserHolder.transform.position, desiredPosition);
            laserHolder.transform.position = Vector3.Lerp(laserHolder.transform.position, desiredPosition, 1.1f * Time.deltaTime);
        }
        else //Head towards door wall
        {
            currentDistance = Vector3.Distance(laserHolder.transform.position, startingPosition);
            laserHolder.transform.position = Vector3.Lerp(laserHolder.transform.position, startingPosition, 1.1f * Time.deltaTime);
        }

        if (currentDistance <= 0.5f) //Swap directions
        {
            switchDirection = !switchDirection;
        }
    }
}
