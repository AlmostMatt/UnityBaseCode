using UnityEngine;

public class SteeringUtilities
{
	public static Vector2 getSeekForce(Steering steering, Vector2 targetPosition) {
		return getForceForDirection(steering, targetPosition - steering.getPosition());
	}

	public static Vector2 getForceForDirection(Steering steering, Vector2 desiredDirection) {
		return getForceForDesiredVelocity(steering, scaledVector(steering.getMaxSpeed(), desiredDirection));
	}

	public static Vector2 getForceForDesiredVelocity(Steering steering, Vector2 desiredVelocity) {
		// The velocity will be changed by force * deltaTime, so the desired force is deltaV/deltaTime.
		Vector2 desiredForce = (desiredVelocity - steering.getVelocity()) / Time.fixedDeltaTime;
		return scaledDownVector(steering.getAcceleration(), desiredForce);
	}

	// Returns a vector that has a specified magnitude and direction
	public static Vector2 scaledVector(float magnitude, Vector2 directionVector) {
		return magnitude * directionVector.normalized;
	}

	// If the vector is larger than maxMagnitude, it is scaled down to that amount. otherwise it is returned as-is.
	public static Vector2 scaledDownVector(float maxMagnitude, Vector2 vector) {
		return vector.sqrMagnitude > maxMagnitude * maxMagnitude ? maxMagnitude * vector.normalized : vector;
	}

	// The component of v1 in the direction of v2
	public static Vector2 parallelComponent(Vector2 vector, Vector2 direction) {
		return direction * Vector2.Dot(vector, direction)/direction.sqrMagnitude;
	}

	// The component of v1 that is perpindicular to v2
	public static Vector2 perpindicularComponent(Vector2 vector, Vector2 direction) {
		return parallelComponent(vector, new Vector2(-direction.y, direction.x));
	}

	// Returns the difference between two angles within the [-180,180) range
	public static float angleDiff(float oldAngle, float newAngle) {
		return (180 + newAngle - oldAngle) % 360 - 180;
	}

	public static float angleForVector(Vector2 direction) {
		return Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
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

	public static bool sameDirection(Vector2 v1, Vector2 v2) {
		return Vector2.Dot(v1, v2) >= 0f;
	}

	public static void drawDebugVector(Steering steering, Vector2 vector, Color color) {
		Debug.DrawLine(steering.getPosition(), steering.getPosition() + vector, color, 0f, false);
	}

	public static void drawDebugPoint(Vector2 point, Color color) {
		drawDebugCircle(point, 0.1f, color, 4);
	}

	public static void drawDebugCircle(Vector2 point, float radius, Color color, int numPoints = 12) {
		Vector2 prevPoint = point;
		for (int i=0; i< numPoints+1; i++) {
			float angle = 2 * Mathf.PI * i / numPoints;
			Vector2 curPoint = point + radius * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			if (i > 0) {
				Debug.DrawLine(prevPoint, curPoint, color, 0f, false);
			}
			prevPoint = curPoint;
		}
	}
}
