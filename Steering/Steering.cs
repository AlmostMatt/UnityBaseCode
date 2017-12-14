using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Steering behaviours as described by https://red3d.com/cwr/steer/
 */
public class Steering : MonoBehaviour
{
	public bool turnAutomatically = true;

	public float MAXV = 5f;
	public float ACCEL = 20f;
	public float TURN_RATE = 900f;
	private float forceRemaining;

	private static float dt;

	private static float MAXPREDICTIONTIME = 2.5f; 

	private Rigidbody2D rb;

	/*
	 * Utility functions
	 */

	public void setSpeed(float maxV, float accel) {
		MAXV = maxV;
		ACCEL = accel;
	}

	public float getMaxSpeed() {
		return MAXV;
	}

	private Vector2 scaled(float n, Vector2 v) {
		return n * v.normalized;
	}

	private float angleDiff(float a1, float a2) {
		float diff = a2 - a1;
		// (diff + 180 mod 360) - 180
		if (diff > 180) {
			return diff - 360;
		} else if (diff < -180) {
			return diff + 360;
		} else {
			return diff;
		}
	}
	
	// spaceship operator to an interval
	private int intervalComp(float f, float intervalMin, float intervalMax) {
		if (f < intervalMin) {
			return -1;
		} else if (f <= intervalMax) {
			return 0;
		} else {
			return 1;
		}
	}

