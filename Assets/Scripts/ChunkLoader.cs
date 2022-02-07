using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    List<List<Chunk>> chunks;

    public GameObject chunkPrefab;

    // measured in chunks
    // should render the current chunk, and [renderDistance] chunks in all 4 cardinal directions, making a square
    public int renderDistance = 4;

    // the first chunk to load, from the south-west
    Vector3Int minChunk;

    // placeholder
    public Vector3 playerPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        InitChunks();
    }

    void InitChunks()
    {
        chunks = new List<List<Chunk>>();
        minChunk = new Vector3Int((int) playerPosition.x / 16 - renderDistance, 0, (int) playerPosition.y / 16 - renderDistance);
        for (int x = minChunk.x; x <= minChunk.x + 2 * renderDistance; x++)
        {
            List<Chunk> row = new List<Chunk>();
            for (int z = minChunk.z; z <= minChunk.z + 2 * renderDistance; z++)
            {
                // instantiate chunk GameObject
                GameObject gameObject = Instantiate(chunkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                // prep the Chunk script attached to the GameObject
                Chunk chunk = gameObject.GetComponent<Chunk>();
                chunk.cox = x * 16;
                chunk.coz = z * 16;
                row.Add(chunk);
            }
            chunks.Add(row);
        }
    }

    void UpdateChunkBounds()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // get player position
        // compute which chunks need to be loaded
        Vector3Int tempMinChunk = new Vector3Int((int) playerPosition.x / 16 - renderDistance, 0, (int) playerPosition.y / 16 - renderDistance);
        if (tempMinChunk != minChunk)
        {
            minChunk = tempMinChunk;
            // update the chunks
            
            // update the chunks inner neighboring references
        }
    }
}
