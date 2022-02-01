using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    [SerializeField] bool lockCursor = true;

    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    private PlayerControls playerControls;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        playerControls = new PlayerControls();
    }

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool PlayerCrouchedThisFrame()
    {
        return playerControls.Player.CrouchDown.triggered;
    }

    public bool PlayerStoodUpThisFrame()
    {
        return playerControls.Player.CrouchUp.triggered;
    }

    public bool PlayerIsCharging()
    {
        return playerControls.Player.Shoot.ReadValue<float>() > 0.1f;
    }

    public bool PlayerIsSprinting()
    {
        return playerControls.Player.Sprint.ReadValue<float>() > 0.1f;
    }

    public bool PlayerToggledRun()
    {
        return playerControls.Player.RunToggle.triggered;
    }

    public bool PlayerToggledCrouch()
    {
        return playerControls.Player.CrouchToggle.triggered;
    }

    
}
