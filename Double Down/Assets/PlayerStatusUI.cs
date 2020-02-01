using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public Image healthBar = null;
    public Image tpBar = null;
    public List<Color> colors = null;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.color = colors[0];
    }

    public void ChangeHealth(float newHealthPercent)
    {
        StartCoroutine(AlterHealthBar(newHealthPercent));
    }

    IEnumerator AlterHealthBar(float newHealthPercent)
    {
        bool decrease = true;
        float multiplier = 1;
        if (healthBar.fillAmount < newHealthPercent)
        {
            decrease = false;
            multiplier = -1;
        }

        while ((decrease && healthBar.fillAmount > newHealthPercent) 
            || (!decrease && healthBar.fillAmount < newHealthPercent))
        {
            healthBar.fillAmount -= 0.005f * multiplier;

            if (healthBar.fillAmount > 0.5f)
                healthBar.color = colors[0];
            else if (healthBar.fillAmount <= 0.5f && healthBar.fillAmount > 0.25f)
                healthBar.color = colors[1];

            yield return new WaitForSeconds(0.005f);
        }

        healthBar.fillAmount = newHealthPercent;

        yield return null;
    }
}
