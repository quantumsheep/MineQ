using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldChunk : MonoBehaviour
{
    public static float Size = 16.0f;
    public static int SizeFloored
    {
        get
        {
            return Mathf.FloorToInt(WorldChunk.Size);
        }
    }

    /// <summary>
    /// listedObjects[x][y][z]
    /// </summary>
    private List<GameObject> listedObjects = new List<GameObject>();

    private WorldManager world;

    public Bounds bounds;
    private bool active = true;

    void Awake()
    {
        this.world = this.transform.parent.GetComponent<WorldManager>();

        this.CreateBoundsBox();
    }

    private void CreateBoundsBox()
    {
        var chunkSize = WorldChunk.Size;
        var axis = chunkSize / 2.0f;

        var center = new Vector3(axis, axis, axis) + this.transform.position;
        var size = new Vector3(chunkSize, chunkSize, chunkSize);

        this.bounds = new Bounds(center, size);
    }

    void Update()
    {
        var visible = Vector3.Distance(this.bounds.center, Player.main.transform.position) <= (this.world.renderDistance * WorldChunk.Size);

        if (visible)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            visible = GeometryUtility.TestPlanesAABB(planes, this.bounds);
        }

        if (visible != this.active)
        {
            this.active = visible;

            int size = this.listedObjects.Count;
            for (int i = 0; i < size; i++)
            {
                this.listedObjects[i].GetComponent<Renderer>().enabled = visible;
            }
        }
    }

    public void AddObject(GameObject obj)
    {
        obj.transform.parent = this.transform;
        this.listedObjects.Add(obj);
    }

    public static Vector3 WorldCoordinatesToChunk(Vector3 worldCoordinates)
    {
        return worldCoordinates / WorldChunk.Size;
    }

    public static Vector3 ChunkCoordinatesToWorld(Vector3 chunkCoordinates)
    {
        return chunkCoordinates * WorldChunk.Size;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
    }
}
