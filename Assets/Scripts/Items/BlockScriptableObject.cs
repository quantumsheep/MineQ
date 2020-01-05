using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Block", order = 1)]
public class BlockScriptableObject : ItemScriptableObject
{
    public float restistance = 1.0f;

    public Texture2D back;
    public Texture2D front;
    public Texture2D top;
    public Texture2D bottom;
    public Texture2D left;
    public Texture2D right;

    public Texture2D[] GetTextures()
    {
        Texture2D[] textures = {
            this.front,
            this.back,
            this.top,
            this.bottom,
            this.left,
            this.right,
        };

        return textures;
    }
}
