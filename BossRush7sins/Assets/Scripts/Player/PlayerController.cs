using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 플레이어 상태
    public enum State
    {
        Normal, Rolling, Casting, Stagger, Dead,
    }
    public State state;

    // 플레이어 공격 방식
    public enum AttackMethod
    { 
        Ranged, Melee, Meteor,
    }
    public AttackMethod attackMethod;

    // 애니메이션
    private Animator animator;
    private SpriteRenderer sr;

    // 스텟
    [Header("스텟")]
    public int maxHealth;
    [SerializeField] private int health;

    // 이동
    [Header("이동")]
    public float moveSpeed;
    private bool fixAngle;
    private Rigidbody2D rb;
    private float xInput;
    private float yInput;
    private Vector3 moveDir;
    private Vector3 lastMoveDir;

    // 구르기
    [Header("구르기")]
    public float maxRollSpeed;
    public float minRollSpeed;
    public float rollSpeedDropMultiplier;
    private float rollSpeed;
    private Vector3 rollDir;

    // 무기
    [Header("무기")]
    public GameObject projectile;
    public GameObject projectileSpawnPos;
    public GameObject weapon;
    public float preDelay;
    public float aftDelay;
    private Camera viewCamera;
    private Vector2 mousePos;
    private float angle;
    private Coroutine currentRoutine;

    // 피격
    [Header("피격")]
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;
    public Collider2D triggerCollider;
    public SpriteRenderer mySprite;

    public void Init()
    {
        state = State.Normal;
        lastMoveDir = Vector3.down;
        health = maxHealth;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        viewCamera = Camera.main;
        Init();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        #region 마우스 위치 얻기
        if (!fixAngle)
        {
            // 마우스 위치 얻기
            mousePos = Input.mousePosition;
            mousePos = viewCamera.ScreenToWorldPoint(mousePos);
            // 마우스 위치를 쳐다보기 위한 각도
            angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
            weapon.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        #endregion

        #region 상하좌우, 최근 방향 값 받기
        // 상하좌우 움직임 입력
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(xInput, yInput).normalized;

        // 애니메이션 값 전달
        animator.SetFloat("xInput", xInput);
        animator.SetFloat("yInput", yInput);


        // 애니메이션 움직이고 있는지 아닌지 전달
        if (moveDir == Vector3.zero) animator.SetBool("isMoving", false);
        else animator.SetBool("isMoving", true);

        // 최근 움직였던 방향
        if (xInput != 0 || yInput != 0)
        {
            lastMoveDir = moveDir;
        }
        #endregion

        switch (state)
        {
            // 일반 상태
            case State.Normal:
                // 좌우반전
                if (xInput < 0) sr.flipX = true;
                else if (xInput > 0) sr.flipX = false;

                // 구르기 입력
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    xInput = Input.GetAxisRaw("Horizontal");
                    yInput = Input.GetAxisRaw("Vertical");
                    moveDir = new Vector3(xInput, yInput).normalized;
                    if (xInput != 0 || yInput != 0)
                    {
                        lastMoveDir = moveDir;
                    }

                    animator.SetTrigger("doDash");
                    rollDir = lastMoveDir;
                    rollSpeed = maxRollSpeed;
                    state = State.Rolling;
                }

                // 주문 시작
                if (Input.GetMouseButtonDown(0))
                {
                    state = State.Casting;
                }
                break;

            // 구르기 상태
            case State.Rolling:
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;
                triggerCollider.enabled = false;
                if (rollSpeed < minRollSpeed)
                {
                    triggerCollider.enabled = true;
                    state = State.Normal;
                }
                break;

            // 주문 시전 중
            case State.Casting:
                {
                    // 코루틴 한번만 실행
                    if (currentRoutine == null)
                    {
                        currentRoutine = StartCoroutine(Shoot(preDelay, aftDelay));
                    }

                    // 구르기로 캔슬
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        animator.SetTrigger("doDash");
                        StopCurrentRoutine();
                        fixAngle = false;
                        rollDir = lastMoveDir;
                        rollSpeed = maxRollSpeed;
                        state = State.Rolling;
                    }
                }
                break;

            case State.Stagger:
                TakeDamage(1);
                break;

            case State.Dead:

                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                rb.velocity = moveDir * moveSpeed;
                break;

            case State.Rolling:
                rb.velocity = rollDir * rollSpeed;
                break;

            case State.Casting:
                rb.velocity = Vector3.zero;
                break;

            case State.Dead:
                rb.velocity = Vector3.zero;
                break;
        }
        
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(HitStop());
        health -= damage;
        if (health < 0)
        {
            health = 0;
            state = State.Dead;
        }
    }

    private IEnumerator Shoot(float preDelay, float aftDelay)
    {
        // 공격 방향 바라보기
        Vector3 spawnPos = projectileSpawnPos.transform.position;
        if (spawnPos.x - transform.position.x < 0) sr.flipX = true;
        else if (spawnPos.x - transform.position.x > 0) sr.flipX = false;

        // 선딜
        yield return new WaitForSeconds(preDelay);
        
        // 투사체 소환
        switch (attackMethod)
        {
            case AttackMethod.Ranged:
                Instantiate(projectile, spawnPos, Quaternion.AngleAxis(angle - 90, Vector3.forward));
                break;

            case AttackMethod.Melee:
                Instantiate(projectile, spawnPos, Quaternion.AngleAxis(angle - 90, Vector3.forward));
                break;

            case AttackMethod.Meteor:
                Instantiate(projectile, mousePos, Quaternion.AngleAxis(angle - 90, Vector3.forward));
                break;
        }
        // 후딜
        fixAngle = true;
        yield return new WaitForSeconds(aftDelay);
        // 원래 상태로 복귀
        state = State.Normal;
        fixAngle = false;
        currentRoutine = null;
    }

    private void StopCurrentRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    public void Knockback(float knockTime)
    {
        StartCoroutine(KnockCo(knockTime));
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (rb != null)
        {
            StartCoroutine(FlashCo());
            yield return new WaitForSeconds(knockTime);
            rb.velocity = Vector2.zero;
            state = State.Normal;
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator FlashCo()
    {
        int temp = 0;
        triggerCollider.enabled = false;
        while(temp < numberOfFlashes)
        {
            mySprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            mySprite.color = regularColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        triggerCollider.enabled = true;
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1.0f;
    }
}
