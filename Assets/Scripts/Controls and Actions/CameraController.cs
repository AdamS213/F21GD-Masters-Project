using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    
    private bool isLerping = false;
    private float timer = 0.0f;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    private PlayerControls playerControls;
    private InputAction moveCamera;
    private InputAction rotateCamera;
    private Vector3 movement;
    private Vector3 newPostion;
    private Quaternion newRotation;

    private void Awake()
    {
        playerControls = new PlayerControls();
        newPostion = transform.position;
        movement = Vector3.zero;
        newRotation = transform.rotation;
    }
    private void OnEnable()
    {
        moveCamera = playerControls.Camera.move;
        moveCamera.Enable();
        rotateCamera = playerControls.Camera.rotate;
        rotateCamera.Enable();
    }
    private void OnDisable()
    {
        moveCamera.Disable();
        rotateCamera.Disable();
    }

    

    

    private void FixedUpdate()
    {
        handleMovement();
        handleRotation();
    }

    void handleMovement()
    {
        //receive movement from input
        Vector2 temp = moveCamera.ReadValue<Vector2>();
        
        //convert into Vector3 so that it works properly with our worldspace
        movement = new Vector3(temp.x,0,temp.y);
        //applies the cameras current rotation angle to the movement vector, this makes the camera behave intuitively 
        Matrix4x4 movementAdjustment = Matrix4x4.Rotate(transform.rotation);
        movement = movementAdjustment.MultiplyPoint3x4(movement);
        //calculate new postition
        newPostion  += (movement * -movementSpeed);
        //lerp to new position
        transform.position = Vector3.Lerp(transform.position, newPostion, Time.fixedDeltaTime * movementTime);

    }

    void handleRotation()
    {
        //receive rotation from input
        float rotateAmount = rotateCamera.ReadValue<float>();
        newRotation *= Quaternion.Euler(Vector3.up * rotateAmount);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.fixedDeltaTime * movementTime);

    }
}
