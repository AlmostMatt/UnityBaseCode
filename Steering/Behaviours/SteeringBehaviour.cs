using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public interface SteeringBehaviour {
			Vector2 getForce(Steering steering);
		}
	}
}
