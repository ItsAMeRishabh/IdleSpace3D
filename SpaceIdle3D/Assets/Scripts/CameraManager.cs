using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Tutorial: https://www.youtube.com/watch?v=rnqF6S7PfFA

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    /*[SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;

    private Vector3 targetPosition;
    private Vector2 movementInput;

    private InputManager inputManager;

    private void Awake()
    {
        inputManager = new InputManager();
        targetPosition = transform.position;
    }

    private void OnEnable()
    {
        inputManager.Enable();

        inputManager.Camera.MoveDelta.started += OnMoveDelta;
        inputManager.Camera.MoveDelta.performed += OnMoveDelta;
        inputManager.Camera.MoveDelta.canceled += OnMoveDelta;
    }

    private void Update()
    {
        targetPosition += ;    
    }

    private void OnMoveDelta(InputAction.CallbackContext context)
    {

    }*/
    [SerializeField] private float moveSpeed = 50;
    [SerializeField] private float moveSmooth = 5;

    private Camera mainCamera;
    private InputManager inputManager;
    private bool isMoving = false;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 skewedInput = Vector2.zero;

    private Transform root = null;
    private Transform pivot = null;
    private Transform target = null;

    private Vector3 center = Vector3.zero;
    private float right = 10;
    private float left = 10;
    private float up = 10;
    private float down = 10;
    private Vector3 angle = Vector3.zero;

    private Matrix4x4 matrix = Matrix4x4.identity;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        inputManager = new InputManager();

        root = new GameObject("CameraHelper").transform;
        pivot = new GameObject("CameraPivot").transform;
        target = new GameObject("CameraTarget").transform;

        angle = transform.rotation.eulerAngles;
        matrix = Matrix4x4.Rotate(transform.rotation);
    }

    private void OnEnable()
    {
        inputManager.Enable();
        inputManager.Camera.Move.started += MoveStarted;
        inputManager.Camera.Move.canceled += MoveReleased;
    }

    private void Start()
    {
        Initialize(Vector3.zero, 10, 10, 10, 10, angle);
    }

    private void Update()
    {
        if (isMoving)
        {
            movementInput = inputManager.Camera.MoveDelta.ReadValue<Vector2>();

            if (movementInput != Vector2.zero)
            {
                movementInput.x /= Screen.width;
                movementInput.y /= Screen.height;

                skewedInput = matrix.MultiplyPoint3x4(movementInput);
                skewedInput.Normalize();
                Debug.Log($"Input: {movementInput}, Skewed Input: {skewedInput}");

                root.position += root.right.normalized * skewedInput.x * moveSpeed;
                root.position += root.forward.normalized * skewedInput.y * moveSpeed;
            }

            if (mainCamera.transform.position != target.position)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position, moveSmooth * Time.deltaTime);
            }

            if (mainCamera.transform.rotation != target.rotation)
            {
                mainCamera.transform.rotation = target.rotation;
            }
        }
    }

    private void OnDisable()
    {
        inputManager.Disable();
    }

    private void Initialize(Vector3 center, float right, float left, float up, float down, Vector3 angle)
    {
        this.center = center;
        this.right = right;
        this.left = left;
        this.up = up;
        this.down = down;
        this.angle = angle;

        isMoving = false;
        pivot.SetParent(root);
        target.SetParent(pivot);

        root.position = center;
        root.localEulerAngles = Vector3.zero;

        pivot.localPosition = Vector3.zero;
        pivot.localEulerAngles = angle;

        target.localPosition = new Vector3(0, 0, -10);
        target.localEulerAngles = Vector3.zero;
    }

    private void MoveStarted(InputAction.CallbackContext context)
    {
        isMoving = true;
    }

    private void MoveReleased(InputAction.CallbackContext context)
    {
        isMoving = false;
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

#region Bad Tutorial Code

/*[SerializeField] private float moveSpeed = 50;
[SerializeField] private float moveSmooth = 5;

private Camera mainCamera;
private InputManager inputManager;
private bool isMoving = false;
private Vector2 movementInput = Vector2.zero;
private Vector2 skewedInput = Vector2.zero;

private Transform root = null;
private Transform pivot = null;
private Transform target = null;

private Vector3 center = Vector3.zero;
private float right = 10;
private float left = 10;
private float up = 10;
private float down = 10;
private Vector3 angle = Vector3.zero;

private Matrix4x4 matrix = Matrix4x4.identity;

private void Awake()
{
    mainCamera = GetComponent<Camera>();
    inputManager = new InputManager();

    root = new GameObject("CameraHelper").transform;
    pivot = new GameObject("CameraPivot").transform;
    target = new GameObject("CameraTarget").transform;

    angle = transform.rotation.eulerAngles;
    matrix = Matrix4x4.Rotate(Quaternion.Euler(angle));
}

private void OnEnable()
{
    inputManager.Enable();
    inputManager.Camera.Move.started += MoveStarted;
    inputManager.Camera.Move.canceled += MoveReleased;
}

private void Start()
{
    Initialize(Vector3.zero, 10, 10, 10, 10, angle);
}

private void Update()
{
    if (isMoving)
    {
        movementInput = inputManager.Camera.MoveDelta.ReadValue<Vector2>();
        //Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(angle));


        if (movementInput != Vector2.zero)
        {
            skewedInput = matrix.MultiplyPoint3x4(movementInput);
            skewedInput.Normalize();
            Debug.Log($"Input: {movementInput}, Skewed Input: {skewedInput}");

            movementInput.x /= Screen.width;
            movementInput.y /= Screen.height;

            root.position += root.right.normalized * skewedInput.x * moveSpeed;
            root.position += root.forward.normalized * skewedInput.y * moveSpeed;
        }

        if (mainCamera.transform.position != target.position)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position, moveSmooth * Time.deltaTime);
        }

        if (mainCamera.transform.rotation != target.rotation)
        {
            mainCamera.transform.rotation = target.rotation;
        }
    }
}

private void OnDisable()
{
    inputManager.Disable();
}

private void Initialize(Vector3 center, float right, float left, float up, float down, Vector3 angle)
{
    this.center = center;
    this.right = right;
    this.left = left;
    this.up = up;
    this.down = down;
    this.angle = angle;

    isMoving = false;
    pivot.SetParent(root);
    target.SetParent(pivot);

    root.position = center;
    root.localEulerAngles = Vector3.zero;

    pivot.localPosition = Vector3.zero;
    pivot.localEulerAngles = angle;

    target.localPosition = new Vector3(0, 0, -10);
    target.localEulerAngles = Vector3.zero;
}

private void MoveStarted(InputAction.CallbackContext context)
{
    isMoving = true;
}

private void MoveReleased(InputAction.CallbackContext context)
{
    isMoving = false;
}*/

#endregion