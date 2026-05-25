using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    private const float WarpCooldown = 1f;

    [SerializeField]
    private string toMapName;

    [SerializeField]
    private int enterPointId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !Settings.isChangingMap && Time.time >= Settings.nextWarpTime)
        {
            Debug.Log("Player enters Warp");
            Settings.nextWarpTime = Time.time + WarpCooldown;
            MapManager.instance.GoToMap(toMapName, enterPointId);
        }
    }
}