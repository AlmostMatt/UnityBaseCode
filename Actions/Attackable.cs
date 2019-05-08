using UnityEngine;
using UnityBaseCode.Statuses;

namespace UnityBaseCode
{
	namespace Actions
	{
        // TODO: remove this class, use GameObject
		public interface Attackable {
			Transform transform
			{
				get;
			}
			float Radius
			{
				get;
			}
			bool Dead
			{
				get;
			}
			void Damage(GameObject attacker, int amount);
		}
	}
}