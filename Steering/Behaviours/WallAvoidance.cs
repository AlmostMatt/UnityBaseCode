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
				float raycastDistance = 2f * (2f * steering.GetSize()) + 2f * (steering.GetMaxSpeed() * steering.GetMaxSpeed() / steering.GetAcceleration());
				// TODO: send out 3 rays and define static quaternions to determine the rotated direction vectors.
				Vector3 directionVector = steering.GetVelocity();
				// TODO: update the side raycast lengths based on steering's size, and the center raycast based on speed.
                // TODO: constrain hit normals to the relevant plane in case the collider has a complicated shape
				Vector3 hitNormal1 = RaycastNormal(steering, (Steering.UseXZ ? xzRotateLeft : xyRotateLeft) * directionVector, raycastDistance * 0.5f);
				Vector3 hitNormal2 = RaycastNormal(steering, directionVector, raycastDistance);
				Vector3 hitNormal3 = RaycastNormal(steering, (Steering.UseXZ ? xzRotateRight : xyRotateRight) * directionVector, raycastDistance * 0.5f);
				// If multiple raycasts hit, sum the normals.
				// TODO: weight the normals differently based on which collision point is closest.
				Vector3 combinedNormal = SteeringUtilities.scaledDownVector(1f, hitNormal1 + hitNormal2 + hitNormal3);
                if (combinedNormal.sqrMagnitude > 0f) {
                    // For the normal of the wall ahead, steer to have a velocity that is perpindicular to it.
                    Vector3 leftVector = new Vector3(combinedNormal.y + combinedNormal.z, -Steering.YMult * combinedNormal.x, -Steering.ZMult * combinedNormal.x);
                    Vector3 rightVector = new Vector3((-combinedNormal.y) - combinedNormal.z, Steering.YMult * combinedNormal.x, Steering.ZMult * combinedNormal.x);
                    float rayLeftDistance = RaycastDistance(steering, leftVector, raycastDistance);
                    float rayRightDistance = RaycastDistance(steering, rightVector, raycastDistance);
                    // TODO: Consider updating the logic when approaching a corner.
                    // Currently the unit kind of turns toward the side that they are coming from even if the other turn is shorter.
                    // Case 1: Wall in front, wall on left, wall on right.
                    if (rayLeftDistance < raycastDistance && rayRightDistance < raycastDistance)
                    {
                        // Move towards the left/right wall that is closer. That should be moving away from the corner point.
                        if (rayLeftDistance < rayRightDistance)
                        {
                            return SteeringUtilities.getForceForDirection(steering, leftVector);
                        }
                        else // rayRightDistance < rayLeftDistance
                        {
                            return SteeringUtilities.getForceForDirection(steering, rightVector);
                        }
                    }
                    // Case 2: Wall in front, wall on left
                    else if (rayLeftDistance < raycastDistance)
                    {
                        return SteeringUtilities.getForceForDirection(steering, rightVector);
                    }
                    // Case 3: Wall in front, wall on right
                    else if (rayRightDistance < raycastDistance)
                    {
                        return SteeringUtilities.getForceForDirection(steering, leftVector);
                    }
                    // Case 4: Wall in front
                    else
                    {
                        // Move towards whichever of left or right is closer to the current velocity
                        Vector3 perpindicularVector = SteeringUtilities.perpindicularComponent(combinedNormal, steering.GetVelocity());
                        // Edge case: the normal is parallel to velocity. In this case, pick one of the two perpindicular vectors at random.
                        if (perpindicularVector == Vector3.zero)
                        {
                            Debug.Log("Exact perpindicular!");
                            int randomSign = Random.value < .5 ? 1 : -1;
                            perpindicularVector = new Vector3((-combinedNormal.y) - combinedNormal.z, Steering.YMult * combinedNormal.x * randomSign, Steering.ZMult * combinedNormal.x * randomSign);
                        }
                        return SteeringUtilities.getForceForDirection(steering, perpindicularVector);
                    }
                }
				return Vector3.zero;
			}

            private RaycastHit Raycast(Steering steering, Vector3 directionVector, float raycastDistance)
            {
                RaycastHit hitInfo;
                Physics.Raycast(steering.GetPosition(), directionVector, out hitInfo, raycastDistance, layerMask);
                //SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), Color.white);
                if (hitInfo.collider != null)
                {
                    SteeringUtilities.drawDebugPoint(hitInfo.point, Color.magenta);
                    SteeringUtilities.drawDebugVector(steering, hitInfo.point - steering.GetPosition(), Color.magenta);
                    SteeringUtilities.drawDebugVector(hitInfo.point, hitInfo.normal, Color.white);
                }
                else
                {
                    SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.25f));
                }
                return hitInfo;
            }

            private RaycastHit2D Raycast2D(Steering steering, Vector3 directionVector, float raycastDistance)
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(steering.GetPosition(), directionVector, raycastDistance, layerMask);
                //SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), Color.white);
                if (hitInfo.collider != null)
                {
                    SteeringUtilities.drawDebugPoint(hitInfo.point, Color.magenta);
                    SteeringUtilities.drawDebugVector(steering, (Vector3)hitInfo.point - steering.GetPosition(), Color.magenta);
                    SteeringUtilities.drawDebugVector((Vector3)hitInfo.point, hitInfo.normal, Color.white);
                }
                else
                {
                    SteeringUtilities.drawDebugVector(steering, SteeringUtilities.scaledVector(raycastDistance, directionVector), new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.25f));
                }
                return hitInfo;
            }

            // Performs a raycast and returns the normal of the collision or the zero vector.
            private Vector3 RaycastNormal(Steering steering, Vector3 directionVector, float raycastDistance)
            {
                if (Steering.Using3D)
                {
                    RaycastHit hitInfo = Raycast(steering, directionVector, raycastDistance);
                    return hitInfo.collider == null ? Vector3.zero : hitInfo.normal;
                }
                RaycastHit2D hitInfo2D = Raycast2D(steering, directionVector, raycastDistance);
                return hitInfo2D.collider == null ? Vector3.zero : (Vector3)hitInfo2D.normal;
            }

            // Performs a raycast and returns the normal of the collision or the zero vector.
            private float RaycastDistance(Steering steering, Vector3 directionVector, float raycastDistance)
            {
                if (Steering.Using3D)
                {
                    RaycastHit hitInfo = Raycast(steering, directionVector, raycastDistance);
                    return hitInfo.collider == null ? raycastDistance : (hitInfo.point - steering.GetPosition()).magnitude;
                }
                RaycastHit2D hitInfo2D = Raycast2D(steering, directionVector, raycastDistance);
                return hitInfo2D.collider == null ? raycastDistance : ((Vector3)hitInfo2D.point - steering.GetPosition()).magnitude;
            }
        }
	}
}