using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class JumpingState : MonoBehaviour
{
    public LayerMask whatIsWall;

    public float jumpHeight = 1f;
    public float airSpeed = 1f;

    private Motor motor;

    private FallingState fallingState;
    private WallSlidingState wallSlideState;

    private Vector3 inputDir;

    private float initialMag;
    private Vector3 currentVel;
    private Vector3 initialVel;
    private Vector3 initialVelXZ;

    private bool hasJumped = false;

    private void Awake()
    {
        motor = GetComponent<Motor>();
        fallingState = GetComponent<FallingState>();
        wallSlideState = GetComponent<WallSlidingState>();
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
        if (!hasJumped)
        {
            currentVel = motor.GetVelocity();
            motor.SetVelocity(new Vector3(currentVel.x, Mathf.Sqrt(jumpHeight * -3.0f * motor.gravity), currentVel.z));
            hasJumped = true;
        }

        //Transition to Wallslide
        if(initialMag >= wallSlideState.minVelToWallSlide)
        {
            if (wallSlideState.WallCheckRight())
            {
                hasJumped = false;
                wallSlideState.SetIsWallSlidingRight(true);
                wallSlideState.enabled = true;
                this.enabled = false;
            }

            if (wallSlideState.WallCheckLeft())
            {
                hasJumped = false;
                wallSlideState.SetIsWallSlidingRight(false);
                wallSlideState.enabled = true;
                this.enabled = false;
            }
        }
        

        if (motor.controller.velocity.y < 0f && hasJumped)
        {
            hasJumped = false;
            fallingState.enabled = true;
            this.enabled = false;
        }

        inputDir = motor.GetInputMoveDir();
        if(Vector3.Dot(initialVel.normalized, inputDir.normalized) > 0)
        {
            motor.SetVelocity(new Vector3(inputDir.x * initialMag, motor.GetVelocity().y, inputDir.z * initialMag));
        }
        else
        {
            motor.SetVelocity(new Vector3(inputDir.x * airSpeed, motor.GetVelocity().y, inputDir.z * airSpeed));
        }
    }

    
}
