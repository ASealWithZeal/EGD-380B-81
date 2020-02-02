using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    Attack = 0,
    Defend,
    Ability0,
    Ability1
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
                //else if (character == Char.Char1)
                //    GetComponent<Medic>().Attack();
                break;

            case (int)Actions.Defend:
                if (character == Char.Char0)
                    GetComponent<Attacker>().Defend();
                //else if (character == Char.Char1)
                //    GetComponent<Medic>().Attack();
                break;

            case (int)Actions.Ability0:
                break;

            case (int)Actions.Ability1:
                break;
        }
            
    }
}
