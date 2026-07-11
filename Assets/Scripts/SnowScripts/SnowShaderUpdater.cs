using UnityEngine;

public class SnowShaderUpdater : MonoBehaviour
{
    [Header("Shader")]
    public Material snowMaterial;

    public RenderTexture snowMaskRT;

    public Transform localSnowMesh;

    //public float worldSize = 20f;

    //public float snowDepth = 0.05f;

    void LateUpdate()
    {
        if (snowMaterial == null)
            return;

        snowMaterial.SetTexture(
            "_SnowMask",
            snowMaskRT
        );

        snowMaterial.SetVector(
            "_SnowCenter",
            localSnowMesh.position
        );

        /*
        snowMaterial.SetFloat(
            "_WorldSize",
            worldSize
        );*/

        /*
        snowMaterial.SetFloat(
            "_SnowDepth",
            snowDepth
        );*/
    }
}
