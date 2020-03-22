using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneChangeManager : Singleton<SceneChangeManager>
    {
        public ScreenTransitionCanvas uiTransition = null;
        public SceneType type;

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ChangeSceneWithFade(string sceneName)
        {
            uiTransition.BlackOut(sceneName, Color.black, 0.05f);
        }

        public void WinCombat(List<GameObject> players)
        {
            //BattleDataScript.Instance.SetMaxCharValues();
            CombatTransitionManager.Instance.DestroyCombatInstance(players);
        }

        public void LoseCombat()
        {
            //BattleDataScript.Instance.SetMaxCharValues();
            uiTransition.BlackOut("LoseScene", Color.black, 0.01f);
        }

        public void WinGameplayInstance()
        {
            uiTransition.BlackOut("WinScene", Color.white, 0.01f);
        }
    }
}