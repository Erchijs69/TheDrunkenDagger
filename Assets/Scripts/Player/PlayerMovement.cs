using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public static PlayerMovement Instance;

    public float speed = 12f;
    public float crouchSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool canMove = true;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    public bool IsCrouching => isCrouching;

    private float originalHeight;
    private float crouchHeight = 0.5f;
    private float crouchTransitionTime = 0.1f;

    private bool isInWater = false;
    private float waterSurfaceY;
    public float floatHeight = 1.0f;
    
    // New shrunk float height for when the player is small
    [Header("Shrunken Water Float Height")]
    public float shrunkFloatHeight = 0.5f; // Float height when shrunk (editable in the Inspector)

    public float slopeLimit = 75f;

    public ItemPickup itemPickup;

    // Buffs
    private float speedMultiplier = 1f;
    private float jumpMultiplier = 1f;

    // Stealth
    public bool IsStealthed { get; private set; } = false;

    // small
    public bool IsSmall { get; private set; }

    [Header("Swimming")]
    [Tooltip("Controls how fast the player swims. 0 = no movement, higher = faster")]
    public float swimControlSpeed = 0.1f;

    [Header("Player Shrink")]
    public bool isShrinking = false; // Toggle this in the inspector to shrink the player
    public float shrinkDuration = 2f; // Time it takes to shrink
    public Vector3 shrinkScale = new Vector3(0.2f, 0.2f, 0.2f); // Target scale after shrinking
    public float reducedSpeed = 4f; // Reduced speed when the player is small
    public float reducedJumpHeight = 1f; // Reduced jump height when the player is small

    private float originalSpeed;
    private float originalJumpHeight;

    [Header("Stealth Boost")]
    public bool fastStealthMode = false;
    public float fastCrouchSpeed = 8f;


    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances if any
        }
    }

    void Start()
    {
        originalHeight = controller.height;
        originalSpeed = speed; // Store the original speed
        originalJumpHeight = jumpHeight; // Store the original jump height
        controller.slopeLimit = slopeLimit;
    }

    void Update()
{
    if (!canMove) return;

    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    if (isGrounded && velocity.y < 0)
    {
        velocity.y = -2f;
    }

    float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");

    // Prevent crouching when swimming
    if (!isInWater) 
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            if (!isCrouching)
            {
                StartCoroutine(ChangeCrouchState(true));
            }
        }
        else
        {
            if (isCrouching)
            {
                StartCoroutine(ChangeCrouchState(false));
            }
        }
    }

    float baseSpeed = isCrouching ? (fastStealthMode ? fastCrouchSpeed : crouchSpeed) : speed;
    float moveSpeed = (isCrouching ? fastStealthMode ? fastCrouchSpeed : crouchSpeed : speed) * speedMultiplier;

    if (itemPickup != null && itemPickup.IsDrinking())
    {
        moveSpeed *= 0.3f;
    }

    Vector3 move = transform.right * x + transform.forward * z;

    if (!isInWater)
    {
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching && !(itemPickup != null && itemPickup.IsDrinking()))
    {
        velocity.y = Mathf.Sqrt(jumpHeight * jumpMultiplier * -2f * gravity);
    }

    velocity.y += gravity * Time.deltaTime;

    // Swimming logic
    if (isInWater)
    {
        float waterMoveSpeed = Mathf.Max(0f, swimControlSpeed);
        Vector3 waterMovement = move.normalized * waterMoveSpeed * Time.deltaTime;

        controller.Move(waterMovement);

        // Adjust float height for small players
        Vector3 pos = transform.position;
        float targetY = waterSurfaceY - controller.height / 2 + (IsSmall ? shrunkFloatHeight : floatHeight);
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10f);
        transform.position = pos;
    }
    else
    {
        controller.Move(velocity * Time.deltaTime);
    }

    // If shrinking, run the shrinking coroutine if not already running
    if (isShrinking)
    {
        StartCoroutine(ShrinkPlayer());
        isShrinking = false; // Set it to false so it only shrinks once per toggle
    }
    else
    {
        // Apply reduced values for speed and jump height when the player is small
        if (transform.localScale == shrinkScale)
        {
            speed = reducedSpeed;
            jumpHeight = reducedJumpHeight;
        }
        else
        {
            speed = originalSpeed;
            jumpHeight = originalJumpHeight;
        }
    }
}


    IEnumerator ChangeCrouchState(bool crouching)
    {
        float timeElapsed = 0f;
        float startHeight = controller.height;
        float targetHeight = crouching ? crouchHeight : originalHeight;

        while (timeElapsed < crouchTransitionTime)
        {
            controller.height = Mathf.Lerp(startHeight, targetHeight, timeElapsed / crouchTransitionTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        isCrouching = crouching;
    }

    IEnumerator ShrinkPlayer()
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = shrinkScale;

        float timeElapsed = 0f;

        while (timeElapsed < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, timeElapsed / shrinkDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Finalize scale
        IsSmall = true; // ✅ Now the player is small
        Debug.Log("Player is now small: " + IsSmall);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            waterSurfaceY = other.bounds.max.y;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
        Debug.Log($"Movement speed multiplier set to: {multiplier}");
    }

    public void SetJumpMultiplier(float multiplier)
    {
        jumpMultiplier = multiplier;
        Debug.Log($"Jump multiplier set to: {multiplier}");
    }

    public void SetSwimSpeed(float newSwimSpeed)
    {
        swimControlSpeed = newSwimSpeed;
        Debug.Log($"Swim speed set to: {newSwimSpeed}");
    }

    public void SetStealth(bool isStealthed)
    {
        IsStealthed = isStealthed;
        Debug.Log($"Stealth state: {isStealthed}");
    }

    public void SetSmallState(bool state)
    {
        IsSmall = state;
    }

    public void ToggleSize()
    {
        IsSmall = !IsSmall;
        Debug.Log("Toggled IsSmall. Now: " + IsSmall);
    }

    public void EnableFastStealthMode(bool enable)
{
    fastStealthMode = enable;
    Debug.Log($"Fast stealth mode: {enable}");
}

}













