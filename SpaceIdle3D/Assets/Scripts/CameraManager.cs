using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float cameraSpeed = 10f;
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
    }
}
