using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public Image healthBar = null;
    public Image tpBar = null;
    public List<Color> colors = null;
    private float lastHealth = 0;

    public TextMeshProUGUI cHP;
    public TextMeshProUGUI mHP;

    public TextMeshProUGUI cTP;
    public TextMeshProUGUI mTP;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.color = colors[0];
    }

    public void SetNewHP(int nCHP, int nMHP, int nCTP, int nMTP)
    {
        lastHealth = nCHP;
        cHP.SetText(nCHP.ToString());
        mHP.SetText(nMHP.ToString());

        cTP.SetText(nCTP.ToString());
        mTP.SetText(nMTP.ToString());
    }

    public void ChangeHealth(float newHealthPercent, int currentHealth)
    {
        StartCoroutine(AlterHealthBar(newHealthPercent, currentHealth));
    }

    IEnumerator AlterHealthBar(float newHealthPercent, int currentHealth)
    {
        bool decrease = true;
        float multiplier = 1;
        if (healthBar.fillAmount < newHealthPercent)
        {
            decrease = false;
            multiplier = -1;
        }

        if (currentHealth < 0)
            currentHealth = 0;

        while ((decrease && healthBar.fillAmount > newHealthPercent) 
            || (!decrease && healthBar.fillAmount < newHealthPercent))
        {
            healthBar.fillAmount -= 0.005f * multiplier;

            if ((decrease && lastHealth > currentHealth)
                || (!decrease && lastHealth < currentHealth))
            {
                lastHealth -= (0.5f * multiplier);
                int newLastHealth = (int)lastHealth;
                cHP.SetText(newLastHealth.ToString());
            }

            if (healthBar.fillAmount > 0.5f)
                healthBar.color = colors[0];
            else if (healthBar.fillAmount <= 0.5f && healthBar.fillAmount > 0.25f)
                healthBar.color = colors[1];

            yield return new WaitForSeconds(0.005f);
        }

        healthBar.fillAmount = newHealthPercent;
        cHP.SetText(currentHealth.ToString());
        lastHealth = currentHealth;

        yield return null;
    }
}
