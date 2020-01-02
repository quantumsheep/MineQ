using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Vector2Int size;

    [Header("World Gen - Noise")]
    public float noiseScale = 2.0f;
    public string worldSeed = "";
    public float seed = 0;

    [Header("World Gen")]
    public int minimumSeeLevel = 40;
    public int maximumLandHeight = 80;

    [Header("Misc")]
    public int renderDistance = 5;
    public int viewDistance = 10;
    public ItemTableScriptableObject itemTable;

    public GameObject ChunkPrefab;

    public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    private Vector3Int lastPlayerChunk;

    void Start()
    {
        this.itemTable.GenerateTextureAtlas();

        this.lastPlayerChunk = this.WorldCoordinateToChunk(Player.main.transform.position);

        this.seed = this.GenerateSeed(this.worldSeed.GetHashCode());
        StartCoroutine(this.GenerateChunks());
    }

    void Update()
    {
        var currentPlayerChunk = this.WorldCoordinateToChunk(Player.main.transform.position);

        if (this.lastPlayerChunk != currentPlayerChunk)
        {
            Debug.Log("Generating new chunks");

            this.lastPlayerChunk = currentPlayerChunk;

            this.DestroyOutOfRangeChunks();
            StartCoroutine(this.GenerateChunks());
        }
    }

    private float GenerateSeed(int hashCode)
    {
        System.Random random = new System.Random(hashCode);
        return (float)(random.NextDouble() * (Int16.MaxValue - (double)Int16.MinValue)) + Int16.MinValue;
    }

    private void DestroyOutOfRangeChunks()
    {
        var toRemove = new List<Vector3Int>();

        foreach (var chunk in chunks.Values)
        {
            var coordinates = chunk.coordinates;
            if (this.IsOutOfRange(coordinates))
            {
                Destroy(chunk.gameObject);
                toRemove.Add(coordinates);
            }
        }

        foreach (var i in toRemove)
        {
            this.chunks.Remove(i);
        }
    }

    private bool IsOutOfRange(Vector3Int chunkCoordinates)
    {
        return Vector3Int.Distance(this.lastPlayerChunk, chunkCoordinates) > (this.viewDistance * 2);
    }

    public Vector3Int WorldCoordinateToChunk(Vector3 coordinates)
    {
        coordinates /= Chunk.Size;

        return new Vector3Int(
            Mathf.FloorToInt(coordinates.x),
            Mathf.FloorToInt(coordinates.y),
            Mathf.FloorToInt(coordinates.z)
        );
    }

    private IEnumerator GenerateChunks()
    {
        var radius = this.renderDistance;
        var center = this.lastPlayerChunk;

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    var coordinates = new Vector3Int(x, y, z) + center;

                    if (!this.IsOutOfRange(coordinates) && coordinates.y >= 0)
                    {
                        this.GetChunk(coordinates);
                        yield return null;
                    }
                }
            }
        }
    }

    public float Noise(float x, float z)
    {
        return Mathf.PerlinNoise(((x / this.noiseScale) + this.seed), ((z / this.noiseScale) + this.seed));
    }

    public Chunk SetBlock(string id, Vector3Int coordinates)
    {
        var item = this.itemTable.GetItem(id);
        var block = item.item as BlockScriptableObject;

        var chunk = this.GetBlockChunk(coordinates);

        var x = coordinates.x % Chunk.Size;
        var y = coordinates.y % Chunk.Size;
        var z = coordinates.z % Chunk.Size;

        chunk.SetBlock(new Vector3Int(x, y, z), block);

        return chunk;
    }

    public Chunk GetBlockChunk(Vector3Int blockCoordinates)
    {
        var coordinates = blockCoordinates / Chunk.Size;
        return this.chunks[coordinates];
    }

    public Chunk GetChunk(Vector3Int coordinates)
    {
        if (this.chunks.ContainsKey(coordinates))
        {
            return this.chunks[coordinates];
        }

        return this.CreateChunk(coordinates);
    }

    public Chunk GetChunkNoCheck(Vector3Int coordinates)
    {
        return this.chunks.TryGetValue(coordinates, out var chunk) ? chunk : null;
    }

    public Chunk CreateChunk(Vector3Int coordinates)
    {
        var chunkCoordinates = coordinates * Chunk.Size;

        var obj = Instantiate(ChunkPrefab, chunkCoordinates, Quaternion.identity, this.transform);

        var chunk = obj.GetComponent<Chunk>();
        chunk.coordinates = coordinates;

        this.chunks.Add(coordinates, chunk);

        return chunk;
    }
}
