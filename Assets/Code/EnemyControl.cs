using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl: MonoBehaviour
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

            Debug.Log("Enemy took damage and died");
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

