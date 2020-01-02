using UnityEngine;

public class ChunkNeighbours
{
    private Vector3Int frontCoordinates;
    public Chunk front;

    private Vector3Int backCoordinates;
    public Chunk back;

    private Vector3Int topCoordinates;
    public Chunk top;

    private Vector3Int bottomCoordinates;
    public Chunk bottom;

    private Vector3Int leftCoordinates;
    public Chunk left;

    private Vector3Int rightCoordinates;
    public Chunk right;

    private World world;

    public ChunkNeighbours(World world, Vector3Int coordinates)
    {
        this.world = world;

        this.frontCoordinates = coordinates + new Vector3Int(0, 0, -1);
        this.backCoordinates = coordinates + new Vector3Int(0, 0, 1);
        this.topCoordinates = coordinates + new Vector3Int(0, 1, 0);
        this.bottomCoordinates = coordinates + new Vector3Int(0, -1, 0);
        this.leftCoordinates = coordinates + new Vector3Int(1, 0, 0);
        this.rightCoordinates = coordinates + new Vector3Int(-1, 0, 0);
    }

    public void Update()
    {
        this.front = this.world.GetChunkNoCheck(this.frontCoordinates);
        this.back = this.world.GetChunkNoCheck(this.backCoordinates);
        this.top = this.world.GetChunkNoCheck(this.topCoordinates);
        this.bottom = this.world.GetChunkNoCheck(this.bottomCoordinates);
        this.left = this.world.GetChunkNoCheck(this.leftCoordinates);
        this.right = this.world.GetChunkNoCheck(this.rightCoordinates);
    }
}
