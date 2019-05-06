using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		// Chases another Steering object and considers the velocity of the target.
		public class Pursue : SteeringBehaviour
		{
			private const float MAX_PREDICTION_TIME = 2.5f; 
			private Steering target;

			public Pursue(Steering otherObject) {
				target = otherObject;
			}

			public Vector2 getForce(Steering steering) {
				Vector2 currentOffset = target.getPosition() - steering.getPosition();
				float dist = currentOffset.magnitude;

				Vector2 unitV = steering.getVelocity().normalized;

				float parallelness = Vector2.Dot(unitV, target.getVelocity().normalized);
				float forwardness = Vector2.Dot (unitV, currentOffset/dist);

				float halfsqrt2 = 0.707f;
				int f = SteeringUtilities.intervalComp(forwardness, -halfsqrt2, halfsqrt2);
				int p = SteeringUtilities.intervalComp(parallelness, -halfsqrt2, halfsqrt2);

				// approximate how far to lead the target
				float timeFactor = 1f;
				// case logic based on (ahead, aside, behind) X (parallel, perp, anti-parallel)
				switch (f) {
				case 1: //target is ahead
					switch (p) {
					case 1:
						timeFactor = 4f;
						break;
					case 0:
						timeFactor = 1.8f;
						break;
					case -1:
						timeFactor = 0.85f;
						break;
					}
					break;
				case 0: //target is aside
					switch (p) {
					case 1:
						timeFactor = 1f;
						break;
					case 0:
						timeFactor = 0.8f;
						break;
					case -1:
						timeFactor = 4f;
						break;
					}
					break;
				case -1: //target is behind
					switch (p) {
					case 1:
						timeFactor = 0.5f;
						break;
					case 0:
						timeFactor = 2f;
						break;
					case -1:
						timeFactor = 2f;
						break;
					}
					break;
				}

				// Multiply the timeToArrive by some approximate constants based on how similar the two velocities are.
				float approximateArrivalTime = dist/steering.getMaxSpeed();
				float improvedArrivalTimeEstimate = Mathf.Min (MAX_PREDICTION_TIME, approximateArrivalTime * timeFactor);
				Vector2 newTargetPosition = (Vector2) target.getPosition() + improvedArrivalTimeEstimate * target.getVelocity();

				SteeringUtilities.drawDebugVector(target, newTargetPosition - target.getPosition(), Color.white);
				SteeringUtilities.drawDebugVector(steering, newTargetPosition - steering.getPosition(), Color.magenta);

				return SteeringUtilities.getForceForDirection(steering, newTargetPosition - steering.getPosition());
			}
		}
	}
}
