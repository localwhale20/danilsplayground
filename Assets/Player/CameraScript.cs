using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // <-- rotation variables -->
    public float cameraSensitivity = 100f; // camera's sensitivity
    public Transform playerTransform;      // player's transform used to rotate player object

    // <-- private etc. -->
    private float xRotation = 0f;          // current rotation by x
    
    // Start is called before the first frame update
    void Start()
    {
        // lock player's cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get axis and multiply them by sensitivity & time
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        
        // change rotation which will be set in camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90); // clamp it in angles (straight top/bottom)
        
        // set camera rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // set player rotation
        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
