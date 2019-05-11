using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		// Moves directly away from a point
		public class Flee : SteeringBehaviour
		{
			Vector3 target;

			public Flee(Vector3 target) {
				this.target = target;
            }

            public void setTarget(Vector3 newTarget)
            {
                this.target = newTarget;
            }

            public Vector3 GetForce(Steering steering) {
				return -SteeringUtilities.getSeekForce(steering, target);
			}
		}
	}
}