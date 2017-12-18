using UnityEngine;

public class UnalignedCollisionAvoidance : SteeringBehaviour
{
	private bool isResponsibleForNeighbourUpdate = false;
	Neighbours<Steering, Steering> nearbyUnits;

	public UnalignedCollisionAvoidance(Steering currentObject) {
		isResponsibleForNeighbourUpdate = true;
		nearbyUnits = new Neighbours<Steering, Steering>(currentObject);
		nearbyUnits.AddRange(Object.FindObjectsOfType<Steering>());
		nearbyUnits.Remove(currentObject);
	}

	public UnalignedCollisionAvoidance(Neighbours<Steering, Steering> neighbours) {
		isResponsibleForNeighbourUpdate = false;
		nearbyUnits = neighbours;
	}

	public Vector2 getForce(Steering steering) {
		if (isResponsibleForNeighbourUpdate) {
			// TODO: figure out how to add or remove neighbours automatically here or in neighbours
			nearbyUnits.Update();
		}

		/* 
		 * Avoid collisions by determining for each neighbor when their paths will be closest to each other
		 * and then steer laterally to avoid collision.
		 * https://www.red3d.com/cwr/steer/Unaligned.html
		 */
		float distanceToBeginReacting = 4f * (steering.getSize() + steering.getStoppingDistance());
		//Debug.Log(doubleStopDistance);
		foreach (Neighbour<Steering> neighbour in nearbyUnits) {
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
			SteeringUtilities.drawDebugVector(otherUnit, timeToCollision * otherUnit.getVelocity(), Color.cyan);
			return SteeringUtilities.getForceForDirection(steering, -closestOffset);
		}
		return Vector2.zero;
	}
}
