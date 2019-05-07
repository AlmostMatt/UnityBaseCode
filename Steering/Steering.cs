using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityBaseCode
{
	namespace Steering
	{
		/*
		 * Steering behaviours as described by https://red3d.com/cwr/steer/
		 * See also: https://github.com/antonpantev/unity-movement-ai
		 */
		public class Steering : MonoBehaviour, ObjectWithPosition
		{
			private Rigidbody2D rb;

			// For debug lines:
			private Color VELOCITY_COLOR = Color.blue;
			private Color[] BEHAVIOUR_COLORS = {Color.red, Color.green, Color.white, Color.cyan, Color.yellow};

			// Whether or not the object turns toward the current velocity vector.
			private bool turnAutomatically = true;
			private float turnRate = 900f;

			private float maxSpeed = 5f;
			private float acceleration = 20f;
			private float radius = 0.5f;

			// The behaviour is the key so that the weight can be modified.
			private Dictionary<SteeringBehaviour, float> weightedBehaviours = new Dictionary<SteeringBehaviour, float>();

			public void Start() {
				rb = GetComponent<Rigidbody2D>();
                if (!rb)
                {
                    rb = gameObject.AddComponent<Rigidbody2D>();
                }
			}

			public void addBehaviour(float weight, SteeringBehaviour behaviour) {
				weightedBehaviours.Add(behaviour, weight);
			}

			public void updateWeight(SteeringBehaviour behaviour, float newWeight) {
				weightedBehaviours[behaviour] = newWeight;
			}

			public void removeBehaviour(SteeringBehaviour behaviour) {
				weightedBehaviours.Remove(behaviour);
			}

			public void clearBehaviours() {
				weightedBehaviours.Clear();
			}

			// TODO: call getForce for each behaviour and provide a reference to the Steering object, or provide MAXV ACCEL etc.
			// TODO: call rb.AddForce on the weighted sum of the available forces. Also if a behaviour provides a force of size < accel, it counts less towards the total weight

			public void FixedUpdate () {
				float totalWeight = 0f;
				Vector2 totalForce = new Vector2();
				int i = 0;
				foreach (KeyValuePair<SteeringBehaviour, float> behaviourAndWeight in weightedBehaviours) {
					Vector2 behaviourForce = behaviourAndWeight.Key.getForce(this);
					totalForce += behaviourAndWeight.Value * behaviourForce;
					totalWeight += behaviourAndWeight.Value * behaviourForce.magnitude / acceleration;
					// TODO: define a mapping from behaviour to color, and provide some way to only draw lines for some behaviours
					SteeringUtilities.drawDebugVector(this, 0.1f * behaviourForce, BEHAVIOUR_COLORS[i++ % BEHAVIOUR_COLORS.Length]);
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

			public void setSize(float radius) {
				this.radius = radius;
			}

			public void setSpeed(float maxSpeed, float acceleration) {
				this.maxSpeed = maxSpeed;
				this.acceleration = acceleration;
			}

			public float getSize() {
				return radius;
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
	}
}