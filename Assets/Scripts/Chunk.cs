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

    // chunk offset x
    public int cox;
    // chunk offset z
    public int coz;

    // stored x,z,y
    BlockType[,,] blocks;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new BlockType[WIDTH, WIDTH, HEIGHT];
        for (int x = 0; x < WIDTH; x++)
            for (int z = 0; z < WIDTH; z++)
                for (int y = 0; y < HEIGHT; y++)
                    blocks[x,z,y] = BlockType.None;

        for (int x = 0; x < WIDTH; x++)
        {
            for (int z = 0; z < WIDTH; z++)
            {
                int top_block = (int)(Mathf.PerlinNoise((x + cox) * .03f, (z + coz) * .03f) * 10f) + SEA_LEVEL;
                Debug.Log(top_block);
                Debug.Log(blocks.Length);
                Debug.Log(x + z * WIDTH + top_block * WIDTH * WIDTH);
                // blocks[x,z,0] = BlockType.Dirt;
                for (int y = top_block; y >= 0; y--) {
                    blocks[x,z,y] = BlockType.Dirt;
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
                    if (blocks[x, z, y] == BlockType.None) continue;
                    if (blocks[x, z, y] == BlockType.Test) continue;
                    // top
                    if (y == HEIGHT-1 || blocks[x, z, y+1] == BlockType.None)
                    {
                        int vi = vertices.Count;

                        // top
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + 1 + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + 1 + z));

                        triangles.AddRange(new int[] { vi, vi + 2, vi + 1, vi + 1, vi + 2, vi + 3 });
                    }
                    if (y == 0 || blocks[x, z, y-1] == BlockType.None) {
                        int vi = vertices.Count;

                        // bottom
                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 1, vi + 2, vi + 1, vi + 3, vi + 2 });
                    }
                    if (z == 0 || blocks[x,z-1,y] == BlockType.None) {
                        // sides
                        // front
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z));

                        triangles.AddRange(new int[] { vi, vi + 2, vi + 1, vi + 1, vi + 2, vi + 3 });
                    }
                    if (z == WIDTH-1 || blocks[x,z+1,y] == BlockType.None)
                    {
                        // back
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x + 1, 1 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 1, vi + 2, vi + 1, vi + 3, vi + 2 });
                    }
                    if (x == 0 || blocks[x-1, z, y] == BlockType.None)
                    {
                        // left
                        int vi = vertices.Count;

                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 0 + y, coz + z + 1));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z));
                        vertices.Add(new Vector3(cox + x, 1 + y, coz + z + 1));

                        triangles.AddRange(new int[] { vi, vi + 1, vi + 2, vi + 1, vi + 3, vi + 2 });
                    }
                    if (x == WIDTH-1 || blocks[x+1,z,y] == BlockType.None) {
                        // right
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
