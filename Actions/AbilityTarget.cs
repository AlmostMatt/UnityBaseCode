using UnityEngine;
using UnityBaseCode.Statuses;

namespace UnityBaseCode
{
	namespace Actions
	{
        // TODO: consider having GameObject or Vector as target directly
		public class AbilityTarget
		{
			// one of the following will be present
			private Attackable attackableTarget;
			private Vector3 positionTarget;

			public AbilityTarget(Attackable attackableTarget) {
				this.attackableTarget = attackableTarget;
			}

			public AbilityTarget(Vector3 positionTarget) {
				this.positionTarget = positionTarget;
			}

			public Attackable getAttackableTarget() {
				return this.attackableTarget;
			}

			public Vector3 getTargetPosition() {
				if (attackableTarget != null) {
					return attackableTarget.transform.position;
				}
				return positionTarget;
			}
		}
	}
}