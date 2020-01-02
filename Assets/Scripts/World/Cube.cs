using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Cube
{
    public static readonly Vector3[] vertices = {
        // Back
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),

        // Top 1
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),

        // Top 2
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),

        // Front 1
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),

        // Front 2
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),

        // Bottom
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),

        // Right
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),

        // Left
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
    };

    public static readonly int[] triangles = {
        // Back
        0,  2,  3,
        0,  3,  1,

        // Top
        6,  4,  5,
        6,  5,  7,

        // Front
        10, 8,  9,
        10, 9,  11,

        // Bottom
        12, 13, 14,
        12, 14, 15,

        // Right
        16, 17, 18,
        16, 18, 19,

        // Left
        20, 21, 22,
        20, 22, 23,
    };

    public static Vector2[] GetUV(Rect[] rects)
    {
        return new Vector2[] {
            // Back
            new Vector2(rects[1].xMin, rects[1].yMin),
            new Vector2(rects[1].xMin, rects[1].yMax),
            new Vector2(rects[1].xMax, rects[1].yMax),
            new Vector2(rects[1].xMax, rects[1].yMin),

            // Top
            new Vector2(rects[2].xMin, rects[2].yMin),
            new Vector2(rects[2].xMin, rects[2].yMax),
            new Vector2(rects[2].xMax, rects[2].yMax),
            new Vector2(rects[2].xMax, rects[2].yMin),

            // Front
            new Vector2(rects[0].xMax, rects[0].yMax),
            new Vector2(rects[0].xMax, rects[0].yMin),
            new Vector2(rects[0].xMin, rects[0].yMin),
            new Vector2(rects[0].xMin, rects[0].yMax),

            // Bottom
            new Vector2(rects[3].xMin, rects[3].yMin),
            new Vector2(rects[3].xMin, rects[3].yMax),
            new Vector2(rects[3].xMax, rects[3].yMax),
            new Vector2(rects[3].xMax, rects[3].yMin),

            // Right
            new Vector2(rects[5].xMin, rects[5].yMin),
            new Vector2(rects[5].xMin, rects[5].yMax),
            new Vector2(rects[5].xMax, rects[5].yMax),
            new Vector2(rects[5].xMax, rects[5].yMin),

            // Left
            new Vector2(rects[4].xMin, rects[4].yMin),
            new Vector2(rects[4].xMin, rects[4].yMax),
            new Vector2(rects[4].xMax, rects[4].yMax),
            new Vector2(rects[4].xMax, rects[4].yMin),
        };
    }

    public static (Vector3[] vertices, int[] triangles, Vector2[] uv) Generate(Rect[] rects)
    {
        return (vertices, triangles.ToArray(), GetUV(rects));
    }
}
