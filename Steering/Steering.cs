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
            // TODO: have axis be a property of the instance (and a static variable to change the default)
            internal static int YMult = 1;
            internal static int ZMult = 0;
            // TODO: support 3D steering with no position constraints? Some angle math becomes complicated, and perpindicularTo is different
            // TODO: support YZ plane and/or arbitrary axis combinations
            public static bool UseXZ
            {
                get { return ZMult > 0 && YMult == 0; }
                set { YMult = value ? 0 : 1; ZMult = value ? 1 : 0; }
            }
            internal static bool Has2D = false;
            internal static bool Has3D = false;
            internal static bool Using3D
            {
                get { return Has3D; }
            }

            // TODO: standardize naming (capitalize public things)
            private Rigidbody2D _rb2D;
            private Rigidbody _rb3D;

            // For debug lines:
            private Color VELOCITY_COLOR = Color.blue;
			private Color[] BEHAVIOUR_COLORS = {Color.red, Color.green, Color.white, Color.cyan, Color.yellow};

			// Whether or not the object turns toward the current velocity vector.
			private bool turnAutomatically = true;
			private float turnRate = 900f;

			private float maxSpeed = 5f;
			private float acceleration = 20f;
			private float radius = 0.5f;

            // The direction that this object is moving. Starts as positive x axis.
            private Vector3 _movingDirection = Vector3.right;

			// The behaviour is the key so that the weight can be modified.
			private Dictionary<SteeringBehaviour, float> weightedBehaviours = new Dictionary<SteeringBehaviour, float>();

			public void Start()
            {
                //Debug.Log("YMult is: " + YMult);
                //Debug.Log("ZMult is: " + ZMult);
                _rb2D = GetComponent<Rigidbody2D>();
                _rb3D = GetComponent<Rigidbody>();
                if (!UseXZ && !Using3D && !_rb2D)
                {
                    _rb2D = gameObject.AddComponent<Rigidbody2D>();
                } else if (!_rb2D && !_rb3D)
                {
                    if (Has2D)
                    {
                        // TODO: Throw an exception, or find and remove the problematic 2D physics component
                        Debug.Log("WARNING: Something has 2D physics despite Steering using 3D physics.");
                    }
                    Has3D = true;
                    _rb3D = gameObject.AddComponent<Rigidbody>();
                }
                if (_rb2D)
                {
                    _rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                if (_rb3D)
                {
                    _rb3D.constraints = RigidbodyConstraints.FreezeRotation | (UseXZ ? RigidbodyConstraints.FreezePositionY : RigidbodyConstraints.FreezePositionZ);
                }
			}

			public void AddBehaviour(float weight, SteeringBehaviour behaviour) {
				weightedBehaviours.Add(behaviour, weight);
			}

            // TODO: getBehaviours - get all behaviours

            // TODO: getBehaviours<T> - get behaviour(s) of a specific type

            // TODO: getWeight(Behaviour)

			public void UpdateWeight(SteeringBehaviour behaviour, float newWeight) {
				weightedBehaviours[behaviour] = newWeight;
			}

			public void RemoveBehaviour(SteeringBehaviour behaviour) {
				weightedBehaviours.Remove(behaviour);
			}

			public void ClearBehaviours() {
				weightedBehaviours.Clear();
			}

			// TODO: call GetForce for each behaviour and provide a reference to the Steering object, or provide MAXV ACCEL etc.
			// TODO: call rb.AddForce on the weighted sum of the available forces. Also if a behaviour provides a force of size < accel, it counts less towards the total weight

			public void FixedUpdate () {
				float totalWeight = 0f;
				Vector3 totalForce = new Vector3();
				int i = 0;
				foreach (KeyValuePair<SteeringBehaviour, float> behaviourAndWeight in weightedBehaviours) {
					Vector3 behaviourForce = behaviourAndWeight.Key.GetForce(this);
					totalForce += behaviourAndWeight.Value * behaviourForce;
					totalWeight += behaviourAndWeight.Value * behaviourForce.magnitude / acceleration;
					// TODO: define a mapping from behaviour-type to color, and provide some way to only draw lines for some behaviours
					SteeringUtilities.drawDebugVector(this, 0.1f * behaviourForce, BEHAVIOUR_COLORS[i++ % BEHAVIOUR_COLORS.Length]);
				}
				// TODO: consider averaging the desired velocities instead of forces
				if (totalWeight > 0f) {
                    //SteeringUtilities.drawDebugVector(this, 0.1f * totalForce / totalWeight, Color.blue);
                    // TODO: scale the force with the object's mass
                    AddForce(totalForce / totalWeight);
                }
				// Enforce a maximum speed
				float velocitySquared = GetVelocity().sqrMagnitude;
                if (velocitySquared > maxSpeed * maxSpeed)
                {
                    SetVelocity(SteeringUtilities.scaledVector(maxSpeed, GetVelocity()));
                }
				// Turn toward the current velocity (so that the x axis is forward)
				if (turnAutomatically && velocitySquared > 0.5) {
					TurnToward(SteeringUtilities.angleForVector(GetVelocity()));
				}
                if (velocitySquared > 0f)
                {
                    _movingDirection = GetVelocity();
                }
				//SteeringUtilities.drawDebugVector(this, (0.5f * getVelocity()), VELOCITY_COLOR);
			}

			public void SetSize(float radius) {
				this.radius = radius;
			}

			public void SetSpeed(float maxSpeed, float acceleration) {
				this.maxSpeed = maxSpeed;
				this.acceleration = acceleration;
			}

			public float GetSize() {
				return radius;
			}

			public float GetMaxSpeed() {
				return maxSpeed;
			}

			public float GetAcceleration() {
				return acceleration;
			}

			public Vector3 GetPosition() {
                // TODO: consistently zero out irrelevant axis for any logic related to positions
				return new Vector3(transform.position.x, YMult * transform.position.y, ZMult * transform.position.z);
            }

            public Vector3 GetVelocity()
            {
                // TODO: maybe zero out irrelevant axis, maybe it is fine because of the physics constraint
                return _rb2D ? (Vector3)_rb2D.velocity : _rb3D.velocity;
            }

            public Vector3 GetDirection()
            {
                return _movingDirection.normalized;
            }

            public float GetStoppingTime() {
				return GetVelocity().magnitude / GetAcceleration();
			}

			public float GetStoppingDistance() {
				// StopTime = speed/accel, AvgSpeed while stoppping = speed/2, Distance = speed^2 / (accel * 2)
				return GetVelocity().sqrMagnitude / (GetAcceleration() * 2);
            }

            private void SetVelocity(Vector3 velocity)
            {
                if (_rb2D)
                {
                    _rb2D.velocity = velocity;
                }
                else
                {
                    _rb3D.velocity = velocity;
                }
            }

            private void AddForce(Vector3 force)
            {
                if (_rb2D)
                {
                    _rb2D.AddForce((Vector2)force);
                }
                else
                {
                    _rb3D.AddForce(force);
                }
            }

            private void TurnToward(float desiredAngle) {
                // TODO: verify that the sign of these numbers is correct for both XY and XZ
				float currentAngle = (ZMult * transform.localEulerAngles.y) + (YMult * transform.localEulerAngles.z);
				float angleDiff = SteeringUtilities.angleDiff(currentAngle, desiredAngle);
				float rotationPerFrame = turnRate * Time.fixedDeltaTime;
				if (angleDiff > rotationPerFrame) {
                    float newAngle = currentAngle + rotationPerFrame;
                    transform.localEulerAngles = new Vector3(0, ZMult * newAngle, YMult * newAngle);
				} else if (angleDiff < -rotationPerFrame) {
                    float newAngle = currentAngle - rotationPerFrame;
                    transform.localEulerAngles = new Vector3(0, ZMult * newAngle, YMult * newAngle);
				} else {
					transform.localEulerAngles = new Vector3(0, ZMult * desiredAngle, YMult * desiredAngle);
				}
			}
		}
	}
}