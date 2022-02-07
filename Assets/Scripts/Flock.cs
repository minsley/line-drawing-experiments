using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Freya;

public class Flock
{
    public List<Boid> boids = new List<Boid>();
    public int nPredators = 0;
    public float containerRadius = 1f;
    public float containerPadding = 0.1f;
    public float turn = 1f;
    public float minSpeed = 0.01f;
    public float maxSpeed = 1f;

    public bool cluster = false;
    public bool align = false;
    public bool avoid = false;
    public bool prey = false;

    private Vector3 _center;
    
    public Flock(int n, Vector3 fieldPosition, float fieldRadius, float fieldPadding, float turnSpeed, float minSpeed, float maxSpeed)
    {
        _center = fieldPosition;
        containerRadius = fieldRadius;
        containerPadding = fieldPadding;
        turn = turnSpeed;
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
        nPredators = 0;
        
        SetNumberOfBoids(n);
    }

    public void SetNumberOfBoids(int n)
    {
        if (boids.Count < n)
        {
            boids.AddRange(Enumerable.Range(0, n).Select(_ => 
                new Boid(
                    _center + Freya.Random.InUnitSphere * containerRadius, 
                    Freya.Random.Rotation)));
        } 
        else if (boids.Count > n)
        {
            boids.RemoveRange(n, boids.Count-n);
        }
    }
    
    public void Advance(bool bounceOffWalls = true, bool wrapAroundEdges = false)
    {
        // update void speed and direction (velocity) based on rules
        for (var i=0; i<boids.Count; i++)
        {
            if(cluster) boids[i].velocity += Cluster(boids[i], 50, 0.0003f);
            if(align) boids[i].velocity += Align(boids[i], 50, 0.01f);
            if(avoid) boids[i].velocity += Avoid(boids[i], 20, 0.001f);
            if(prey) boids[i].velocity += Predator(boids[i], 150, 0.00005f);
        }

        // move all boids forward in time
        foreach (var boid in boids)
        {
            boid.MoveForward(minSpeed, maxSpeed);
            if (bounceOffWalls)
                BounceOffWalls(boid);
            if (wrapAroundEdges)
                WrapAround(boid);
        }
    }
    
    private Vector3 Cluster(Boid boid, float distance, float power)
    {
        var count = 0;
        var sum = Vector3.zero;
        foreach (var neighbor in boids.Where(x => x.GetDistance(boid) < distance))
        {
            count++;
            sum += neighbor.pose.position;
        }
        var mean = sum / count;
        var delta = mean - boid.pose.position;
        return delta * power;
    }
    
    private Vector3 Align(Boid boid, float distance, float power)
    {
        var count = 0;
        var sum = Vector3.zero;
        foreach (var neighbor in boids.Where(x => x.GetDistance(boid) < distance))
        {
            count++;
            sum += neighbor.velocity;
        }
        var mean = sum / count;
        var delta = mean - boid.velocity;
        return delta * power;
    }
    
    private Vector3 Avoid(Boid boid, float distance, float power)
    {
        var sum = Vector3.zero;
        foreach (var neighbor in boids.Where(x => x.GetDistance(boid) < distance))
        {
            sum += (boid.pose.position - neighbor.pose.position) * (distance - boid.GetDistance(neighbor));
        }
        return sum * power;
    }
    
    private void BounceOffWalls(Boid boid)
    {
        if ((_center - boid.pose.position).magnitude > (containerRadius - containerPadding))
        {
            var angle = Vector3.Angle(boid.pose.forward, _center - boid.pose.position);
            var okAngle = 30f;
            
            var turnFactor = Mathf.InverseLerp(okAngle, 180, angle);
            var lookRot = Quaternion.LookRotation(_center-boid.pose.position, boid.pose.up);
            boid.pose.rotation = Quaternion.Slerp(boid.pose.rotation, lookRot, turn * turnFactor * Time.deltaTime); 
            boid.velocity = boid.pose.forward * boid.velocity.magnitude; 
        }
    }
    
    private void WrapAround(Boid boid)
    {
        // boid.position.x += boid.position.x < 0 ? Width : boid.position.x > Width ? -turn : 0;
        // boid.position.y += boid.position.y < pad ? turn : boid.position.y > (Width - pad) ? -turn : 0;
        // boid.position.z += boid.position.z < pad ? turn : boid.position.z > (Width - pad) ? -turn : 0;
        //
        // if (boid.X < 0)
        //     boid.X += Width;
        // if (boid.X > Width)
        //     boid.X -= Width;
        // if (boid.Y < 0)
        //     boid.Y += Height;
        // if (boid.Y > Height)
        //     boid.Y -= Height;
    }
    private Vector3 Predator(Boid boid, float distance, float power)
    {
        var sum = Vector3.zero;
        foreach (var predator in boids.GetRange(0, nPredators).Where(x => x.GetDistance(boid) < distance))
        {
            sum += (boid.pose.position - predator.pose.position) * (distance - boid.GetDistance(predator));
        }
        return sum * power;
    }
}