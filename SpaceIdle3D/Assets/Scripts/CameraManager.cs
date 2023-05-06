using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private Vector3 buildingOffset;
    [SerializeField] private Vector3 movementLowerLimit;
    [SerializeField] private Vector3 movementUpperLimit;

    [Header("Zoom")]
    [SerializeField] private Vector3 zoomAmount;
    [SerializeField] private float buildingZoomLevel;
    [SerializeField] private float zoomLowerLimit;
    [SerializeField] private float zoomUpperLimit;

    private Vector3 newPosition;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private BuildingManager buildingManager;

    private void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        newPosition = transform.position;
        newZoom = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        if (buildingManager != null && buildingManager.selectedBuilding != null)
        {
            newPosition = buildingManager.selectedBuilding.transform.position + buildingOffset;

            newZoom = new Vector3(newZoom.x, buildingZoomLevel, -buildingZoomLevel);
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
            newZoom -= Input.mouseScrollDelta.y * zoomAmount;
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