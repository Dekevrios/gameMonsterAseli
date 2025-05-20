using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public enum BossState
    {
        Gap,
        Attack,
        Summon,
        Tired
    }

    [Header("Stats")]
    public float maxHp = 100f;
    public float currentHp;
    public int hitsTillNextPhase = 5;
    private int currentHit = 0;

    //Attack
    [Header("Attack settings")]
    public float attackInterval = 2f;
    public int bulletAmount = 12;
    public float bulletSpread = 360f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public float bulletLifetime = 5f;

    //Summon 
    [Header("Summon settings")]
    public GameObject enemyPrefab;
    public int enemyCount = 4;
    public float summonRadius = 5f;
    public Transform[] spawnPoints;

    [Header("HEALTH BAR")]
    public Slider slider;

    public float phaseTimer = 60f;
    public float currentTime = 0f;
    public float attackTimer = 0f;
    public float tiredDuration = 10f;

    public GameObject weakPoint;
    public Animator anima;
    public BossState currentState;
    public SpriteRenderer sr;

    private bool enemyDefeate = false;

    public bool isTired = false;
    private List<GameObject> summonEnemy = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        slider.maxValue = maxHp;
        slider.value = currentHp;
        currentState = BossState.Attack;
        isTired = false;

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points for boss summons");
        }

        if (!anima)
        {
            anima = GetComponent<Animator>();
        }

        if (!sr)
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case BossState.Attack:
                //UpdateIdleState();
                UpdateAttack();
                break;
            case BossState.Summon:
                //UpdateAttackAState();
                UpdateSummon();
                break;
            case BossState.Tired:
                UpdateTired();
                break;
            case BossState.Gap:

                break;
        }

        if (currentState == BossState.Attack || currentState == BossState.Summon)
        {
            currentTime = Time.deltaTime;
            if (currentTime >= phaseTimer)
            {
                StartCoroutine(TransitionTired());
            }
        }
    }

    void UpdateAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            SpawnBulletPattern();
            if (Random.Range(0, 100) < 30)
            {
                Debug.Log(" to summon state");
                currentState = BossState.Summon;
                if (anima)
                {
                    anima.SetTrigger("isSummoning");

                }
                StartCoroutine(SummonEnemy());
            }
        }
    }

    void UpdateSummon()
    {
        if (enemyDefeate && summonEnemy.Count == 0)
        {
            Debug.Log("All summoned enemies defeated. Going to vulnerable state.");
            StartCoroutine(TransitionTired());
        }
    }

    void UpdateTired()
    {

    }

    void SpawnBulletPattern()
    {
        int patternType = Random.Range(0, 3);
        switch (patternType)
        {
            case 0:
                SpawnBulletCircle(bulletAmount / 2, 180f, transform.rotation.eulerAngles.z);
                break;
            case 1:
                SpawnBulletRandom(bulletAmount / 2);
                break;
            case 2:
                SpawnBulletFullCircle(bulletAmount / 2, transform.rotation.eulerAngles.z);
                break;
        }
        if (anima)
        {
            anima.SetTrigger("Attack");
        }
    }
    void SpawnBulletFullCircle(int count, float baseAngle = 0f)
    {
        // Sudut penuh 360 derajat untuk lingkaran penuh
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            // Hitung sudut untuk setiap bullet
            float angle = baseAngle + (i * angleStep);
            SpawnBullet(angle);
        }
    }

    void SpawnBulletCircle(int count, float spreadAngle, float baseAngle = 0f)
    {
        float angleStep = spreadAngle / count;
        float startAngle = baseAngle - spreadAngle;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            SpawnBullet(angle);
        }
    }

    void SpawnBulletRandom(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            SpawnBullet(randomAngle);
        }
    }

    void SpawnBullet(float angle)
    {
        Vector3 spawnPosition = transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.Euler(0, 0, angle));

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.bulletSpeed = bulletSpeed;
            bulletScript.bulletLife = bulletLifetime;
            bulletScript.rotation = angle;
        }
        else
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                rb.velocity = direction * bulletSpeed;
                Destroy(bullet, bulletLifetime);
            }
            Debug.LogWarning("Bullet script not found on bullet prefab");
        }
    }

    IEnumerator SummonEnemy()
    {
        enemyDefeate = false;
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPosition = spawnPoints[i % spawnPoints.Length].position;
            }
            else
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector2 spawnOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * summonRadius;
                spawnPosition = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0);
            }

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            summonEnemy.Add(enemy);

            EnemyControl enemyController = enemy.GetComponent<EnemyControl>();
            if (enemyController != null)
            {
                enemyController.OnEnemyDeath += OnSummonEnemyDeath;
            }


            yield return new WaitForSeconds(0.5f);
        }

        //yield return null;
    }

    void OnSummonEnemyDeath(GameObject Enemy)
    {
        if (summonEnemy.Contains(Enemy))
        {
            summonEnemy.Remove(Enemy);
            //if (summonEnemy.Count == 0)
            //{
            //    enemyDefeate = true;
            //}
        }
        if (summonEnemy.Count == 0)
        {
            enemyDefeate = true;
        }

    }

    IEnumerator TransitionTired()
    {
        currentState = BossState.Gap;
        if (anima)
        {
            anima.SetTrigger("Exhausted");
        }

        yield return new WaitForSeconds(1.5f);
        //weakPoint.SetActive(true);
        isTired = true;

        currentHit = 0;
        currentState = BossState.Tired;
        Debug.Log("now can damage");

        yield return new WaitForSeconds(tiredDuration);

        if (currentState == BossState.Tired)
        {
            EndTired();
        }
    }

    void EndTired()
    {
        isTired = false;
        currentState = BossState.Attack;
        currentTime = 0f;

        if (anima)
        {
            anima.SetTrigger("EndTired");
        }

        if (anima) anima.SetTrigger("Recover");
    }


    public void TakeDamage(int damage)
    {
        if (isTired)
        {
            currentHp -= damage;
            slider.value = currentHp;

            currentHit++;
            Debug.Log($"Boss hit! Current health: {currentHp}/{maxHp}, Hits: {currentHit}/{hitsTillNextPhase}");

            if (anima)
            {
                anima.SetTrigger("Hit");
            }

            if (currentHp <= 0)
            {
                Debug.Log("boss defeat");
                Die();
            }
            else if (currentHit >= hitsTillNextPhase)
            {
                EndTired();
            }


        }
        else
        {
            Debug.Log("cannot attack boss");
        }

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        TakeDamage(10f);
    //    }
    //}

    void Die()
    {
        Debug.Log("Boss defeated!");
        foreach (GameObject enemy in summonEnemy)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        StartCoroutine(ReturnToMainMenu());
        GetComponent<Collider2D>().enabled = false;

        sr.enabled = false;

        IEnumerator ReturnToMainMenu()
        {
            // Tunggu beberapa detik
            yield return new WaitForSeconds(2f);

        
            try
            {
                // Kembali ke menu utama
                SceneManager.LoadScene("Main Menu");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading main menu scene: " + e.Message);
                
            }
        }

    }
}
