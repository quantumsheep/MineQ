using UnityEngine;

public static class Player
{
    public static PlayerController main
    {
        get
        {
            return GameObject.FindGameObjectWithTag("MainPlayer").GetComponent<PlayerController>();
        }
    }
}
