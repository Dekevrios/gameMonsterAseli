using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class charStats : MonoBehaviour
{
    public int damage = 10;
    public int hp = 100;
    public int currHp;

    [Header("HEALTH BAR")]
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        currHp = hp;
        slider.maxValue = hp;
        slider.value = currHp;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currHp -= damage;
        slider.value = currHp;

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
