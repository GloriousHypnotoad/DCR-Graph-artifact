using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    private CameraMode _currentMode;
    private float _xRotation = 0f;
    private float _thirdPersonZoomSpeend = 4f;
    private float _birdsEyeZoomSpeend = 40f;
    private float _birdsEyeMovementSpeend = 40f;
    private float sprintMultiplier = 2f;

    public void SetCameraMode(CameraMode mode)
    {
        _currentMode = mode;
    }

    void Update()
    {
        switch (_currentMode)
        {
            case CameraMode.FirstPerson:
                HandleFirstPersonModeInput();
                break;

            case CameraMode.ThirdPerson:
                HandleThirdPersonModeInput();
                break;

            case CameraMode.BirdsEye:
                HandleBirdsEyeModeInput();
                break;
        }
    }

    private void HandleFirstPersonModeInput()
    {  
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    private void HandleThirdPersonModeInput()
    {
        HandleCameraZoom(_thirdPersonZoomSpeend);
    }

    private void HandleBirdsEyeModeInput()
    {
        HandleCameraZoom(_birdsEyeZoomSpeend);


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement = new Vector3(-horizontal, 0, -vertical) * _birdsEyeMovementSpeend * sprintMultiplier;
        }
        else
        {
            movement = new Vector3(-horizontal, 0, -vertical) * _birdsEyeMovementSpeend;
        }

        
        transform.Translate(movement * Time.deltaTime, Space.World);
    }

    void HandleCameraZoom(float movementSpeed)
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * scrollInput * movementSpeed, Space.Self);
    }
}
