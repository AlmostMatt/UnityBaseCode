using UnityEngine;
using System;
using UnityBaseCode.Statuses;

namespace UnityBaseCode
{
	namespace Actions
    {
        public delegate void AbilityWithTargetObject(GameObject targetObject);
        public delegate void AbilityWithTargetPosition(Vector3 targetPosition);

        public class Ability
		{
			// maxcd is constant, cd is remaining
			private float _maxCooldown;
			private float _cooldown;

			// castTime is constant, animTime is the remaining amount
			private float _castTime;
			private float _animTime;

            private GameObject _owner;

            private AbilityWithTargetObject _abilityCallback1;
            private AbilityWithTargetPosition _abilityCallback2;
            private GameObject _target1;
            private Vector3 _target2;

            public Ability(AbilityWithTargetObject callback, float maxCooldown, float castTime = 0f)
            {
                _abilityCallback1 = callback;
                _cooldown = 0f;
                _castTime = castTime;
                _maxCooldown = maxCooldown;
            }

            public Ability(AbilityWithTargetPosition callback, float maxCooldown, float castTime = 0f)
            {
                _abilityCallback2 = callback;
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
                        if (_abilityCallback1 != null)
                        {
                            _abilityCallback1(_target1);
                        } else
                        {
                            _abilityCallback2(_target2);
                        }
					} else {
                        _animTime -= dt;
					}
				}
            }

            private void AbilityEffect()
            {
                if (_abilityCallback1 != null)
                {
                    _abilityCallback1(_target1);
                }
                else
                {
                    _abilityCallback2(_target2);
                }
            }

            private void Use()
            {
                _cooldown = _maxCooldown;
                if (_castTime == 0f)
                {
                    AbilityEffect();
                }
                else
                {
                    _animTime = _castTime;
                    StatusMap statusMap = _owner.GetComponent<StatusMap>();
                    if (statusMap != null)
                    {
                        statusMap.Add(new Status(State.ANIMATION), _castTime);
                    }
                }
            }

            internal void Use(GameObject owner, GameObject targetObject)
            {
                // Store the target so that it is still known after the animation time has completed.
                this._target1 = targetObject;
                this._owner = owner;
                Use();
            }

            internal void Use(GameObject owner, Vector3 targetPosition)
            {
                // Store the target so that it is still known after the animation time has completed.
                this._target2 = targetPosition;
                this._owner = owner;
                Use();
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