using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class WallJumpingState : MonoBehaviour
{
    public float wallJumpForce;

    private Motor motor;

    private WallSlidingState wallSlidingState;
    private RunningState runningState;
    private FallingState fallingState;

    private Vector3 directionOffWall;

    private bool hasWallJumped = false;

    private void Awake()
    {
        motor = GetComponent<Motor>();

        wallSlidingState = GetComponent<WallSlidingState>();
        runningState = GetComponent<RunningState>();
        fallingState = GetComponent<FallingState>();
    }

    private void OnEnable()
    {
        hasWallJumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasWallJumped)
        {
            Vector3 vel = new Vector3(-directionOffWall.x * wallJumpForce, motor.GetVelocity().y, -directionOffWall.z * wallJumpForce);
            //motor.SetVelocity(vel.normalized, vel.magnitude);
            motor.AddVelocity(-directionOffWall, wallJumpForce);
            hasWallJumped = true;
        }

        if (motor.GetCurrentMagnitude() >= wallSlidingState.minVelToWallSlide)
        {
            if (wallSlidingState.WallCheckRight())
            {
                wallSlidingState.enabled = true;
                wallSlidingState.SetIsWallSlidingRight(true);
                this.enabled = true;
            }

            if (wallSlidingState.WallCheckLeft())
            {
                wallSlidingState.enabled = true;
                wallSlidingState.SetIsWallSlidingRight(false);
                this.enabled = true;
            }
        }

        if (motor.controller.velocity.y < 0f)
        {
            fallingState.enabled = true;
            this.enabled = false;
        }

        if (motor.controller.isGrounded)
        {
            runningState.enabled = true;
            this.enabled = false;
        }
    }

    public void SetDirectionOffWall(Vector3 dir)
    {
        directionOffWall = dir;
    }
}
