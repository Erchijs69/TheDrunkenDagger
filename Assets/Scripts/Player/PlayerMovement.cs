using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float crouchSpeed = 6f; // Speed when crouching
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;

    private float originalHeight;
    private float crouchHeight = 0.5f;
    private float timeToCrouchDown = 0.1f; // Faster time to crouch down
    private float timeToStandUp = 0.25f; // Slower time to stand up
    private float currentHeight;

    void Start()
    {
        originalHeight = controller.height; // Save the original height of the player
        currentHeight = originalHeight; // Set the initial height to the original height
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset the velocity when grounded
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Handle crouch input
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }

        // Set crouch speed or normal speed
        float moveSpeed = isCrouching ? crouchSpeed : speed;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jump only when standing
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Toggle crouch state
    void ToggleCrouch()
    {
        if (isCrouching)
        {
            StartCoroutine(StandUp());
        }
        else
        {
            StartCoroutine(CrouchDown());
        }
    }

    // Smoothly crouch the player down with a faster transition
    IEnumerator CrouchDown()
    {
        float timeElapsed = 0f;
        while (timeElapsed < timeToCrouchDown) // Faster time to crouch down
        {
            controller.height = Mathf.Lerp(originalHeight, crouchHeight, timeElapsed / timeToCrouchDown);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        controller.height = crouchHeight; // Ensure the final height is set
        isCrouching = true;
    }

    // Smoothly stand the player up
    IEnumerator StandUp()
    {
        float timeElapsed = 0f;
        while (timeElapsed < timeToStandUp) // Slower time to stand up
        {
            controller.height = Mathf.Lerp(crouchHeight, originalHeight, timeElapsed / timeToStandUp);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        controller.height = originalHeight; // Ensure the final height is set
        isCrouching = false;
    }
}


