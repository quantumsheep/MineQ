using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class ChunkMesh : MonoBehaviour
{
    private readonly int size = WorldChunk.SizeFloored;

    private World world;
    public Vector3Int coordinates;

    [HideInInspector]
    public BlockScriptableObject[,,] blocks = null;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uv = new List<Vector2>();

    private MeshFilter filter;
    private new MeshCollider collider;
    private new MeshRenderer renderer;

    void Awake()
    {
        this.blocks = new BlockScriptableObject[this.size, this.size, this.size];

        this.filter = this.GetComponent<MeshFilter>();
        this.collider = this.GetComponent<MeshCollider>();
        this.renderer = this.GetComponent<MeshRenderer>();
        this.world = this.transform.parent.GetComponent<World>();

        this.filter.mesh = new Mesh();
        this.collider.sharedMesh = this.filter.mesh;
    }

    void Start()
    {
        this.renderer.material = new Material(Shader.Find("Standard"))
        {
            mainTexture = this.world.itemTable.GetBlockAtlasTexture()
        };
    }

    void Update()
    {

    }

    public void BreakBlock(Vector3Int coordinates)
    {
        Debug.Log(coordinates);

        var x = coordinates.x;
        var y = coordinates.y;
        var z = coordinates.z;

        this.blocks[x, y, z] = null;
        this.UpdateChunk();
    }

    public void SetBlock(Vector3Int coordinates, BlockScriptableObject block)
    {
        var x = this.CorrectChunkCoordinate(coordinates.x);
        var y = this.CorrectChunkCoordinate(coordinates.y);
        var z = this.CorrectChunkCoordinate(coordinates.z);

        this.blocks[x, y, z] = block;
    }

    private int CorrectChunkCoordinate(int axis)
    {
        return axis >= 0 ? axis : (axis + this.size);
    }

    public void UpdateChunk()
    {
        this.filter.mesh = this.GenerateMesh();
        this.collider.sharedMesh = this.filter.mesh;
    }

    private Mesh GenerateMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        var max = this.size - 1;

        for (int x = 0; x < this.size; x++)
        {
            for (int y = 0; y < this.size; y++)
            {
                for (int z = 0; z < this.size; z++)
                {
                    var block = this.blocks[x, y, z];
                    if (block != null)
                    {
                        var hasFront = (z == 0) || (this.blocks[x, y, z - 1] == null);
                        var hasBack = (z == max) || (this.blocks[x, y, z + 1] == null);
                        var hasTop = (y == max) || (this.blocks[x, y + 1, z] == null);
                        var hasBottom = (y == 0) || (this.blocks[x, y - 1, z] == null);
                        var hasLeft = (x == max) || (this.blocks[x + 1, y, z] == null);
                        var hasRight = (x == 0) || (this.blocks[x - 1, y, z] == null);

                        var cube = this.GenerateMeshProperties(block, hasFront, hasBack, hasTop, hasBottom, hasLeft, hasRight);
                        var pos = new Vector3(x, y, z);

                        triangles.AddRange(cube.triangles.Select(t => t + vertices.Count).ToList());
                        vertices.AddRange(cube.vertices.Select(v => v + pos).ToList());
                        uv.AddRange(cube.uv);
                    }
                }
            }
        }

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        mesh.uv = uv.ToArray();

        return mesh;
    }

    public (List<Vector3> vertices, List<int> triangles, List<Vector2> uv) GenerateMeshProperties(BlockScriptableObject block, bool hasFront, bool hasBack, bool hasTop, bool hasBottom, bool hasLeft, bool hasRight)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        var cube = Cube.GenerateAsList(block.rects);

        if (hasBack)
        {
            vertices.AddRange(cube.vertices.Skip(0).Take(4));

            triangles.AddRange(cube.triangles.Skip(0).Take(6));
            uv.AddRange(cube.uv.Skip(0).Take(4));
        }
        else
        {
            this.RemoveVertices(0, 4, ref cube.triangles);
        }

        if (hasTop)
        {
            vertices.AddRange(cube.vertices.Skip(4).Take(4));

            triangles.AddRange(cube.triangles.Skip(6).Take(6));
            uv.AddRange(cube.uv.Skip(4).Take(4));
        }
        else
        {
            this.RemoveVertices(0, 4, ref cube.triangles);
        }

        if (hasFront)
        {
            vertices.AddRange(cube.vertices.Skip(8).Take(4));

            triangles.AddRange(cube.triangles.Skip(12).Take(6));
            uv.AddRange(cube.uv.Skip(8).Take(4));
        }
        else
        {
            this.RemoveVertices(0, 4, ref cube.triangles);
        }

        if (hasBottom)
        {
            vertices.AddRange(cube.vertices.Skip(12).Take(4));

            triangles.AddRange(cube.triangles.Skip(18).Take(6));
            uv.AddRange(cube.uv.Skip(12).Take(4));
        }
        else
        {
            this.RemoveVertices(0, 4, ref cube.triangles);
        }

        if (hasRight)
        {
            vertices.AddRange(cube.vertices.Skip(16).Take(4));

            triangles.AddRange(cube.triangles.Skip(24).Take(6));
            uv.AddRange(cube.uv.Skip(16).Take(4));
        }
        else
        {
            this.RemoveVertices(0, 4, ref cube.triangles);
        }

        if (hasLeft)
        {
            vertices.AddRange(cube.vertices.Skip(20).Take(4));

            triangles.AddRange(cube.triangles.Skip(30).Take(6));
            uv.AddRange(cube.uv.Skip(20).Take(4));
        }

        return (vertices, triangles, uv);
    }

    private void RemoveVertices(int index, int count, ref List<int> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i] > index)
            {
                triangles[i] -= count;
            }
        }
    }
}
