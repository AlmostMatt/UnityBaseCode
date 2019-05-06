using UnityEngine;
using System;
using UnityBaseCode.Statuses;

namespace UnityBaseCode
{
	namespace Actions
	{
		public delegate void AbilityCallback(AbilityTarget target);

		public class Ability
		{
			// maxcd is constant, cd is remaining
			private float maxcd;
			private float cd;

			// castTime is constant, animTime is the remaining amount
			public float castTime;
			private float animTime;

			private AbilityCallback abilityCallback;
			private AbilityTarget target;

			public Actor owner;

			public Ability (AbilityCallback callback, float cooldown, float castT = 0f)
			{
				abilityCallback = callback;
				cd = 0f;
				castTime = castT;
				maxcd = cooldown;
			}

			public bool ready() {
				return cd <= 0f;
			}

			public void update(float dt) {
				cd = Math.Max(0f, cd - dt);
				if (animTime > 0) {
					if (animTime <= dt) {
						animTime = 0f;
						abilityCallback(target);
					} else {
						animTime -= dt;
					}
				}
			}

			public void use(AbilityTarget abilityTarget) {
				cd = maxcd;
				if (castTime == 0f) {
					abilityCallback(abilityTarget);
				} else {
					// record the target so that it is still known after the animation time has completed.
					this.target = abilityTarget;
					animTime = castTime;
					owner.statusMap.add(new Status(State.ANIMATION), castTime);
				}
			}

			// When something external affects the current cooldown.
			public void setCurrentCooldown(float currentCooldown) {
				cd = currentCooldown;
			}
		}
	}
}