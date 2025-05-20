using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public delegate void EnemyDeathEvent(GameObject Enemy);
    public event EnemyDeathEvent OnEnemyDeath;
    public int enemyHp = 50;
    public int currHp;

    public int enemyDamage = 10;


    void Start()
    {
        currHp = enemyHp;
    }

    public void TakeDamage(int damage)
    {
        currHp -= damage;
        if (currHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath(gameObject);
        }

        Destroy(gameObject);
    }
}
