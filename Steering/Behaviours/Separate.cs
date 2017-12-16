using UnityEngine;

public class Separate : SteeringBehaviour
{
	Vector2 target;

	public Separate(Vector2 target) {
		this.target = target;
	}

	public Vector2 getForce(Steering steering) {
		return SteeringUtilities.getSeekForce(steering, target);
		/*
		 * 	public void separate<T>(Neighbours<T> neighbours) where T : MonoBehaviour {
		float TOO_CLOSE = 0.8f; // radius is 0.
		Vector2 steer = new Vector2(0f, 0f);
		float totalforce = 0f;
		// steer away from each object that is too close with a weight of up to 0.5 for each
		foreach (Neighbour<T> neighbour in neighbours) {
			if (neighbour.dd > TOO_CLOSE * TOO_CLOSE) {
				break;
			}
			GameObject other = neighbour.obj.gameObject;
			Vector2 offset = other.transform.position - transform.position;
			float d = Mathf.Sqrt(neighbour.dd);
			if (d == 0f) {
				// Units spawn with identical position.
				offset = new Vector2(0.01f, 0f);
				d = 0.01f;
			}
			// only prioritize separation if the objects are moving toward each other
			float importance = 0.5f;//(sameDir(v1, v2) || !sameDir(v1, offset)) ? 0.3f : 0.6f;
			// force of 0.5 per other
			float force = importance * (TOO_CLOSE - d)/TOO_CLOSE;
			totalforce += force;
			steer += (- force / d) * offset;
			break;
		}
		float m = steer.magnitude * ACCEL;
		if (m >= forceRemaining) {
			steer = scaled(forceRemaining, steer);
			forceRemaining = 0f;
		} else {
			steer = ACCEL * steer;
			forceRemaining -= m;
		}
		rb.AddForce(steer);
	}
		 */
	}
}
