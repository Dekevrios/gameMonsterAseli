using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charStats : MonoBehaviour
{
    public int damage = 10;
    public int hp = 100;
    public int currHp;
    // Start is called before the first frame update
    void Start()
    {
        currHp = hp;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currHp -= damage;
        if (currHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void setHp(int hpPlus)
    {
        currHp += hpPlus;
    }
}
