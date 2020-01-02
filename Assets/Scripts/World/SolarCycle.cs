using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarCycle : MonoBehaviour
{
    public float fullCycleSeconds = 600.0f;

    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, (Time.deltaTime / this.fullCycleSeconds) * 360.0f));
    }
}
