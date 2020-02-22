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

    // Start is called before the first frame update
    void Start()
    {
        if (data.learnedAbilities[ability])
            SetColors();
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
        transform.parent.GetComponent<AbilityLearnScreen>().LearnAbility(ability);
    }

    public void SelectPassiveButton(int passive)
    {
        SetColors();
        transform.parent.GetComponent<AbilityLearnScreen>().LearnPassiveAbility(ability, passive);
    }

    private void SetColors()
    {
        Button button = GetComponent<Button>();
        button.image.color = setColors[0];

        ColorBlock colors = button.colors;
        colors.pressedColor = setColors[1];
        button.colors = colors;
    }
}
