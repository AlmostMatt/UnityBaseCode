using UnityEngine;

public class Containment : SteeringBehaviour
{
	Vector2 target;

	public Containment(Vector2 target) {
		this.target = target;
	}

	public Vector2 getForce(Steering steering) {
		return SteeringUtilities.getSeekForce(steering, target);
	}
}
