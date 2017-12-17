using UnityEngine;

public class WallAvoidance : SteeringBehaviour
{
	private Quaternion rotateLeftQuaternion = Quaternion.AngleAxis(25f, Vector3.forward);
	private Quaternion rotateRightQuaternion = Quaternion.AngleAxis(-25f, Vector3.forward);

	private float desiredDistance;
	private int layerMask;

	public WallAvoidance(float desiredDistance = 0.75f, int layerMask = Physics2D.DefaultRaycastLayers) {
		this.desiredDistance = desiredDistance;
		this.layerMask = layerMask;
	}

	public Vector2 getForce(Steering steering) {
		// TODO: scale raycast distance with current velocity (aka stoppping distance)
		// TODO: send out 3 rays and define static quaternions to determine the rotated direction vectors.
		Vector3 directionVector = steering.getVelocity();
		Vector2 hitNormal1 = raycast(steering, rotateLeftQuaternion * directionVector, 1f);
		Vector2 hitNormal2 = raycast(steering, directionVector, 2.25f);
		Vector2 hitNormal3 = raycast(steering, rotateRightQuaternion * directionVector, 1f);
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
			SteeringUtilities.drawDebugPoint(hitInfo.point, Color.white);
			SteeringUtilities.drawDebugVector(steering, hitInfo.point - steering.getPosition(), Color.white);
			return hitInfo.normal;
		}
		return Vector2.zero;
	}
}
