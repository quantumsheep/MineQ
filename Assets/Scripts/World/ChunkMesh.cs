using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;

public class ChunkMesh
{
    public delegate void MeshRenderCallback();

    private MeshRenderCallback renderCallback = null;

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

    private World world;
    private Vector3Int coordinates;

    private ChunkNeighbours neighbours;

    private BlockScriptableObject[,,] blocks;

    public ChunkMesh(World world, Vector3Int chunkCoordinates, BlockScriptableObject[,,] blocks, MeshRenderCallback renderCallback)
    {
        this.world = world;
        this.coordinates = chunkCoordinates;
        this.blocks = blocks;

        this.renderCallback = renderCallback;

        this.neighbours = new ChunkNeighbours(this.world, this.coordinates);
    }

    public void StartRender()
    {
        var thread = new Thread(this.GenerateMeshThread);
        thread.Start();
    }

    private void GenerateMeshThread()
    {
        this.neighbours.Update();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        var max = Chunk.Size - 1;

        for (int x = 0; x < Chunk.Size; x++)
        {
            for (int y = 0; y < Chunk.Size; y++)
            {
                for (int z = 0; z < Chunk.Size; z++)
                {
                    var block = this.blocks[x, y, z];
                    if (block != null)
                    {
                        var hasFront = (z > 0) ? (this.blocks[x, y, z - 1] == null) : (this.neighbours.front?.blocks[x, y, max] == null);
                        var hasBack = (z < max) ? (this.blocks[x, y, z + 1] == null) : (this.neighbours.back?.blocks[x, y, 0] == null);
                        var hasTop = (y < max) ? (this.blocks[x, y + 1, z] == null) : (this.neighbours.top?.blocks[x, 0, z] == null);
                        var hasBottom = (y > 0) ? (this.blocks[x, y - 1, z] == null) : (this.neighbours.bottom?.blocks[x, max, z] == null);
                        var hasLeft = (x < max) ? (this.blocks[x + 1, y, z] == null) : (this.neighbours.left?.blocks[0, y, z] == null);
                        var hasRight = (x > 0) ? (this.blocks[x - 1, y, z] == null) : (this.neighbours.right?.blocks[max, y, z] == null);

                        if (hasFront || hasBack || hasTop || hasBottom || hasLeft || hasRight)
                        {
                            var cube = this.GenerateBlock(block, hasFront, hasBack, hasTop, hasBottom, hasLeft, hasRight);
                            var pos = new Vector3(x, y, z);

                            triangles.AddRange(cube.triangles.Select(t => t + vertices.Count).ToList());
                            vertices.AddRange(cube.vertices.Select(v => v + pos).ToList());
                            uv.AddRange(cube.uv);
                        }
                    }
                }
            }
        }

        this.vertices = vertices.ToArray();
        this.triangles = triangles.ToArray();
        this.uv = uv.ToArray();

        this.renderCallback();
    }

    private (List<Vector3> vertices, List<int> triangles, List<Vector2> uv) GenerateBlock(BlockScriptableObject block, bool hasFront, bool hasBack, bool hasTop, bool hasBottom, bool hasLeft, bool hasRight)
    {
        var cube = Cube.Generate(block.rects);

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

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
            this.RemoveVertices(4, 4, ref cube.triangles);
        }

        if (hasFront)
        {
            vertices.AddRange(cube.vertices.Skip(8).Take(4));

            triangles.AddRange(cube.triangles.Skip(12).Take(6));
            uv.AddRange(cube.uv.Skip(8).Take(4));
        }
        else
        {
            this.RemoveVertices(8, 4, ref cube.triangles);
        }

        if (hasBottom)
        {
            vertices.AddRange(cube.vertices.Skip(12).Take(4));

            triangles.AddRange(cube.triangles.Skip(18).Take(6));
            uv.AddRange(cube.uv.Skip(12).Take(4));
        }
        else
        {
            this.RemoveVertices(12, 4, ref cube.triangles);
        }

        if (hasRight)
        {
            vertices.AddRange(cube.vertices.Skip(16).Take(4));

            triangles.AddRange(cube.triangles.Skip(24).Take(6));
            uv.AddRange(cube.uv.Skip(16).Take(4));
        }
        else
        {
            this.RemoveVertices(16, 4, ref cube.triangles);
        }

        if (hasLeft)
        {
            vertices.AddRange(cube.vertices.Skip(20).Take(4));

            triangles.AddRange(cube.triangles.Skip(30).Take(6));
            uv.AddRange(cube.uv.Skip(20).Take(4));
        }

        return (vertices, triangles, uv);
    }

    private void RemoveVertices(int index, int count, ref int[] triangles)
    {
        for (int i = (index / 4) * 6; i < triangles.Length; i++)
        {
            triangles[i] -= count;
        }
    }
}
