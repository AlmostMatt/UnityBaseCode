//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityBaseCode.Statuses;

namespace UnityBaseCode {
	namespace Actions {
		public class ActionMap : MonoBehaviour
		{
			Dictionary<int, Ability> actions = new Dictionary<int, Ability>();

			public void Add(int id, Ability action) {
				if (actions.ContainsKey(id)) {
					throw new ArgumentException();
				}
				actions.Add(id, action);
			}

			public void Use(int abilityId, GameObject targetObject) {
                actions[abilityId].Use(gameObject, targetObject);
			}

			public void Use(int abilityId, Vector3 targetPosition) {
                actions[abilityId].Use(gameObject, targetPosition);
			}

            public float GetCooldown(int id)
            {
                return actions[id].GetCooldown();
            }

            public void SetCooldown(int id, float cooldown)
            {
                actions[id].SetCooldown(cooldown);
            }

            public bool IsReady(int id) {
				return actions[id].GetCooldown() <= 0f;
			}
			
			void FixedUpdate() {
				foreach (Ability action in actions.Values) {
					action.Update(Time.fixedDeltaTime);
				}
			}
		}
	}
}