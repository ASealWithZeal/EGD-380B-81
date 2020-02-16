using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneChangeManager : Singleton<SceneChangeManager>
    {
        // Start is called before the first frame update
        void Awake()
        {
            GameObject obj = GameObject.Find("SceneChangeManager");

            if (obj == null)
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void EndCombat()
        {
            Managers.BattleDataScript.Instance.SetMaxCharValues();
            ChangeScene("WinScene");
        }
    }
}