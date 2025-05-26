using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public static PlayerMovement Instance;

    private GameManager gameManager;


    private PlayerBuffs playerBuffs;

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


    public bool IsMoving { get; private set; }
    public bool JustStoodUp { get; private set; }
    private float stoodUpTimer = 0f;
    private float gracePeriod = 1f;

    private float originalHeight;
    private float crouchHeight = 0.5f;
    private float crouchTransitionTime = 0.1f;

    private bool isInWater = false;
    private float waterSurfaceY;
    public float floatHeight = 1.0f;

    [Header("Shrunken Water Float Height")]
    public float shrunkFloatHeight = 0.5f;

    public float slopeLimit = 75f;
    public ItemPickup itemPickup;

    private float speedMultiplier = 1f;
    private float jumpMultiplier = 1f;

    public bool IsStealthed { get; private set; } = false;
    public bool IsSmall { get; private set; }

    [Header("Swimming")]
    public float swimControlSpeed = 0.1f;

    

    [Header("Player Shrink")]
    public bool isShrinking = false;
    public float shrinkDuration = 2f;
    public Vector3 shrinkScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float reducedSpeed = 4f;
    public float reducedJumpHeight = 1f;

    private float originalSpeed;
    private float originalJumpHeight;

    [Header("Stealth Boost")]
    public bool fastStealthMode = false;
    public float fastCrouchSpeed = 8f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate player in new scene
        }
    }


    void Start()
    {
        playerBuffs = GetComponent<PlayerBuffs>();
        gameManager = GameManager.Managerinstance;
        if (gameManager != null)
        {
            // Optional: let the player register itself to GameManager (if needed)
            gameManager.player = this;
        }
        originalHeight = controller.height;
        originalSpeed = speed;
        originalJumpHeight = jumpHeight;
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

        // ✅ Movement detection
        IsMoving = (x != 0 || z != 0);

        if (!isInWater)
        {
            bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

            if (crouchInput)
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

                    // ✅ Mark player as just stood up
                    JustStoodUp = true;
                    stoodUpTimer = gracePeriod;
                }
            }
        }

        float moveSpeed = (isCrouching ? (fastStealthMode ? fastCrouchSpeed : crouchSpeed) : speed) * speedMultiplier;

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

        if (isInWater)
{
    if (playerBuffs != null && playerBuffs.isTinyTinasCurseActive)
    {
        // Fly above water
        Vector3 floatPosition = transform.position;
        float targetY = waterSurfaceY + floatHeight + 0.5f; // Slightly above water
        floatPosition.y = Mathf.Lerp(floatPosition.y, targetY, Time.deltaTime * 5f);
        transform.position = floatPosition;

        Vector3 flyMove = move.normalized * speed * speedMultiplier * Time.deltaTime;
        controller.Move(flyMove);

        velocity.y = 0f; // Cancel gravity
    }
    else
    {
        // Normal swim behavior
        float waterMoveSpeed = Mathf.Max(0f, swimControlSpeed);
        Vector3 waterMovement = move.normalized * waterMoveSpeed * Time.deltaTime;
        controller.Move(waterMovement);

        Vector3 pos = transform.position;
        float targetY = waterSurfaceY - controller.height / 2 + (IsSmall ? shrunkFloatHeight : floatHeight);
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10f);
        transform.position = pos;
    }
}
else
{
    controller.Move(velocity * Time.deltaTime);
}


        if (isShrinking)
        {
            StartCoroutine(ShrinkPlayer());
            isShrinking = false;
        }
        else
        {
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

        // ✅ Countdown to clear JustStoodUp
        if (JustStoodUp)
        {
            stoodUpTimer -= Time.deltaTime;
            if (stoodUpTimer <= 0f)
            {
                JustStoodUp = false;
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

        transform.localScale = targetScale;
        IsSmall = true;
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
    
    public void TeleportTo(Vector3 newPosition)
{
    controller.enabled = false;  // Disable controller to safely change position
    transform.position = newPosition;
    controller.enabled = true;   // Re-enable controller
    velocity = Vector3.zero;     // Reset velocity to avoid falling issues right after teleport
}

}














