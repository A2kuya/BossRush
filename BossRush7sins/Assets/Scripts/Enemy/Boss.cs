using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected int maxHp;
    [SerializeField] protected List<int> phaseTrigger;
    protected int curHp;
    [SerializeField] protected int curPhase;
    protected int curGimmick;
    protected IEnumerator currentPattern;
    protected float delay;

    [SerializeField] protected bool inAction;
    protected bool inDelay;
    protected bool isInvincible;

    protected Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        curHp = maxHp;
        delay = 1;
        curPhase = 1;
        PhaseTriggerSort();
        StartCoroutine(ManageAction());  

        
    }


    virtual protected void NextPattern(){
        if(PhaseCheck()){
            
        }
        currentPattern = SamplePattern();
    }

    IEnumerator ManageAction(){
        while(true){
            NextPattern();
            if(currentPattern != null)
                StartCoroutine(currentPattern); //보스 패턴 진행            
            yield return new WaitUntil(() => inAction == false);               

            StartCoroutine(Wating(delay));  //다음 패턴까지 딜레이
            yield return new WaitUntil(() => inDelay == false);
        }
    }

    IEnumerator Wating(float s){
        inDelay = true;
        yield return new WaitForSeconds(s);   
        inDelay = false;
    }

    protected IEnumerator SamplePattern(){
        //change animatrion
        ActionControl(true);

        //do potterns
        yield return new WaitForSeconds(1f);

        //change animation
        delay = 1f;
        ActionControl(false);
    }


    void PhaseTriggerSort(){
        phaseTrigger.Sort((int a, int b) => b.CompareTo(a));
    }
    
    int p;
    virtual protected bool PhaseCheck(){
        p = 0;
        while(p < phaseTrigger.Count && phaseTrigger[p] > curHp) { p++; }
        p++;

        if(curPhase != p){
            curPhase = p;
            return true;
        }else{
            return false;
        }
    }

    protected void ActionControl(bool b){
        inAction = b;
    }
}
