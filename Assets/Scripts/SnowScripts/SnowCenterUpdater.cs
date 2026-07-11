using UnityEngine;

public class SnowCenterUpdater : MonoBehaviour
{
    public Material snowMaterial;

    public Transform snowMaskCamera;

    void LateUpdate()
    {
        Vector3 center =
            snowMaskCamera.position;

        snowMaterial.SetVector(
            "_SnowCenter",
            transform.position
        );
    }
}
