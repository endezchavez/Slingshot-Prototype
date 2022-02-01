using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class WallSlidingState : MonoBehaviour
{
    public LayerMask whatIsWall;

    private Motor motor;

    private RunningState runningState;
    private FallingState fallingState;
    private SlidingState slidingState;
    private WallJumpingState wallJumpingState;

    public float minVelToWallSlide = 5f;
    public float wallSlideSpeedMultiplier = 2f;
    public float wallSlideUpForce = 5f;
    public float wallSlideExitSpeedMultiplier = 0.8f;

    private bool isWallSlidingRight = false;

    private Vector3 directionToWall;
    private Vector3 vel;

    private float initialMag;
    private Vector3 initialVel;
    private Vector3 initialVelXZ;

    private bool isWallSliding = false;
    private bool hasSetWallForces = false;

    private void Awake()
    {
        motor = GetComponent<Motor>();

        runningState = GetComponent<RunningState>();
        fallingState = GetComponent<FallingState>();
        slidingState = GetComponent<SlidingState>();
        wallJumpingState = GetComponent<WallJumpingState>();
    }

    private void OnEnable()
    {
        if (isWallSlidingRight)
        {
            directionToWall = transform.right;
        }
        else
        {
            directionToWall = -transform.right;
        }

        initialVel = motor.GetVelocity();
        initialVelXZ = new Vector3(initialVel.x, 0f, initialVel.z);
        initialMag = initialVelXZ.magnitude;

        isWallSliding = false;
        hasSetWallForces = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isWallSliding)
        {
            motor.MultiplyVelocity(wallSlideSpeedMultiplier);
            isWallSliding = true;
        }

        //Apply Force Into Wall
        if (!hasSetWallForces)
        {
            motor.SetVelocity(motor.GetVelocity() + (directionToWall * 0.5f) + (transform.up * wallSlideUpForce));
            hasSetWallForces = true;
        }

        if (motor.controller.isGrounded)
        {
            runningState.enabled = true;
            this.enabled = false;
        }

        //Transition to falling if end of wall is reached
        if (!WallCheckInOriginalDirection())
        {
            fallingState.enabled = true;
            this.enabled = false;
        }

        //Transition to falling if player moves away from wall
        if (isWallSlidingRight && InputManager.Instance.GetPlayerMovement().x < 0 || !isWallSlidingRight && InputManager.Instance.GetPlayerMovement().x > 0)
        {
            motor.MultiplyVelocity(wallSlideExitSpeedMultiplier);
            fallingState.enabled = true;
            this.enabled = false;
        }

        
        //Transition to jump and apply force off wall
        if (InputManager.Instance.PlayerJumpedThisFrame())
        {
            wallJumpingState.enabled = true;
            wallJumpingState.SetDirectionOffWall(directionToWall);
            this.enabled = false;
        }
        

        //Transition to slide
        if (InputManager.Instance.PlayerToggledCrouch())
        {
            if (slidingState.IsPlayerLowEnoughToSlide() && motor.GetCurrentMagnitude() >= slidingState.minVelToSlide)
            {
                slidingState.enabled = true;
                this.enabled = false;
            }
        }
    }

    public void SetIsWallSlidingRight(bool i)
    {
        isWallSlidingRight = i;
    }

    public bool WallCheckRight()
    {
        return Physics.Raycast(transform.position, transform.right, 1f, whatIsWall);
    }

    public bool WallCheckLeft()
    {
        return Physics.Raycast(transform.position, -transform.right, 1f, whatIsWall);
    }

    public bool WallCheckInOriginalDirection()
    {
        return Physics.Raycast(transform.position, directionToWall, 1f, whatIsWall);

    }


}
