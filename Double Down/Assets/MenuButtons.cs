using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    private enum Menu
    {
        Start = 0,
        HowTo,
        Quit,
        Menu
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
    }
}
