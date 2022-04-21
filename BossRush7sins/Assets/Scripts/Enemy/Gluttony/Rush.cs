using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : StateMachineBehaviour
{
    Skull skull;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        skull = animator.gameObject.GetComponent<Skull>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        skull.RushCheckWall();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

}
