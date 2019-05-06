using UnityEngine;
using UnityBaseCode.Statuses;

namespace UnityBaseCode
{
	namespace Actions
	{
		public interface Attackable {
			Transform transform
			{
				get;
			}
			float radius
			{
				get;
			}
			bool dead
			{
				get;
			}
			void damage(Actor attacker, int amount);
		}
	}
}