using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Rotator : MonoBehaviour
{
    public float period = 5;
    
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(360*(Time.time%period/period), Vector3.up);
    }
}
