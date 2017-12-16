using UnityEngine;

// Moves directly away from a point
public class Flee : SteeringBehaviour
{
	Vector2 target;

	public Flee(Vector2 target) {
		this.target = target;
	}

	public Vector2 getForce(Steering steering) {
		return -SteeringUtilities.getSeekForce(steering, target);
	}
}
