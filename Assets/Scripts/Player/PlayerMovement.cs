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
    private float crouchTransitionTime = 0.1f;

    void Start()
    {
        originalHeight = controller.height;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Handle crouching
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
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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
}


