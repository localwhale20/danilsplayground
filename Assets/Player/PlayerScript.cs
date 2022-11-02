using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float jumpHeight = 3;
    public float fallingSpeed = -2;

    public Transform groundCheckTransform;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundCheckMask;

    public float gravity = -9.83f;
    
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 lastMousePos;

    private GameObject target;

    private bool cloneKeyPressed;
    
    Vector3 GetMouseDelta(){
        return Input.mousePosition - lastMousePos;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 mouseDelta = GetMouseDelta();
        Debug.Log(mouseDelta);
        lastMousePos = Input.mousePosition;

        Debug.DrawRay(transform.position, Camera.main.transform.forward * 5, Color.blue);

        if (target)
        {
            if (Input.GetKeyDown(KeyCode.K) && !cloneKeyPressed)
            {
                var clone = Instantiate(target);
                clone.GetComponent<Rigidbody>().useGravity = true;
                clone.transform.SetParent(GameObject.Find("Environment").transform);
                clone.transform.position = target.transform.position + Vector3.up;

                cloneKeyPressed = true;
            }
            else if (Input.GetKeyUp(KeyCode.K))
            {
                cloneKeyPressed = false;
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(target);
                return;
            }

            target.transform.position = Vector3.Lerp(target.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 3, 0.1f);
            target.transform.rotation = Quaternion.Lerp(target.transform.rotation, Quaternion.Euler(Vector3.zero), 0.1f);
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;

        }

        if (Input.GetMouseButtonDown(0) && 
            Physics.Raycast(
                new Ray(transform.position, Camera.main.transform.forward * 5), 
                out RaycastHit hit) && 
            hit.collider.GetComponent<Rigidbody>() && !target)
        {
            target = hit.collider.gameObject;
            var rb = target.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;
        }

        if (Input.GetMouseButtonUp(0) && target)
        {
            var rb = target.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.freezeRotation = false;
            rb.AddForce(mouseDelta, ForceMode.Impulse);
            target = null;
        }

        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckDistance, groundCheckMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = fallingSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 pos = transform.right * x + transform.forward * z;
        
        controller.Move(pos * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }
    
}
