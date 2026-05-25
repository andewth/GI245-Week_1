using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBase : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private Transform corner1;
    [SerializeField] private Transform corner2;

    InputAction moveAction;
    Vector2 moveValue;

    [Header("Zoom")]
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed;

    InputAction zoomAction;
    Vector2 zoomValue;

    private float xInput;
    private float zInput;

    public static CameraBase instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (cam == null)
            cam = GetComponent<Camera>(); // กัน null
    }


    void Start()
    {
        moveSpeed = 25f;
        zoomSpeed = 0.05f;
        moveAction = InputSystem.actions.FindAction("Move");
        zoomAction = InputSystem.actions.FindAction("Zoom");
    }

    void Update()
    {
        MoveByKB();
        Zoom();
        // MoveByMouse();
    }

    private void MoveByKB()
    {
        // xInput = Input.GetAxis("Horizontal");
        // zInput = Input.GetAxis("Vertical");

        moveValue = moveAction.ReadValue<Vector2>();
        xInput = moveValue.x;
        zInput = moveValue.y;

        Vector3 dir = (transform.forward * zInput) + (transform.right * xInput);
        transform.position += dir * moveSpeed * Time.deltaTime;

        transform.position = ClampPosition(corner1.position, corner2.position);
    }

    private Vector3 ClampPosition(Vector3 a, Vector3 b)
    {
        float minX = Mathf.Min(a.x, b.x);
        float maxX = Mathf.Max(a.x, b.x);
        float minZ = Mathf.Min(a.z, b.z);
        float maxZ = Mathf.Max(a.z, b.z);

        return new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minZ, maxZ)
        );
    }



    private void Zoom()
    {
        if (Keyboard.current == null)
            return;

        zoomValue =  zoomAction.ReadValue<Vector2>();
        float zoomInput = zoomValue.y * 5f;

        if (Keyboard.current.zKey.isPressed)
            zoomInput = -1f;
        if (Keyboard.current.xKey.isPressed)
            zoomInput = 1f;

        cam.orthographicSize -= zoomInput * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 4f, 10f);
    }


    private void MoveByMouse()
    {
        if (Input.mousePosition.x >= Screen.width)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
    }
}
