using UnityEngine;

public class UnalignedCollisionAvoidance : SteeringBehaviour
{
	Steering[] allUnits;
	Neighbours<Steering> nearbyUnits = new Neighbours<Steering>();

	public UnalignedCollisionAvoidance() {
		allUnits = Object.FindObjectsOfType<Steering>();
	}

	public Vector2 getForce(Steering steering) {
		nearbyUnits.Clear();
		// TODO: figure out how to efficiently track 'which units are nearby'
		foreach (Steering unit in allUnits) {
			if (unit != steering) {
				nearbyUnits.Add(unit, (unit.getPosition() - steering.getPosition()).sqrMagnitude);
			}
		}
		/* 
		 * Avoid collisions by determining for each neighbor when their paths will be closest to each other
		 * and then steer laterally to avoid collision.
		 * https://www.red3d.com/cwr/steer/Unaligned.html
		 */
		foreach (Neighbour<Steering> neighbour in nearbyUnits) {
			if (neighbour.dd > 4f * 4f) {
				break;
			}
			Steering otherUnit = neighbour.obj;
			Vector2 offset = otherUnit.getPosition() - steering.getPosition();
			Vector2 relativeVelocity = steering.getVelocity() - otherUnit.getVelocity();
			// Decrease the timeToCollision so that closestOffset is nonZero.
			float timeToCollision = 0.99f * offset.magnitude / SteeringUtilities.parallelComponent(relativeVelocity, offset).magnitude;
			if (timeToCollision > 2 * steering.getStoppingTime()) {
				continue;
			}
			Vector2 closestOffset = (offset - (timeToCollision * relativeVelocity));
			if (closestOffset.sqrMagnitude > 0.8f * 0.8f) {
				continue;
			}
			SteeringUtilities.drawDebugVector(steering, timeToCollision * steering.getVelocity(), Color.cyan);
			SteeringUtilities.drawDebugVector(otherUnit, timeToCollision * otherUnit.getVelocity(), Color.cyan);
			return SteeringUtilities.getForceForDirection(steering, -closestOffset);
		}
		return Vector2.zero;
	}
}
