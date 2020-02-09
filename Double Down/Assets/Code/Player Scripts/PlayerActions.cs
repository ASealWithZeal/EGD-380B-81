﻿using System.Collections;
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
                    GetComponent<Attacker>().Attack();
                else if (character == Char.Char1)
                    GetComponent<Healer>().Attack();
                break;

            case (int)Actions.Defend:
                if (character == Char.Char0)
                    GetComponent<Attacker>().Defend();
                else if (character == Char.Char1)
                    GetComponent<Healer>().Defend();
                break;

            case (int)Actions.Ability0:
                if (character == Char.Char0)
                    GetComponent<Attacker>().Ability0();
                else if (character == Char.Char1)
                    GetComponent<Healer>().Ability0();
                break;

            case (int)Actions.Ability1:
                break;
        }
            
    }

    public string GetAbilityName(int i)
    {
        string returnVal = "";

        switch (i)
        {
            case 0:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability0Name;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability0Name;
                break;

            //case 1:
            //    if (character == Char.Char0)
            //        returnVal = GetComponent<Attacker>().ability1Name;
            //    else if (character == Char.Char1)
            //        returnVal = GetComponent<Healer>().ability1Name;
            //    break;
        }

        return returnVal;
    }

    public int GetAbilityCost(int i)
    {
        int returnVal = 0;

        switch (i)
        {
            case 0:
                if (character == Char.Char0)
                    returnVal = GetComponent<Attacker>().ability0Cost;
                else if (character == Char.Char1)
                    returnVal = GetComponent<Healer>().ability0Cost;
                break;

                //case 1:
                //    if (character == Char.Char0)
                //        returnVal = GetComponent<Attacker>().ability1Name;
                //    else if (character == Char.Char1)
                //        returnVal = GetComponent<Healer>().ability1Name;
                //    break;
        }

        return returnVal;
    }
}
