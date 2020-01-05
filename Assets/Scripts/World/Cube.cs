using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Cube
{
    public static readonly Vector3[] vertices = {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),

        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
    };

    public static readonly int[,] triangles = {
        {0, 3, 1, 2}, // Back
        {5, 6, 4, 7}, // Front
        {3, 7, 2, 6}, // Top
        {1, 5, 0, 4}, // Bottom
        {4, 7, 0, 3}, // Left
        {1, 2, 5, 6}, // Right
    };

    public static Vector2[] GetUV(Rect[] rects)
    {
        return new Vector2[] {
            // Back
            new Vector2(rects[0].xMax, rects[0].yMax),
            new Vector2(rects[0].xMax, rects[0].yMin),
            new Vector2(rects[0].xMin, rects[0].yMin),
            new Vector2(rects[0].xMin, rects[0].yMax),

            // Front
            new Vector2(rects[1].xMin, rects[1].yMin),
            new Vector2(rects[1].xMin, rects[1].yMax),
            new Vector2(rects[1].xMax, rects[1].yMin),
            new Vector2(rects[1].xMax, rects[1].yMax),

            // Top
            new Vector2(rects[2].xMin, rects[2].yMin),
            new Vector2(rects[2].xMin, rects[2].yMax),
            new Vector2(rects[2].xMax, rects[2].yMax),
            new Vector2(rects[2].xMax, rects[2].yMin),

            // Bottom
            new Vector2(rects[3].xMin, rects[3].yMin),
            new Vector2(rects[3].xMin, rects[3].yMax),
            new Vector2(rects[3].xMax, rects[3].yMax),
            new Vector2(rects[3].xMax, rects[3].yMin),

            // Left
            new Vector2(rects[4].xMin, rects[4].yMin),
            new Vector2(rects[4].xMin, rects[4].yMax),
            new Vector2(rects[4].xMax, rects[4].yMax),
            new Vector2(rects[4].xMax, rects[4].yMin),

            // Right
            new Vector2(rects[5].xMin, rects[5].yMin),
            new Vector2(rects[5].xMin, rects[5].yMax),
            new Vector2(rects[5].xMax, rects[5].yMax),
            new Vector2(rects[5].xMax, rects[5].yMin),
        };
    }
}
