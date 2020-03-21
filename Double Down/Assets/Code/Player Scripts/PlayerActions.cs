using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    Attack = 0,
    Defend,
    Ability0,
    Ability1,
    Ability2,
    Ability3,
    Ability4
}

public enum Char
{
    Char0 = 0,
    Char1 = 1
}

public class PlayerActions : MonoBehaviour
{
    public Char character;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckAction(int action)
    {
        switch (action)
        {
            case (int)Actions.Attack:
                if (character == Char.Char0)
                    GetComponent<Attacker>().SetAttack();
                else if (character == Char.Char1)
                    GetComponent<Healer>().SetAttack();
                break;

            case (int)Actions.Defend:
                if (character == Char.Char0)
                    GetComponent<Attacker>().SetDefend();
                else if (character == Char.Char1)
                    GetComponent<Healer>().SetDefend();
                break;

            // Sets the character's basic (or upgraded basic) ability
            case (int)Actions.Ability0:
                if (character == Char.Char0)
                    GetComponent<Attacker>().SetAbility0();
                else if (character == Char.Char1)
                    GetComponent<Healer>().SetAbility0();
                break;

            // Sets the character's second ability
            case (int)Actions.Ability1:
                if (character == Char.Char0)
                    GetComponent<Attacker>().SetAbility1();
                else if (character == Char.Char1)
                    GetComponent<Healer>().SetAbility1();
                break;
        }

    }

    public void PerformAction(int action)
    {
        switch (action)
        {
            case (int)Actions.Attack:
                if (character == Char.Char0)
                    GetComponent<Attacker>().PerformAttack();
                else if (character == Char.Char1)
                    GetComponent<Healer>().PerformAttack();
                break;

            case (int)Actions.Defend:
                if (character == Char.Char0)
                    GetComponent<Attacker>().PerformDefend();
                else if (character == Char.Char1)
                    GetComponent<Healer>().PerformDefend();
                break;

            // Performs the character's basic (or upgraded basic) ability
            case (int)Actions.Ability0:
                if (!GetComponent<CharData>().learnedAbilities[0])
                {
                    if (character == Char.Char0)
                        GetComponent<Attacker>().PerformAbility0();
                    else if (character == Char.Char1)
                        GetComponent<Healer>().PerformAbility0();
                }

                else
                {
                    if (character == Char.Char0)
                        GetComponent<Attacker>().PerformAbility0Upgrade();
                    else if (character == Char.Char1)
                        GetComponent<Healer>().PerformAbility0Upgrade();
                }
                break;

            case (int)Actions.Ability1:
                if (character == Char.Char0)
                    GetComponent<Attacker>().PerformAbility1();
                else if (character == Char.Char1)
                    GetComponent<Healer>().PerformAbility1();
                break;
        }
    }

    public string GetAbilityName(int i)
    {
        string returnVal = "";

        switch (i)
        {
            case 0:
                if (!GetComponent<CharData>().learnedAbilities[0])
                {
                    if (character == Char.Char0)
                        returnVal = GetComponent<Attacker>().ability0Name;
                    else if (character == Char.Char1)
                        returnVal = GetComponent<Healer>().ability0Name;
                }

                else
                {
                    if (character == Char.Char0)
                        returnVal = GetComponent<Attacker>().ability0UpgradeName;
                    else if (character == Char.Char1)
                        returnVal = GetComponent<Healer>().ability0UpgradeName;
                }
                break;

            case 1:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability1Name;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability1Name;
                break;

            case 2:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability2Name;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability2Name;
                break;

            case 3:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability3Name;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability3Name;
                break;
        }

        return returnVal;
    }

    public int GetAbilityCost(int i)
    {
        int returnVal = 0;

        switch (i)
        {
            case 0:
                if (!GetComponent<CharData>().learnedAbilities[0])
                {
                    if (character == Char.Char0)
                        returnVal = GetComponent<Attacker>().ability0Cost;
                    else if (character == Char.Char1)
                        returnVal = GetComponent<Healer>().ability0Cost;
                }

                else
                {
                    if (character == Char.Char0)
                        returnVal = GetComponent<Attacker>().ability0UpgradeCost;
                    else if (character == Char.Char1)
                        returnVal = GetComponent<Healer>().ability0UpgradeCost;
                }
                break;

            case 1:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability1Cost;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability1Cost;
                break;
        }

        return returnVal;
    }

    public string GetAbilityDescription(int i)
    {
        string returnVal = "";
        switch (i)
        {
            case 0:
                if (!GetComponent<CharData>().learnedAbilities[0])
                {
                    if (character == Char.Char0)
                        returnVal = GetComponent<Attacker>().ability0Description;
                    else if (character == Char.Char1)
                        returnVal = GetComponent<Healer>().ability0Description;
                }

                else
                {
                    if (character == Char.Char0)
                        returnVal = GetComponent<Attacker>().ability0UpgradeDescription;
                    else if (character == Char.Char1)
                        returnVal = GetComponent<Healer>().ability0UpgradeDescription;
                }
                break;

            case 1:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability1Description;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability1Description;
                break;

            case 2:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability2Description;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability2Description;
                break;

            case 3:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability3Description;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability3Description;
                break;
        }

        return returnVal;
    }

    public bool GetAbilityActive(int i)
    {
        switch (i)
        {
            case 0:
                if (character == Char.Char0)
                    return GetComponent<Attacker>().ability0Active;
                else if (character == Char.Char1)
                    return GetComponent<Healer>().ability0Active;
                break;

            case 1:
                if (character == Char.Char0)
                    return GetComponent<Attacker>().ability1Active;
                else if (character == Char.Char1)
                    return GetComponent<Healer>().ability1Active;
                break;

            case 2:
                if (character == Char.Char0)
                    return GetComponent<Attacker>().ability2Active;
                else if (character == Char.Char1)
                    return GetComponent<Healer>().ability2Active;
                break;

            case 3:
                if (character == Char.Char0)
                    return GetComponent<Attacker>().ability3Active;
                else if (character == Char.Char1)
                    return GetComponent<Healer>().ability3Active;
                break;
        }

        return false;
    }
}
