using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Managers
{
    public class MenuButtons : Singleton<MenuButtons>
    {
        private enum Menu
        {
            Start = 0,
            HowTo,
            Quit,
            Menu,
            Controls,

            Hub
        }

        public Button startButton;
        public GameObject lastselect;
        public GameObject[] images;
        private int index = 0;

        // Start is called before the first frame update
        void Start()
        {
            lastselect = new GameObject();
            if (Cursor.visible)
                Cursor.visible = false;

            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;

            if (startButton != null)
                startButton.Select();
        }

        // Update is called once per frame
        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null && Input.GetMouseButtonDown(0))
            {
                EventSystem.current.SetSelectedGameObject(lastselect);
            }
            else
            {
                lastselect = EventSystem.current.currentSelectedGameObject;
            }
        }

        public void GetButton(int i)
        {
            switch (i)
            {
                case (int)Menu.Start:
                    Managers.SceneChangeManager.Instance.ChangeSceneWithFade("Hub");
                    break;
                case (int)Menu.HowTo:
                    Managers.SceneChangeManager.Instance.ChangeSceneWithFade("ControlsScene");
                    break;
                case (int)Menu.Quit:
                    Invoke("QuitButton", 0.1f);
                    break;
                case (int)Menu.Menu:
                    Managers.SceneChangeManager.Instance.ChangeSceneWithFade("Start");
                    break;
                case (int)Menu.Controls:
                    SwapImages(0);
                    break;


                    //case (int)Menu.PreCombatScene:
                    //    Managers.SceneChangeManager.Instance.ChangeScene("BattleSelectScene");
                    //    break;
                    //case (int)Menu.Battle1:
                    //    Managers.SceneChangeManager.Instance.ChangeScene("CombatSceneTEMP2");
                    //    break;
                    //case (int)Menu.Battle2:
                    //    Managers.SceneChangeManager.Instance.ChangeScene("CombatSceneTEMP1");
                    //    break;
                    //case (int)Menu.Battle3:
                    //    Managers.SceneChangeManager.Instance.ChangeScene("CombatScene");
                    //    break;
            }
        }

        private void QuitButton()
        {
            Application.Quit();
        }

        // Simple image swap
        public void SwapImages(int num)
        {
            //index++;
            //if (index >= images.Length)
            //    index = 0;

            for (int i = 0; i < images.Length; ++i)
            {
                if (i == num)
                    images[i].SetActive(true);
                else
                    images[i].SetActive(false);
            }
        }
    }
}