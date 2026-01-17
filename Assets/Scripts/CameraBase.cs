using UnityEngine;

public class CameraBase : MonoBehaviour
{
    [SerializeField] private CameraController cam;

    [Header("Move")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float xInput;
    [SerializeField] private float zInput;

    public static CameraBase instance;

    void Start()
    {
        moveSpeed = 50;
    }


    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }


    void Update()
    {
        MoveByKB();
    }


    private void MoveByKB()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        Vecter3 dir = (transform.forward * zInput) + (transform.right * xInput);
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
