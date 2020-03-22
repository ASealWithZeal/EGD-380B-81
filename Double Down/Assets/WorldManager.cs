using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType
{
    Hub = 0,
    Combat
}

public class WorldManager : MonoBehaviour
{
    public SceneType scene;

    // Start is called before the first frame update
    void Start()
    {
        Managers.SceneChangeManager.Instance.type = scene;
        Init();
    }

    // Sets up data for the combat and hub scenes
    void Init()
    {
        //if (Managers.BattleDataScript.Instance != null)
        //    Managers.BattleDataScript.Instance.PopulateData();

        if (scene == SceneType.Hub)
            Managers.CombatTransitionManager.Instance.RetrieveCharacterHubPositions();
        else if (scene == SceneType.Combat)
            Managers.CombatTransitionManager.Instance.RetrieveCharacterCombatPositions();

        GameObject.Find("ScreenTransitionObject").GetComponent<ScreenTransitionCanvas>().EnterScene(scene);
    }
}
