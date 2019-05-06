using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class Separate : SteeringBehaviour
		{
			private float preferredDistance;
			private bool isResponsibleForNeighbourUpdate = false;
			Neighbours<Steering, Steering> neighbours;

			/**
			 * Will move away from nearby units if the current distance is less than the preferred distance.
			 */
			public Separate(Steering currentObject, float preferredDistance) {
				this.preferredDistance = preferredDistance;
				isResponsibleForNeighbourUpdate = true;
				this.neighbours = new Neighbours<Steering, Steering>(currentObject);
				this.neighbours.AddRange(Object.FindObjectsOfType<Steering>());
				this.neighbours.Remove(currentObject);
			}

			public Separate(Neighbours<Steering, Steering> neighbours, float preferredDistance) {
				this.preferredDistance = preferredDistance;
				isResponsibleForNeighbourUpdate = false;
				this.neighbours = neighbours;
			}

			public Vector2 getForce(Steering steering) {
				Vector2 steeringVector = new Vector2(0f, 0f);
				// steer away from each object that is too close with a weight of up to 0.5 for each
				foreach (Neighbour<Steering> neighbour in neighbours) {
					if (neighbour.dd > preferredDistance * preferredDistance) {
						break;
					}
					Steering otherUnit = neighbour.obj;
					Vector2 offset = otherUnit.getPosition() - steering.getPosition();
					float currentDistance = Mathf.Sqrt(neighbour.dd);
					// TODO: consider relative velocity when computing importance
					float importance = (preferredDistance - currentDistance)/preferredDistance;
					steeringVector += -importance * offset;
				}
				if (steeringVector.sqrMagnitude > 0) {
					// TODO: do something like arrival to avoid over-separating
					return SteeringUtilities.getForceForDirection(steering, steeringVector);
				}
				return Vector2.zero;
			}
		}
	}
}