# UnityBaseCode
Contains some generic concepts that I find myself reusing across projects

## UnityBaseCode.Steering:
Defines steering behaviours similar to those described by https://www.red3d.com/cwr/steer/

Usage:
* add a Steering component to an object
* call `GetComponent<Steering>().addBehaviour(weight, behaviour)` in the Start() function of a script

Supported Behaviours:
* Seek
* Pursue
* Flee
* Evade
* WallAvoidance
* UnalignedCollisionAvoidance
* Wander
* Separation (in progress)
* Cohesion (in progress)
* Alignment (in progress)

## UnityBaseCode.Statuses:
Tracks temporary status effects on an object. Status effects can optionally have begin and end callback functions.

Usage:
* `statusMap = new StatusMap(this);`
* in FixedUpdate: `statusMap.update(Time.fixedDeltaTime);`
* `statusMap.add(new Status(STATE.STUNNED), duration);`
* `statusMap.has(STATE.STUNNED)`

## UnityBaseCode.Actions:
Abilities have cooldowns and callback functions.

Usage:
* add a statusMap property to the object (the ANIMATION status is added during the cast time of an ability).
* `actionMap = new ActionMap(this);`
* `actionMap.add(abilityNumber, new Ability(callbackFunction, cooldown, castTime));`
* in FixedUpdate: `actionMap.update(Time.fixedDeltaTime);`
* `if (actionMap.ready(abilityNumber) && !statusMap.has(STATE.STUNNED))`
* `actionMap.use(abilityNumber, target);`


Some of the functionality is demonstrated in https://github.com/AlmostMatt/SteeringDemo

Note: I will likely change many things over time.
