using UnityEngine;

namespace UnityBaseCode
{
	namespace Steering
    {
        // Moves directly towards a point or target
        public class Seek : SteeringBehaviour
		{
            Steering _target;
            Vector3 _point;

            public Seek(Vector3 point)
            {
                this._point = point;
            }

            public Seek(Steering target)
            {
                this._target = target;
            }

            public void setTarget(Vector3 newPoint)
            {
                this._point = newPoint;
                this._target = null;
            }

            public void setTarget(Steering newTarget)
            {
                this._target = newTarget;
            }

            public Vector3 GetForce(Steering steering) {
                if (_target != null)
                {
                    return SteeringUtilities.getSeekForce(steering, _target.GetPosition());
                }
                else
                {
                    return SteeringUtilities.getSeekForce(steering, _point);
                }
            }
		}
	}
}