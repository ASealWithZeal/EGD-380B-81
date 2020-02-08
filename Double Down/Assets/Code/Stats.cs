using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Main Stats")]
    public int maxHP = 100;
    public int currentHP = 100;
    public int maxTP = 25;
    public int currentTP = 25;

    public int atk = 10;
    public int def = 10;
    public int spd = 10;
    
    // Stat Modifiers
    private float atkMod = 1;
    private float defMod = 1;
    private float spdMod = 1;

    // Modifier Turns
    private int atkModTurns = 0;
    private int defModTurns = 0;
    private float spdModTurns = 0;

    [Header("Level Gains")]
    public int HPGain = 25;
    public int TPGain = 25;

    public int atkGain = 3;
    public int defGain = 3;
    public int spdGain = 3;

    [Header("Level and Experience")]
    public int level = 1;
    public int exp = 0;
    public int nextLevelExp = 50;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        currentTP = maxTP;
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

    // Returns the character's current HP out of their Max HP
    public float HPPercent()
    {
        return ((float)currentHP / maxHP);
    }

    public float TPPercent()
    {
        return ((float)currentTP / maxTP);
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
        if (atkModTurns > 1)
            atkModTurns--;
        else
            atkMod = 1;

        // Ticks down defense changes
        if (defModTurns > 1)
            defModTurns--;
        else
            defMod = 1;

        // Ticks down speed changes
        if (spdModTurns > 1)
            spdModTurns--;
        else
            spdMod = 1;
    }

    public void SetAtkMod(float mod, int turns)
    {
        atkMod = mod;
        atkModTurns = turns;
    }

    public void SetDefMod(float mod, int turns)
    {
        defMod = mod;
        defModTurns = turns;
    }

    public void SetSpdMod(float mod, int turns)
    {
        spdMod = mod;
        spdModTurns = turns;
    }
}
