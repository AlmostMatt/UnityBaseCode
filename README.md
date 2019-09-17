# UnityBaseCode
Contains some generic concepts that I find myself reusing across projects

## UnityBaseCode.Steering:
Defines steering behaviours similar to those described by https://www.red3d.com/cwr/steer/

Usage:
* boolean Steering.UseXZ can be set to move along the XZ plane instead of XY plane
* By default, a Rigidbody2D will be added when using XY plane and Rigibody when using XZ plane
* Add a Steering component to a GameObject.
* (Optional) Add a Rigidbody2D or Rigidbody component - will be added if not found
* (Optional) Add a Collider2D - only if you want collisions
* Add behaviours and set properties programatically.

Note:
* The associated Rigidbody will be contrained to movement on the relevant axis and rotation disabled.
* Steering logic assumes position and velocity of 0 along the constrained axis. Steering towards non-zero position on this axis or towards objects with nonzero velocity on this axis may cause problems.

Example:
```csharp
using UnityBaseCode.Steering;

void Start () {
  Steering steering = GetComponent<Steering>();
  steering.SetSpeed(3f, 12f);
  steering.SetSize(0.25f);
  steering.AddBehaviour(3f, new WallAvoidance());
  steering.AddBehaviour(1f, new Wander());
}
```

Steering:
```csharp
Steering.AddBehaviour(float weight, SteeringBehaviour behaviour)

Steering.RemoveBehaviour(SteeringBehaviour behaviour)

Steering.ClearBehaviours()

Steering.UpdateWeight(SteeringBehaviour behaviour, float newWeight)

Steering.SetSize(float radius)

Steering.SetSpeed(float maxSpeed, float acceleration)

float Steering.GetSize()

float Steering.GetMaxSpeed()

float Steering.GetAcceleration()

Vector2 Steering.GetPosition()

Vector2 Steering.GetVelocity()
```

Behaviours:
```csharp
Seek(Vector2 target)
  SetTarget(Vector2 newTarget)

Flee(Vector2 target)
  SetTarget(Vector2 newTarget)

Pursue(Steering otherObject)
   SetTarget(Steering newTarget)

Evade(Steering otherObject)
  SetTarget(Steering newTarget)

WallAvoidance(int layerMask = Physics2D.DefaultRaycastLayers)

Wander()

UnalignedCollisionAvoidance(Steering currentObject)
UnalignedCollisionAvoidance(Neighbours<Steering, Steering> neighbours)

Separate(Steering currentObject, float preferredDistance)
Separate(Neighbours<Steering, Steering> neighbours, float preferredDistance)

Cohesion (in progress)

Alignment (in progress)
```

Note: Behaviours that care about neighbors have a version that reacts to all other steering objects, and a version that takes a specific Neighbors object to indicate which steerings to interact with.

## UnityBaseCode.Statuses:
Tracks temporary status effects on an object. Status effects can optionally have begin and end callback functions.

Usage:
A class that implements Actor should have a public gettable StatusMap, and should call the StatusMap's update function with the amount of time that has passed as an argument. 

```csharp
public class Unit : MonoBehaviour
{
    void Start()
    {
        _statusMap = gameObject.AddComponent<StatusMap>();
    }
}
```

public enum State { ANIMATION, STUNNED, INVULNERABLE, DEAD };

Status is a wrapper around State + duration
It can be superclassed to have custom begin and end callbacks 

StatusMap.Add(Status s, float duration);`
StatusMap.Has(State state)
StatusMap.Duration(State state)

## UnityBaseCode.Actions:
Abilities have cooldowns and callback functions.

Abilities have an ID that is specified when the ability is added, and are called by ID

API:

```csharp
public delegate void AbilityCallback(AbilityTarget target);
    
Ability(AbilityCallback callback, float maxCooldown, float castTime = 0f)

Add(int id, Ability action)
Use(int abilityId, Attackable attackable)
Use(int abilityId, Vector3 position)
GetCooldown(int id)
SetCooldown(int id, float cooldown)
IsReady(int id)
```

Example:
```csharp
private ActionMap _actionMap;
_actionMap = gameObject.AddComponent<ActionMap>();
_actionMap.Add(1, new Ability(target => target.getTargetObject().GetComponent<Unit>().Damage(5), 1f, 0.1f));
if (_actionMap.IsReady(1))
{
    _actionMap.Use(1, otherUnit.gameObject);
}
```

When an ability is used, if the owner GameObject has the StatusMap component and cast time has a non-zero value, the ANIMATION state will be added with a duration equal to the cast time.
The ability's USE effect will happen after the animation time

Some of the functionality is demonstrated in https://github.com/AlmostMatt/SteeringDemo

Note: I will likely change many things over time.
