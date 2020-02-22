using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    private enum Menu
    {
        Start = 0,
        HowTo,
        Quit,
        Menu,

        Battle1,
        Battle2,
        Battle3,

        PreCombatScene
    }

    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        if (startButton != null)
            startButton.Select();
    }

    public void GetButton(int i)
    {
        switch (i)
        {
            case (int)Menu.Start:
                Managers.SceneChangeManager.Instance.ChangeScene("CombatScene");
                break;
            case (int)Menu.HowTo:
                Managers.SceneChangeManager.Instance.ChangeScene("HowToScene");
                break;
            case (int)Menu.Quit:
                Application.Quit();
                break;
            case (int)Menu.Menu:
                Managers.SceneChangeManager.Instance.ChangeScene("Start");
                break;


            case (int)Menu.PreCombatScene:
                Managers.SceneChangeManager.Instance.ChangeScene("BattleSelectScene");
                break;
            case (int)Menu.Battle1:
                Managers.SceneChangeManager.Instance.ChangeScene("CombatSceneTEMP2");
                break;
            case (int)Menu.Battle2:
                Managers.SceneChangeManager.Instance.ChangeScene("CombatSceneTEMP1");
                break;
            case (int)Menu.Battle3:
                Managers.SceneChangeManager.Instance.ChangeScene("CombatScene");
                break;
        }
    }
}
