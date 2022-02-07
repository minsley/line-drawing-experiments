using UnityEngine;
using Random = Freya.Random;

public class Boid
{
    public Vector3 velocity;
    public Pose pose;

    public Boid(Vector3 position, Quaternion rotation)
    {
        pose = new Pose(position, rotation);
        velocity = Vector3.zero;
    }

    public float GetDistance(Boid other) => Vector3.Distance(pose.position , other.pose.position);

    public void MoveForward(float minSpeed, float maxSpeed)
    {
        if (velocity == Vector3.zero) velocity = pose.forward * Random.Range(minSpeed, maxSpeed);

        var speed = velocity.magnitude;
        if (speed > maxSpeed)
        {
            velocity *= maxSpeed / speed;
        }
        else if (speed < minSpeed)
        {
            velocity *= minSpeed / speed;
        }
            
        pose.position += velocity*Time.deltaTime;
    }
}