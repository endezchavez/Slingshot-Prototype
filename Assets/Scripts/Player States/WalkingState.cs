using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class WalkingState : MonoBehaviour
{
    public float walkSpeed = 4f;

    private Motor motor;
    private IdleState idleState;
    private RunningState runningState;
    private JumpingState jumpingState;
    private CrouchingState crouchingState;
    private SlidingState slidingState;

    private void Awake()
    {
        motor = GetComponent<Motor>();
        runningState = GetComponent<RunningState>();
        idleState = GetComponent<IdleState>();
        jumpingState = GetComponent<JumpingState>();
        crouchingState = GetComponent<CrouchingState>();
        slidingState = GetComponent<SlidingState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!motor.controller.isGrounded)
        {
            return;
        }

        if (InputManager.Instance.PlayerToggledRun())
        {
            runningState.enabled = true;
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerToggledCrouch())
        {
            slidingState.enabled = true;
            this.enabled = false;
        }

        if (motor.GetInputMoveDir() == Vector3.zero)
        {
            idleState.enabled = true;
            this.enabled = false;
        }

        if (motor.controller.isGrounded)
        {
            if (InputManager.Instance.PlayerJumpedThisFrame())
            {
                jumpingState.enabled = true;
                this.enabled = false;
            }
        }

        motor.SetVelocity(motor.GetInputMoveDir(), walkSpeed);
    }
}
