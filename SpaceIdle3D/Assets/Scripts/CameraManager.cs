using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private Vector3 zoomAmount;
    [SerializeField] private float zoomLowerLimit;
    [SerializeField] private float zoomUpperLimit;

    private Vector3 newPosition;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private void Start()
    {
        newPosition = transform.position;
        newZoom = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    private void HandleTouchInput() // Replace HandleMouseInput() with HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == UnityEngine.TouchPhase.Moved) // Check if touch is moving
            {
                newZoom += touch.deltaPosition.y * zoomAmount * 0.01f; // Scale touch delta by a factor to adjust zoom speed

                Vector3 touchWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane)); // Convert touch position to world position
                touchWorldPosition.y = transform.position.y; // Set the y position to match the camera's y position

                if (touch.deltaPosition.magnitude > 0.01f) // Check if touch has moved enough to be considered a drag
                {
                    if (touch.phase == UnityEngine.TouchPhase.Began) // Check if touch just started
                    {
                        dragStartPosition = touchWorldPosition;
                    }
                    else if (touch.phase == UnityEngine.TouchPhase.Moved) // Check if touch is still moving
                    {
                        dragCurrentPosition = touchWorldPosition;
                        newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                    }
                }
            }
        }
    }

    private void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry) && !DetectClickOnUI.IsPointerOverUIElement())
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if(Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry) && !DetectClickOnUI.IsPointerOverUIElement())
            {
                dragCurrentPosition = ray.GetPoint(entry);

                // Calculate the difference between the current drag position and the initial drag position
                Vector3 dragOffset = dragStartPosition - dragCurrentPosition;

                // Update the camera's position by adding the drag offset to the initial position
                newPosition = transform.position + dragOffset;
            }
        }
    }

    private void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.W))
        {
            newPosition += transform.forward * movementSpeed;
        }
        if(Input.GetKey(KeyCode.A))
        {
            newPosition += transform.right * -movementSpeed;
        }
        if(Input.GetKey(KeyCode.S))
        {
            newPosition += transform.forward * -movementSpeed;
        }
        if(Input.GetKey(KeyCode.D))
        {
            newPosition += transform.right * movementSpeed;
        }

        if(Input.GetKey(KeyCode.E))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newZoom -= zoomAmount;
        }

        newZoom.y = Mathf.Clamp(newZoom.y, zoomLowerLimit, zoomUpperLimit);
        newZoom.z = Mathf.Clamp(newZoom.z, -zoomUpperLimit, -zoomLowerLimit);

        transform.position = Vector3.Lerp(transform.position, newPosition, movementTime * Time.deltaTime);
        
        mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, newZoom, movementTime * Time.deltaTime);
    }
}

#region Old Code

/*public float cameraSpeed = 10f;
    public Transform cameraLimiterMin;
    public Transform cameraLimiterMax;

    private Vector3 lastMousePosition;
    private Vector3 desiredPosition;
    Vector3 mouseMovement;
    private UIManager uiManager;
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (uiManager.BuildingUI.activeSelf || uiManager.BuildingBuyUI.activeSelf || uiManager.BoostUI.activeSelf)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            mouseMovement = Input.mousePosition - lastMousePosition;

            mouseMovement.x = -mouseMovement.x;
            desiredPosition = transform.position;
            desiredPosition -= mouseMovement * cameraSpeed * Time.deltaTime;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, cameraLimiterMin.position.x, cameraLimiterMax.position.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, cameraLimiterMin.position.y, cameraLimiterMax.position.y);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, cameraLimiterMin.position.z, cameraLimiterMax.position.z);

            transform.position = desiredPosition;
        }

        lastMousePosition = Input.mousePosition;
    }*/

#endregion