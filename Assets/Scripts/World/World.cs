using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        StartCoroutine(this.GenerateChunksTask());
    }

    void Update()
    {
        var currentPlayerChunk = this.WorldCoordinateToChunk(Player.main.transform.position);

        if (this.lastPlayerChunk != currentPlayerChunk)
        {
            this.lastPlayerChunk = currentPlayerChunk;

            this.DestroyOutOfRangeChunks();
            StartCoroutine(this.GenerateChunksTask());
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
        return Vector3Int.Distance(this.lastPlayerChunk, chunkCoordinates) > this.viewDistance;
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

    private IEnumerator GenerateChunksTask()
    {
        StartCoroutine(this.GenerateChunks());

        yield return null;
    }

    private IEnumerator GenerateChunks()
    {
        var radius = this.renderDistance;
        var center = this.lastPlayerChunk;

        this.GenerateChunkIfInRange(center);
        for (int i = 1; i <= radius; i++)
        {
            this.GenerateChunkIfInRange(new Vector3Int(i, 0, 0) + center);
            this.GenerateChunkIfInRange(new Vector3Int(-i, 0, 0) + center);
            this.GenerateChunkIfInRange(new Vector3Int(0, i, 0) + center);
            this.GenerateChunkIfInRange(new Vector3Int(0, -i, 0) + center);
            this.GenerateChunkIfInRange(new Vector3Int(0, 0, i) + center);
            this.GenerateChunkIfInRange(new Vector3Int(0, 0, -i) + center);

            yield return null;

            for (int j = 1; j <= i; j++)
            {
                // +x
                this.GenerateChunkIfInRange(new Vector3Int(i, j, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, -j, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, 0, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, 0, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, j, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, -j, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, j, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(i, -j, -j) + center);

                yield return null;

                // -x
                this.GenerateChunkIfInRange(new Vector3Int(-i, j, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, -j, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, 0, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, 0, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, j, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, -j, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, j, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-i, -j, -j) + center);

                yield return null;

                // +z
                this.GenerateChunkIfInRange(new Vector3Int(0, j, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(0, -j, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, 0, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, 0, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, j, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, -j, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, j, i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, -j, i) + center);

                yield return null;

                // -z
                this.GenerateChunkIfInRange(new Vector3Int(0, j, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(0, -j, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, 0, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, 0, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, j, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, -j, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, j, -i) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, -j, -i) + center);

                yield return null;

                // +y
                this.GenerateChunkIfInRange(new Vector3Int(0, i, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(0, i, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, i, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, i, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, i, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, i, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, i, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, i, -j) + center);

                yield return null;

                // -y
                this.GenerateChunkIfInRange(new Vector3Int(0, -i, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(0, -i, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, -i, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, -i, 0) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, -i, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(j, -i, -j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, -i, j) + center);
                this.GenerateChunkIfInRange(new Vector3Int(-j, -i, -j) + center);

                yield return null;

                // +x
                for (int k = 1; k < j; k++)
                {
                    this.GenerateChunkIfInRange(new Vector3Int(i, j, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, -j, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, j, -k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, -j, -k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, k, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, -k, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, k, -j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(i, -k, -j) + center);

                    yield return null;
                }

                // -x
                for (int k = 1; k < j; k++)
                {
                    this.GenerateChunkIfInRange(new Vector3Int(-i, j, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, -j, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, j, -k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, -j, -k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, k, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, -k, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, k, -j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-i, -k, -j) + center);

                    yield return null;
                }

                // +z
                for (int k = 1; k < j; k++)
                {
                    this.GenerateChunkIfInRange(new Vector3Int(k, j, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(k, -j, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, j, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, -j, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, k, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, -k, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, k, i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, -k, i) + center);

                    yield return null;
                }

                // -z
                for (int k = 1; k < j; k++)
                {
                    this.GenerateChunkIfInRange(new Vector3Int(k, j, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(k, -j, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, j, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, -j, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, k, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, -k, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, k, -i) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, -k, -i) + center);

                    yield return null;
                }

                // +y
                for (int k = 1; k < j; k++)
                {
                    this.GenerateChunkIfInRange(new Vector3Int(k, i, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(k, i, -j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, i, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, i, -j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, i, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, i, -k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, i, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, i, -k) + center);

                    yield return null;
                }

                // -y
                for (int k = 1; k < j; k++)
                {
                    this.GenerateChunkIfInRange(new Vector3Int(k, -i, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(k, -i, -j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, -i, j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-k, -i, -j) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, -i, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(j, -i, -k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, -i, k) + center);
                    this.GenerateChunkIfInRange(new Vector3Int(-j, -i, -k) + center);

                    yield return null;
                }
            }

            yield return null;
        }
    }

    public void GenerateChunkIfInRange(Vector3Int coordinates)
    {
        // if (!this.IsOutOfRange(coordinates) && coordinates.y >= 0)
        // {
        //     this.GetChunk(coordinates);
        // }

        this.GetChunk(coordinates);
    }

    public float Noise(float x, float z)
    {
        return Perlin.Noise(((x / this.noiseScale) + this.seed), ((z / this.noiseScale) + this.seed));
    }

    public float Noise(float x, float z, float y)
    {
        return Perlin.Noise(((x / this.noiseScale) + this.seed), ((z / this.noiseScale) + this.seed), ((y / this.noiseScale) + this.seed));
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
