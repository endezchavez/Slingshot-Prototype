using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Motor : MonoBehaviour
{
    public LayerMask whatIsWall;
    public CharacterController controller;

    public float gravity = -15f;
    public float drag = 1f;
    public float airDrag = 5f;

    private float currentDrag;
    private float currentGravity;

    [Header("Player Settings")]
    
   
    

    [Header("Transition Settings")]
    public float crouchTransitionSpeed = 1f;


    private Vector3 playerVelocity;
    private Vector3 movementDir;
    private Vector2 playerInput;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        currentGravity = gravity;
        currentDrag = drag;
    }

    private void Update()
    {
        WallCheckRight();

    }


    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0f, cameraTransform.eulerAngles.y, 0f);

        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        
        playerVelocity.y += currentGravity * Time.deltaTime;
        playerVelocity.x /= 1 + currentDrag * Time.deltaTime;
        playerVelocity.z /= 1 + currentDrag * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void AddVelocity(Vector3 dir, float speed)
    {
        playerVelocity += dir * speed;
    }

    public void SetVelocity(Vector3 dir, float speed)
    {
        playerVelocity = dir * speed;
    }

    public void SetVelocity(Vector3 vel)
    {
        playerVelocity = vel;
    }

    public Vector3 GetVelocity()
    {
        return playerVelocity;
    }

    public void MultiplyVelocity(float speedMultiplier)
    {
        playerVelocity.x *= speedMultiplier;
        playerVelocity.z *= speedMultiplier;
    }

    public void SetGravity(float g)
    {
        currentGravity = g;
    }

    public float GetCurrentMagnitude()
    {
        Vector3 currentVelXZ = new Vector3(playerVelocity.x, 0f, playerVelocity.z);
        return currentVelXZ.magnitude;
    }
    

    public Vector3 GetInputMoveDir()
    {
        playerInput = InputManager.Instance.GetPlayerMovement();
        movementDir = new Vector3(playerInput.x, 0f, playerInput.y).normalized;
        movementDir = cameraTransform.forward * movementDir.z + cameraTransform.right * movementDir.x;
        movementDir.y = 0f;
        return movementDir;
    }

    bool WallCheckRight()
    {
        //Debug.DrawRay(transform.position, transform.right * 0.8f, Color.green, 0.1f);
        return Physics.Raycast(transform.position, transform.right, 0.8f, whatIsWall);
    }


}
