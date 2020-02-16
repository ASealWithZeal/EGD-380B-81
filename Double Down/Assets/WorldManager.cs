using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Sets up data for the combat and hub scenes
    void Init()
    {
        if (Managers.BattleDataScript.Instance != null)
            Managers.BattleDataScript.Instance.PopulateData();
    }
}
