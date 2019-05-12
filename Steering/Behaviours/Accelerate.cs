using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class Accelerate : SteeringBehaviour
		{
			public Accelerate() {}

			public Vector3 GetForce(Steering steering) {
				return SteeringUtilities.getForceForDesiredVelocity(steering, SteeringUtilities.scaledVector(steering.GetMaxSpeed(), steering.GetDirection()));
			}
		}
	}
}