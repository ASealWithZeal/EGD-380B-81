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

    [Header("Stat Passives")]
    public float HPPassives = 0.0f;
    public float TPPassives = 0.0f;

    public float atkPassives = 0.0f;
    public float defPassives = 0.0f;
    public float spdPassives = 0.0f;

    [Header("Level and Experience")]
    public int level = 1;
    [HideInInspector] public int startingLevel = 1;
    public int exp = 0;
    [HideInInspector] public int startingExp = 0;
    public List<int> nextLevelExp = new List<int>();
    [HideInInspector] public int lastLevelUpEXP = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        currentTP = maxTP;
        lastLevelUpEXP = 0;
        startingExp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReduceHP(int i)
    {
        currentHP -= i;
        if (currentHP <= 0)
            currentHP = 0;
        else if (currentHP > maxHP)
            currentHP = maxHP;
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
        int i = (int)(atk * atkMod) + (int)(atk * atkPassives);
        if (i <= 0)
            i = 1;
        return i;
    }

    // Returns the character's defense stat
    public int Defense()
    {
        int i = (int)(def * defMod) + (int)(def * defPassives);
        if (i <= 0)
            i = 1;
        return i;
    }

    // Returns the character's speed stat
    public int Speed()
    {
        int i = (int)(spd * spdMod) + (int)(spd * spdPassives);
        if (i <= 0)
            i = 1;
        return i;
    }

    // Returns the expected speed for NEXT TURN
    public int NextSpeed(bool hasActed)
    {
        if (hasActed && spdModTurns - 1 < 0)
            return spd;
        else if (!hasActed && spdModTurns - 2 < 0)
            return spd;
        else
            return Speed();
    }

    // Ticks down the modifier turn count at the end of each turn
    public void TickModifierChanges()
    {
        // Ticks down attack changes
        atkModTurns--;
        if (atkModTurns <= 0)
        {
            atkMod = 1;
            atkModTurns = 0;
        }

        // Ticks down defense changes
        defModTurns--;
        if (defModTurns <= 0)
        {
            defMod = 1;
            defModTurns = 0;
        }

        // Ticks down speed changes
        spdModTurns--;
        if (spdModTurns <= 0)
        {
            spdMod = 1;
            spdModTurns = 0;
        }
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

    // Give a character experience and let them level up, if possible
    public void GainEXP(int winEXP)
    {
        startingLevel = level;
        startingExp = exp;
        exp += winEXP;
        if (level < nextLevelExp.Count + 1 && exp >= nextLevelExp[level - 1])
        {
            LevelUp(winEXP);
        }
    }

    public void LevelUp(int winEXP)
    {
        level++;
        exp = winEXP - nextLevelExp[level - 2];
        lastLevelUpEXP = nextLevelExp[level - 2];

        maxHP += HPGain;
        maxTP += TPGain;
        atk += atkGain;
        def += defGain;
        spd += spdGain;

        if (level < nextLevelExp.Count + 1 && exp > nextLevelExp[level - 1])
        {
            LevelUp(winEXP);
        }
    }

    public void CopyStats(Stats s)
    {
        // Consumable stats
        maxHP = s.maxHP;
        currentHP = s.currentHP;
        maxTP = s.maxTP;
        currentTP = s.currentTP;

        // Non-consumable stats
        atk = s.atk;
        def = s.def;
        spd = s.spd;

        // Stat Modifiers
        atkMod = s.atkMod;
        defMod = s.defMod;
        spdMod = s.spdMod;

        // Modifier Turns
        atkModTurns = s.atkModTurns;
        defModTurns = s.defModTurns;
        spdModTurns = s.spdModTurns;

        // Level Up Gains
        HPGain = s.HPGain;
        TPGain = s.TPGain;
        atkGain = s.atkGain;
        defGain = s.defGain;
        spdGain = s.spdGain;

        // Levels and EXP Data
        level = s.level;
        startingLevel = s.startingLevel;
        exp = s.exp;
        startingExp = s.startingExp;
        nextLevelExp.Clear();
        for (int i = 0; i < s.nextLevelExp.Count; ++i)
            nextLevelExp.Add(s.nextLevelExp[i]);
        lastLevelUpEXP = s.lastLevelUpEXP;
    }
}
