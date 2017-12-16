using UnityEngine;

public class Wander : SteeringBehaviour
{
	private float wanderNoise = 1f;
	private Vector2 wanderDirection;

	public Vector2 getForce(Steering steering) {
		wanderDirection += new Vector2(
			Time.fixedDeltaTime * wanderNoise * Random.Range(-1f, 1f),
			Time.fixedDeltaTime * wanderNoise * Random.Range(-1f, 1f));
		wanderDirection.Normalize();
		return SteeringUtilities.getForceForDirection(steering, wanderDirection);
	}
}

