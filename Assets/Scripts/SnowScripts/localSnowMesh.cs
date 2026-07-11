using UnityEngine;

public class LocalSnowMesh : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Settings")]
    public float gridSnapSize = 1f;
    public float meshHeightOffset = 0.02f;

    [Header("Optional Terrain")]
    public Terrain terrain;

    void Update()
    {
        if (player == null) return;

        Vector3 targetPos = player.position;

        // Snap to grid for stability
        
        float snappedX =
            Mathf.Floor(targetPos.x / gridSnapSize)
            * gridSnapSize;

        float snappedZ =
            Mathf.Floor(targetPos.z / gridSnapSize)
            * gridSnapSize;
        

        float y = 0f;

        // Match terrain height if available
        if (terrain != null)
        {
            y = terrain.SampleHeight(
                new Vector3(targetPos.x, 0, targetPos.z)
            );

            y += terrain.transform.position.y;
        }

        transform.position = new Vector3(
            targetPos.x,
            y + meshHeightOffset,
            targetPos.z
        );
    }
}
