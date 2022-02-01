using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class CrouchingState : MonoBehaviour
{
    public float crouchSpeed = 2f;
    public float crouchHeight = 0.7f;
    public float gravityMidCrouch = -100f;

    private Motor motor;
    private IdleState idleState;
    private RunningState runningState;
    private JumpingState jumpingState;

    private float originalHeight;

    private bool hasCrouched = false;
    private bool hasStoodUp = false;
    private bool isStandingUp = false;

    private bool isRunning = false;



    private void Awake()
    {
        motor = GetComponent<Motor>();
        idleState = GetComponent<IdleState>();
        runningState = GetComponent<RunningState>();
        jumpingState = GetComponent<JumpingState>();
        originalHeight = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!motor.controller.isGrounded)
        {
            motor.SetGravity(gravityMidCrouch);
        }
        else
        {
            motor.SetGravity(motor.gravity);
        }
        

        if (!hasCrouched)
        {
            Crouch();
            hasCrouched = true;
        }

        if (InputManager.Instance.PlayerToggledCrouch())
        {
            StandUp();
            idleState.enabled = true;
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerToggledRun())
        {
            StandUp();
            runningState.enabled = true;
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerJumpedThisFrame())
        {
            StandUp();
            jumpingState.enabled = true;
            this.enabled = false;
        }

        

        motor.SetVelocity(motor.GetInputMoveDir(), crouchSpeed);
        
    }

    void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
    }

    void StandUp()
    {
        transform.localScale = new Vector3(transform.localScale.x, originalHeight, transform.localScale.z);
        hasCrouched = false;
    }
}
