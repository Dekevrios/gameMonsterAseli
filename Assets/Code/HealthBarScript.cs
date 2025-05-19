using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText; // the text that says 100/100

    public int maxHealth;  // maximum health
    public int currHealth; // current health

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth; // set the current health to max health
    }

    // Update is called once per frame
    void Update()
    {
        // set the health bar text
        healthBarValueText.text = currHealth.ToString() + "/" + maxHealth.ToString();

        // set the slider values
        healthBarSlider.value = currHealth;
        healthBarSlider.maxValue = maxHealth;
    }
}
