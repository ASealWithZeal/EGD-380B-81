using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpContainer : MonoBehaviour
{
    public List<int> winExp = null;
    public int currentCombatInst = -1;

    public void SetCombatInst(int inst)
    {
        currentCombatInst = inst;
    }

    public void AddWinExp(int newExp)
    {
        winExp[currentCombatInst] += newExp;
    }

    public int GetWinExp()
    {
        return winExp[currentCombatInst];
    }

    public void FlushWinExp()
    {
        winExp[currentCombatInst] = 0;
    }
}
