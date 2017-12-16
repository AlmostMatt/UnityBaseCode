using UnityEngine;

public class Seek : SteeringBehaviour
{
	Vector2 target;

	public Seek(Vector2 target) {
		this.target = target;
	}

	public Vector2 getForce(Steering steering) {
		return SteeringUtilities.getSeekForce(steering, target);
	}
}
