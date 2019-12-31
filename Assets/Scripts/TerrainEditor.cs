using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldManager))]
public class TerrainEditor : MonoBehaviour
{
    // [HideInInspector]
    // public const float HEIGHT_STEP = 0.00390625f;

    // public ItemTableScriptableObject itemTable;

    // public int width = 65;
    // public int height = 65;

    // private sbyte[,] heightmap = null;
    // private WorldManager world;

    // void Start()
    // {
    //     this.world = this.GetComponent<WorldManager>();

    //     this.heightmap = this.GenerateHeightmap();

    //     this.UpdateWorld();
    // }

    // void Update()
    // {
    //     // var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //     // if (Physics.Raycast(ray, out var hit, this.mask))
    //     // {
    //     //     Debug.DrawLine(ray.origin, hit.point, Color.magenta);

    //     //     if (Input.GetMouseButtonDown(0))
    //     //     {
    //     //         var localHitPoint = this.transform.InverseTransformPoint(hit.point);

    //     //         // this.heightmap[Mathf.FloorToInt(localHitPoint.z), Mathf.FloorToInt(localHitPoint.x)]++;

    //     //         // this.UpdateWorld();
    //     //     }
    //     // }
    // }

    // public sbyte[,] GenerateHeightmap()
    // {
    //     var heightmap = new sbyte[this.height, this.width];

    //     for (int i = 0; i < this.height; i++)
    //     {
    //         for (int j = 0; j < this.width; j++)
    //         {
    //             heightmap[i, j] = System.Convert.ToSByte(Random.Range(-1, -1));
    //         }
    //     }

    //     return heightmap;
    // }

    // private void UpdateWorld()
    // {
    //     for (int x = 0; x < this.width; x++)
    //     {
    //         for (int z = 0; z < this.height; z++)
    //         {
    //             for (int y = this.heightmap[z, x]; y >= 0; y--)
    //             {
    //                 this.SpawnCube(new Vector3(x, y, z), "test:grass");
    //             }

    //             for (int y = this.heightmap[z, x]; y < 0; y++)
    //             {
    //                 this.SpawnCube(new Vector3(x, y, z), "test:grass");
    //             }
    //         }
    //     }
    // }

    // private void SpawnCube(Vector3 coordinates, string blockId)
    // {
    //     var item = this.itemTable.GetItem(blockId);
    //     var block = item.item as BlockScriptableObject;

    //     var obj = Instantiate(this.cubePrefab, coordinates, Quaternion.identity);
    //     this.world.AddObject(ref obj);


    // }
}
