using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class RunningState : MonoBehaviour
{
    public float runSpeed = 8f;

    private Motor motor;
    private WalkingState walkingState;
    private JumpingState jumpingState;
    private SlidingState slidingState;
   

    private bool isRunning = false;

    private void Awake()
    {
        motor = GetComponent<Motor>();
        walkingState = GetComponent<WalkingState>();
        jumpingState = GetComponent<JumpingState>();
        slidingState = GetComponent<SlidingState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!motor.controller.isGrounded)
        {
            return;
        }
        if (InputManager.Instance.PlayerToggledRun() || motor.GetInputMoveDir() == Vector3.zero)
        {
            walkingState.enabled = true;
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerToggledCrouch())
        {
            slidingState.enabled = true;
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

        motor.SetVelocity(motor.GetInputMoveDir(), runSpeed);
    }

    void CheckForRunInput()
    {
        if (motor.controller.isGrounded)
        {
            if (InputManager.Instance.PlayerToggledRun())
            {
                ToggleRun();
            }
        }
    }

    

    void ToggleRun()
    {
        if (isRunning)
        {
            isRunning = false;
        }
        else
        {
            isRunning = true;
        }
    }
}
