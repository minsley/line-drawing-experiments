using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject template;
    public float density = 1;
    public float layerDelay = 4;
    public int maxLayers = 8;

    private int layers = 0;
    private float nextLayer = 0;

    private void Update()
    {
        if (layers < maxLayers && Time.time >= nextLayer)
        {
            SpawnLayer();
        }
    }

    void SpawnLayer()
    {
        nextLayer = Time.time + layerDelay;
        layers++;
        
        var scale = transform.localScale;
        for (int i = 0; i < scale.x/density; i++)
        {
            for (int j = 0; j < scale.z/density; j++)
            {
                Instantiate(template, new Vector3(i - scale.x/2, 0, j - scale.z/2), Quaternion.identity);
            }
        }
    }
}
