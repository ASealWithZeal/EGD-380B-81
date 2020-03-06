using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneChangeManager : Singleton<SceneChangeManager>
    {
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void WinCombat(List<GameObject> players)
        {
            //BattleDataScript.Instance.SetMaxCharValues();
            CombatTransitionManager.Instance.DestroyCombatInstance(players);
        }

        public void LoseCombat()
        {
            Managers.BattleDataScript.Instance.SetMaxCharValues();
            ChangeScene("LoseScene");
        }
    }
}