using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �÷��̾� ����
    public enum State
    {
        Normal, Rolling, Casting, Stagger, Dead,
    }
    public State state;

    // �÷��̾� ���� ���
    public enum AttackMethod
    { 
        Ranged, Melee, Meteor,
    }
    public AttackMethod attackMethod;

    // �ִϸ��̼�
    private Animator animator;
    private SpriteRenderer sr;

    // ����
    [Header("����")]
    public int maxHealth;
    [SerializeField] private int health;

    // �̵�
    [Header("�̵�")]
    public float moveSpeed;
    private bool fixAngle;
    private Rigidbody2D rb;
    private float xInput;
    private float yInput;
    private Vector3 moveDir;
    private Vector3 lastMoveDir;

    // ������
    [Header("������")]
    public float maxRollSpeed;
    public float minRollSpeed;
    public float rollSpeedDropMultiplier;
    private float rollSpeed;
    private Vector3 rollDir;

    // ����
    [Header("����")]
    public GameObject projectile;
    public GameObject projectileSpawnPos;
    public GameObject weapon;
    public float preDelay;
    public float aftDelay;
    private Camera viewCamera;
    private Vector2 mousePos;
    private float angle;
    private Coroutine currentRoutine;

    // �ǰ�
    [Header("�ǰ�")]
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

        #region ���콺 ��ġ ���
        if (!fixAngle)
        {
            // ���콺 ��ġ ���
            mousePos = Input.mousePosition;
            mousePos = viewCamera.ScreenToWorldPoint(mousePos);
            // ���콺 ��ġ�� �Ĵٺ��� ���� ����
            angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
            weapon.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        #endregion

        #region �����¿�, �ֱ� ���� �� �ޱ�
        // �����¿� ������ �Է�
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(xInput, yInput).normalized;

        // �ִϸ��̼� �� ����
        animator.SetFloat("xInput", xInput);
        animator.SetFloat("yInput", yInput);


        // �ִϸ��̼� �����̰� �ִ��� �ƴ��� ����
        if (moveDir == Vector3.zero) animator.SetBool("isMoving", false);
        else animator.SetBool("isMoving", true);

        // �ֱ� �������� ����
        if (xInput != 0 || yInput != 0)
        {
            lastMoveDir = moveDir;
        }
        #endregion

        switch (state)
        {
            // �Ϲ� ����
            case State.Normal:
                // �¿����
                if (xInput < 0) sr.flipX = true;
                else if (xInput > 0) sr.flipX = false;

                // ������ �Է�
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

                // �ֹ� ����
                if (Input.GetMouseButtonDown(0))
                {
                    state = State.Casting;
                }
                break;

            // ������ ����
            case State.Rolling:
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;
                triggerCollider.enabled = false;
                if (rollSpeed < minRollSpeed)
                {
                    triggerCollider.enabled = true;
                    state = State.Normal;
                }
                break;

            // �ֹ� ���� ��
            case State.Casting:
                {
                    // �ڷ�ƾ �ѹ��� ����
                    if (currentRoutine == null)
                    {
                        currentRoutine = StartCoroutine(Shoot(preDelay, aftDelay));
                    }

                    // ������� ĵ��
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
        // ���� ���� �ٶ󺸱�
        Vector3 spawnPos = projectileSpawnPos.transform.position;
        if (spawnPos.x - transform.position.x < 0) sr.flipX = true;
        else if (spawnPos.x - transform.position.x > 0) sr.flipX = false;

        // ����
        yield return new WaitForSeconds(preDelay);
        
        // ����ü ��ȯ
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
        // �ĵ�
        fixAngle = true;
        yield return new WaitForSeconds(aftDelay);
        // ���� ���·� ����
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
