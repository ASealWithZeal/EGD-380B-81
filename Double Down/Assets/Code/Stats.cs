using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Main Stats")]
    [SerializeField] private int maxHP = 100;
    private int currentHP = 100;
    [SerializeField] private int maxTP = 25;
    private int currentTP = 25;

    [SerializeField] private int atk = 10;
    [SerializeField] private int def = 10;
    [SerializeField] private int spd = 10;
    
    // Stat Modifiers
    private float atkMod = 1;
    private float defMod = 1;
    private float spdMod = 1;

    // Modifier Turns
    private int atkModTurns = 0;
    private int defModTurns = 0;
    private float spdModTurns = 0;

    [Header("Level Gains")]
    [SerializeField] private int HPGain = 25;
    [SerializeField] private int TPGain = 25;

    [SerializeField] private int atkGain = 3;
    [SerializeField] private int defGain = 3;
    [SerializeField] private int spdGain = 3;

    [Header("Level and Experience")]
    [SerializeField] private int level = 1;
    [SerializeField] private int exp = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReduceHP(int i)
    {
        currentHP -= i;
    }

    // Returns the character's current HP
    public int HP()
    {
        return currentHP;
    }

    // Returns the character's current TP
    public int TP()
    {
        return currentTP;
    }

    // Returns the character's attack stat
    public int Attack()
    {
        int i = (int)(atk * atkMod);
        if (i <= 0)
            i = 1;
        return i;
    }

    // Returns the character's defense stat
    public int Defense()
    {
        int i = (int)(def * defMod);
        if (i <= 0)
            i = 1;
        return i;
    }

    // Returns the character's speed stat
    public int Speed()
    {
        int i = (int)(spd * spdMod);
        if (i <= 0)
            i = 1;
        return i;
    }

    // Ticks down the modifier turn count at the end of each turn
    public void TickModifierChanges()
    {
        // Ticks down attack changes
        if (atkModTurns > 0)
            atkModTurns--;
        else
            atkMod = 1;

        // Ticks down defense changes
        if (defModTurns > 0)
            defModTurns--;
        else
            defMod = 1;

        // Ticks down speed changes
        if (spdModTurns > 0)
            spdModTurns--;
        else
            spdMod = 1;
    }
}
