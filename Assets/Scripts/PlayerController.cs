using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public LayerMask whatIsWall;
    public Transform eyeLevel;
    public Transform ledgeGrabCheckBottom;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    
    [SerializeField]
    private float walkSpeed = 5.0f;
    [SerializeField]
    private float runSpeed = 7.0f;
    [SerializeField]
    private float slideSpeed = 10.0f;
    [SerializeField]
    private float wallSlideSpeed = 50.0f;
    [SerializeField]
    private float wallClimbSpeed = 5.0f;
    [SerializeField]
    private float wallClimbTime = 1f;
    [SerializeField]
    private float crouchHeight = 1.2f;
    [SerializeField]
    private float crouchSpeed = 2.5f;
    [SerializeField]
    private float crouchTransitionSpeed = 500f;
    [SerializeField]
    private float jumpHeight = 1f;
    [SerializeField]
    private float wallJumpHeight = 3f;
    [SerializeField]
    private float wallSlideJumpHeight = 3f;
    [SerializeField]
    private float ledgeGrabJumpHeight = 3f;
    [SerializeField]
    private float slideJumpForce = 3f;
    [SerializeField]
    private float wallSlideUpForce = 2f;
    [SerializeField]
    private float wallSlideHorizontalJumpSpeed = 10f;
    [SerializeField]
    private float wallClimbHorizontalJumpSpeed = 5f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float wallSlideGravityValue = -5f;
    [SerializeField]
    private float dragValue = -0.5f;
    [SerializeField]
    private float airDragMultiplier = 0.7f;
    private InputManager inputManager;
    private Transform cameraTransform;

    private float currentSpeed;
    private float originalColHeight;

    private bool isWalking = false;
    private bool isRunning = false;
    private bool isSliding = false;
    private bool isWallSlidingRight = false;
    private bool isWallSlidingLeft = false;
    private bool canWallSlide = true;
    private bool isCrouching = false;
    private bool hasDoubleJumped = false;
    private bool isCollidingWithWall = false;
    private bool hasWallJumped = false;
    private bool isWallClimbing = false;
    private bool canWallClimb = true;
    private bool isCrouchTransitioning = false;
    private bool isLedgeGrabbing = false;

    private float currentGravity;

    private Vector3 dirOffWall;
    private Vector2 playerInputVector;
    private Vector3 movementDir;



    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        originalColHeight = controller.height;

        currentSpeed = walkSpeed;
        currentGravity = gravityValue;

    }

    void Update()
    {
        //Freeze Rotation
        transform.eulerAngles = new Vector3(0f, cameraTransform.eulerAngles.y, 0f);

        groundedPlayer = controller.isGrounded;
        
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        
        if (groundedPlayer)
        {
            hasDoubleJumped = false;
            isWallSlidingRight = false;
            isWallSlidingLeft = false;
            hasWallJumped = false;
            canWallClimb = true;
            canWallSlide = true;
            if (currentGravity != gravityValue)
            {
                currentGravity = gravityValue;
            }
        }

        if (inputManager.PlayerToggledRun())
        {
            ToggleRun();
        }

        if (isRunning && playerInputVector.y <= 0)
        {
            ToggleRun();
        }

        if (inputManager.PlayerToggledCrouch())
        {
            ToggleCrouch();
        }

        if (isCrouchTransitioning)
        {
            if(isCrouching || isSliding)
            {
                controller.height -= crouchTransitionSpeed * Time.deltaTime;
                if (controller.height <= crouchHeight + 0.01f)
                {
                    controller.height = crouchHeight;
                    isCrouchTransitioning = false;
                }
            }
            else
            {
                controller.height += crouchTransitionSpeed * Time.deltaTime;
                if (controller.height >= originalColHeight - 0.01f)
                {
                    controller.height = originalColHeight;
                    isCrouchTransitioning = false;
                }
                
            }
            
        }

        if (inputManager.PlayerJumpedThisFrame())
        {
            if (groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                if (isCrouching)
                {
                    ToggleCrouch();
                }

                if (isSliding)
                {

                    playerVelocity += transform.forward * slideJumpForce;
                }
            }
            else if (!hasDoubleJumped && !groundedPlayer && !isWallSlidingLeft && !isWallSlidingRight && !isWallClimbing)
            {
                hasDoubleJumped = true;
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            else if(isWallSlidingLeft || isWallSlidingRight && !hasWallJumped && isCollidingWithWall)
            {
                playerVelocity.y += Mathf.Sqrt(wallSlideJumpHeight * -3.0f * gravityValue);
                playerVelocity += dirOffWall * wallSlideHorizontalJumpSpeed;
                hasWallJumped = true;
            }else if (isWallClimbing && isCollidingWithWall)
            {
                playerVelocity.y += Mathf.Sqrt(wallJumpHeight * -3.0f * gravityValue);
                playerVelocity += dirOffWall * wallClimbHorizontalJumpSpeed;
            }
            
        }

        if (isWallSlidingRight && playerInputVector.x < 0)
        {
            isWallSlidingRight = false;
            canWallSlide = false;
        }

        if (isWallSlidingLeft && playerInputVector.x > 0)
        {
            isWallSlidingLeft = false;
            canWallSlide = false;
        }

        if (!isSliding && !isWallSlidingRight && !isWallSlidingLeft && !hasWallJumped && !isWallClimbing && !isLedgeGrabbing)
        {
            if (groundedPlayer)
            {

                playerVelocity.x = movementDir.x * currentSpeed;
                playerVelocity.z = movementDir.z * currentSpeed;
            }
            else
            {
                playerVelocity.x = movementDir.x * currentSpeed * airDragMultiplier;
                playerVelocity.z = movementDir.z * currentSpeed * airDragMultiplier;
            }
        }
        else if(controller.velocity.magnitude < 1f)
        {
            if (isSliding)
            {
                isSliding = false;
                isRunning = true;
                isCrouching = false;
                isCrouchTransitioning = true;
            }

        }

        playerInputVector = inputManager.GetPlayerMovement();
        movementDir = new Vector3(playerInputVector.x, 0, playerInputVector.y);
        movementDir = cameraTransform.forward * movementDir.z + cameraTransform.right * movementDir.x;
        movementDir.y = 0f;

        if (Physics.CheckSphere(transform.position, 1f, whatIsWall))
        {
            isCollidingWithWall = true;
        }
        else
        {
            isCollidingWithWall = false;
        }


        if (!isCollidingWithWall && isWallSlidingRight)
        {
            isWallSlidingRight = false;
            currentGravity = gravityValue;

        }

        if (!isCollidingWithWall && isWallSlidingLeft)
        {
            isWallSlidingLeft = false;
            currentGravity = gravityValue;
        }

        float mag = new Vector3(playerVelocity.x, 0f, playerVelocity.z).magnitude;
        if (WallCheckRight() && !isWallSlidingRight && !isWallSlidingLeft && canWallSlide && GetDistanceFromGround() > 2f && mag > walkSpeed)
        {
            WallSlideRight();
        }
        if (WallCheckLeft() && !isWallSlidingLeft && !isWallSlidingRight && canWallSlide && GetDistanceFromGround() > 2f && mag > walkSpeed)
        {
            WallSlideLeft();
        }
        

        if (isWallSlidingRight)
        {
            playerVelocity += transform.right * 0.01f;
        }

        if (isWallSlidingLeft)
        {
            playerVelocity += -transform.right * 0.01f;
        }

        //Wall Climb
        if (WallCheckForward() && canWallClimb && !isWallSlidingLeft && !isWallSlidingRight && !isLedgeGrabbing)
        {
            WallClimb();
            StartCoroutine(WallClimbTimer());
            canWallSlide = false;
        }

        //Ledge Grab Check

        if (!groundedPlayer && LedgeGrabCheck() && !isLedgeGrabbing)
        {
            isLedgeGrabbing = true;
        }

        if (isLedgeGrabbing && !isSliding)
        {
            playerVelocity.y = 0f;
            currentGravity = 0f;

            if(playerInputVector.y >= 0)
            {
                playerVelocity.y = ledgeGrabJumpHeight;
                currentGravity = gravityValue;
                isLedgeGrabbing = false;
            }
        }

        playerVelocity.y += currentGravity * Time.deltaTime;
        playerVelocity.x /= 1 + dragValue * Time.deltaTime;
        playerVelocity.z /= 1 + dragValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }

  

    void ToggleCrouch()
    {
        if (!isCrouching)
        {
            if (GetDistanceFromGround() < 2.0f && !isSliding && playerInputVector != Vector2.zero)
            {

                isSliding = true;
                playerVelocity = new Vector3(movementDir.x, 0f, movementDir.z) * slideSpeed;
            }
            
            else
            {
                isCrouching = true;
                currentSpeed = crouchSpeed;
            }
            isCrouchTransitioning = true;

        }
        else
        {

            isCrouching = false;
            currentSpeed = walkSpeed;
            isCrouchTransitioning = true;
            if (isSliding)
            {
                isSliding = false;
            }
        }
        
    }

    

    void ToggleRun()
    {
        if (isCrouching)
        {
            isCrouching = false;

            isCrouchTransitioning = true;

        }
        if (!isRunning)
        {
            isRunning = true;
            currentSpeed = runSpeed;
            //Debug.Log(currentSpeed);
            if (isSliding)
            {
                isSliding = false;
            }
        }
        else
        {
            isRunning = false;
            currentSpeed = walkSpeed;
        }
    }


    void StandUp()
    {
        controller.height = originalColHeight;
    }

    bool WallCheckRight()
    {
        return Physics.Raycast(transform.position, transform.right, 0.8f, whatIsWall);
    }

    bool WallCheckLeft()
    {
        return Physics.Raycast(transform.position, -transform.right, 0.8f, whatIsWall);
    }

    bool WallCheckForward()
    {
        return Physics.Raycast(transform.position, transform.forward, 0.9f, whatIsWall);
    }

    void WallClimb()
    {
        hasWallJumped = false;

        if (!isWallClimbing)
        {
            dirOffWall = -transform.forward;
        }

        if (playerInputVector.y > 0)
        {
            isWallClimbing = true;
            playerVelocity = transform.up * playerInputVector.y * wallClimbSpeed;
        }

    }

    void WallSlideRight()
    {
        hasWallJumped = false;

        isWallSlidingRight = true;
        currentGravity = wallSlideGravityValue;

        playerVelocity = transform.forward * wallSlideSpeed;
        playerVelocity += transform.up * wallSlideUpForce;

      
        dirOffWall = -transform.right;

    }


    void WallSlideLeft()
    {
        hasWallJumped = false;

        isWallSlidingLeft = true;
        currentGravity = wallSlideGravityValue;

        playerVelocity = transform.forward * wallSlideSpeed;
        playerVelocity += transform.up * wallSlideUpForce;

        dirOffWall = transform.right;
    }

    float GetDistanceFromGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            return hit.distance;
        }

        return Mathf.Infinity;
    }

    IEnumerator WallClimbTimer()
    {
        yield return new WaitForSeconds(wallClimbTime);

        canWallClimb = false;
        isWallClimbing = false;

    }

    bool LedgeGrabCheck()
    {
        //Debug.DrawRay(EyeLevel.position, transform.forward * 0.8f, Color.green, 0.1f);
        return !Physics.Raycast(eyeLevel.position, transform.forward, 0.8f, whatIsWall) && Physics.Raycast(ledgeGrabCheckBottom.position, transform.forward, 0.8f, whatIsWall);
    }

  
}
