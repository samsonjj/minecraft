using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Dirt,
    None,
    Test,
}

[RequireComponent(typeof(MeshFilter))]
public class Chunk: MonoBehaviour
{
    static int WIDTH = 16;
    static int HEIGHT = 300;

    static int SEA_LEVEL = 64;

    // refs to neighbor chunks, used for reducing the mesh
    // can be null, in which case this chunk is on the edge of the map
    // if the chunk is on the edge, don't add that edge to the mesh
    public Chunk northChunk;
    public Chunk southChunk;
    public Chunk westChunk;
    public Chunk eastChunk;

    // chunk offset x
    public int cox;
    // chunk offset z
    public int coz;

    // stored x,y,z
    BlockType[,,] blocks;

    public Chunk(int cox, int coz): this(cox, coz, null, null, null, null) { }

    public Chunk(int cox, int coz, Chunk n, Chunk s, Chunk w, Chunk e)
    {
        this.cox = cox;
        this.coz = coz;
        this.northChunk = n;
        this.southChunk = s;
        this.westChunk = w;
        this.eastChunk = e;
    }

    // Start is called before the first frame update
    void Start()
    {
        blocks = new BlockType[WIDTH, HEIGHT, WIDTH];
        for (int x = 0; x < WIDTH; x++)
            for (int z = 0; z < WIDTH; z++)
                for (int y = 0; y < HEIGHT; y++)
                    blocks[x,y,z] = BlockType.None;

        for (int x = 0; x < WIDTH; x++)
        {
            for (int z = 0; z < WIDTH; z++)
            {
                int top_block = (int)(Mathf.PerlinNoise((x + cox) * .03f, (z + coz) * .03f) * 10f) + SEA_LEVEL;
                Debug.Log(top_block);
                Debug.Log(blocks.Length);
                Debug.Log(x + z * WIDTH + top_block * WIDTH * WIDTH);
                // blocks[x,0,z] = BlockType.Dirt;
                for (int y = top_block; y >= 0; y--) {
                    blocks[x,y,z] = BlockType.Dirt;
                }
            }
        }

        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateMesh();
    }

    void UpdateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // vertex index: keeps track of base index of the first vertex of the current block
        // this prevents us from having to do a bunch of multiplication, as long as we keep it updated
        for (int x = 0; x < WIDTH; x++)
        {
            for (int z = 0; z < WIDTH; z++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (blocks[x, y, z] == BlockType.None) continue;
                    if (blocks[x, y, z] == BlockType.Test) continue;
                    // top
                    if (y == HEIGHT-1 || blocks[x, y+1, z] == BlockType.None)
                    {
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + 1 + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + 1 + z));

                        triangles.AddRange(new int[] { vi, vi + 2, vi + 1, vi + 1, vi + 2, vi + 3 });
                    }
                    // bottom
                    if (y == 0 || blocks[x, y-1, z] == BlockType.None) {
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 1, vi + 2, vi + 1, vi + 3, vi + 2 });
                    }
                    // front
                    if (z == 0 || blocks[x,y,z-1] == BlockType.None) {
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z));

                        triangles.AddRange(new int[] { vi, vi + 2, vi + 1, vi + 1, vi + 2, vi + 3 });
                    }
                    // back
                    if (z == WIDTH-1 || blocks[x,y, z+1] == BlockType.None)
                    {
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 1, vi + 2, vi + 1, vi + 3, vi + 2 });
                    }
                    // left
                    if (x == 0 || blocks[x-1, y, z] == BlockType.None)
                    {
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 1, vi + 2, vi + 1, vi + 3, vi + 2 });
                    }
                    // right
                    if (x == WIDTH-1 || blocks[x+1,y,z] == BlockType.None) {
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 2, vi + 1, vi + 1, vi + 2, vi + 3 });
                    }
                }
            }
        }

        // uvs map 1 to 1 with vertices
        for (int i = 0; i < vertices.Count / 4; i++)
        {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
        };

        Mesh mesh = new Mesh();

        // Debug.Log("vertCount: " + vertCount);
        Debug.Log("vertices: " + vertices.ToArray().Length);
        Debug.Log("triangles: " + triangles.ToArray().Length);
        Debug.Log("uv: " + uvs.ToArray().Length);
        mesh.vertices   = vertices.ToArray();
        mesh.triangles  = triangles.ToArray();
        mesh.uv         = uvs.ToArray();

        // why is this throwing a resource of bounds error?
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
