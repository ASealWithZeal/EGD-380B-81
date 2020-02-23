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

        public void EndCombat()
        {
            Managers.BattleDataScript.Instance.SetMaxCharValues();
            ChangeScene("BattleSelectScene");
        }

        public void LoseCombat()
        {
            Managers.BattleDataScript.Instance.SetMaxCharValues();
            ChangeScene("LoseScene");
        }
    }
}