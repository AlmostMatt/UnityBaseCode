using System;
using UnityEngine;

namespace UnityBaseCode
{
	namespace Statuses
	{
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

            // TODO: have callback delegates instead of superclassing virtual functions
            public virtual void Begin(GameObject owner) {
				switch (type) {
				case State.ANIMATION:
					//owner.canTurn = false;
					break;
				}
			}

			public virtual void Expire(GameObject owner) {
				switch (type) {
				case State.ANIMATION:
					//owner.canTurn = true;
					break;
				}
			}
		}
	}
}