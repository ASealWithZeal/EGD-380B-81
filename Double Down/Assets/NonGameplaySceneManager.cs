using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonGameplaySceneManager : MonoBehaviour
{
    public ScreenTransitionCanvas uiTransition = null;
    public Color fadeColor;
    public float fadeSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        if (Managers.BattleDataScript.Instance != null)
            Managers.BattleDataScript.Instance.DestroyObj();

        uiTransition.FadeIn(fadeColor, fadeSpeed);
    }


}
