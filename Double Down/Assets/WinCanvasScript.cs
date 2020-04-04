using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WinCanvasScript : MonoBehaviour
{
    public Stats charStats;
    public bool done = false;
    public List<Color> fullTextColor;

    [Header("Misc")]
    public TextMeshProUGUI charName;
    public Image charPortrait;

    [Header("EXP Meter")]
    public TextMeshProUGUI expText;
    public TextMeshProUGUI neededEXPText;
    public Image expMeter;
    public CanvasGroup levelUpGroup;

    [Header("Stats and Gains")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelGainText;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI hpGainText;

    public TextMeshProUGUI tpText;
    public TextMeshProUGUI tpGainText;

    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atkGainText;

    public TextMeshProUGUI defText;
    public TextMeshProUGUI defGainText;

    public TextMeshProUGUI spdText;
    public TextMeshProUGUI spdGainText;
    
    public void Setup()
    {
        charName.SetText(charStats.gameObject.GetComponent<CharData>().name);
        charPortrait.color = Color.white;
        charPortrait.sprite = charStats.gameObject.GetComponent<CharData>().normalPortrait;
    }

    public void Init()
    {
        levelText.SetText(charStats.level.ToString() + " (");
        hpText.SetText(charStats.MaxHP().ToString() + " (");
        tpText.SetText(charStats.MaxTP().ToString() + " (");
        atkText.SetText(charStats.Attack().ToString() + " (");
        defText.SetText(charStats.Defense().ToString() + " (");
        spdText.SetText(charStats.Speed().ToString() + " (");

        ResetText(levelGainText);
        ResetText(hpGainText);
        ResetText(tpGainText);
        ResetText(atkGainText);
        ResetText(defGainText);
        ResetText(spdGainText);

        levelUpGroup.alpha = 0.0f;
    }

    private void ResetText(TextMeshProUGUI txt)
    {
        txt.SetText("+0");
        txt.color = Color.white;
    }

    public void UpdateNonCombatUI()
    {
        if (charStats.level == charStats.nextLevelExp.Count + 1)
        {
            expText.SetText("+0");
            neededEXPText.SetText("--");
            expMeter.fillAmount = 1.0f;
        }

        else
        {
            expText.SetText("+0");
            neededEXPText.SetText((charStats.nextLevelExp[(charStats.level - 1)] - charStats.exp).ToString());
        }
    }

    // Update is called once per frame
    public void UpdateUI(int earnedEXP)
    {
        StartCoroutine(UpdatePlayerUI(earnedEXP));
    }

    IEnumerator UpdatePlayerUI(int earnedEXP)
    {
        int levelUp = 0;
        int cStartingEXP = charStats.startingExp;
        int cEXP = charStats.startingExp + earnedEXP;
        bool levelingUp = true;
        float e = earnedEXP;
        float n = 0;

        if (charStats.startingLevel < charStats.nextLevelExp.Count + 1)
        {
            expText.SetText("+" + earnedEXP.ToString());
            expMeter.fillAmount = cStartingEXP / (float)charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp];
        }
        else if (charStats.startingLevel == charStats.nextLevelExp.Count + 1)
        {
            expText.SetText("+0");
            neededEXPText.SetText("--");
            expMeter.fillAmount = 1.0f;
            levelingUp = false;
        }

        while (levelingUp && charStats.startingLevel + levelUp < charStats.nextLevelExp.Count + 1)
        {
            levelingUp = false;
            neededEXPText.SetText((charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp] - cStartingEXP).ToString());
            n = charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp] - cStartingEXP;

            float fill = cEXP / (float)charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp];
            Debug.Log(fill);
            if (fill >= 1.0f)
            {
                levelUp++;
                cStartingEXP = 0;
                levelingUp = true;
                fill = 1.0f;
            }

            float elapser = (e * ((1 / fill - expMeter.fillAmount) * Managers.TurnManager.Instance.tracker.timeIncrements));
            while (expMeter.fillAmount < fill)
            {
                expMeter.fillAmount += 0.01f;
                if (e > 0)
                    e -= elapser;
                if (n > 0)
                    n -= elapser;
                expText.SetText("+" + Mathf.FloorToInt(e).ToString());
                neededEXPText.SetText(Mathf.FloorToInt(n).ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            // Normalizes the displayed text
            expText.SetText("+0");
            if (charStats.startingLevel + levelUp < charStats.nextLevelExp.Count + 1)
                neededEXPText.SetText(((charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp]) - charStats.exp).ToString());
            else
                neededEXPText.SetText("0");
            expMeter.fillAmount = fill;

            if (cEXP - charStats.nextLevelExp[(charStats.startingLevel - 1)] == 0)
            {
                expMeter.fillAmount = 0;
                levelingUp = false;
            }
            else if (charStats.startingLevel + levelUp < charStats.nextLevelExp.Count + 1 && cEXP - charStats.nextLevelExp[(charStats.startingLevel - 1)] != 0 && levelingUp)
            {
                expMeter.fillAmount = 0;
                cEXP = Mathf.Abs(cEXP - charStats.nextLevelExp[(charStats.startingLevel - 1)]);
            }
            if (charStats.startingLevel + levelUp == charStats.nextLevelExp.Count + 1)
            {
                expMeter.fillAmount = 1;
                expText.SetText("+0");
                neededEXPText.SetText("--");
            }
        }

        StartCoroutine(LevelUpVisual(levelUp));

        yield return null;
    }

    IEnumerator LevelUpVisual(int num)
    {
        if (num > 0)
        {
            while (levelUpGroup.alpha < 1.0f)
            {
                levelUpGroup.alpha += 0.1f;
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            
            // LEVEL GAIN
            int t = num;
            int d = 0;
            while (d < t)
            {
                d++;
                levelGainText.SetText("+" + num.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            levelGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.level;
            d = charStats.level - num;
            while (d < t)
            {
                d++;
                levelText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // HP GAIN
            t = (charStats.HPGain + (int)(charStats.HPGain * charStats.HPPassives)) * num;
            d = 0;
            while (d < t)
            {
                d++;
                hpGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            hpGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.MaxHP();
            d = charStats.MaxHP() - ((charStats.HPGain + (int)(charStats.HPGain * charStats.HPPassives)) * num);
            while (d < t)
            {
                d++;
                hpText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // TP GAIN
            t = (charStats.TPGain + (int)(charStats.TPGain * charStats.TPPassives)) * num;
            d = 0;
            while (d < t)
            {
                d++;
                tpGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            tpGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.MaxTP();
            d = charStats.MaxTP() - ((charStats.TPGain + (int)(charStats.TPGain * charStats.HPPassives)) * num);
            while (d < t)
            {
                d++;
                tpText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // ATK GAIN
            t = (charStats.atkGain + (int)(charStats.atkGain * charStats.atkPassives)) * num;
            d = 0;
            while (d < t)
            {
                d++;
                atkGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            atkGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.Attack();
            d = charStats.Attack() - ((charStats.atkGain + (int)(charStats.atkGain * charStats.atkPassives)) * num);
            while (d < t)
            {
                d++;
                atkText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // DEF GAIN
            t = (charStats.defGain + (int)(charStats.defGain * charStats.defPassives)) * num;
            d = 0;
            while (d < t)
            {
                d++;
                defGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            defGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.Defense();
            d = charStats.Defense() - ((charStats.defGain + (int)(charStats.defGain * charStats.defPassives)) * num);
            while (d < t)
            {
                d++;
                defText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // SPD GAIN
            t = (charStats.spdGain + (int)(charStats.spdGain * charStats.spdPassives)) * num;
            d = 0;
            while (d < t)
            {
                d++;
                spdGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            spdGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.Speed();
            d = charStats.Speed() - ((charStats.spdGain + (int)(charStats.spdGain * charStats.spdPassives)) * num);
            while (d < t)
            {
                d++;
                spdText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        done = true;
    }
}
