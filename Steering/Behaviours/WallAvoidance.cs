using UnityEngine;

public class WallAvoidance : SteeringBehaviour
{
	private float desiredDistance;
	private int layerMask;

	public WallAvoidance(float desiredDistance = 0.75f, int layerMask = Physics2D.DefaultRaycastLayers) {
		this.desiredDistance = desiredDistance;
		this.layerMask = layerMask;
	}

	public Vector2 getForce(Steering steering) {
		float raycastDistance = 3f * desiredDistance;
		// TODO: scale raycast distance with current velocity (aka stoppping distance)
		// TODO: send out 3 rays and define static quaternions to determine the rotated direction vectors.
		RaycastHit2D hitInfo = raycast(steering, raycastDistance);
		if (hitInfo.collider != null) {
			SteeringUtilities.drawDebugPoint(hitInfo.point, Color.white);
			SteeringUtilities.drawDebugVector(steering, hitInfo.point - steering.getPosition(), Color.white);
			// Steer in the direction of the component of the collision normal that is perpindicular to the wall.
			// This way the unit will turn instead of just slowing down.
			//Vector2 targetPoint = hitInfo.point + desiredDistance * hitInfo.normal;
			//SteeringUtilities.drawDebugPoint(hitInfo.point, Color.white);
			//SteeringUtilities.drawDebugPoint(targetPoint, Color.green);
			//return SteeringUtilities.getSeekForce(steering, targetPoint);
			return steering.getAcceleration() * SteeringUtilities.perpindicularComponent(hitInfo.normal, steering.getVelocity());
		}
		return Vector2.zero;
	}

	private RaycastHit2D raycast(Steering steering, float raycastDistance) {
		return Physics2D.Raycast(steering.getPosition(), steering.getVelocity(), raycastDistance, layerMask);
	}
}
