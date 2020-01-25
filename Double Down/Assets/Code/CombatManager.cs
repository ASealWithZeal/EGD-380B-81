using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public GameObject playerChars;
    public GameObject enemyChars;
    public TurnTracker tracker;
    [HideInInspector] public List<GameObject> playerCharsList = null;
    [HideInInspector] public List<GameObject> enemyCharsList = null;
    [HideInInspector] public List<GameObject> combatChars = null;

    private void Start()
    {
        for (int i = 0; i < playerChars.transform.childCount; ++i)
        {
            playerCharsList.Add(playerChars.transform.GetChild(i).gameObject);
            combatChars.Add(playerChars.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < enemyChars.transform.childCount; ++i)
        {
            enemyCharsList.Add(enemyChars.transform.GetChild(i).gameObject);
            combatChars.Add(enemyChars.transform.GetChild(i).gameObject);
        }

        SetTurnOrder();
    }

    private void StartTurn()
    {

    }

    // Sorts the character order for each turn
    // Can be used to re-sort the order mid-turn
    private void SetTurnOrder()
    {
        bool sorted = false;
        bool sorting = true;
        GameObject temp = null;
        
        while (sorting)
        {
            for (int i = 0; i < combatChars.Count; ++i)
            {
                if (i != 0 && combatChars[i].GetComponent<Stats>().Speed() > combatChars[i - 1].GetComponent<Stats>().Speed())
                {
                    sorted = false;

                    temp = combatChars[i];
                    combatChars[i] = combatChars[i - 1];
                    combatChars[i - 1] = temp;
                }
            }

            if (!sorted)
                sorted = true;

            else
                sorting = false;
        }

        tracker.SetUpTrackers(combatChars);
    }

    private void TurnTrackerVisuals()
    {

    }

    private void DealDamage()
    {
        //target.ReduceHP(DamageFormula(chara, target, 1));
        //Debug.Log(target.HP());
    }

    private int DamageFormula(Stats a, Stats t, float mod)
    {
        int i = (int)(((a.Attack() * a.Attack()) / t.Defense()) * mod * Random.Range(0.8f, 1.0f));
        return i;
    }
}
