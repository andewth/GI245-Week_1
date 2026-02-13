using UnityEngine;

public class MoveMarker : MonoBehaviour
{

    [SerializeField] float liftTime = 1f;


    void Start()
    {
        Destroy(gameObject, liftTime);
    }


}
