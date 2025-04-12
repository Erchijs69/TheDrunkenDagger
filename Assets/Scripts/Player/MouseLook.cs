using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    public bool canLook = true;

    // Custom cursor properties
    public Texture2D customCursor; // Drag your custom cursor texture here in the Inspector
    public Vector2 hotSpot = new Vector2(16, 16); // Set the hotspot (the point on the cursor that will act as the pointer)

    float xRotation = 0f;

    void Start()
    {
        // Hide the default system cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Set the custom cursor
        Cursor.SetCursor(customCursor, hotSpot, CursorMode.Auto);
    }

    void Update()
    {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

