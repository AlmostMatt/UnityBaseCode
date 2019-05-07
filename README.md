# UnityBaseCode
Contains some generic concepts that I find myself reusing across projects

## UnityBaseCode.Steering:
Defines steering behaviours similar to those described by https://www.red3d.com/cwr/steer/

Usage:
* Add a Steering component to a gameobject. Add behaviours and set properties programatically.

API:
* Steering.addBehaviour(float weight, SteeringBehaviour behaviour)
* Steering.updateWeight(SteeringBehaviour behaviour, float newWeight)
* Steering.removeBehaviour(SteeringBehaviour behaviour)
* Steering.clearBehaviours()
* Steering.setSize(float radius)
* Steering.setSpeed(float maxSpeed, float acceleration)
* float Steering.getSize()
* float Steering.getMaxSpeed()
* float Steering.getAcceleration()
* Vector2 Steering.getPosition()
* Vector2 Steering.getVelocity()

Example:
```csharp
void Start () {
  Steering steering = GetComponent<Steering>();
  steering.setSpeed(3f, 12f);
  steering.setSize(0.25f);
  steering.addBehaviour(3f, new WallAvoidance());
  steering.addBehaviour(1f, new Wander());
}
```

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
