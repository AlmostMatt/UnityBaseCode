using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		// Moves away from another Steering object and considers the speed of the other object.
		public class Evade : SteeringBehaviour
		{
			Pursue pursue;

			public Evade(Steering otherObject) {
				this.pursue = new Pursue(otherObject);
			}

			public Vector2 getForce(Steering steering) {
				return -pursue.getForce(steering);
			}
		}
	}
}