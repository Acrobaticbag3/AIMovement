// The basic movement part was done by Kevin
// The advanced movement vas done by Johannes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private Rigidbody rb;
    public float speed = 4f;
    public float sprintSpeed = 9f;

    public float jumpForce = 400f;
    public bool isGrounded;

    private float vertical;
    private float horizontal;
   
    void Start() {
        rb = GetComponent<Rigidbody>();
    }  

    void FixedUpdate() {
        float verticalPlayerInput = Input.GetAxisRaw(axisName: "Vertical"); //Gets vertical input
        float horizontalPlayerInput = Input.GetAxisRaw(axisName: "Horizontal"); //Gets horizontal input

        Vector3 forward = transform.InverseTransformVector (vector: Camera.main.transform.forward);
        Vector3 right = transform.InverseTransformVector (vector: Camera.main.transform.right);
        forward.y = 0;
        right.y = 0;

        forward = forward.normalized;
        right = right.normalized;

        float speed = this.speed;
        Vector3 forwardRelativeVerticalInput = verticalPlayerInput * forward * Time.fixedDeltaTime;
        Vector3 rightRelativeHorizontalInput = horizontalPlayerInput * right * Time.fixedDeltaTime;

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        if(Input.GetKey(key: KeyCode.LeftShift)) { //Sprinting 
            speed = sprintSpeed;
        }
        transform.Translate(translation: cameraRelativeMovement * speed, relativeTo: Space.World);

        if (Input.GetButtonDown(buttonName: "Jump") && isGrounded) {
            rb.AddForce(force: Vector3.up * jumpForce);
        }
    
    }

    //Should we be grounded?
    void OnCollisionEnter(Collision collision) {  // Yes
        if(collision.gameObject.tag == ("Ground")) {
            isGrounded = true;
        }
    }
    
    void OnCollisionExit(Collision collision) { // no
        if(collision.gameObject.tag == ("Ground")) {
            isGrounded = false;
        }
    }
}
