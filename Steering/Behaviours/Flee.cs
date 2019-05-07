using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		// Moves directly away from a point
		public class Flee : SteeringBehaviour
		{
			Vector2 target;

			public Flee(Vector2 target) {
				this.target = target;
            }

            public void setTarget(Vector2 newTarget)
            {
                this.target = newTarget;
            }

            public Vector2 getForce(Steering steering) {
				return -SteeringUtilities.getSeekForce(steering, target);
			}
		}
	}
}