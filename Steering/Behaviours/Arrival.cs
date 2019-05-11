using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class Arrival : SteeringBehaviour
		{
			Vector3 target;

			public Arrival(Vector3 target) {
				this.target = target;
			}

			public void setTarget(Vector3 newTarget) {
				this.target = newTarget;
			}

			public Vector3 GetForce(Steering steering) {
				return SteeringUtilities.getSeekForce(steering, target);
                // TODO: actually use stopping distance to arrive
				/*
				 * // can approximate with (if too close : break)
				// alternatively stopdist from current pos and current speed
				// or expected stopdist after the current frame assuming accel wont change the current frame much

				// for the duration it takes to go from maxV to 0
				//stoptime = v/ACCEL;
				//stopdist = stoptime * v/2;
				//dist = desiredv * desiredV/(2 * accel) 
				//desiredv = sqrt(dist * 2 * accel) 

				Vector3 offset = dest - ((Vector3) transform.position + (dt * rb.velocity));

				float dist = offset.magnitude;
				float stopdist = MAXV * MAXV / (2 * ACCEL);
				float desiredV = dist < stopdist ? Mathf.Sqrt(dist * 2 * ACCEL) : MAXV;
				Vector3 deltaV = scaled (desiredV, offset) - rb.velocity;
				float dvmagn = deltaV.magnitude;
				if (dvmagn > forceRemaining * forceRemaining * dt * dt) {
					rb.AddForce(scaled(forceRemaining, deltaV));
					forceRemaining = 0f;
				} else {
					rb.AddForce(deltaV);
					forceRemaining -= dvmagn;
				}
				*/
			}
		}
	}
}