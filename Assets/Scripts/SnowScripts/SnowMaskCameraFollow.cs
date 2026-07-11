using Unity.VisualScripting;
using UnityEngine;

public class SnowMaskCameraFollow : MonoBehaviour
{
    public Transform target;

    public float height = 20f;

    public float worldSize = 20f;

    public int textureResolution = 512;

    void LateUpdate()
    {
        if (target == null)
            return;

        float texelSize =
            worldSize / textureResolution;

        Vector3 pos =
            target.position;

        transform.position =
            new Vector3(
                pos.x,
                pos.y + height,
                pos.z
            ); ;
    }
}
