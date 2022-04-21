using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackReady : StateMachineBehaviour
{
    Skull skull;
    bool b = true;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        skull = animator.gameObject.GetComponent<Skull>();
        b = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && b){
            skull.SetInRush(true);
            b = false;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
