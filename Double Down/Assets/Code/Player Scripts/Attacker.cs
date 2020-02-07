using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public Stats charStats = null;
    public float attackMod = 1.0f;

    // Perform a simple attack
    public void Attack()
    {
        // Animation
        Managers.CombatManager.Instance.DealDamage(attackMod);
    }

    // Guards for a turn, raising defense
    public void Defend()
    {
        // Animation
        charStats.SetDefMod(1.5f, 1);
        Managers.CombatManager.Instance.FollowUpAction();
    }
}
