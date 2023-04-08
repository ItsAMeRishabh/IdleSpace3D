using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float cameraSpeed = 10f;
    private Vector3 lastMousePosition;
    Vector3 mouseMovement;
    private UIManager uiManager;
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (uiManager.BuildingUI.activeSelf || uiManager.BuildingBuyUI.activeSelf)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            mouseMovement = Input.mousePosition - lastMousePosition;

            mouseMovement.x = -mouseMovement.x;
            transform.position -= mouseMovement * cameraSpeed * Time.deltaTime;
        }

        lastMousePosition = Input.mousePosition;
    }
}
