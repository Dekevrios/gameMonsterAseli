using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManage : MonoBehaviour
{
    public static GameManage instance;

    [Header("Enemy setting")]
    public int totalEnemy = 0;
    public int enemyLeft = 0;

    [Header("UI setting")]
    public Text enemyCountTxt;

    [Header("Level settings")]
    public string nextLevel = "BossStage";
    public bool isNextLevel = false;


    // Start is called before the first frame update

    void Awake()
    {
        Debug.Log("game awake");
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.Log("game start");
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        totalEnemy = enemy.Length;
        enemyLeft = totalEnemy;

        UpdateEnemyCount();
    }

    public void EnemyDeath()
    {
        enemyLeft--;
        Debug.Log("Enemy defeated. Remaining: " + enemyLeft); // Debug

        UpdateEnemyCount();

        if (enemyLeft <= 0)
        {
            StartCoroutine(CompleteLevel());
        }
    }

        private void UpdateEnemyCount()
    {
        if (enemyCountTxt != null)
        {
            enemyCountTxt.text = " / " + enemyLeft;
        }
    }

    private IEnumerator CompleteLevel()
    {
        Debug.Log("Level Complete");

        yield return new WaitForSeconds(2f);

        if (isNextLevel)
        {
            SceneManager.LoadScene("BossStage");
        }
        else
        {
            SceneManager.LoadScene("Level1");
        }
    }
}
