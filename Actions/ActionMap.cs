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

public class ActionMap
{
	Dictionary<int, Ability> actions; 
	private Actor owner;

	public ActionMap (Actor unit)
	{
		actions = new Dictionary<int, Ability>();
		owner = unit;
	}

	public void add(int id, Ability action) {
		if (actions.ContainsKey(id)) {
			throw new ArgumentException();
		}
		actions.Add(id, action);
		action.owner = owner;
	}

	public void use(int abilityId, Attackable attackable) {
		use(abilityId, new AbilityTarget(attackable));
	}

	public void use(int abilityId, Vector3 position) {
		use(abilityId, new AbilityTarget(position));
	}

	private void use(int abilityId, AbilityTarget target) {
		actions[abilityId].use(target);
	}

	public void setCurrentCooldown(int id, float cooldown) {
		actions[id].setCurrentCooldown(cooldown);
	}
	
	public bool ready(int id) {
		return actions[id].ready();
	}
	
	public void update(float dt) {
		foreach (Ability action in actions.Values) {
			action.update(dt);
		}
	}

	public float getCastTime(int id) {
		return actions[id].castTime;
	}
}