using UnityEngine;

public class CameraBase : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private Transform corner1;
    [SerializeField] private Transform corner2;

    [Header("Zoom")]
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 5f;

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

    void Update()
    {
        MoveByKB();
        Zoom();
        // MoveByMouse();
    }

    private void MoveByKB()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

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
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.Z))
            zoomInput = -0.1f;
        if (Input.GetKey(KeyCode.X))
            zoomInput = 0.1f;

        cam.orthographicSize -= zoomInput * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 4f, 10f);
    }


    private void MoveByMouse()
    {
        if (Input.mousePosition.x >= Screen.width)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
    }
}
