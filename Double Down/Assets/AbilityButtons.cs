using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButtons : MonoBehaviour, ISelectHandler
{
    public string buttonText = null;
    public int ability;
    public CharData data = null;
    public List<Color> setColors;
    public Sprite pressedSprite;

    public void InitValues(PlayerActions actions, int num)
    {
        ability = num;
        buttonText = "<b>" + actions.GetAbilityName(num) + ":</b> ";
        if (actions.GetAbilityActive(num))
            buttonText += "Cost: " + actions.GetAbilityCost(num).ToString() + " TP. ";
        else
            buttonText += "Passive. ";

        buttonText += actions.GetAbilityDescription(num);

        //if (data.learnedAbilities[ability])
        //    SetColors();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.parent.GetComponent<AbilityLearnScreen>().SetAbilityText(buttonText);
    }

    public void SelectAbilityButton()
    {
        SetColors();
        Invoke("DisableClickSound", 0.1f);
        transform.parent.GetComponent<AbilityLearnScreen>().LearnAbility(ability);
    }

    public void SelectPassiveButton(int passive)
    {
        SetColors();
        Invoke("DisableClickSound", 0.1f);
        transform.parent.GetComponent<AbilityLearnScreen>().LearnAbility(ability);
    }

    private void SetColors()
    {
        Button button = GetComponent<Button>();
        button.image.color = setColors[0];

        ColorBlock colors = button.colors;
        colors.pressedColor = setColors[1];
        button.image.sprite = pressedSprite;
        button.colors = colors;
    }

    private void DisableClickSound()
    {
        GetComponent<ButtonAudio>().canClick = false;
    }
}
