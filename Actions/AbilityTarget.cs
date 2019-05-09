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
			private GameObject objectTarget;
			private Vector3 positionTarget;

			public AbilityTarget(GameObject objectTarget) {
				this.objectTarget = objectTarget;
			}

			public AbilityTarget(Vector3 positionTarget) {
				this.positionTarget = positionTarget;
			}

			public GameObject getTargetObject() {
				return this.objectTarget;
			}

			public Vector3 getTargetPosition() {
				if (objectTarget != null) {
					return objectTarget.transform.position;
				}
				return positionTarget;
			}
		}
	}
}