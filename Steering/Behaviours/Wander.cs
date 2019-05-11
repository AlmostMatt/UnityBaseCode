using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		// As described by https://www.red3d.com/cwr/steer/Wander.html
		public class Wander : SteeringBehaviour
		{
			private float wanderNoise = 6f;
			private Vector3 wanderPoint;
			private bool hasMoved = false;
			public Wander() {}

			public Vector3 GetForce(Steering steering) {
				if (steering.GetVelocity().sqrMagnitude == 0f) {
					float initialAngle = Random.Range(0f, 2 * Mathf.PI);
                    float sinAngle = Mathf.Sin(initialAngle);
                    wanderPoint = 2.4f * new Vector3(Mathf.Cos(initialAngle), Steering.YMult * sinAngle, Steering.ZMult * sinAngle);
				}
				if (steering.GetVelocity().sqrMagnitude > 0f && !hasMoved) {
					hasMoved = true;
					wanderPoint = SteeringUtilities.scaledVector(2.4f, steering.GetVelocity());
				}
                float xNoise = Time.fixedDeltaTime * wanderNoise * Random.Range(-1f, 1f);
                float yzNoise = Time.fixedDeltaTime * wanderNoise * Random.Range(-1f, 1f);
                wanderPoint += new Vector3(xNoise, Steering.YMult * yzNoise, Steering.ZMult * yzNoise);
				Vector3 forwardPoint = steering.GetPosition() + SteeringUtilities.scaledVector(1.41f, steering.GetVelocity());
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