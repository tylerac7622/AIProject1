using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour {

    //vectors for movement
    protected Vector3 position;
    protected Vector3 velocity;
    protected Vector3 acceleration;
    protected Vector3 direction;

    private Vector3 desiredVelocity;

    //vectors for movement
    public float maxSpeed;
    public float radius;

    protected bool overBridge;

    private Terrain terrain;//reference to the terrain data

    public bool OverBridge
    {
        set { overBridge = value;  }
    }

    // Use this for initialization
    public virtual void Start () {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
	}
	
	// Update is called once per frame
	public virtual void Update() {
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
        position.y = terrain.SampleHeight(new Vector3(/*(int)*/position.x, 0, /*(int)*/position.z)); //sets the Y position so that the entities sit on top of the terrain, may need adjustment for height of the model
        //sets the proper position if on top of a bridge
        if (overBridge)
        {
            position.y = 50;
        }
        //calculate direction from velocity
        if (velocity.magnitude != 0)
        {
            direction = velocity.normalized;
        }
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

    /// <summary>
    /// Steers the entity away from the obstacle
    /// </summary>
    /// <param name="obst">Obstacle to avoid</param>
    /// <param name="safeDistance">The radius of where the entity cares about the obstacle</param>
    /// <returns></returns>
    protected Vector3 ObstacleAvoidance(GameObject obst, float safeDistance)
    {
        Vector3 steer = new Vector3(0, 0, 0);
        //create a vector from character to the center of the obstacle
        Vector3 vecToCenter = obst.transform.position - position;
        //get diff as mag squared of that vector
        float distance = vecToCenter.sqrMagnitude;

        float obstRad = obst.transform.localScale.x;//temp placeholder for the size of the obstacles radius, maybe pass in as a parameter?

        //if its too far away dismiss it
        if (distance > safeDistance * safeDistance)
        {
            return Vector3.zero;
        }
        if (Vector3.Dot(transform.forward, vecToCenter) < 0)//dismiss it if its behind you
        {
            return Vector3.zero;
        }
        //calc dot product between right vector and vector between entities
        float dot = Vector3.Dot(transform.right, vecToCenter);
        //if it won't collide dismiss it
        if (Mathf.Abs(dot) > obstRad + radius)
        {
            return Vector3.zero;
        }
        //steer away
        if (dot < 0)
        {
            desiredVelocity = transform.right * maxSpeed;
        }
        else
        {
            desiredVelocity = -transform.right * maxSpeed;
        }
        //compute steering force
        steer = desiredVelocity - velocity;
        //vary strength to reflect proximity
        steer *= (safeDistance / distance);
        //return the steering force
        return steer;
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
