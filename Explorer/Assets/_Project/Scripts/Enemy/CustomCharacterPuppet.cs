using UnityEngine;
using System.Collections;
using RootMotion.Demos;
using RootMotion.Dynamics;

public class CustomCharacterPuppet : CustomCharacterThirdPerson {

[Header("Puppet")]
	
public PropMuscle propMuscle;

public BehaviourPuppet puppet { get; private set; }

protected override void Start() {
    base.Start();

    puppet = behaviourPuppet;
    if (!puppet)
        puppet = transform.parent.GetComponentInChildren<BehaviourPuppet>();
}

public override void Move(Vector3 deltaPosition, Quaternion deltaRotation) {
    
    // Disable movement while the puppet is not balanced or getting up.
    if (puppet && puppet.state != BehaviourPuppet.State.Puppet) {
        customUserControlThirdPerson.state.move = Vector3.zero;
        return;
    }

    base.Move(deltaPosition, deltaRotation);
}

protected override void Rotate() {
    // Disable rotation while the puppet is not balanced or getting up.
    if (puppet.state != BehaviourPuppet.State.Puppet) {
        if (gravityTarget != null) transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - gravityTarget.position) * transform.rotation;

        return;
    }

    base.Rotate();
}

protected override bool Jump() {
    if (puppet.state != BehaviourPuppet.State.Puppet) return false;

    return base.Jump();
}

}