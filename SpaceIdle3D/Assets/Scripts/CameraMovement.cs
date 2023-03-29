using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed = 10f;
    private Vector3 lastMousePosition;
    Vector3 mouseMovement;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouseMovement = Input.mousePosition - lastMousePosition;

            mouseMovement.x = -mouseMovement.x;
            transform.position -= mouseMovement * cameraSpeed * Time.deltaTime;
        }

        lastMousePosition = Input.mousePosition;
    }
}
