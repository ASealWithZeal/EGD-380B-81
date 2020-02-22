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
    private float lastTP = 0;

    public TextMeshProUGUI cHP;
    public TextMeshProUGUI mHP;

    public TextMeshProUGUI cTP;
    public TextMeshProUGUI mTP;

    public TextMeshProUGUI name;
    public TextMeshProUGUI level;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.color = colors[0];
    }

    public void SetNewHP(string name, int level, int nCHP, int nMHP, int nCTP, int nMTP)
    {
        lastHealth = nCHP;
        lastTP = nCTP;
        cHP.SetText(nCHP.ToString());
        mHP.SetText(nMHP.ToString());

        cTP.SetText(nCTP.ToString());
        mTP.SetText(nMTP.ToString());

        this.name.SetText(name);
        this.level.SetText("Lv " + level.ToString());
    }

    public void ChangeHealth(float newHealthPercent, int currentHealth)
    {
        StartCoroutine(AlterHealthBar(newHealthPercent, currentHealth));
    }

    public void ChangeTP(float newTPPercent, int currentTP)
    {
        StartCoroutine(AlterTPBar(newTPPercent, currentTP));
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
            healthBar.fillAmount -= 0.01f * multiplier;

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

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        healthBar.fillAmount = newHealthPercent;
        cHP.SetText(currentHealth.ToString());
        lastHealth = currentHealth;

        yield return null;
    }

    IEnumerator AlterTPBar(float newTPPercent, int currentTP)
    {
        bool decrease = true;
        float multiplier = 1;
        if (tpBar.fillAmount < newTPPercent)
        {
            decrease = false;
            multiplier = -1;
        }

        if (currentTP < 0)
            currentTP = 0;

        while ((decrease && tpBar.fillAmount > newTPPercent)
            || (!decrease && tpBar.fillAmount < newTPPercent))
        {
            tpBar.fillAmount -= 0.005f * multiplier;

            if ((decrease && lastTP > currentTP)
                || (!decrease && lastTP < currentTP))
            {
                lastTP -= (0.15f * multiplier);
                int newLastTP = (int)lastTP;
                cTP.SetText(newLastTP.ToString());
            }

            //if (tpBar.fillAmount > 0.5f)
            //    tpBar.color = colors[0];
            //else if (tpBar.fillAmount <= 0.5f && tpBar.fillAmount > 0.25f)
            //    tpBar.color = colors[1];

            yield return new WaitForSeconds(0.005f);
        }

        tpBar.fillAmount = newTPPercent;
        cTP.SetText(currentTP.ToString());
        lastTP = currentTP;

        yield return null;
    }
}
