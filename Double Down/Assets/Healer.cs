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
    public float ability0Mod = 1.0f;
    public int ability0Cost = 5;
    public Targeting ability0Target;
    public Targeting ability0HealTarget;

    // Perform a simple attack
    public void Attack()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(attackName);
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        Managers.CombatManager.Instance.DealDamage(attackMod);
    }

    // Guards for a turn, raising defense
    public void Defend()
    {
        // Animation
        Managers.CombatManager.Instance.DisplayAbilityName(defendName);
        Managers.CombatManager.Instance.SetTarget((int)defendTarget);
        Managers.CombatManager.Instance.UseStatusAbility(1, 2.0f, 1, true);
    }

    // Uses Blood Draw
    public void Ability0()
    {
        // Animation
        charStats.currentTP -= ability0Cost;
        charStats.gameObject.GetComponent<CharData>().ChangeTP();
        Managers.CombatManager.Instance.DisplayAbilityName(ability0Name);

        // Deals damage to one enemy
        Managers.CombatManager.Instance.SetTarget((int)attackTarget);
        int damage = Managers.CombatManager.Instance.DealDamageWithAbsorb(attackMod);

        // Restores health to all allies
        Managers.CombatManager.Instance.SetTarget((int)ability0HealTarget);
        Managers.CombatManager.Instance.RestoreHealth(damage / Managers.TurnManager.Instance.playerCharsList.Count);
    }
}
