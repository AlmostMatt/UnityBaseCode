using UnityEngine;

public class Brake : SteeringBehaviour
{
	public Brake() {}

	public Vector2 getForce(Steering steering) {
		return SteeringUtilities.getForceForDesiredVelocity(steering, new Vector2());
	}
}
