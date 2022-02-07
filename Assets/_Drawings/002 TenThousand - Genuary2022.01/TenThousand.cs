using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Freya;
using Shapes;
using UnityEngine;
using Random = Freya.Random;

[ExecuteAlways]
public class TenThousand : ImmediateModeShapeDrawer
{
    public Vector3 startFieldPosition;
    public float fieldRadius = 10f;
    public float boidLength = 0.1f;
    public int nBoids = 10000;
    public float updatePeriod = 0.05f;
    public float minSpeed = 0.01f;
    public float maxSpeed = 1f;
    public float turnSpeed = 1f;
    public float containerPadding = 0.1f;
    public float ringThickness = 0.01f;
    public Color boidColor = Color.black;
    public Color ringColor = Color.black;

    private Flock _flock;
    private float _nextUpdate;

    public override void OnEnable()
    {
        base.OnEnable();
        Init();
    }

    private void Update()
    {
        if (_flock == null)
            return;
        
        if (Time.time < _nextUpdate) return;

        _nextUpdate = Time.time + updatePeriod;
        _flock.Advance();
    }

    private void OnValidate()
    {
        if (_flock == null)
            return;
        
        _flock.turn = turnSpeed;
        _flock.containerPadding = containerPadding;
        _flock.minSpeed = minSpeed;
        _flock.maxSpeed = maxSpeed;
        _flock.SetNumberOfBoids(nBoids);
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.LineGeometry = LineGeometry.Billboard;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = 0.005f;
            Draw.Matrix = transform.localToWorldMatrix;
            Draw.Color = boidColor;

            if (_flock == null) return;
            foreach (var boid in _flock.boids)
            {
                var pose = boid.pose;
                var pos = pose.position;
                var right = pose.right;
                Draw.Triangle(pos+pose.forward*boidLength, pos+right*boidLength/2, pos-right*boidLength/2);
                // Draw.Cone(boid.position, boid.rotation, boidTailRadius, boidLength);
            }

            Draw.Color = ringColor;
            Draw.Ring(startFieldPosition, Camera.main.transform.position-startFieldPosition, fieldRadius, ringThickness);
        }
    }
    
    private void Init()
    {
        _flock = new Flock(nBoids, startFieldPosition, fieldRadius, containerPadding, turnSpeed, minSpeed, maxSpeed);
    }
}
