using UnityEngine;

public class BirdsEyeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 15f;
    public float maxZoom = 60f;

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void ZoomCamera()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        Camera camera = GetComponent<Camera>();

        if (camera.fieldOfView <= maxZoom && camera.fieldOfView >= minZoom)
        {
            camera.fieldOfView -= scrollInput * zoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minZoom, maxZoom);
        }
    }
}
