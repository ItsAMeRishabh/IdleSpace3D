using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private Vector3 movementLowerLimit;
    [SerializeField] private Vector3 movementUpperLimit;

    [Header("Zoom")]
    [SerializeField] private Vector3 zoomAmount;
    [SerializeField] private float zoomLowerLimit;
    [SerializeField] private float zoomUpperLimit;

    private Vector3 newPosition;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private Vector3 lastCameraPosition;
    private BuildingManager buildingManager;

    private void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        newPosition = transform.position;
        newZoom = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        if (buildingManager.selectedBuilding != null)
        {
            newPosition = buildingManager.selectedBuilding.transform.position;
        }
        else
        {
            HandleMouseInput();
        }

        LerpCamera();
    }

    private void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry) && !DetectClickOnUI.IsPointerOverUIElement())
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);


            if (plane.Raycast(ray, out float entry) && !DetectClickOnUI.IsPointerOverUIElement())
            {
                dragCurrentPosition = ray.GetPoint(entry);

                Vector3 dragOffset = dragStartPosition - dragCurrentPosition;

                // Update the camera's position by adding the drag offset to the initial position
                newPosition = transform.position + dragOffset;
                lastCameraPosition = newPosition;
            }
        }
    }

    private void LerpCamera()
    {
        newZoom.y = Mathf.Clamp(newZoom.y, zoomLowerLimit, zoomUpperLimit);
        newZoom.z = Mathf.Clamp(newZoom.z, -zoomUpperLimit, -zoomLowerLimit);

        newPosition.x = Mathf.Clamp(newPosition.x, movementLowerLimit.x, movementUpperLimit.x);
        newPosition.y = Mathf.Clamp(newPosition.y, movementLowerLimit.y, movementUpperLimit.y);
        newPosition.z = Mathf.Clamp(newPosition.z, movementLowerLimit.z, movementUpperLimit.z);

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