using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class Brake : SteeringBehaviour
		{
			public Brake() {}

			public Vector3 GetForce(Steering steering) {
				return SteeringUtilities.getForceForDesiredVelocity(steering, Vector3.zero);
			}
		}
	}
}