using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestBlock : MonoBehaviour
{
    public World world;
    public Rect[] rects;
    // public Vector2[] uvs;

    private MeshFilter filter;
    private new MeshRenderer renderer;
    private BlockScriptableObject block;

    private bool initialized = false;

    void Start()
    {
        this.filter = this.GetComponent<MeshFilter>();
        this.renderer = this.GetComponent<MeshRenderer>();

        this.block = this.world.itemTable.GetItem("default").item as BlockScriptableObject;
        this.rects = this.block.rects;
    }

    void Update()
    {
        if (!this.initialized || Input.GetKeyDown(KeyCode.K))
        {
            this.renderer.material = new Material(Shader.Find("Standard"))
            {
                mainTexture = this.world.itemTable.GetBlockAtlasTexture()
            };

            this.filter.mesh = this.CreateCubeMesh();

            this.initialized = true;
        }
        else
        {
            // this.filter.mesh.uv = this.uvs;
        }
    }

    private Mesh CreateCubeMesh()
    {
        Vector3[] vertices = {
            // Front
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),

            // Back
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            // Top
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),

            // Bottom
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            // Left
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            // Right
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
        };

        int[] triangles = {
            0,  2,  3,
            0,  3,  1,

            8,  4,  5,
            8,  5,  9,

            10, 6,  7,
            10, 7,  11,

            12, 13, 14,
            12, 14, 15,

            16, 17, 18,
            16, 18, 19,

            20, 21, 22,
            20, 22, 23,
        };

        Vector2[] uvs = {
            new Vector2(rects[1].xMin, rects[1].yMin),
            new Vector2(rects[1].xMin, rects[1].yMax),
            new Vector2(rects[1].xMax, rects[1].yMax),
            new Vector2(rects[1].xMax, rects[1].yMin),

            new Vector2(rects[2].xMin, rects[2].yMin),
            new Vector2(rects[2].xMin, rects[2].yMax),
            new Vector2(rects[2].xMax, rects[2].yMax),
            new Vector2(rects[2].xMax, rects[2].yMin),

            new Vector2(rects[0].xMax, rects[0].yMax),
            new Vector2(rects[0].xMax, rects[0].yMin),
            new Vector2(rects[0].xMin, rects[0].yMin),
            new Vector2(rects[0].xMin, rects[0].yMax),

            new Vector2(rects[3].xMin, rects[3].yMin),
            new Vector2(rects[3].xMin, rects[3].yMax),
            new Vector2(rects[3].xMax, rects[3].yMax),
            new Vector2(rects[3].xMax, rects[3].yMin),

            new Vector2(rects[4].xMin, rects[4].yMin),
            new Vector2(rects[4].xMin, rects[4].yMax),
            new Vector2(rects[4].xMax, rects[4].yMax),
            new Vector2(rects[4].xMax, rects[4].yMin),

            new Vector2(rects[5].xMin, rects[5].yMin),
            new Vector2(rects[5].xMin, rects[5].yMax),
            new Vector2(rects[5].xMax, rects[5].yMax),
            new Vector2(rects[5].xMax, rects[5].yMin),
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.Optimize();
        mesh.uv = uvs;

        return mesh;
    }
}
