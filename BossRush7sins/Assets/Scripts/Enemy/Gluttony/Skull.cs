using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    GameObject player;
    Animator anim;
    int id;
    int maxHp;
    int curHp;
    float rushSpeed;
    [SerializeField] bool inPattern = false;
    [SerializeField] LayerMask wallLayer;

    [SerializeField] LayerMask stone;
    [SerializeField] private int _gorggyCount = 0;
    public int gorggyCount{
        get { return _gorggyCount; }
    }

    SpriteRenderer sp;    
    private void Start() {
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        CheckDead();
    }

    public void Init(GameObject p, int hp, float rs, int i){
        player = p;
        maxHp = hp;
        curHp = maxHp;
        rushSpeed = rs;
        id = i;
    }

    public void SetTrigger(string s){
        anim.SetTrigger(s);
    }
    

    #region rush
    public void SetRushSpeed(float rs){
        rushSpeed = rs;
    }
    public void Rush(){
        StartCoroutine(Rush_co());
    }
    bool inRush = false;
    float rushReturnDelay;
    Vector3 originPosition;
    Vector3 targetPosition;
    IEnumerator Rush_co(){
        originPosition = transform.position;
        targetPosition = player.transform.position - originPosition;
        rushReturnDelay = 1.5f;
        SetInPattern(true);
        anim.SetTrigger("attackReady");

        yield return new WaitUntil(() => inRush == true);
        anim.SetTrigger("rush");
        iTween.MoveAdd(gameObject, iTween.Hash(
            "name", "rush"+id,
            "amount", targetPosition * 10,
            "speed", rushSpeed,
            "onstart", "CheckWall",
            "oncomplete", "SetInRush",
            "oncompleteparams", true,
            "easeType", iTween.EaseType.linear
        ));

        yield return new WaitUntil(() => inRush == false);
        anim.SetTrigger("rushEnd");
        yield return new WaitForSeconds(rushReturnDelay);

        iTween.MoveTo(gameObject, iTween.Hash(
            "name", "rushreturn"+id,
            "position", originPosition,
            "speed", 5f,
            "onstart", "CheckWall",
            "oncomplete", "SetInPattern",
            "oncompleteparams", false,
            "easeType", iTween.EaseType.linear
        ));
    }
    public void RushCheckWall(){
        RaycastHit2D wall = Physics2D.Raycast(transform.position, targetPosition, 1f, wallLayer);
        
        if(wall){
            iTween.StopByName("rush"+id);
            inRush = false;
            rushReturnDelay = 2f;
        }
    }
    public void SetInRush(bool inRush){
        this.inRush = inRush;
    }
    
    #endregion
    

    

    [SerializeField] private bool _onGroggy;
    public bool onGroggy {
        get { return _onGroggy; }
        set { _onGroggy = value; }
    }
    IEnumerator Blink(){
        sp.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sp.color = new Color(255, 255, 255);
        onGroggy = false;
    }

    public void Groggy(){
        StartCoroutine(Blink());
    }


    public void SetInPattern(bool b){        
        inPattern = b;
    }

    public bool GetInPattern(){
        return inPattern;
    }
    

    public void TakeDamaged(int damage){
        curHp -= damage;
    }
    void CheckDead(){
        if(maxHp <= 0){
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == Mathf.Log(stone.value, 2)){
            Destroy(other.gameObject);
            if(gorggyCount == 0){
                _onGroggy = true;
                _gorggyCount++;
            }
        }
    }

}
