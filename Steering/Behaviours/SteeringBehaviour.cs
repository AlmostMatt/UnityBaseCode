using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public interface SteeringBehaviour {
			Vector3 GetForce(Steering steering);
		}
	}
}
