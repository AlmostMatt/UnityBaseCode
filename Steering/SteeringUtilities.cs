using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
	{
		public class SteeringUtilities
		{
			public static Vector3 getSeekForce(Steering steering, Vector3 targetPosition) {
				return getForceForDirection(steering, targetPosition - steering.GetPosition());
			}

			public static Vector3 getForceForDirection(Steering steering, Vector3 desiredDirection) {
				return getForceForDesiredVelocity(steering, scaledVector(steering.GetMaxSpeed(), desiredDirection));
			}

			public static Vector3 getForceForDesiredVelocity(Steering steering, Vector3 desiredVelocity) {
				// The velocity will be changed by force * deltaTime, so the desired force is deltaV/deltaTime.
				Vector3 desiredForce = (desiredVelocity - steering.GetVelocity()) / Time.fixedDeltaTime;
				return scaledDownVector(steering.GetAcceleration(), desiredForce);
			}

			// Returns a vector that has a specified magnitude and direction
			public static Vector3 scaledVector(float magnitude, Vector3 directionVector) {
				return magnitude * directionVector.normalized;
			}

			// If the vector is larger than maxMagnitude, it is scaled down to that amount. otherwise it is returned as-is.
			public static Vector3 scaledDownVector(float maxMagnitude, Vector3 vector) {
				return vector.sqrMagnitude > maxMagnitude * maxMagnitude ? maxMagnitude * vector.normalized : vector;
			}

			// The component of v1 in the direction of v2
			public static Vector3 parallelComponent(Vector3 vector, Vector3 direction) {
				return direction * Vector3.Dot(vector, direction)/direction.sqrMagnitude;
			}

			// The component of v1 that is perpindicular to v2 along the relevant XY or XZ plane
			public static Vector3 perpindicularComponent(Vector3 vector, Vector3 direction) {
                Vector3 perpindicularVector = new Vector3((-direction.y) - direction.z, Steering.YMult * direction.x, Steering.ZMult * direction.x);
				return parallelComponent(vector, perpindicularVector);
			}

			// Returns the difference between two angles within the [-180,180) range
			public static float angleDiff(float oldAngle, float newAngle) {
				// Csharp doesn't handle negative modulo the way that I want, so I add 180+360+360 to make it positive.
				return ((900 + newAngle - oldAngle) % 360f) - 180;
			}

			// Returns the angle in degrees corresponding to a vector.
			public static float angleForVector(Vector3 direction) {
				return Mathf.Rad2Deg * Mathf.Atan2(direction.y + direction.z, direction.x);
			}

			// spaceship operator to an interval
			public static int intervalComp(float f, float intervalMin, float intervalMax) {
				if (f < intervalMin) {
					return -1;
				} else if (f <= intervalMax) {
					return 0;
				} else {
					return 1;
				}
			}

			public static bool sameDirection(Vector3 v1, Vector3 v2) {
				return Vector3.Dot(v1, v2) >= 0f;
            }

            public static void drawDebugVector(Vector3 point1, Vector3 offsetVector, Color color)
            {
                Debug.DrawLine(point1, point1 + offsetVector, color, 0f, false);
            }

            public static void drawDebugVector(Steering steering, Vector3 offsetVector, Color color)
            {
                drawDebugVector(steering.GetPosition(), offsetVector, color);
            }

            public static void drawDebugPoint(Vector3 point, Color color) {
				drawDebugCircle(point, 0.1f, color, 4);
			}

			public static void drawDebugCircle(Vector3 point, float radius, Color color, int numPoints = 12) {
				Vector3 prevPoint = point;
				for (int i=0; i< numPoints+1; i++) {
					float angle = 2 * Mathf.PI * i / numPoints;
					Vector3 curPoint = point + radius * new Vector3(Mathf.Cos(angle), Steering.YMult * Mathf.Sin(angle), Steering.ZMult * Mathf.Sin(angle));
					if (i > 0) {
						Debug.DrawLine(prevPoint, curPoint, color, 0f, false);
					}
					prevPoint = curPoint;
				}
			}
		}
	}
}