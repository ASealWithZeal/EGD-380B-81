using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainStats
{
    HP = 0,
    TP,
    ConsumedHP,
    ConsumedTP,
    ATK,
    DEF,
    SPD
}

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
    [HideInInspector] public float atkMod = 1;
    [HideInInspector] public float defMod = 1;
    [HideInInspector] public float spdMod = 1;

    // Modifier Turns
    [HideInInspector] public int atkModTurns = 0;
    [HideInInspector] public int defModTurns = 0;
    [HideInInspector] public int spdModTurns = 0;
    [HideInInspector] public int aggroModTurns = 0;
    [HideInInspector] public bool defending = false;

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

    [Header("Secondary Stat Passives")]
    public int atkBonus = 0;
    public int defBonus = 0;

    [Header("Other")]
    public float aggro = 1.0f;

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
        currentHP = MaxHP();
        currentTP = MaxTP();
        lastLevelUpEXP = 0;
        startingExp = 0;
        startingLevel = level;
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
        else if (currentHP > MaxHP())
            currentHP = MaxHP();
    }

    public void ReduceTP(int i)
    {
        currentTP -= i;
        if (currentTP <= 0)
            currentTP = 0;
        else if (currentTP > MaxTP())
            currentTP = MaxTP();
    }

    public int MaxHP()
    {
        return maxHP + (int)(maxHP * HPPassives);
    }

    // Returns the character's current HP
    public int HP()
    {
        return currentHP;
    }

    // Returns the character's current HP out of their Max HP
    public float HPPercent()
    {
        return ((float)currentHP / MaxHP());
    }

    public float TPPercent()
    {
        return ((float)currentTP / MaxTP());
    }

    public int MaxTP()
    {
        return maxTP + (int)(maxTP * TPPassives);
    }

    // Returns the character's current TP
    public int TP()
    {
        return currentTP;
    }

    // Returns the character's attack stat
    public int Attack()
    {
        int i = (int)(atk * atkMod) + (int)(atk * atkPassives) + atkBonus;
        if (i <= 0)
            i = 1;
        return i;
    }

    // Returns the character's defense stat
    public int Defense()
    {
        int i = (int)(def * defMod) + (int)(def * defPassives) + defBonus;
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
            return spd + (int)(spd * spdPassives);
        else if (!hasActed && spdModTurns - 2 < 0)
            return spd + (int)(spd * spdPassives);
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

        // Ticks down aggro changes
        aggroModTurns--;
        if (aggroModTurns <= 0)
        {
            aggro = 1;
            aggroModTurns = 0;
        }
    }

    public void DestroyMods()
    {
        atkMod = defMod = spdMod = aggro = 1.0f;
        atkModTurns = defModTurns = spdModTurns = aggroModTurns = 0;
    }

    public void SetAtkMod(float mod, int turns)
    {
        atkMod += mod;

        if (atkMod >= 1.5f)
            atkMod = 1.5f;
        else if (atkMod <= 0.5f)
            atkMod = 0.5f;
        else if (atkMod == 1.0f)
            turns = 0;

        atkModTurns = turns;
    }

    public void SetDefMod(float mod, int turns)
    {
        defMod += mod;

        if (defMod >= 1.5f)
            defMod = 1.5f;
        else if (defMod <= 0.5f)
            defMod = 0.5f;
        else if (defMod == 1.0f)
            turns = 0;

        defModTurns = turns;
    }

    public void SetSpdMod(float mod, int turns)
    {
        spdMod += mod;

        if (spdMod >= 1.5f)
            spdMod = 1.5f;
        else if (spdMod <= 0.5f)
            spdMod = 0.5f;
        else if (spdMod == 1.0f)
            turns = 0;

        spdModTurns = turns;
    }

    public void SetAggro(float mod, int turns)
    {
        aggro = mod;
        aggroModTurns = turns;
    }

    // Give a character experience and let them level up, if possible
    public void GainEXP(int winEXP)
    {
        startingLevel = level;
        startingExp = exp;
        exp += winEXP;
        if (level < nextLevelExp.Count + 1 && exp >= nextLevelExp[level - 1])
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        exp = Mathf.Abs(exp - nextLevelExp[level - 2]);
        lastLevelUpEXP = nextLevelExp[level - 2];

        maxHP += HPGain;
        maxTP += TPGain;
        atk += atkGain;
        def += defGain;
        spd += spdGain;

        if (level < nextLevelExp.Count + 1 && exp > nextLevelExp[level - 1])
        {
            LevelUp();
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
        //atkMod = s.atkMod;
        //defMod = s.defMod;
        //spdMod = s.spdMod;
        //
        // Modifier Turns
        //atkModTurns = s.atkModTurns;
        //defModTurns = s.defModTurns;
        //spdModTurns = s.spdModTurns;

        // Level Up Gains
        HPGain = s.HPGain;
        TPGain = s.TPGain;
        atkGain = s.atkGain;
        defGain = s.defGain;
        spdGain = s.spdGain;

        HPPassives = s.HPPassives;
        TPPassives = s.TPPassives;
        atkPassives = s.atkPassives;
        defPassives = s.defPassives;
        spdPassives = s.spdPassives;

        // Levels and EXP Data
        level = s.level;
        exp = s.exp;
        startingExp = s.startingExp;
        nextLevelExp.Clear();
        for (int i = 0; i < s.nextLevelExp.Count; ++i)
            nextLevelExp.Add(s.nextLevelExp[i]);
        lastLevelUpEXP = s.lastLevelUpEXP;
    }

    public void GetPassiveAbilityStats(MainStats recipient, float additiveValue)
    {
        bool @bool = false;
        switch (recipient)
        {
            case MainStats.HP:
                if (currentHP == MaxHP())
                    @bool = true;
                HPPassives += additiveValue;
                if (@bool)
                    currentHP = MaxHP();
                break;

            case MainStats.TP:
                if (currentTP == MaxTP())
                    @bool = true;
                TPPassives += additiveValue;
                if (@bool)
                    currentTP = MaxTP();
                break;

            case MainStats.ATK:
                atkPassives += additiveValue;
                break;

            case MainStats.DEF:
                defPassives += additiveValue;
                break;

            case MainStats.SPD:
                spdPassives += additiveValue;
                break;
        }
    }

    // Acquires stat bonuses earned from passive abilities
    public void GetPassiveAbilityStats(MainStats recipient, MainStats giver, int receivedAmount, int givenAmount)
    {
        switch (recipient)
        {
            case MainStats.ATK:
                atkBonus += CheckStatBonus(giver, receivedAmount, givenAmount);
                break;
            case MainStats.DEF:
                defBonus += CheckStatBonus(giver, receivedAmount, givenAmount);
                break;
            case MainStats.SPD:
                break;
        }
    }

    public int CheckStatBonus(MainStats giver, int receivedAmount, int givenAmount)
    {
        switch (giver)
        {
            case MainStats.ConsumedHP:
                return (receivedAmount * (((maxHP + (int)(maxHP * HPPassives)) - currentHP) / givenAmount));
            case MainStats.ConsumedTP:
                return (receivedAmount * (((maxTP + (int)(maxTP * TPPassives)) - currentTP) / givenAmount));
            case MainStats.ATK:
                return (receivedAmount * ((atk + (int)(atk * atkPassives)) / givenAmount));
            case MainStats.DEF:
                return (receivedAmount * ((def + (int)(def * defPassives)) / givenAmount));
            case MainStats.SPD:
                return (receivedAmount * ((spd + (int)(spd * spdPassives)) / givenAmount));
            default:
                return 0;
        }
    }

    public void ResetBonusStats()
    {
        atkBonus = 0;
        defBonus = 0;

        HPPassives = 0;
        TPPassives = 0;
        atkPassives = 0;
        defPassives = 0;
        spdPassives = 0;
    }
}
