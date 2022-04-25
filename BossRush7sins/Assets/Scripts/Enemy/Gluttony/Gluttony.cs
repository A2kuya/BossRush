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
            onChangePhase = true;
        }
        
        switch(curPhase){
            case 1:
                currentPattern = RushTogether();
                Debug.Log("1페");
                break;
            case 2:
                if(onChangePhase){
                    currentPattern = Between1n2();
                    Debug.Log("무적");
                }
                else{
                    currentPattern = EnforceRush();
                    Debug.Log("2페");
                }
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


    [SerializeField] float rushTerm;
    IEnumerator EnforceRush(){
        ActionControl(true);
        inRush = true;
        
        foreach(Skull sk in skull){
            if(sk.gameObject.activeSelf){
                sk.SetRushSpeed(rushSpeed);
                sk.Rush();
                yield return new WaitForSeconds(rushTerm);
            }
        }
        
        yield return new WaitUntil(() => inRush == false);
        ActionControl(false);
    }

    
    IEnumerator Between1n2(){
        ActionControl(true);
        
        inRush = true;
        if(CheckEatRock()){
            foreach (Skull sk in skull)
            {
                if (sk.gameObject.activeSelf && sk.onGroggy)
                {
                    sk.Groggy();
                }
            }
            yield return new WaitForSeconds(1f);
            inRush = false;
        }
        else{
            foreach (Skull sk in skull)
            {
                if (sk.gameObject.activeSelf && !sk.onGroggy)
                {
                    sk.SetRushSpeed(rushSpeed);
                    sk.Rush();
                    yield return new WaitForSeconds(rushTerm);
                }
            }            
        yield return new WaitUntil(() => inRush == false);
        }
        ActionControl(false);
    }

    bool CheckEatRock(){
        bool b = false;
        foreach(Skull sk in skull){
            if(sk.gameObject.activeSelf){
                b = b || sk.onGroggy;
            }
        }
        return b;
    }

   [SerializeField]  bool onChangePhase;
    protected override bool PhaseCheck()
    {
        bool stayInPhase = true;

        if(curPhase == 1){
            foreach (Skull sk in skull)
            {
                stayInPhase = stayInPhase && sk.gameObject.activeSelf;
            }

            if (!stayInPhase)   // 페이즈가 바뀌면
            {
                curPhase = 2;
                return true;
            }
            else
                return false;
        }
        else if(onChangePhase){
            foreach (Skull sk in skull)
            {
                if(sk.gameObject.activeSelf){
                    stayInPhase = stayInPhase && (sk.gorggyCount > 0) && !sk.onGroggy;
                }
            }

            if(stayInPhase){   //페이즈가 바뀌면
                onChangePhase = false;
                return false;
            }
            else
                return true;
        }
        return false;
    }

}
