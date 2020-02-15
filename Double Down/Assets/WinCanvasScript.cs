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

    // Start is called before the first frame update
    void Start()
    {
        levelText.SetText(charStats.level.ToString() + " (");
        hpText.SetText(charStats.maxHP.ToString() + " (");
        tpText.SetText(charStats.maxTP.ToString() + " (");
        atkText.SetText(charStats.atk.ToString() + " (");
        defText.SetText(charStats.def.ToString() + " (");
        spdText.SetText(charStats.spd.ToString() + " (");
    }

    // Update is called once per frame
    public void UpdateUI(int earnedEXP)
    {
        StartCoroutine(UpdatePlayerUI(earnedEXP));
    }

    IEnumerator UpdatePlayerUI(int earnedEXP)
    {
        int levelUp = 0;
        int cEXP = charStats.startingExp + earnedEXP;
        bool levelingUp = true;

        expText.SetText("+" + earnedEXP.ToString());
        expMeter.fillAmount = charStats.startingExp / (float)charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp];

        while (levelingUp && charStats.startingLevel + levelUp < charStats.nextLevelExp.Count + 1)
        {
            levelingUp = false;
            expMeter.fillAmount = 0;
            neededEXPText.SetText("Next: " + (charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp]).ToString());

            float fill = cEXP / (float)charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp];
            if (fill >= 1.0f)
            {
                levelUp++;
                levelingUp = true;
                fill = 1.0f;
            }

            while (expMeter.fillAmount < fill)
            {
                expMeter.fillAmount += 0.01f;
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            expMeter.fillAmount = fill;

            if (charStats.exp == 0)
                levelingUp = false;
            else if (charStats.startingLevel + levelUp < charStats.nextLevelExp.Count + 1 && charStats.exp != 0 && levelingUp)
            {
                expMeter.fillAmount = 0;
                cEXP -= charStats.nextLevelExp[(charStats.startingLevel - 1) + levelUp];
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

            t = charStats.level + num;
            d = charStats.level;
            while (d < t)
            {
                d++;
                levelText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // HP GAIN
            t = charStats.HPGain * num;
            d = 0;
            while (d < t)
            {
                d++;
                hpGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            hpGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.maxHP + (charStats.HPGain * num);
            d = charStats.maxHP;
            while (d < t)
            {
                d++;
                hpText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // TP GAIN
            t = charStats.TPGain * num;
            d = 0;
            while (d < t)
            {
                d++;
                tpGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            tpGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.maxTP + (charStats.TPGain * num);
            d = charStats.maxTP;
            while (d < t)
            {
                d++;
                tpText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // ATK GAIN
            t = charStats.atkGain * num;
            d = 0;
            while (d < t)
            {
                d++;
                atkGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            atkGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.atk + (charStats.atkGain * num);
            d = charStats.atk;
            while (d < t)
            {
                d++;
                atkText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // DEF GAIN
            t = charStats.defGain * num;
            d = 0;
            while (d < t)
            {
                d++;
                defGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            defGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.def + (charStats.defGain * num);
            d = charStats.def;
            while (d < t)
            {
                d++;
                defText.SetText(d.ToString() + " (");
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);


            // SPD GAIN
            t = charStats.spdGain * num;
            d = 0;
            while (d < t)
            {
                d++;
                spdGainText.SetText("+" + d.ToString());
                yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
            }
            spdGainText.color = fullTextColor[0];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);

            t = charStats.spd + (charStats.spdGain * num);
            d = charStats.spd;
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
