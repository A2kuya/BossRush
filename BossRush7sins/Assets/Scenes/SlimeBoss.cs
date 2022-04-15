using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : Boss
{

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void NextPattern()
    {
        if(PhaseCheck()){
            currentPattern = ChangePhase();
            return;
        }
        
        switch(curPhase){
            case 1:
                currentPattern = Rush();
                break;
            case 2:
                currentPattern = EnforceRush();
                break;
            default:
                currentPattern = SamplePattern();
                break;
        }
    }

    IEnumerator Rush(){
        inAction = true;
        Debug.Log("rush");
        yield return new WaitForSeconds(1f);
        inAction = false;
    }

    IEnumerator EnforceRush(){
        inAction = true;
        Debug.Log("enforce rush");
        yield return new WaitForSeconds(1f);
        inAction = false;
    }

    IEnumerator ChangePhase(){
        inAction = true;
        Debug.Log("change phase");
        yield return new WaitForSeconds(1f);
        inAction = false;
    }

    IEnumerator Groggy(){
        inAction = true;
        yield return new WaitForSeconds(1f);
        inAction = false;
    }
}
