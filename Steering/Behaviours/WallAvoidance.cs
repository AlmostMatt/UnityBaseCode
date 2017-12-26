using UnityEngine;

public class WallAvoidance : SteeringBehaviour
{
	private Quaternion rotateLeftQuaternion = Quaternion.AngleAxis(25f, Vector3.forward);
	private Quaternion rotateRightQuaternion = Quaternion.AngleAxis(-25f, Vector3.forward);

	private int layerMask;

	public WallAvoidance(int layerMask = Physics2D.DefaultRaycastLayers) {
		this.layerMask = layerMask;
	}

	public Vector2 getForce(Steering steering) {
		float raycastDistance = (2f * steering.getSize()) + 2f * (steering.getMaxSpeed() * steering.getMaxSpeed() / steering.getAcceleration());
		// TODO: send out 3 rays and define static quaternions to determine the rotated direction vectors.
		Vector3 directionVector = steering.getVelocity();
		// TODO: update the side raycast lengths based on steering's size, and the center raycast based on speed.
		Vector2 hitNormal1 = raycast(steering, rotateLeftQuaternion * directionVector, raycastDistance * 0.5f);
		Vector2 hitNormal2 = raycast(steering, directionVector, raycastDistance);
		Vector2 hitNormal3 = raycast(steering, rotateRightQuaternion * directionVector, raycastDistance * 0.5f);
		// If multiple raycasts hit, sum the normals.
		// TODO: weight the normals differently based on which collision point is closest.
		Vector2 combinedNormal = SteeringUtilities.scaledDownVector(1f, hitNormal1 + hitNormal2 + hitNormal3);
		if (combinedNormal.sqrMagnitude > 0f) {
			// Steer in the direction of the component of the collision normal that is perpindicular to the current velocity.
			// This way the unit will turn instead of just slowing down.
			return SteeringUtilities.scaledVector(steering.getAcceleration(), SteeringUtilities.perpindicularComponent(combinedNormal, steering.getVelocity()));
			//return SteeringUtilities.getForceForDirection(steering, combinedNormal);
		}
		return Vector2.zero;
	}

	// Performs a raycast and returns the normal of the collision or the zero vector.
	private Vector2 raycast(Steering steering, Vector2 directionVector, float raycastDistance) {
		RaycastHit2D hitInfo = Physics2D.Raycast(steering.getPosition(), directionVector, raycastDistance, layerMask);
		//SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), Color.white);
		if (hitInfo.collider != null) {
			SteeringUtilities.drawDebugPoint(hitInfo.point, Color.magenta);
			SteeringUtilities.drawDebugVector(steering, hitInfo.point - steering.getPosition(), Color.magenta);
			return hitInfo.normal;
		}
		return Vector2.zero;
	}
}
