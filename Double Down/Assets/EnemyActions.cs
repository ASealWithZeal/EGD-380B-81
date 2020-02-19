using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy
{
    Enem0 = 0,
    Enem1 = 1
}

public class EnemyActions : MonoBehaviour
{
    public Enemy enemy;

    // Checks the character's action based on an input value
    public void PerformAction()
    {
        if (enemy == Enemy.Enem0)
            GetComponent<NormalEnemy>().Act();
        else if (enemy == Enemy.Enem1)
            GetComponent<BossEnemy>().Act();
    }
}
