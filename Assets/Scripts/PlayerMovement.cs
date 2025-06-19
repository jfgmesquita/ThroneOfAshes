using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float groundDrag;
    public float jumpForce = 6f;
    public float jumpCooldown;
    public float airMultiplier = 0.4f;
    bool readyToJump;
    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public AudioClip footstepClip;
    public AudioClip breathClip;
    public AudioClip heartbeatClip;
    private AudioSource footstepSource;
    private AudioSource breathSource;
    private AudioSource heartbeatSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        // Breath audio source
        breathSource = gameObject.AddComponent<AudioSource>();
        breathSource.clip = breathClip;
        breathSource.loop = true;
        breathSource.playOnAwake = true;
        breathSource.volume = 0.4f;
        breathSource.Play();

        // Footstep audio source (only when moving)
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.clip = footstepClip;
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        footstepSource.volume = 0.1f;

        // Heartbeat audio source
        heartbeatSource = gameObject.AddComponent<AudioSource>();
        heartbeatSource.clip = heartbeatClip;
        heartbeatSource.loop = true;
        heartbeatSource.playOnAwake = false;
        heartbeatSource.volume = 0.25f;
    }

    private void Update()
    {
        if (PauseMenuGUI.IsPaused) return;

        GameOverMenu gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        Victory victoryMenu = FindFirstObjectByType<Victory>();
        bool menuActive = (gameOverMenu != null && gameOverMenu.IsMenuActive()) ||
                          (victoryMenu != null && victoryMenu.IsMenuActive());

        if (menuActive)
        {
            if (breathSource.isPlaying)
            {
                breathSource.Pause();
            }
            if (footstepSource.isPlaying)
            {
                footstepSource.Pause();
            }
            if (heartbeatSource.isPlaying)
            {
                heartbeatSource.Pause();
            }
            return;
        }
        else
        {
            if (!breathSource.isPlaying)
                breathSource.Play();
        }

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        bool isMoving = (horizontalInput != 0 || verticalInput != 0) && grounded;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Pause();
        }

        // get PlayerUI intoxication
        float intoxication = 0f, maxIntoxication = 1f;
        PlayerUI playerUI = FindFirstObjectByType<PlayerUI>();
        if (playerUI != null)
        {
            intoxication = playerUI.intoxication;
            maxIntoxication = playerUI.maxIntoxication;
        }

        float intoxPercent = intoxication / maxIntoxication;

        if (intoxPercent >= 0.7f)
        {
            if (!heartbeatSource.isPlaying && heartbeatClip != null)
                heartbeatSource.Play();
        }
        else
        {
            if (heartbeatSource.isPlaying)
                heartbeatSource.Pause();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void StopHeartbeat()
    {
        if (heartbeatSource != null && heartbeatSource.isPlaying)
        {
            heartbeatSource.Stop();
        }
    }
}