using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityLearnScreen : MonoBehaviour
{
    public CharData characterData;
    public TextMeshProUGUI nameText = null;
    public TextMeshProUGUI abilityText = null;
    public Button button = null;
    public AbilityButtons[] abilityButtons = null;
    public string defaultText = null;
    public bool done = false;

    public void Setup()
    {
        PlayerActions p = characterData.gameObject.GetComponent<PlayerActions>();

        abilityButtons[0].InitValues(p, 0);
        for (int i = 1; i < abilityButtons.Length; ++i)
        {
            if (i == 1)
            {
                characterData.learnedAbilities[0] = true;
                abilityButtons[i].InitValues(p, 0);
                characterData.learnedAbilities[0] = false;
            }
            else
                abilityButtons[i].InitValues(p, i - 1);
        }

        // Sets up the default text
        defaultText = "<b>" + p.GetAbilityName(0) + ":</b> ";
        if (p.GetAbilityActive(0))
            defaultText += "Cost: " + p.GetAbilityCost(0).ToString() + " TP. ";
        else
            defaultText += "Passive. ";
        defaultText += p.GetAbilityDescription(0);
    }

    // After fading in, selects the button and changes the "ability" text
    public void Init()
    {
        SetAbilityText(defaultText);
        button.Select();
    }

    public void SetAbilityText(string s)
    {
        abilityText.SetText(s);
    }

    public void SetNameText()
    {
       nameText.SetText("Teach " + characterData.name + " a new ability!");
    }

    public void LearnAbility(int i)
    {
        if (!characterData.learnedAbilities[i])
        {
            characterData.learnedAbilities[i] = true;
            done = true;
        }
    }

    public void LearnPassiveAbility(int i, int p)
    {
        if (!characterData.learnedAbilities[i])
        {
            characterData.learnedAbilities[i] = true;

            switch (p)
            {
                case 0:
                    characterData.gameObject.GetComponent<Stats>().HPPassives += 0.25f;
                    break;
                case 1:
                    characterData.gameObject.GetComponent<Stats>().TPPassives += 0.25f;
                    break;
                case 2:
                    characterData.gameObject.GetComponent<Stats>().atkPassives += 0.25f;
                    break;
                case 3:
                    characterData.gameObject.GetComponent<Stats>().defPassives += 0.25f;
                    break;
                case 4:
                    characterData.gameObject.GetComponent<Stats>().spdPassives += 0.25f;
                    break;
            }
            
            done = true;
        }
    }
}
