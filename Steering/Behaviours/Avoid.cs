using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class Avoid : SteeringBehaviour
		{
			Vector2 target;

			public Avoid(Vector2 target) {
				this.target = target;
			}

			public Vector2 getForce(Steering steering) {
				return SteeringUtilities.getSeekForce(steering, target);
			}
		}
	}
}