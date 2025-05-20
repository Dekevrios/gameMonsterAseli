using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    Rigidbody2D rb;
    Animator anima;
    SpriteRenderer sr;

    public EnemyState currentState;

    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Transform player;
    public LayerMask layerPlayer;

    public bool facingRight = true;
    public float enemySpeed = 2f;

    public float attackCd = 1f;
    public int attackDamage = 10;
    public float attackTimer;
    private bool canAttack = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anima = GetComponent<Animator>();

        FindPlayer();
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCd)
            {
                canAttack = true;
                attackTimer = 0f;
            }
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdleState();
                break;
            case EnemyState.Chase:
                UpdateChaseState();
                break;
            case EnemyState.Attack:
                UpdateAttackState();
                break;
        }

    }


    //state machine
    private void ChangeState(EnemyState newState)
    {

        switch (currentState)
        {
            case EnemyState.Idle:
                ExitIdleState();
                break;
            case EnemyState.Chase:
                ExitChaseState();
                break;
            case EnemyState.Attack:
                ExitAttackState();
                break;
        }

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                EnterIdleState();
                break;
            case EnemyState.Chase:
                EnterChaseState();
                break;
            case EnemyState.Attack:
                EnterAttackState();
                break;
        }
    }

    void FindPlayer()
    {
        // Cara 1: Cari berdasarkan tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Enemy: Player ditemukan dengan tag 'Player'");
        }

    }

    //idle
    void EnterIdleState()
    {
        rb.velocity = Vector2.zero; //stop moving
        if (anima != null)
        {
            anima.SetBool("isCatching", false); //set animation to idle
        }
        Debug.Log("Enemy is idle");
    }

    void UpdateIdleState()
    {
        // cek jarak player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void ExitIdleState()
    {

    }

    // chase state
    void EnterChaseState()
    {
        if (anima != null)
        {
            anima.SetBool("isCatching", true); //set animation to run
        }
        Debug.Log("Enemy is chasing");
    }

    void UpdateChaseState()
    {
        //cek jarak player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }
        if (distanceToPlayer > detectionRange)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        //move towards player
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * enemySpeed;

        //flip enemy
        if (direction.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void ExitChaseState()
    {
        rb.velocity = Vector2.zero; //stop moving

    }

    //attack state
    void EnterAttackState()
    {
        rb.velocity = Vector2.zero; //stop moving
        if (anima != null)
        {
            anima.SetBool("isCatching", false); //set animation to idle
        }
        Debug.Log("Enemy is want attack");
    }

    void UpdateAttackState()
    {
        //cek jarak player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (canAttack)
        {
            EnemyAttack();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip();
        }

    }

    void ExitAttackState()
    {

    }
    void EnemyAttack()
    {
        canAttack = false;

        if (anima != null)
        {
            anima.SetTrigger("Attack"); //set animation to attack'
            Debug.Log("Enemy is attacking");
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, layerPlayer);

        if (hitPlayer != null)
        {
            charStats playerHealth = hitPlayer.GetComponent<charStats>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Hit player");
            }

        }
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        //range detection
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        //range attack
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
