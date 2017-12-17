using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Steering behaviours as described by https://red3d.com/cwr/steer/
 * See also: https://github.com/antonpantev/unity-movement-ai
 */
public class Steering : MonoBehaviour
{
	private Rigidbody2D rb;
	private Vector2 forwardVector;

	// For debug lines:
	private Color VELOCITY_COLOR = Color.blue;
	private Color[] BEHAVIOUR_COLORS = {Color.red, Color.green, Color.white, Color.cyan, Color.yellow};

	// Whether or not the object turns toward the current velocity vector.
	private bool turnAutomatically = true;
	private float turnRate = 900f;

	private float maxSpeed = 5f;
	private float acceleration = 20f;

	private List<SteeringBehaviour> behaviours = new List<SteeringBehaviour>();
	private List<float> behaviourWeights = new List<float>();

	public void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	public void setSpeed(float maxSpeed, float acceleration) {
		this.maxSpeed = maxSpeed;
		this.acceleration = acceleration;
	}

	public void addBehaviour(float weight, SteeringBehaviour behaviour) {
		behaviours.Add(behaviour);
		behaviourWeights.Add(weight);
	}

	// TODO: provide a way to change the weight of a behaviour
	// aka either make steeringbehaviour abstract and give it properties
	// or provide some sort of conditional/variable weight function when adding a behaviour

	public void clearBehaviours() {
		behaviours.Clear();
		behaviourWeights.Clear();
	}

	// TODO: call getForce for each behaviour and provide a reference to the Steering object, or provide MAXV ACCEL etc.
	// TODO: call rb.AddForce on the weighted sum of the available forces. Also if a behaviour provides a force of size < accel, it counts less towards the total weight

	public void FixedUpdate () {
		float totalWeight = 0f;
		Vector2 totalForce = new Vector2();
		for (int i=0; i< behaviours.Count; i++) {
			Vector2 behaviourForce = behaviours[i].getForce(this);
			totalForce += behaviourWeights[i] * behaviourForce;
			totalWeight += behaviourWeights[i] * behaviourForce.magnitude / acceleration;
			SteeringUtilities.drawDebugVector(this, 0.1f * behaviourForce, BEHAVIOUR_COLORS[i % BEHAVIOUR_COLORS.Length]);
		}
		// TODO: consider averaging the desired velocities instead of forces
		if (totalWeight > 0f) {
			//SteeringUtilities.drawDebugVector(this, 0.1f * totalForce / totalWeight, Color.blue);
			rb.AddForce(totalForce / totalWeight);
		}
		// Enforce a maximum speed
		float velocitySquared = rb.velocity.sqrMagnitude;
		if (velocitySquared > maxSpeed * maxSpeed) {
			rb.velocity = SteeringUtilities.scaledVector(maxSpeed, rb.velocity);
		}
		// Turn toward the current velocity (so that the x axis is forward)
		if (turnAutomatically && velocitySquared > 0.5) {
			turnToward(SteeringUtilities.angleForVector(rb.velocity));
		}
		//SteeringUtilities.drawDebugVector(this, (0.5f * getVelocity()), VELOCITY_COLOR);
	}

	public float getMaxSpeed() {
		return maxSpeed;
	}

	public float getAcceleration() {
		return acceleration;
	}

	public Vector2 getPosition() {
		return transform.position;
	}

	public Vector2 getVelocity() {
		return rb.velocity;
	}

	public float getStoppingTime() {
		return getVelocity().magnitude / getAcceleration();
	}

	public float getStoppingDistance() {
		// StopTime = speed/accel, AvgSpeed while stoppping = speed/2, Distance = speed^2 / (accel * 2)
		return getVelocity().sqrMagnitude / (getAcceleration() * 2);
	}

	private void turnToward(float desiredAngle) {
		float currentAngle = transform.localEulerAngles.z;
		float angleDiff = SteeringUtilities.angleDiff(currentAngle, desiredAngle);
		float rotationPerFrame = turnRate * Time.fixedDeltaTime;
		if (angleDiff > rotationPerFrame) {
			transform.localEulerAngles = new Vector3(0, 0, currentAngle + rotationPerFrame);
		} else if (angleDiff < -rotationPerFrame) {
			transform.localEulerAngles = new Vector3(0, 0, currentAngle - rotationPerFrame);
		} else {
			transform.localEulerAngles = new Vector3(0, 0, desiredAngle);
		}
	}
}
