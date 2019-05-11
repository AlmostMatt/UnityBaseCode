using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
    {
        // Moves directly towards a point
        public class Seek : SteeringBehaviour
		{
			Vector3 target;

			public Seek(Vector3 target) {
				this.target = target;
			}

			public void setTarget(Vector3 newTarget) {
				this.target = newTarget;
			}

			public Vector3 GetForce(Steering steering) {
				return SteeringUtilities.getSeekForce(steering, target);
			}
		}
	}
}