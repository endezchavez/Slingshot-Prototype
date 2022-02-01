using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class IdleState : MonoBehaviour
{
    private Motor motor;
    private WalkingState walkingState;
    private JumpingState jumpingState;
    private CrouchingState crouchingState;

    private void Awake()
    {
        motor = GetComponent<Motor>();

        walkingState = GetComponent<WalkingState>();
        jumpingState = GetComponent<JumpingState>();
        crouchingState = GetComponent<CrouchingState>();
    }
    // Update is called once per frame
    void Update()
    {
        if (motor.controller.isGrounded)
        {
            if (InputManager.Instance.PlayerJumpedThisFrame())
            {
                jumpingState.enabled = true;
                this.enabled = false;
            }
        }

        if(motor.GetInputMoveDir() != Vector3.zero)
        {
            walkingState.enabled = true;
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerToggledCrouch())
        {
            crouchingState.enabled = true;
            this.enabled = false;
        }


    }
}
