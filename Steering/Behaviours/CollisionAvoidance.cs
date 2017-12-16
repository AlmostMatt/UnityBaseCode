using UnityEngine;

/* 
 * Avoid collisions by determining for each neighbor when their paths will be closest to each other
 * and then steer laterally to avoid collision.
 * https://www.red3d.com/cwr/steer/Unaligned.html
 */
// Neighbours<T> neighbours where T : Steering

public class CollisionAvoidance : SteeringBehaviour
{
	Vector2 target;

	public CollisionAvoidance(Vector2 target) {
		this.target = target;
	}

	public Vector2 getForce(Steering steering) {
		return SteeringUtilities.getSeekForce(steering, target);
	}
}
