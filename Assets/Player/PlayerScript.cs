using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // <-- movement variables -->
    public CharacterController controller; // used to move/apply velocity
    public float speed = 12f;              // speed of player movement
    
    // <-- physic/jump variables -->
    public Transform groundCheckTransform;   // transform used to get position of check sphere
    public float groundCheckDistance = 0.4f; // radius of check sphere
    public LayerMask groundCheckMask;        // mask used to check ground
    public float jumpHeight = 3;             // height of player's jump
    public float fallingSpeed = -2;          // player's fall speed
    public float gravity = -9.83f;           // gravity

    // <-- private etc. -->
    private float objectDist = 3;  

    private bool isGrounded;      // contains if player on ground
    private Vector3 velocity;     // current player's velocity (used in physics)

    private GameObject target;    // current object which player is holding now

    private bool cloneKeyPressed; // contains if user pressed clone key (K key)

    // Update is called once per frame
    void Update()
    {
        // if player holding object now
        if (target)
        {
            // if player wants to clone current holding object
            if (Input.GetKeyDown(KeyCode.K) && !cloneKeyPressed)
            {
                var clone = Instantiate(target); // create clone
                clone.transform.SetParent(target.transform.parent); // set parent same as current holding object
                clone.transform.position = target.transform.position + Vector3.up; // place clone on current holding object 

                cloneKeyPressed = true; // set that key now holding
            }
            // reset clone key hold state
            else if (Input.GetKeyUp(KeyCode.K))
            {
                cloneKeyPressed = false;
            }

            // if player wants to delete current holding object
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(target); // delete it
                return; // skip to next frame to prevent exception because of disposed object
            }

            // if player scrolls then distance between player and object is changes
            objectDist += Input.mouseScrollDelta.y;
            objectDist = Mathf.Clamp(objectDist, 1.75f, 8f); // clamp distance

            // make smooooth object movement using lerps
            // move object in front of player with distance set by 'objectDist'
            target.transform.position = Vector3.Lerp(
                target.transform.position, // current pos
                Camera.main.transform.position + Camera.main.transform.forward * objectDist, // future pos
                0.1f // lerp speed
            );
            // keep object's rotation same (euler: 0, 0, 0)
            target.transform.rotation = Quaternion.Lerp(
                target.transform.rotation, // current rotation
                Quaternion.Euler(Vector3.zero), // future rotation
                0.1f // lerp speed
            );

            // reset velocity to make object not drop so hard
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        // if raycast is touching some rigidbody and no object is holding now
        if (Physics.Raycast(
                new Ray(transform.position, Camera.main.transform.forward * 5), 
                out RaycastHit hit) && 
            hit.collider.GetComponent<Rigidbody>() && !target)
        {
            // if player hold down left mouse button
            if (Input.GetMouseButtonDown(0)){
                target = hit.collider.gameObject; // set holding object
            }

            // TODO: rotate cursor when is player hovering at rigidbody
            // ...
        }

        // if player not holding down left mouse button and object is holding now
        if (Input.GetMouseButtonUp(0) && target)
        {
            objectDist = 3; // reset object distance
            target = null; // reset holding object
        }

        // check if player now stands on something
        isGrounded = Physics.CheckSphere(
            groundCheckTransform.position, // check sphere position
            groundCheckDistance,           // check sphere radius
            groundCheckMask                // check sphere mask
        );

        // if player stands on something but velocity is less then zero
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = fallingSpeed; // make smooth falling
        }

        // get axis used to move player
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // get new position
        Vector3 pos = transform.right * x + transform.forward * z;
        
        // move player using created position multiplied by speed and time
        controller.Move(pos * speed * Time.deltaTime);

        // if player pressing jump key and it grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // make player jump
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        // apply gravity to velocity
        velocity.y += gravity * Time.deltaTime;
        
        // apply velocity on player
        controller.Move(velocity * Time.deltaTime);
    }
}
