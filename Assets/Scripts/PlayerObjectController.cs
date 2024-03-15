using System;
using UnityEngine;

public class PlayerObjectController : MonoBehaviour
{

    private float moveSpeed = 15f;
    private float sprintMultiplier = 2f;
    private float fixedVerticalPosition = 1f;

    private Vector3 moveDirection;
    private CharacterController controller;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("CharacterController not found on the player object.");
        }
    }

    void Update()
    {
        switch (GameSettings.ActiveCamera)
        {
            case CameraMode.FirstPerson:
                HandleFirstPersonInput();
                break;
            case CameraMode.ThirdPerson:
                HandleThirdPersonInput();
                break;
            case CameraMode.BirdsEye:
                break;
        }
    }

    void HandleFirstPersonInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        MoveCharacter(moveX, moveZ);

        float rotateY = Input.GetAxis("Mouse X");
        RotateCharacter(rotateY);
    }

    void HandleThirdPersonInput()
    {
        float rotateY = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        transform.Rotate(0, rotateY, 0);
        MoveCharacter(0, moveZ);
    }

    void MoveCharacter(float moveX, float moveZ)
    {
        moveDirection = transform.right * moveX + transform.forward * moveZ;
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= moveSpeed * sprintMultiplier;
        }
        else
        {
            moveDirection *= moveSpeed;
        }

        controller.Move(moveDirection * Time.deltaTime);

        if (Mathf.Abs(transform.position.y - fixedVerticalPosition) > 0.01f)
        {
            transform.position = new Vector3(transform.position.x, fixedVerticalPosition, transform.position.z);
        }
    }

    void RotateCharacter(float rotateY)
    {
        float sensitivity = 100f;
        float rotationAmount = rotateY * sensitivity * Time.deltaTime;

        transform.Rotate(0, rotationAmount, 0);
    }
}
