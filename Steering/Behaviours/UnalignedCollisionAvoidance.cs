using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class UnalignedCollisionAvoidance : SteeringBehaviour
		{
			private bool isResponsibleForNeighbourUpdate = false;
			Neighbours<Steering, Steering> neighbours;

			public UnalignedCollisionAvoidance(Steering currentObject) {
				isResponsibleForNeighbourUpdate = true;
				this.neighbours = new Neighbours<Steering, Steering>(currentObject);
				this.neighbours.AddRange(Object.FindObjectsOfType<Steering>());
				this.neighbours.Remove(currentObject);
			}

			// TODO: figure out how to convert back and forth between neighbour<unit> and neighbour<steering>
			// Possibly with GetComponent, with an interface ObjectWithSteering, or with a typed generic function
			public UnalignedCollisionAvoidance(Neighbours<Steering, Steering> neighbours) {
				isResponsibleForNeighbourUpdate = false;
				this.neighbours = neighbours;
			}

			public Vector2 getForce(Steering steering) {
				if (isResponsibleForNeighbourUpdate) {
					// TODO: figure out how to add or remove neighbours automatically here or in neighbours
					neighbours.Update();
				}

				/* 
				 * Avoid collisions by determining for each neighbor when their paths will be closest to each other
				 * and then steer laterally to avoid collision.
				 * https://www.red3d.com/cwr/steer/Unaligned.html
				 */
				float distanceToBeginReacting = 4f * (steering.getSize() + steering.getStoppingDistance());
				//Debug.Log(doubleStopDistance);
				foreach (Neighbour<Steering> neighbour in neighbours) {
					if (neighbour.dd > distanceToBeginReacting  * distanceToBeginReacting) {
						break;
					}
					Steering otherUnit = neighbour.obj;
					Vector2 offset = otherUnit.getPosition() - steering.getPosition();
					Vector2 relativeVelocity = steering.getVelocity() - otherUnit.getVelocity();
					// Decrease the timeToCollision so that closestOffset is nonZero.
					float combinedSize = steering.getSize() + otherUnit.getSize();
					float timeToCollision = (offset.magnitude - combinedSize) / SteeringUtilities.parallelComponent(relativeVelocity, offset).magnitude;
					if (timeToCollision > 2 * steering.getStoppingTime()) {
						continue;
					}
					Vector2 closestOffset = (offset - (timeToCollision * relativeVelocity));
					float preferredDistance = 1.5f * combinedSize;
					if (closestOffset.sqrMagnitude > preferredDistance * preferredDistance) {
						continue;
					}
					SteeringUtilities.drawDebugVector(steering, timeToCollision * steering.getVelocity(), Color.cyan);
					SteeringUtilities.drawDebugPoint(steering.getPosition() + timeToCollision * steering.getVelocity(), Color.cyan);
					SteeringUtilities.drawDebugVector(otherUnit, timeToCollision * otherUnit.getVelocity(), Color.cyan);
					SteeringUtilities.drawDebugPoint(otherUnit.getPosition() + timeToCollision * otherUnit.getVelocity(), Color.cyan);
					// TODO: for head-on collisions steer to the right
					// Steer in the direction of the component of the collision normal that is perpindicular to the current velocity.
					// This way the unit will turn instead of just slowing down.

					// TODO: use an amount of acceleration proportionate to the time until collision and the severity of the collision
					return SteeringUtilities.scaledVector(steering.getAcceleration(), SteeringUtilities.perpindicularComponent(-closestOffset, steering.getVelocity()));
					//return SteeringUtilities.getForceForDirection(steering, -closestOffset);
				}
				return Vector2.zero;
			}
		}
	}
}