	public float angleToward(Vector2 target) {
		var diff = target - (Vector2) transform.position;
		return Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);
	}
	
	public void turnToward(Vector2 target) {
		turnToward(angleToward(target));
	}

	public void turnToward(float angle2) {
		float angle1 = transform.localEulerAngles.z;
		float diff = angleDiff(angle1, angle2);
		float rot = TURN_RATE * Time.fixedDeltaTime;
		if (diff > rot) {
			transform.localEulerAngles = new Vector3(0, 0, angle1 + rot);
		} else if (diff < -rot) {
			transform.localEulerAngles = new Vector3(0, 0, angle1 - rot);
		} else {
			transform.localEulerAngles = new Vector3(0, 0, angle2);
		}
	}

	private bool sameDir(Vector2 v1, Vector2 v2) {
		return Vector2.Dot(v1, v2) >= 0f;
	}

	/*
	 * Unity Events
	 */

	public virtual void Start() {
		rb = GetComponent<Rigidbody2D>();
		dt = Time.fixedDeltaTime;
	}

	public virtual void FixedUpdate () {
		// this should happen after actions are performed
		float vv = rb.velocity.sqrMagnitude;
		if (vv > MAXV * MAXV) {
			rb.velocity = MAXV * rb.velocity.normalized;
		}
		// Turn toward the current velocity (so that the x axis is forward)
		if (turnAutomatically && vv > 0.5) {
			float angle = Mathf.Rad2Deg * Mathf.Atan2(rb.velocity.y, rb.velocity.x);
			turnToward(angle);
		}
		forceRemaining = ACCEL;
	}
	
	/*
	 * Steering forces
	 * I assume that each of these will be applied immediately and use all remainingForce available
	 */
	
	// force to accel toward a point used by seek/arrival/flee
	private Vector2 seekForce(Vector2 dest, float maxforce) {
		Vector2 desiredV = scaled(MAXV, dest - (Vector2) transform.position);
		Vector2 deltaV = desiredV - rb.velocity;
		float dvmagn = deltaV.magnitude;
		if (dvmagn > maxforce * maxforce * dt * dt) {
			forceRemaining -= maxforce;
			return scaled(maxforce, deltaV);
		} else {
			forceRemaining -= dvmagn;
			return deltaV;
		}
	}
	
	// force to accel toward a unit used by pursue/evade
	private Vector2 pursueForce(Vector2 otherPos, Vector2 otherV, float maxforce) {
		Vector2 offset = otherPos - (Vector2) transform.position;
		float dist = offset.magnitude;
		float directtime = dist/MAXV;

		Vector2 unitV = rb.velocity.normalized;

		float parallelness = Vector2.Dot(unitV, otherV.normalized);
		float forwardness = Vector2.Dot (unitV, offset/dist);

		float halfsqrt2 = 0.707f;
		int f = intervalComp(forwardness, -halfsqrt2, halfsqrt2);
		int p = intervalComp(parallelness, -halfsqrt2, halfsqrt2);

		// approximate how far to lead the target
		float timeFactor = 1f;
		// case logic based on (ahead, aside, behind) X (parallel, perp, anti-parallel)
		switch (f) {
		case 1: //target is ahead
			switch (p) {
			case 1:
				timeFactor = 4f;
				break;
			case 0:
				timeFactor = 1.8f;
				break;
			case -1:
				timeFactor = 0.85f;
				break;
			}
			break;
		case 0: //target is aside
			switch (p) {
			case 1:
				timeFactor = 1f;
				break;
			case 0:
				timeFactor = 0.8f;
				break;
			case -1:
				timeFactor = 4f;
				break;
			}
			break;
		case -1: //target is behind
			switch (p) {
			case 1:
				timeFactor = 0.5f;
				break;
			case 0:
				timeFactor = 2f;
				break;
			case -1:
				timeFactor = 2f;
				break;
			}
			break;
		}

		float estTime = Mathf.Min (MAXPREDICTIONTIME, directtime * timeFactor);
		Vector2 estPos = (Vector2) otherPos + estTime * otherV;

		return seekForce(estPos, maxforce);
	}
	
	/*
	 * Steering behaviours
	 */

	public void moveInDirection(Vector2 direction) {
		Vector2 desiredV = direction * MAXV;
		if (desiredV.magnitude > MAXV) {
			desiredV = desiredV * MAXV / desiredV.magnitude;
		}
		rb.AddForce(ACCEL * (desiredV - rb.velocity));
	}
	
	public void brake() {
		Vector2 deltaV = - rb.velocity;
		float dvmagn = deltaV.magnitude;
		if (dvmagn > forceRemaining * forceRemaining * dt * dt) {
			rb.AddForce(scaled(forceRemaining, deltaV));
			forceRemaining = 0f;
		} else {
			rb.AddForce(deltaV);
			forceRemaining -= dvmagn;
		}
	}
	
	public void pursue(Vector2 otherPos, Vector2 otherV) {
		Vector2 force = pursueForce(otherPos, otherV, forceRemaining);
		rb.AddForce(force);
	}
	
	public void pursue(GameObject other) {
		Vector3 otherVelocity = new Vector3();
		if (other.GetComponent<Rigidbody2D>() != null) {
			otherVelocity = other.GetComponent<Rigidbody2D>().velocity;
		}
		pursue(other.transform.position, otherVelocity);
	}
	
	public void seek(Vector2 dest) {
		Vector2 force = seekForce(dest, forceRemaining);
		rb.AddForce(force);
	}

	public void seek(GameObject other) {
		seek(other.transform.position);
	}
	
	public void arrival(Vector2 dest) {
		// can approximate with (if too close : break)
		// alternatively stopdist from current pos and current speed
		// or expected stopdist after the current frame assuming accel wont change the current frame much

		// for the duration it takes to go from maxV to 0
		//stoptime = v/ACCEL;
		//stopdist = stoptime * v/2;
		//dist = desiredv * desiredV/(2 * accel) 
		//desiredv = sqrt(dist * 2 * accel) 

		Vector2 offset = dest - ((Vector2) transform.position + (dt * rb.velocity));

		float dist = offset.magnitude;
		float stopdist = MAXV * MAXV / (2 * ACCEL);
		float desiredV = dist < stopdist ? Mathf.Sqrt(dist * 2 * ACCEL) : MAXV;
		Vector2 deltaV = scaled (desiredV, offset) - rb.velocity;
		float dvmagn = deltaV.magnitude;
		if (dvmagn > forceRemaining * forceRemaining * dt * dt) {
			rb.AddForce(scaled(forceRemaining, deltaV));
			forceRemaining = 0f;
		} else {
			rb.AddForce(deltaV);
			forceRemaining -= dvmagn;
		}
	}

	public void evade(Vector2 otherPos, Vector2 otherV) {
		Vector2 force = pursueForce(otherPos, otherV, forceRemaining);
		rb.AddForce(-force);
	}

	public void evade(GameObject other) {
		evade(other.transform.position, other.GetComponent<Rigidbody2D>().velocity);
	}

	public void flee(Vector2 otherPos) {
		Vector2 force = -seekForce(otherPos, forceRemaining);
		rb.AddForce(force);
	}
	
	public void flee(GameObject other) {
		flee(other.transform.position);
	}

	/* 
	 * Avoid collisions by determining for each neighbor when their paths will be closest to each other
	 * and then steer laterally to avoid collision.
	 * https://www.red3d.com/cwr/steer/Unaligned.html
	 */
	public void collisionAvoidance<T>(Neighbours<T> neighbours) where T : MonoBehaviour {

	}

	public void separate<T>(Neighbours<T> neighbours) where T : MonoBehaviour {
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

	// avoid individual static objects
	public void avoid(List<Vector2> obstacles) {
		
	}

	// avoid edges of the pathable areas (large walls)
	public void containment(List<Rect> walls) {
		
	}
}
