# UnityBaseCode
Contains some generic concepts that I find myself reusing across projects

## Steering:
Defines steering behaviours similar to those described by https://www.red3d.com/cwr/steer/

To use:
* add a Steering component to an object
* call `GetComponent<Steering>().addBehaviour(weight, behaviour)` in the Start() function of a script

## StatusMap:
Tracks temporary status effects on an object. Status effects can optionally have begin and end callback functions.

Usage:
* `statusMap = new StatusMap(this);`
* in FixedUpdate: `statusMap.update(Time.fixedDeltaTime);`
* `statusMap.add(new Status(STATE.STUNNED), duration);`
* `statusMap.has(STATE.STUNNED)`

## ActionMap:
Abilities have cooldowns and callback functions.
To use:
* add a statusMap property to the object (to track the ANIMATION status).
* `actionMap = new ActionMap(this);`
* `actionMap.add(abilityNumber, new Ability(callbackFunction, castTime));`
* in FixedUpdate: `actionMap.update(Time.fixedDeltaTime);`
* `if (actionMap.ready(abilityNumber) && !statusMap.has(STATE>STUNNED))`
* `actionMap.use(abilityNumber, target);`
