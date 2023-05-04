using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform playerTransform;
    public float mouseSensitivity;
    float cameraVerticalRotation = 0f;

    private bool isThrown;

    
    // Start is called before the first frame update
    void Start()
    {
        //cameraTransform.position = new Vector3(1.434f, 2.009f, -46.925f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isThrown = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * This section allows the player to move the camera with their mouse
         */
        if (isThrown)
        {

            if (Input.GetKey(KeyCode.R))
            {
                isThrown = false;
            }
        }
        else
        {
            //Here is just vertically with the camera
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            //rotateY += mouseX;
            cameraVerticalRotation -= mouseY;

            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

            transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

            //Here it is horizontally with the camera and player
            playerTransform.Rotate(Vector3.up * mouseX);

            if (Input.GetKey(KeyCode.T))
            {
                isThrown = true;
            }
        }


    }

    public void playerFell()
    {
        isThrown = false;
    }


}
