using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Vector3Int size;
    public float noiseScale = 2.0f;
    public string worldSeed = "";
    public float seed = 0;

    public ItemTableScriptableObject itemTable;

    public GameObject chunkMeshPrefab;

    private Dictionary<Vector3Int, ChunkMesh> chunks = new Dictionary<Vector3Int, ChunkMesh>();

    private WorldManager manager;

    void Start()
    {
        this.manager = this.GetComponent<WorldManager>();

        this.itemTable.GenerateTextureAtlas();

        this.seed = this.GenerateSeed(this.worldSeed.GetHashCode());
        this.GenerateChunks();
    }

    private float GenerateSeed(int hashCode)
    {
        System.Random random = new System.Random(hashCode);
        return (float)(random.NextDouble() * (Int16.MaxValue - (double)Int16.MinValue)) + Int16.MinValue;
    }

    private void GenerateChunks()
    {
        var set = new HashSet<ChunkMesh>();

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.z; z++)
            {
                Debug.Log(this.Noise(x, z));
                var y = Mathf.FloorToInt(this.Noise(x, z) * size.y);

                // Grass
                set.Add(this.SetBlock("test:grass", new Vector3Int(x, y, z)));

                // Dirt
                var dirtLimit = y - 4;
                for (y -= 1; y >= dirtLimit && y >= 0; y--)
                {
                    var chunk = this.SetBlock("test:dirt", new Vector3Int(x, y, z));
                    set.Add(chunk);
                }

                // Stone
                for (; y >= 0; y--)
                {
                    var chunk = this.SetBlock("test:stone", new Vector3Int(x, y, z));
                    set.Add(chunk);
                }
            }
        }

        foreach (var chunk in set)
        {
            chunk.UpdateChunk();
        }
    }

    private float Noise(float x, float z)
    {
        return Mathf.PerlinNoise(((x / this.noiseScale) + this.seed), ((z / this.noiseScale) + this.seed));
    }

    public ChunkMesh SetBlock(string id, Vector3Int coordinates)
    {
        var item = this.itemTable.GetItem(id);
        var block = item.item as BlockScriptableObject;

        var chunk = this.GetChunkMesh(coordinates);

        var x = coordinates.x % 16;
        var y = coordinates.y % 16;
        var z = coordinates.z % 16;

        chunk.SetBlock(new Vector3Int(x, y, z), block);

        return chunk;
    }

    public ChunkMesh GetChunkMesh(Vector3Int blockCoordinates)
    {
        var chunkSize = WorldChunk.SizeFloored;

        var coordinates = blockCoordinates / chunkSize;

        if (!this.chunks.ContainsKey(coordinates))
        {
            var chunkCoordinates = coordinates * chunkSize;

            var obj = Instantiate(chunkMeshPrefab, chunkCoordinates, Quaternion.identity, this.transform);
            this.manager.AddObject(ref obj);

            var chunk = obj.GetComponent<ChunkMesh>();
            chunk.coordinates = chunkCoordinates;

            this.chunks.Add(coordinates, chunk);

            return chunk;
        }

        return this.chunks[coordinates];
    }
}
