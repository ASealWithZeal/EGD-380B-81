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

    [Header("Ability0 Upgrade")]
    public string ability0UpgradeName = "Overdraw";
    public string ability0UpgradeDescription = "Deal 1.35x damage, then restore 120% of that as HP split evenly between all allies.";
    public float ability0UpgradeMod = 1.35f;
    public int ability0UpgradeCost = 7;
    public Targeting ability0UpgradeTarget;
    public Targeting ability0UpgradeHealTarget;

    [Header("Ability1")]
    public string ability1Name = "Adrenalinjection";
    public string ability1Description = "Give an ally +25% ATK for 3 turns.";
    public int ability1Cost = 4;
    public float ability1Effect = 1.25f;
    public int ability1Duration = 3;
    public Targeting ability1Target;

    // Perform a simple attack
    public void SetAttack()
    {
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.SetUpAction(0);
    }
    public void PerformAttack()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.DealDamage(attackMod);
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
        Managers.CombatManager.Instance.DisplayAbilityName(defendName);
        Managers.CombatManager.Instance.UseStatusAbility(1, 2.0f, 1, true, 0.5f);
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
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);

        // Deals damage as absorption
        int damage = Managers.CombatManager.Instance.DealDamageWithAbsorb(attackMod);

        // Restores health to all allies
        Managers.CombatManager.Instance.SetTarget((int)ability0HealTarget);
        Managers.CombatManager.Instance.RestoreHealth(damage / Managers.TurnManager.Instance.playerCharsList.Count);
    }
    public void PerformAbility0Upgrade()
    {
        // Animation
        charStats.currentTP -= ability0UpgradeCost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0UpgradeName);

        // Deals damage as absorption
        int damage = Managers.CombatManager.Instance.DealDamageWithAbsorb(attackMod);

        // Restores health to all allies
        Managers.CombatManager.Instance.SetTarget((int)ability0UpgradeHealTarget);
        Managers.CombatManager.Instance.RestoreHealth((int)((damage * 1.2f) / Managers.TurnManager.Instance.playerCharsList.Count));
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
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);
        Managers.CombatManager.Instance.UseStatusAbility(0, ability1Effect, 3, true, 0.5f);
    }
}
