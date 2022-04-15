using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected int maxHp;
    [SerializeField] protected List<int> phaseTriggerPercent;
    [SerializeField] protected int curHp;
    [SerializeField] protected int curPhase;
    protected int curGimmick;
    protected IEnumerator currentPattern;
    protected float delay;

    protected bool inAction;
    protected bool inDelay;
    protected bool isInvincible;

    void Start()
    {
        curHp = maxHp;
        delay = 1;
        curPhase = 1;
        PhaseTriggerSort();
        StartCoroutine(ManageAction());  
    }

    void Update()
    {
        
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
        inAction = true;

        //do potterns
        yield return new WaitForSeconds(1f);

        //change animation
        delay = 1f;
        inAction = false;
    }


    void PhaseTriggerSort(){
        phaseTriggerPercent.Sort((int a, int b) => b.CompareTo(a));
    }
    virtual protected bool PhaseCheck(){
        int p = 0;
        while(p < phaseTriggerPercent.Count && phaseTriggerPercent[p] > curHp) { p++; }


        if(curPhase != p){
            curPhase = p;
            return true;
        }else{
            return false;
        }
    }
}
