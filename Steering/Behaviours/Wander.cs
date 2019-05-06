using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		// As described by https://www.red3d.com/cwr/steer/Wander.html
		public class Wander : SteeringBehaviour
		{
			private float wanderNoise = 6f;
			private Vector2 wanderPoint;
			private bool hasMoved = false;
			public Wander() {}

			public Vector2 getForce(Steering steering) {
				if (steering.getVelocity().sqrMagnitude == 0f) {
					float initialAngle = Random.Range(0f, 2 * Mathf.PI);
					wanderPoint = 2.4f * new Vector2(Mathf.Cos(initialAngle), Mathf.Sin(initialAngle));
				}
				if (steering.getVelocity().sqrMagnitude > 0f && !hasMoved) {
					hasMoved = true;
					wanderPoint = SteeringUtilities.scaledVector(2.4f, steering.getVelocity());
				}
				wanderPoint += new Vector2(
					Time.fixedDeltaTime * wanderNoise * Random.Range(-1f, 1f),
					Time.fixedDeltaTime * wanderNoise * Random.Range(-1f, 1f));
				Vector2 forwardPoint = steering.getPosition() + SteeringUtilities.scaledVector(1.41f, steering.getVelocity());
				// Constrain the wander point to the unit circle in front of the player.
				wanderPoint = forwardPoint + (wanderPoint - forwardPoint).normalized;
				//return SteeringUtilities.getForceForDirection(steering, wanderDirection);
				//SteeringUtilities.drawDebugCircle(forwardPoint, 1f, Color.black, 32);
				SteeringUtilities.drawDebugPoint(wanderPoint, Color.red);
				return SteeringUtilities.getSeekForce(steering, wanderPoint);
			}
		}
	}
}