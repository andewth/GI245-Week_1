using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] GameObject doubleRingMarker;
    public GameObject DoubleRingMarker { get { return doubleRingMarker; } }

    public static VFXManager Instance;

    void Start()
    {
        Instance = this;
    }


    void Update()
    {
        
    }
}
