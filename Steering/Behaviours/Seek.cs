using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class Seek : SteeringBehaviour
		{
			Vector2 target;

			public Seek(Vector2 target) {
				this.target = target;
			}

			public void setTarget(Vector2 newTarget) {
				this.target = newTarget;
			}

			public Vector2 getForce(Steering steering) {
				return SteeringUtilities.getSeekForce(steering, target);
			}
		}
	}
}