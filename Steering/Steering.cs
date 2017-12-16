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
	// Whether or not the object turns toward the current velocity vector.
	private bool turnAutomatically = true;
	private float turnRate = 900f;

	private float maxSpeed = 5f;
	private float acceleration = 20f;

	private List<SteeringBehaviour> behaviours = new List<SteeringBehaviour>();
	private List<float> behaviourWeights = new List<float>();

	public void setSpeed(float maxSpeed, float acceleration) {
		this.maxSpeed = maxSpeed;
		this.acceleration = acceleration;
	}

	public void addBehaviour(float weight, SteeringBehaviour behaviour) {
		behaviours.Add(behaviour);
		behaviourWeights.Add(weight);
	}

	public void clearBehaviours() {
		behaviours.Clear();
		behaviourWeights.Clear();
	}

	// TODO: call getForce for each behaviour and provide a reference to the Steering object, or provide MAXV ACCEL etc.
	// TODO: call rb.AddForce on the weighted sum of the available forces. Also if a behaviour provides a force of size < accel, it counts less towards the total weight

	public void FixedUpdate () {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		float totalWeight = 0f;
		Vector2 totalForce = new Vector2();
		for (int i=0; i< behaviours.Count; i++) {
			Vector2 behaviourForce = behaviours[i].getForce(this);
			totalForce += behaviourForce;
			totalWeight += behaviourForce.magnitude / acceleration;
		}
		rb.AddForce(totalForce);
		// Enforce a maximum speed
		float velocitySquared = rb.velocity.sqrMagnitude;
		if (velocitySquared > maxSpeed * maxSpeed) {
			rb.velocity = SteeringUtilities.scaledVector(maxSpeed, rb.velocity);
		}
		// Turn toward the current velocity (so that the x axis is forward)
		if (turnAutomatically && velocitySquared > 0.5) {
			turnToward(SteeringUtilities.angleForVector(rb.velocity));
		}
		Debug.DrawLine(transform.position, (Vector2) transform.position + (0.1f * totalForce));
		Debug.DrawLine(transform.position, (Vector2) transform.position + (1f * rb.velocity));
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
		return GetComponent<Rigidbody2D>().velocity;
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
