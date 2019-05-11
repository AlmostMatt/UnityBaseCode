using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class WallAvoidance : SteeringBehaviour
        {
            private static Quaternion xyRotateLeft = Quaternion.AngleAxis(25f, Vector3.forward);
            private static Quaternion xyRotateRight = Quaternion.AngleAxis(-25f, Vector3.forward);
            private static Quaternion xzRotateLeft = Quaternion.AngleAxis(25f, Vector3.up);
            private static Quaternion xzRotateRight = Quaternion.AngleAxis(-25f, Vector3.up);

            private int layerMask;

			public WallAvoidance(int layerMask = Physics2D.DefaultRaycastLayers) {
				this.layerMask = layerMask;
			}

			public Vector3 GetForce(Steering steering) {
				float raycastDistance = (2f * steering.GetSize()) + 2f * (steering.GetMaxSpeed() * steering.GetMaxSpeed() / steering.GetAcceleration());
				// TODO: send out 3 rays and define static quaternions to determine the rotated direction vectors.
				Vector3 directionVector = steering.GetVelocity();
				// TODO: update the side raycast lengths based on steering's size, and the center raycast based on speed.
				Vector3 hitNormal1 = Raycast(steering, (Steering.UseXZ ? xzRotateLeft : xyRotateLeft) * directionVector, raycastDistance * 0.5f);
				Vector3 hitNormal2 = Raycast(steering, directionVector, raycastDistance);
				Vector3 hitNormal3 = Raycast(steering, (Steering.UseXZ ? xzRotateRight : xyRotateRight) * directionVector, raycastDistance * 0.5f);
				// If multiple raycasts hit, sum the normals.
				// TODO: weight the normals differently based on which collision point is closest.
				Vector3 combinedNormal = SteeringUtilities.scaledDownVector(1f, hitNormal1 + hitNormal2 + hitNormal3);
				if (combinedNormal.sqrMagnitude > 0f) {
					// Steer in the direction of the component of the collision normal that is perpindicular to the current velocity.
					// This way the unit will turn instead of just slowing down.
					return SteeringUtilities.scaledVector(steering.GetAcceleration(), SteeringUtilities.perpindicularComponent(combinedNormal, steering.GetVelocity()));
					//return SteeringUtilities.getForceForDirection(steering, combinedNormal);
				}
				return Vector3.zero;
			}

			// Performs a raycast and returns the normal of the collision or the zero vector.
			private Vector3 Raycast(Steering steering, Vector3 directionVector, float raycastDistance) {
                if (Steering.Using3D)
                {
                    RaycastHit hitInfo;
                    Physics.Raycast(steering.GetPosition(), directionVector, out hitInfo, raycastDistance, layerMask);
                    //SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), Color.white);
                    if (hitInfo.collider != null)
                    {
                        SteeringUtilities.drawDebugPoint(hitInfo.point, Color.magenta);
                        SteeringUtilities.drawDebugVector(steering, hitInfo.point - steering.GetPosition(), Color.magenta);
                        return hitInfo.normal;
                    }
                } else
                {
                    RaycastHit2D hitInfo = Physics2D.Raycast(steering.GetPosition(), directionVector, raycastDistance, layerMask);
                    //SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), Color.white);
                    if (hitInfo.collider != null)
                    {
                        SteeringUtilities.drawDebugPoint(hitInfo.point, Color.magenta);
                        SteeringUtilities.drawDebugVector(steering, (Vector3)hitInfo.point - steering.GetPosition(), Color.magenta);
                        return hitInfo.normal;
                    }
                }
				return Vector3.zero;
			}
		}
	}
}