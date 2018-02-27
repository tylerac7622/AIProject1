using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour {

    //vectors for movement
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    private Vector3 direction;

    private Vector3 desiredVelocity;

    //vectors for movement
    public float maxSpeed;
    public float radius;

    private TerrainData tData;//reference to the terrain data

    // Use this for initialization
    void Start () {
        tData = GameObject.Find("Terrain").GetComponent<TerrainData>();
	}
	
	// Update is called once per frame
	void Update () {
        CalcSteeringForces();
        UpdatePosition();
        SetTransform();
    }

    /// <summary>
    /// Determines the acceleration
    /// by applying the sum of the steering forces
    /// </summary>
    protected abstract void CalcSteeringForces();

    /// <summary>
	/// UpdatePosition
	/// Calculate the velocity and resulting position of an entity
	/// based on any forces
	/// </summary>
	protected void UpdatePosition()
    {
        position = transform.position;
        //add acceleration to velocity
        velocity += acceleration * Time.deltaTime;
        //clamp velocity
        Vector3.ClampMagnitude(velocity, maxSpeed);
        velocity.y = 0;
        //add velocity to position
        position += velocity * Time.deltaTime;
        position.y = tData.GetHeight((int)position.x, (int)position.z); //sets the Y position so that the entities sit on top of the terrain, may need adjustment for height of the model
        //calculate direction from velocity
        direction = velocity.normalized;
        //zero out acceleration
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// calculates the force to cause current object to seek a target
    /// </summary>
    /// <param name="targetPosition">position of object being saught</param>
    /// <returns>Seek force</returns>
	protected Vector3 Seek(Vector3 targetPosition)
    {
        //step 1: calculate desired velocity
        //vector from myself to the target
        desiredVelocity = targetPosition - position;
        //step 2: scale to maxspeed
        desiredVelocity = maxSpeed * desiredVelocity.normalized;
        //step 3: calculate the steering force
        desiredVelocity -= velocity;
        //step 4: return steering force
        return desiredVelocity;
    }

    /// <summary>
    /// calculates the force to cause current object to flee a target
    /// </summary>
    /// <param name="targetPosition">position of object currently fleeing from</param>
    /// <returns>Flee force</returns>
    protected Vector3 Flee(Vector3 targetPosition)
    {
        //step 1: calculate desired velocity
        //vector from myself to the target - negated so it flees
        desiredVelocity = -(targetPosition - position);
        //step 2: scale to maxspeed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        //step 3: calculate the steering force
        desiredVelocity -= velocity;
        //step 4: return steering force
        return desiredVelocity;
    }

    protected Vector3 ObstacleAvoidance(Vector3 obstPos)
    {
        return Vector3.zero;
    }

    /// <summary>
    /// Applies any Vector3 force to the acceleration vector
    /// Assuming mass of 1 in this case
    /// </summary>
    /// <param name="force">Force.</param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    /// <summary>
    /// Sets the transform component to the local positon
    /// </summary>
    void SetTransform()
    {
        //set forward vector equal to the entity's direction
        transform.forward = direction;

        transform.position = position;
    }
}
