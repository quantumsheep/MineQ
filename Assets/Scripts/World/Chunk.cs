using System.Collections;
using System.Collections.Generic;
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

    private ChunkMesh mesh;

    private MeshFilter filter;
    private new MeshCollider collider;
    private new MeshRenderer renderer;

    private Bounds bounds;

    private bool chunkLoaded = false;
    private bool shouldUpdateMesh = false;

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
        this.Init();

        this.mesh = new ChunkMesh(this.world, this.coordinates, this.blocks, this.RenderCallback);
    }

    private void RenderCallback()
    {
        this.shouldUpdateMesh = true;
    }

    void Update()
    {
        if (!this.chunkLoaded)
        {
            this.chunkLoaded = true;
            StartCoroutine(this.StartFirstRender());
        }

        if (this.shouldUpdateMesh)
        {
            this.shouldUpdateMesh = false;

            var mesh = new Mesh();
            mesh.vertices = this.mesh.vertices;
            mesh.triangles = this.mesh.triangles;
            mesh.uv = this.mesh.uv;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();
            mesh.uv = this.mesh.uv;

            this.filter.mesh = mesh;
            this.collider.sharedMesh = this.filter.mesh;
        }

        this.FrustrumCulling();
    }

    public void Init()
    {
        for (int x = 0; x < Chunk.Size; x++)
        {
            var worldX = x + this.transform.position.x;

            for (int z = 0; z < Chunk.Size; z++)
            {
                var worldZ = z + this.transform.position.z;

                var height = Mathf.FloorToInt(this.world.Noise(worldX, worldZ) * this.world.maximumLandHeight) + this.world.minimumSeeLevel;

                for (int y = 0; y < Chunk.Size; y++)
                {
                    var worldY = y + this.transform.position.y;

                    if (worldY <= height)
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
        var x = this.CorrectChunkCoordinate(coordinates.x);
        var y = this.CorrectChunkCoordinate(coordinates.y);
        var z = this.CorrectChunkCoordinate(coordinates.z);

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

        this.RenderChunk();
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
        return axis >= 0 ? axis : (axis + Chunk.Size);
    }

    private IEnumerator StartFirstRender()
    {
        this.RenderChunk();
        yield return null;

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 0, -1))?.NeighbourRenderChunk();
        yield return null;

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 0, 1))?.NeighbourRenderChunk();
        yield return null;

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, 1, 0))?.NeighbourRenderChunk();
        yield return null;

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(0, -1, 0))?.NeighbourRenderChunk();
        yield return null;

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(1, 0, 0))?.NeighbourRenderChunk();
        yield return null;

        this.world.GetChunkNoCheck(this.coordinates + new Vector3Int(-1, 0, 0))?.NeighbourRenderChunk();
        yield return null;
    }

    public void NeighbourRenderChunk()
    {
        if (this.chunkLoaded)
        {
            this.RenderChunk();
        }
    }

    public void RenderChunk()
    {
        this.mesh.StartRender();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
    }
}
