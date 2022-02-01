using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class FallingState : MonoBehaviour
{

    private Motor motor;
    private IdleState idleState;
    private RunningState runningState;
    private JumpingState jumpingState;
    private SlidingState slidingState;
    private WallSlidingState wallSlidingState;

    private Vector3 inputDir;

    private float initialMag;
    private float currentMag;
    private Vector3 currentVel;
    private Vector3 initialVel;
    private Vector3 currentVelXZ;
    private Vector3 initialVelXZ;

    private void Awake()
    {
        motor = GetComponent<Motor>();
        idleState = GetComponent<IdleState>();
        runningState = GetComponent<RunningState>();
        jumpingState = GetComponent<JumpingState>();
        slidingState = GetComponent<SlidingState>();
        wallSlidingState = GetComponent<WallSlidingState>();
        
    }

    private void OnEnable()
    {
        initialVel = motor.GetVelocity();
        initialVelXZ = new Vector3(initialVel.x, 0f, initialVel.z);
        initialMag = initialVelXZ.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        currentVel = motor.GetVelocity();
        currentVelXZ = new Vector3(currentVel.x, 0f, currentVel.z);
        currentMag = currentVelXZ.magnitude;

        if (motor.controller.isGrounded)
        {
            if (motor.GetInputMoveDir() != Vector3.zero && initialMag >= runningState.runSpeed - (runningState.runSpeed / 4))
            {
                runningState.enabled = true;
            }
            else
            {
                idleState.enabled = true;
            }
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerToggledCrouch())
        {
            if(slidingState.IsPlayerLowEnoughToSlide() && currentMag >= slidingState.minVelToSlide)
            {
                slidingState.enabled = true;
                this.enabled = false;
            }
        }

        if(currentMag >= wallSlidingState.minVelToWallSlide)
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
        

        inputDir = motor.GetInputMoveDir();
        if (Vector3.Dot(initialVel.normalized, inputDir.normalized) > 0)
        {
            motor.SetVelocity(new Vector3(inputDir.x * initialMag, motor.GetVelocity().y, inputDir.z * initialMag));
        }
        else
        {
            motor.SetVelocity(new Vector3(inputDir.x * jumpingState.airSpeed, motor.GetVelocity().y, inputDir.z * jumpingState.airSpeed));
        }


    }

    

    
}
