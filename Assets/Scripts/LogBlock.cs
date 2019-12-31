using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBlock : MonoBehaviour
{
    private MeshFilter filter;

    void Start()
    {
        this.filter = this.GetComponent<MeshFilter>();

        var mesh = filter.mesh;

        var vertices = "";
        var triangles = "";
        var normals = "";
        var uvs = "";

        foreach (var vertice in mesh.vertices)
        {
            vertices += $"new Vector3{vertice},\n";
        }

        foreach (var triangle in mesh.triangles)
        {
            triangles += $"{triangle}, ";
        }

        foreach (var normal in mesh.normals)
        {
            normals += $"new Vector3{normal},\n";
        }

        foreach (var uv in mesh.uv)
        {
            uvs += $"new Vector2{uv},\n";
        }

        Debug.Log(vertices);
        Debug.Log(triangles);
        Debug.Log(normals);
        Debug.Log(uvs);
    }

    void Update()
    {

    }
}
