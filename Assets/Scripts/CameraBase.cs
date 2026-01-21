using UnityEngine;

public class CameraBase : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 50f;

    private float xInput;
    private float zInput;

    public static CameraBase instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        MoveByKB();
    }

    private void MoveByKB()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        Vector3 dir = (transform.forward * zInput) + (transform.right * xInput);
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
