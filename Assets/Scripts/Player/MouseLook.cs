using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    public bool canLook = false; // Disable mouse look initially

    public Texture2D customCursor;
    public Vector2 hotSpot = new Vector2(16, 16);

    float xRotation = 0f;

    void Start()
    {
        // Allow cursor movement at start
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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

    public void EnableMouseLook()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        canLook = true;
    }

    public void FreezeLook(bool freeze)
    {
        canLook = !freeze;
    }
}








