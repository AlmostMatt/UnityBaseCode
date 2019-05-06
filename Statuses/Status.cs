using System;

namespace UnityBaseCode
{
	namespace Statuses
	{
		// TODO: refactor this to allow superclassing instead of a case statement
		public enum State {ANIMATION, STUNNED, INVULNERABLE, DEAD};

		public class Status
		{
			// enumerate types

			public State type;
			public float duration;

			public Status (State statusType)
			{
				type = statusType;
				duration = 0;
			}

			public virtual void begin(Actor owner) {
				switch (type) {
				case State.ANIMATION:
					//owner.canTurn = false;
					break;
				}
			}

			public virtual void expire(Actor owner) {
				switch (type) {
				case State.ANIMATION:
					//owner.canTurn = true;
					break;
				}
			}
		}
	}
}