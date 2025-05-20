using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMove : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

    private Vector2 lastMovementDirection;
    public float movSpeed = 2f;
    //float moveX;
    //float moveY;

    Vector2 movemento;

    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;


    [Header("Dash Settings")]
    public float dashSpeed = 5f;
    public float dashLength = 0.5f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTime;
    private float dashCooldownTime;
    private Vector2 dashDirection;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        movemento.x = Input.GetAxisRaw("Horizontal");
        movemento.y = Input.GetAxisRaw("Vertical");

        if (movemento != Vector2.zero)
        {
            lastMovementDirection = movemento.normalized;
        }

        UpdateAttackPosition();
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(" fire press");
            animator.SetBool("isAttacking", true);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartDash();
        }


        //LOGIC DASH
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                EndDash();
            }
        }

        if (!canDash)
        {
            dashCooldownTime -= Time.deltaTime;
            if (dashCooldownTime <= 0)
            {
                canDash = true;
                //dashCooldownTime = dashCooldown;
            }

        }
    }

    private void FixedUpdate()
    {

        animator.SetFloat("Horizontal", movemento.x);
        animator.SetFloat("Vertical", movemento.y);
        animator.SetFloat("Speed", movemento.sqrMagnitude);

        if (!isDashing)
        {
            // Normal movement
            rb.MovePosition(rb.position + movemento.normalized * movSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Dash movement
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
        }

        // Sprite flipping
        if (movemento.x != 0)
        {
            sr.flipX = movemento.x < 0;
        }



    }


    public void StartDash()
    {
        if (movemento != Vector2.zero)
        {
            dashDirection = movemento.normalized;
        }
        else
        {
            dashDirection = lastMovementDirection;
        }

        isDashing = true;
        canDash = false;
        dashTime = dashLength;
        dashCooldownTime = dashCooldown;
        Debug.Log("start dash");
    }

    public void EndDash()
    {
        //canDash = true;
        isDashing = false;
        Debug.Log("end dash");
    }

    private void UpdateAttackPosition()
    {
        if (lastMovementDirection.x > 0)
        {
            attackPoint.transform.localPosition = new Vector3(1f, 0f, 0f);
        }
        else if (lastMovementDirection.x < 0)
        {
            attackPoint.transform.localPosition = new Vector3(-1f, 0f, 0f);
        }
    }

    public void attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);
        foreach (Collider2D enemyGameObject in enemy)
        {
            EnemyController enemyController = enemyGameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(10);
                Debug.Log("hit enemy");
            }

        }
    }

    public void endAttack()
    {
        Debug.Log("attack selesai");
        animator.SetBool("isAttacking", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}
