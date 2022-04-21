using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gluttony : Boss
{
    [SerializeField] GameObject player;
    [SerializeField] Skull[] skull;
    [SerializeField] float rushSpeed;
    
    private void Awake() {
        for(int i = 0; i<skull.Length;i++){
            skull[i].Init(player, maxHp, rushSpeed, i);
        }        
    }

    private void Update() {
        CheckEndRush();
    }
    
    

    protected override void NextPattern()
    {
        if(PhaseCheck()){
            currentPattern = ChangePhase();
            return;
        }
        
        switch(curPhase){
            case 1:
                currentPattern = RushTogether();
                break;
            case 2:
                currentPattern = EnforceRush();
                break;
            default:
                currentPattern = SamplePattern();
                break;
        }
    }

    bool inRush = false;
    IEnumerator RushTogether(){
        ActionControl(true);
        inRush = true;
        
        foreach(Skull sk in skull){
            if(sk.gameObject.activeSelf){
                sk.SetRushSpeed(rushSpeed);
                sk.Rush();
            }
        }
        
        yield return new WaitUntil(() => inRush == false);
        ActionControl(false);
    }
    void CheckEndRush(){
        if(inRush){
            bool CanNext = true;
            foreach(Skull sk in skull){
                if(sk.gameObject.activeSelf){
                    CanNext = (CanNext && !sk.GetInPattern());
                }
            }

            if(CanNext){
                foreach(Skull sk in skull){
                    if(sk.gameObject.activeSelf)
                        sk.SetInPattern(false);
                }
                
                inRush = false;
            }
        }
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

    protected override bool PhaseCheck()
    {
        bool stayInPhase = true;
        foreach(Skull sk in skull){
            stayInPhase = stayInPhase && sk.gameObject.activeSelf;
        }

        if(!stayInPhase && curPhase == 1){
            curPhase = 2;
            return true;
        }
        else
            return false;
    }
}
