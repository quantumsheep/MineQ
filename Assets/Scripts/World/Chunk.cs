using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public static readonly int Size = 16;

    private World world;

    [HideInInspector]
    public Vector3Int coordinates;

    [HideInInspector]
    public BlockScriptableObject[,,] blocks = null;

    private ChunkNeighbours neighbours;

    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uv;
    public int verticesIndex = 0;

    private MeshFilter filter;
    private new MeshCollider collider;
    private new MeshRenderer renderer;

    private Bounds bounds;

    public bool loaded = false;

    void Awake()
    {
        this.blocks = new BlockScriptableObject[Chunk.Size, Chunk.Size, Chunk.Size];

        this.world = this.transform.parent.GetComponent<World>();

        this.filter = this.GetComponent<MeshFilter>();
        this.collider = this.GetComponent<MeshCollider>();
        this.renderer = this.GetComponent<MeshRenderer>();

        this.CreateBoundsBox();

        this.filter.mesh = new Mesh();
        this.collider.sharedMesh = this.filter.mesh;
    }

    void Start()
    {
        this.renderer.material.mainTexture = this.world.itemTable.GetBlockAtlasTexture();

        this.neighbours = new ChunkNeighbours(this.world, this.coordinates);

        this.Init();
    }

    void Update()
    {
        // this.FrustrumCulling();
    }

    public void Init()
    {
        var worldCoordinates = this.coordinates * Chunk.Size;

        for (int x = 0; x < Chunk.Size; x++)
        {
            var worldX = x + worldCoordinates.x;

            for (int z = 0; z < Chunk.Size; z++)
            {
                var worldZ = z + worldCoordinates.z;

                var height = Mathf.FloorToInt(this.world.Noise(worldX, worldZ) * this.world.maximumLandHeight) + this.world.minimumSeeLevel;

                for (int y = 0; y < Chunk.Size; y++)
                {
                    var worldY = y + worldCoordinates.y;

                    if (worldY <= height && this.world.Noise(worldX, worldZ, worldY) <= 0.3f)
                    {
                        if (worldY == height)
                        {
                            var block = this.world.itemTable.GetBlock("test:grass");
                            this.SetBlock(new Vector3Int(x, y, z), block);
                        }
                        else if (worldY >= (height - 4))
                        {
                            var block = this.world.itemTable.GetBlock("test:dirt");
                            this.SetBlock(new Vector3Int(x, y, z), block);
                        }
                        else
                        {
                            var block = this.world.itemTable.GetBlock("test:stone");
                            this.SetBlock(new Vector3Int(x, y, z), block);
                        }
                    }
                }
            }
        }

        this.loaded = true;
        this.StartFirstRender();
    }

    private void CreateBoundsBox()
    {
        var chunkSize = Chunk.Size;
        var axis = chunkSize / 2.0f - 0.5f;

        var center = new Vector3(axis, axis, axis) + this.transform.position;
        var size = new Vector3(chunkSize, chunkSize, chunkSize);

        this.bounds = new Bounds(center, size);
    }

    public void FrustrumCulling()
    {
        var visible = Vector3.Distance(this.bounds.center, Player.main.transform.position) <= (this.world.viewDistance * Chunk.Size);

        if (visible)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            visible = GeometryUtility.TestPlanesAABB(planes, this.bounds);
        }

        this.renderer.enabled = visible;
    }

    public void BreakBlock(Vector3Int coordinates)
    {
        var x = Chunk.CorrectBlockCoordinate(coordinates.x);
        var y = Chunk.CorrectBlockCoordinate(coordinates.y);
        var z = Chunk.CorrectBlockCoordinate(coordinates.z);

        this.blocks[x, y, z] = null;

        var max = Chunk.Size - 1;

        if (x == 0)
        {
            var neighbour = this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(-1, 0, 0));
            neighbour?.NeighbourRenderChunk();
        }
        else if (x == max)
        {
            var neighbour = this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(1, 0, 0));
            neighbour?.NeighbourRenderChunk();
        }

        if (y == 0)
        {
            var neighbour = this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, -1, 0));
            neighbour?.NeighbourRenderChunk();
        }
        else if (y == max)
        {
            var neighbour = this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 1, 0));
            neighbour?.NeighbourRenderChunk();
        }

        if (z == 0)
        {
            var neighbour = this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 0, -1));
            neighbour?.NeighbourRenderChunk();
        }
        else if (z == max)
        {
            var neighbour = this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 0, 1));
            neighbour?.NeighbourRenderChunk();
        }

        StartCoroutine(this.GenerateMesh());
    }

    public void SetBlock(Vector3Int coordinates, BlockScriptableObject block)
    {
        var x = Chunk.CorrectBlockCoordinate(coordinates.x);
        var y = Chunk.CorrectBlockCoordinate(coordinates.y);
        var z = Chunk.CorrectBlockCoordinate(coordinates.z);

        this.blocks[x, y, z] = block;
    }

    public static int CorrectBlockCoordinate(int axis)
    {
        return axis >= 0 ? axis : (axis + Chunk.Size);
    }

    private void StartFirstRender()
    {
        StartCoroutine(this.GenerateMesh());

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 0, -1))?.NeighbourRenderChunk();
        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 0, 1))?.NeighbourRenderChunk();
        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 1, 0))?.NeighbourRenderChunk();
        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, -1, 0))?.NeighbourRenderChunk();
        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(1, 0, 0))?.NeighbourRenderChunk();
        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(-1, 0, 0))?.NeighbourRenderChunk();
    }

    public void NeighbourRenderChunk()
    {
        if (this.loaded)
        {
            StartCoroutine(this.GenerateMesh());
        }
    }

    public IEnumerator GenerateMesh()
    {
        this.neighbours.Update();

        yield return null;

        this.vertices = new List<Vector3>();
        this.triangles = new List<int>();
        this.uv = new List<Vector2>();

        this.verticesIndex = 0;

        for (int x = 0; x < Chunk.Size; x++)
        {
            for (int y = 0; y < Chunk.Size; y++)
            {
                for (int z = 0; z < Chunk.Size; z++)
                {
                    var block = this.blocks[x, y, z];
                    if (block != null)
                    {
                        var position = new Vector3(x, y, z);
                        this.GenerateBlock(block, position, this.CheckBlock(x, y, z));
                    }
                }
            }
        }

        yield return null;

        var mesh = new Mesh();
        mesh.vertices = this.vertices.ToArray();
        mesh.triangles = this.triangles.ToArray();
        mesh.uv = this.uv.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        // mesh.uv = this.mesh.uv;

        this.filter.mesh = mesh;
        this.collider.sharedMesh = this.filter.mesh;
    }

    private bool[] CheckBlock(int x, int y, int z)
    {
        var max = Chunk.Size - 1;

        var hasBack = (z > 0) ? (this.blocks[x, y, z - 1] == null) : (this.neighbours.front?.loaded == true && this.neighbours.front.blocks[x, y, max] == null);
        var hasFront = (z < max) ? (this.blocks[x, y, z + 1] == null) : (this.neighbours.back?.loaded == true && this.neighbours.back.blocks[x, y, 0] == null);
        var hasTop = (y < max) ? (this.blocks[x, y + 1, z] == null) : (this.neighbours.top?.loaded == true && this.neighbours.top.blocks[x, 0, z] == null);
        var hasBottom = (y > 0) ? (this.blocks[x, y - 1, z] == null) : (this.neighbours.bottom?.loaded == true && this.neighbours.bottom.blocks[x, max, z] == null);
        var hasLeft = (x > 0) ? (this.blocks[x - 1, y, z] == null) : (this.neighbours.right?.loaded == true && this.neighbours.right.blocks[max, y, z] == null);
        var hasRight = (x < max) ? (this.blocks[x + 1, y, z] == null) : (this.neighbours.left?.loaded == true && this.neighbours.left.blocks[0, y, z] == null);

        return new bool[] {
            hasBack,
            hasFront,
            hasTop,
            hasBottom,
            hasLeft,
            hasRight,
        };
    }

    private void GenerateBlock(BlockScriptableObject block, Vector3 position, bool[] checks)
    {
        for (int i = 0; i < 6; i++)
        {
            if (checks[i])
            {
                this.vertices.Add(position + Cube.vertices[Cube.triangles[i, 0]]);
                this.vertices.Add(position + Cube.vertices[Cube.triangles[i, 1]]);
                this.vertices.Add(position + Cube.vertices[Cube.triangles[i, 2]]);
                this.vertices.Add(position + Cube.vertices[Cube.triangles[i, 3]]);

                this.triangles.Add(verticesIndex + 0);
                this.triangles.Add(verticesIndex + 1);
                this.triangles.Add(verticesIndex + 2);
                this.triangles.Add(verticesIndex + 2);
                this.triangles.Add(verticesIndex + 1);
                this.triangles.Add(verticesIndex + 3);

                var texture = block.rects[i];

                this.uv.Add(new Vector2(texture.xMax, texture.yMin));
                this.uv.Add(new Vector2(texture.xMax, texture.yMax));
                this.uv.Add(new Vector2(texture.xMin, texture.yMin));
                this.uv.Add(new Vector2(texture.xMin, texture.yMax));

                this.verticesIndex += 4;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
    }
}
