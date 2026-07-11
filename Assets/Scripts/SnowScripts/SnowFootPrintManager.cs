using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static SnowFootPrintManager;

public class SnowFootPrintManager : MonoBehaviour
{
    public static SnowFootPrintManager Instance;

    [Header("References")]
    public Transform localSnowMesh;

    [Header("Footprint Rendering")]
    public GameObject footprintPrefab;
    public float localRadius = 12f;
    public int maxVisibleFootprints = 200;

    public float footprintLifetime = 120f;

    [Header("Chunk Settings")]
    public float chunkSize = 16f;

    //World footprints storage
    //List<footPrintData> footprints = new List<footPrintData>();

    //Chunk Storage
    Dictionary<Vector2Int,List<footPrintData>>
    chunks = new Dictionary< Vector2Int,List<footPrintData>>();

    //Active Quads
    List<GameObject> activeQuads = new List<GameObject>();

    // PROPERTY BLOCK
    MaterialPropertyBlock propertyBlock;

    [System.Serializable]
    public class footPrintData
    {
        public Vector3 position;

        public Vector3 scale;

        public Quaternion rotation;

        public float strength;

        public float createTime;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();

        // CREATE POOL
        for (int i = 0; i < maxVisibleFootprints; i++)
        {
            GameObject quad = Instantiate(footprintPrefab);

            quad.SetActive(false);

            activeQuads.Add(quad);
        }

        InvokeRepeating(nameof(CleanupOldFootprints),1f,1f);
    }

    void Update()
    {
        //CleanupOldFootprints();
        DrawNearbyFootprints();
    }

    public void AddFootprint(Vector3 pos, Vector3 scale, Quaternion rot)
    {
        footPrintData data = new footPrintData();

        data.position = pos;

        data.scale = scale;

        data.rotation = rot;

        data.createTime = Time.time;

        data.strength = 1f;

        //footprints.Add(data);

        //locate chunk
        Vector2Int coord = GetChunkCoord(pos);

        //create chunk if needed
        if (!chunks.ContainsKey(coord))
        {
            chunks[coord] = new List<footPrintData>();
        }

        chunks[coord].Add(data);
    }

    void CleanupOldFootprints()
    {
        float currentTime =
            Time.time;

        List<Vector2Int> emptyChunks =
            new List<Vector2Int>();

        foreach (var pair in chunks)
        {
            List<footPrintData> chunk =pair.Value;

            while (chunk.Count > 0 && currentTime - chunk[0].createTime > footprintLifetime)
            {
                chunk.RemoveAt(0);
            }

            // MARK EMPTY CHUNKS
            if (chunk.Count == 0)
            {
                emptyChunks.Add(pair.Key);
            }
        }

        // REMOVE EMPTY CHUNKS
        foreach (Vector2Int coord in emptyChunks)
        {
            chunks.Remove(coord);
        }
    }

    void DrawNearbyFootprints()
    {
        // HIDE ALL QUADS FIRST
        foreach (GameObject quad in activeQuads)
        {
            quad.SetActive(false);
        }

        Vector3 center = localSnowMesh.position;
        float radius = localRadius;

        int chunkRadius = Mathf.CeilToInt(radius / chunkSize);

        Vector2Int centerChunk = GetChunkCoord(center);

        int activeIndex = 0;

        // SEARCH NEARBY CHUNKS
        for (int x = -chunkRadius; x <= chunkRadius; x++)
        {
            for (int z = -chunkRadius; z <= chunkRadius; z++)
            {
                Vector2Int coord = new Vector2Int(centerChunk.x + x,centerChunk.y + z );

                // CHUNK DOESN'T EXIST
                if (!chunks.ContainsKey(coord))
                    continue;

                List<footPrintData> chunk = chunks[coord];

                foreach (footPrintData fp in chunk)
                {
                    // OUT OF QUADS
                    if (activeIndex >= activeQuads.Count)
                    {
                        return;
                    }

                    float dist = Vector3.Distance(center, fp.position);

                    // OUTSIDE LOCAL RADIUS
                    if (dist > radius)
                        continue;

                    GameObject quad = activeQuads[activeIndex];

                    quad.SetActive(true);

                    // TRANSFORM
                    quad.transform.position = fp.position;

                    quad.transform.rotation = fp.rotation;

                    quad.transform.localScale = fp.scale;

                    // FOOTPRINT AGE
                    float age = Time.time - fp.createTime;

                    float fade = Mathf.Clamp01(1f - age / footprintLifetime);

                    fade *= fp.strength;

                    // APPLY COLOR
                    MeshRenderer renderer = quad.GetComponent<MeshRenderer>();

                    renderer.GetPropertyBlock(propertyBlock);

                    Color color = new Color(fade, fade, fade, fade);

                    propertyBlock.SetColor("_Color", color);

                    renderer.SetPropertyBlock(propertyBlock);

                    activeIndex++;
                }
            }
        }
    }

    Vector2Int GetChunkCoord(
        Vector3 pos
    )
    {
        return new Vector2Int(
            Mathf.FloorToInt(
                pos.x / chunkSize
            ),

            Mathf.FloorToInt(
                pos.z / chunkSize
            )
        );
    }

}
