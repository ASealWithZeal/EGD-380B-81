using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public Stats charStats = null;

    [Header("Attack")]
    public string attackName = "Attack";
    public float attackMod = 1.0f;
    public Targeting attackTarget;
    public GameObject attackAnim;

    [Header("Defend")]
    public string defendName = "Defend";
    public float defendMod = 0.0f;
    public Targeting defendTarget;

    [Header("Ability0")]
    public string ability0Name = "Blood Draw";
    public string ability0Description = "Deal 1.0x damage, then restore 100% of that as HP split evenly between all allies.";
    public float ability0Mod = 1.0f;
    public int ability0Cost = 5;
    public Targeting ability0Target;
    public Targeting ability0HealTarget;
    public GameObject ability0DamageAnim;
    public GameObject ability0HealingAnim;

    [Header("Ability0 Upgrade")]
    public string ability0UpgradeName = "Overdraw";
    public bool ability0Active = true;
    public string ability0UpgradeDescription = "Deal 1.35x damage, then restore 120% of that as HP split evenly between all allies.";
    public float ability0UpgradeMod = 1.35f;
    public int ability0UpgradeCost = 7;
    public Targeting ability0UpgradeTarget;
    public Targeting ability0UpgradeHealTarget;
    public GameObject ability0UpgradeDamageAnim;
    public GameObject ability0UpgradeHealingAnim;

    [Header("Ability1")]
    public string ability1Name = "Injection";
    public bool ability1Active = true;
    public string ability1Description = "Give an ally +25% ATK for 3 turns.";
    public int ability1Cost = 4;
    public float ability1Effect = 1.25f;
    public int ability1Duration = 3;
    public Targeting ability1Target;
    public GameObject ability1Animation;
    public GameObject ability1HealAnimation;

    [Header("PassiveAbility1")]
    public string ability2Name = "TP +25%";
    public bool ability2Active = false;
    public string ability2Description = "User's TP permanently increases by 25%.";
    public GameObject ability2Animation;

    [Header("PassiveAbility2")]
    public string ability3Name = "Desperation";
    public bool ability3Active = false;
    public MainStats ability3Recipient = MainStats.ATK;
    public int ability3RecipientGains = 1;
    public MainStats ability3Giver = MainStats.TP;
    public int ability3GiverGives = 10;
    public string ability3Description = "User gains +1 ATK for every 10 TP currently consumed (<i>only active during combat</i>).";

    // Perform a simple attack
    public void SetAttack()
    {
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.SetUpAction(0);
    }
    public void PerformAttack()
    {
        // Animation
        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.DealDamage(attackAnim, attackMod);
    }

    // Guards for a turn, raising defense
    public void SetDefend()
    {
        Managers.CombatManager.Instance.SetTarget((int)defendTarget);
        Managers.CombatManager.Instance.SetUpAction(1);
    }
    public void PerformDefend()
    {
        // Animation
        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(defendName);

        List<int> type = new List<int>();
        type.Add(4);
        List<float> mod = new List<float>();
        mod.Add(0);
        List<int> length = new List<int>();
        length.Add(0);

        if (!GetComponent<CharData>().learnedAbilities[2])
            Managers.CombatManager.Instance.UseStatusAbility(null, type, mod, length, true, 0.5f);
        else
        {
            Managers.CombatManager.Instance.UseStatusAbility(null, type, mod, length, false, 0.5f);
            Managers.CombatManager.Instance.RestoreTechPoints(ability2Animation, (int)(charStats.MaxTP() * 0.25f), new Vector3());

            if (GetComponent<CharData>().learnedAbilities[3])
                Invoke("SetParticlesFromRefresh", ability2Animation.GetComponent<Animation>().clip.length + 0.01f);
        }
    }
    private void SetParticlesFromRefresh()
    {
        GetComponent<CharData>().ChangeParticleSize(0.1f * Mathf.FloorToInt((charStats.MaxTP() - charStats.currentTP) / 5));
    }

    // Uses Blood Draw
    public void SetAbility0()
    {
        Managers.CombatManager.Instance.SetTarget((int)ability0Target);
        Managers.CombatManager.Instance.SetUpAction(2);
    }
    public void PerformAbility0()
    {
        // Animation
        charStats.currentTP -= ability0Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        if (GetComponent<CharData>().learnedAbilities[3])
            GetComponent<CharData>().ChangeParticleSize(0.1f * Mathf.FloorToInt((charStats.MaxTP() - charStats.currentTP) / 5));
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);

        // Deals damage as absorption
        GetComponent<CharData>().DisplayActionAnimation();
        int damage = Managers.CombatManager.Instance.DealDamageWithAbsorb(ability0DamageAnim, attackMod, true, new Vector3());

        // Restores health to all allies
        Managers.CombatManager.Instance.SetTarget((int)ability0HealTarget);
        //Managers.CombatManager.Instance.RestoreHealth(ability0HealingAnim, damage / Managers.TurnManager.Instance.playerCharsList.Count, true, new Vector3());
        StartCoroutine(PerformAbility0Heal(damage, 1.0f, ability0HealingAnim, ability0DamageAnim.GetComponent<Animation>().clip.length));
    }
    public void PerformAbility0Upgrade()
    {
        // Animation
        charStats.currentTP -= ability0UpgradeCost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        if (GetComponent<CharData>().learnedAbilities[3])
            GetComponent<CharData>().ChangeParticleSize(0.1f * Mathf.FloorToInt((charStats.MaxTP() - charStats.currentTP) / 5));
        Managers.CombatManager.Instance.DisplayAbilityName(ability0UpgradeName);

        // Deals damage as absorption
        GetComponent<CharData>().DisplayActionAnimation();
        int damage = Managers.CombatManager.Instance.DealDamageWithAbsorb(ability0UpgradeDamageAnim, attackMod, true, new Vector3());

        // Restores health to all allies
        Managers.CombatManager.Instance.SetTarget((int)ability0UpgradeHealTarget);
        //Managers.CombatManager.Instance.RestoreHealth(ability0UpgradeHealingAnim, (int)((damage * 1.2f) / Managers.TurnManager.Instance.playerCharsList.Count), true, new Vector3());
        StartCoroutine(PerformAbility0Heal(damage, 1.2f, ability0UpgradeHealingAnim, ability0UpgradeDamageAnim.GetComponent<Animation>().clip.length));
    }
    IEnumerator PerformAbility0Heal(int healAmount, float healMod, GameObject anim, float length)
    {
        yield return new WaitForSeconds(length * 0.75f);
        Managers.CombatManager.Instance.RestoreHealth(anim, (int)((healAmount * healMod) / Managers.TurnManager.Instance.playerCharsList.Count), true, new Vector3());
    }

    // Uses Adrenalinjection
    public void SetAbility1()
    {
        Managers.CombatManager.Instance.SetTarget((int)ability1Target);
        Managers.CombatManager.Instance.SetUpAction(3);
    }
    public void PerformAbility1()
    {
        // Animation
        charStats.currentTP -= ability1Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        if (GetComponent<CharData>().learnedAbilities[3])
            GetComponent<CharData>().ChangeParticleSize(0.1f * Mathf.FloorToInt((charStats.MaxTP() - charStats.currentTP) / 5));

        GetComponent<CharData>().DisplayActionAnimation();
        Managers.CombatManager.Instance.DisplayAbilityName(ability1Name);
        //Managers.CombatManager.Instance.UseStatusAbility(0, ability1Effect, 3, true, 0.5f);

        // Deals damage as absorption
        Vector2Int damage = Managers.CombatManager.Instance.DamageUserAsAbsorb(ability1Animation, charStats.currentHP / 2, charStats.currentTP / 2);
        //int tpDamage = Managers.CombatManager.Instance.DamageUserAsAbsorb(ability1Animation, charStats.currentTP / 2, false, new Vector3(0.2f, -0.33f, 0));

        // Restores health to the target ally
        //Managers.CombatManager.Instance.RestoreHealth(null, damage, false, new Vector3(-0.2f, 0.33f, 0));
        //Managers.CombatManager.Instance.RestoreTechPoints(ability2Animation, tpDamage, new Vector3(0.2f, -0.33f, 0));
        //Managers.CombatManager.Instance.RestoreHybrid(ability1Animation, damage.x, damage.y);

        StartCoroutine(PerformAbility1Heal(damage.x, damage.y, ability1HealAnimation, ability1Animation.GetComponent<Animation>().clip.length));
    }
    IEnumerator PerformAbility1Heal(int hpHealAmount, int tpHealAmount, GameObject anim, float length)
    {
        yield return new WaitForSeconds(length * 0.75f);
        Managers.CombatManager.Instance.RestoreHybrid(anim, hpHealAmount, tpHealAmount);
    }

    public void GetPassiveAbility1()
    {
        //charStats.GetPassiveAbilityStats(MainStats.TP, 0.25f);
    }

    public void GetPassiveAbility2()
    {
        charStats.GetPassiveAbilityStats(ability3Recipient, ability3Giver, ability3RecipientGains, ability3GiverGives);
    }
}
