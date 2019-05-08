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
			private float _maxCooldown;
			private float _cooldown;

			// castTime is constant, animTime is the remaining amount
			private float _castTime;
			private float _animTime;

			private AbilityCallback _abilityCallback;
			private AbilityTarget target;

			public Ability(AbilityCallback callback, float maxCooldown, float castTime = 0f)
			{
				_abilityCallback = callback;
                _cooldown = 0f;
				_castTime = castTime;
                _maxCooldown = maxCooldown;
			}

            // TODO: make a separate assembly so that 'internal' visibility is actually meaningful
            internal void Update(float dt) {
                _cooldown = Math.Max(0f, _cooldown - dt);
				if (_animTime > 0) {
					if (_animTime <= dt) {
                        _animTime = 0f;
                        _abilityCallback(target);
					} else {
                        _animTime -= dt;
					}
				}
			}

            internal void Use(GameObject owner, AbilityTarget abilityTarget) {
                _cooldown = _maxCooldown;
				if (_castTime == 0f) {
                    _abilityCallback(abilityTarget);
				} else {
                    // record the target so that it is still known after the animation time has completed.
					this.target = abilityTarget;
                    _animTime = _castTime;
                    StatusMap statusMap = owner.GetComponent<StatusMap>();
                    if (statusMap != null)
                    {
                        statusMap.Add(new Status(State.ANIMATION), _castTime);
                    }
                }
            }

            // Get the current cooldown (not max cooldown)
            internal float GetCooldown()
            {
                return _cooldown;
            }

            // Set the current cooldown to a specific value.
            internal void SetCooldown(float currentCooldown)
            {
                _cooldown = currentCooldown;
            }
        }
	}
}