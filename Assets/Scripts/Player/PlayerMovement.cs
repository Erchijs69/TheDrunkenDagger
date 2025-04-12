using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

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

    private float originalHeight;
    private float crouchHeight = 0.5f;
    private float crouchTransitionTime = 0.1f;

    // Water floating
    private bool isInWater = false;
    private float waterSurfaceY;
    public float floatHeight = 1.0f; // Increased to keep player higher in water

    // Slope support
    public float slopeLimit = 75f; // Higher slope limit

    void Start()
    {
        originalHeight = controller.height;
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

        float moveSpeed = isCrouching ? crouchSpeed : speed;
        if (isInWater)
        {
            moveSpeed *= 0.5f; // Slower movement in water
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        if (isInWater)
        {
            Vector3 pos = transform.position;
            float targetY = waterSurfaceY - controller.height / 2 + floatHeight;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 2f);
            transform.position = pos;
        }
        else
        {
            controller.Move(velocity * Time.deltaTime);
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
}




