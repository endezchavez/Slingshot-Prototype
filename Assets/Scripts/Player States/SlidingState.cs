using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class SlidingState : MonoBehaviour
{
    public Transform playerFeetTransform;

    public float maxVelocityToExitSlide = 2f;
    public float slideSpeedMultipier = 1.5f;
    public float slideJumpMultipier = 1.5f;
    public float slideHeight = 0.5f;
    public float minHeightOffGroundToSlide = 0.75f;
    public float minVelToSlide = 2f;

    private Motor motor;

    private WalkingState walkingState;
    private RunningState runningState;
    private JumpingState jumpingstate;

    private float originalHeight;

    private float initialMag;
    private float currentMag;
    private Vector3 currentVel;
    private Vector3 currentVelXZ;
    private Vector3 initialVel;
    private Vector3 initialVelXZ;

    private bool isSliding = false;
    private bool hasCrouched = false;
    private bool hasStoodUp = false;
    private bool isStandingUp = false;

    private void Awake()
    {
        motor = GetComponent<Motor>();

        walkingState = GetComponent<WalkingState>();
        runningState = GetComponent<RunningState>();
        jumpingstate = GetComponent<JumpingState>();
        originalHeight = transform.localScale.y;
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

        if (!hasCrouched)
        {
            Crouch();
        }

        if (!isSliding)
        {
            motor.MultiplyVelocity(slideSpeedMultipier);
            isSliding = true;
        }

        if (InputManager.Instance.PlayerJumpedThisFrame())
        {
            StandUp();
            motor.MultiplyVelocity(slideJumpMultipier);
            jumpingstate.enabled = true;
            this.enabled = false;
        }

        if (InputManager.Instance.PlayerToggledCrouch())
        {
            StandUp();
            if (initialMag >= runningState.runSpeed - (runningState.runSpeed / 4))
            {
                isSliding = false;
                runningState.enabled = true;
            }
            else
            {
                isSliding = false;
                walkingState.enabled = true;
            }
            this.enabled = false;
        }

        //Exit Slide
        if (currentMag < maxVelocityToExitSlide)
        {
            StandUp();
            if (initialMag >= runningState.runSpeed - (runningState.runSpeed / 4))
            {
                isSliding = false;
                runningState.enabled = true;
            }
            else
            {
                isSliding = false;
                walkingState.enabled = true;
            }
            this.enabled = false;
        }
  
    }

    void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, slideHeight, transform.localScale.z);
    }

    void StandUp()
    {
        transform.localScale = new Vector3(transform.localScale.x, originalHeight, transform.localScale.z);
        hasCrouched = false;
        isSliding = false;
    }

    public bool IsPlayerLowEnoughToSlide()
    {
        return Physics.Raycast(playerFeetTransform.position, -transform.up, minHeightOffGroundToSlide);
    }
}
