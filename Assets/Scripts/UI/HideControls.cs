using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideControls : MonoBehaviour
{
    public GameObject controlsText;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlsText.SetActive(!controlsText.activeSelf);
        }
    }
}
