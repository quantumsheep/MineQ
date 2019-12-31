using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldManager : MonoBehaviour
{
    public GameObject chunkPrefab;

    public int renderDistance = 5;

    /// <summary>
    /// Chunk[x][y][z]
    /// </summary>
    private Dictionary<int, Dictionary<int, Dictionary<int, WorldChunk>>> chunks = new Dictionary<int, Dictionary<int, Dictionary<int, WorldChunk>>>();

    /// <summary>
    /// Get a chunk from chunk coordinates
    /// </summary>
    /// <param name="coordinates">Normalized coordinates (WorldChunk.WorldCoordinatesToChunk(vec3))</param>
    public WorldChunk GetChunk(Vector3 coordinates)
    {
        var x = Mathf.FloorToInt(coordinates.x);
        var y = Mathf.FloorToInt(coordinates.y);
        var z = Mathf.FloorToInt(coordinates.z);

        if (!this.chunks.ContainsKey(x))
        {
            this.chunks.Add(x, new Dictionary<int, Dictionary<int, WorldChunk>>());
        }

        if (!this.chunks[x].ContainsKey(y))
        {
            this.chunks[x].Add(y, new Dictionary<int, WorldChunk>());
        }

        if (!this.chunks[x][y].ContainsKey(z))
        {
            var chunk = this.InstantiateChunk(coordinates);

            this.chunks[x][y].Add(z, chunk);
        }

        return this.chunks[x][y][z];
    }

    private WorldChunk InstantiateChunk(Vector3 chunkCoordinates)
    {
        var obj = Instantiate(this.chunkPrefab, WorldChunk.ChunkCoordinatesToWorld(chunkCoordinates), Quaternion.identity, this.transform);
        return obj.GetComponent<WorldChunk>();
    }

    /// <summary>
    /// Push a game object in a chunk.
    /// </summary>
    /// <param name="obj">An instantiated GameObject. The transform's parent will be modified.</param>
    public void AddObject(ref GameObject obj)
    {
        var chunkCoordinates = WorldChunk.WorldCoordinatesToChunk(obj.transform.position);
        var chunk = this.GetChunk(chunkCoordinates);

        chunk.AddObject(obj);

        obj.transform.parent = chunk.transform;
    }
}
