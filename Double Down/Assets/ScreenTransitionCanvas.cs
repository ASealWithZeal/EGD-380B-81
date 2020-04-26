using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitionCanvas : MonoBehaviour
{
    public GameObject wipeImage = null;
    public bool init = false;

    // ENTER SCENE FUNCTION
    public void EnterScene(SceneType type)
    {
        wipeImage.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        wipeImage.transform.localPosition = new Vector3(0, 0, 0);
        StartCoroutine(EnterSceneWipe(type));
    }
    IEnumerator EnterSceneWipe(SceneType type)
    {
        Debug.Log(type.ToString());
        GameObject temp = null;
        if (init && Managers.TurnManager.Instance.t1.Count > 0)
            temp = Managers.TurnManager.Instance.t1[0];

        // Resets the turn order
        //if (type == SceneType.Hub)
        //    Managers.TurnManager.Instance.FillTurns();
        if (type == SceneType.Combat)
        {
            Managers.TurnManager.Instance.PrepCombatTurns();

            // While the trackers are not fully sorted, stall the program
            while (!Managers.TurnManager.Instance.tracker.listSorted)
                yield return new WaitForSeconds(0.0125f);
            Managers.TurnManager.Instance.tracker.listSorted = false;
        }

        else if (type == SceneType.Hub && init)
        {
            Managers.TurnManager.Instance.PrepNonCombatTurns();
            
            for (int i = 0; i < Managers.TurnManager.Instance.playerCharsList.Count; ++i)
                Managers.TurnManager.Instance.playerCharsList[i].GetComponent<CharData>().CheckDeath();

            // While the trackers are not fully sorted, stall the program
            while (!Managers.TurnManager.Instance.tracker.listSorted)
                yield return new WaitForSeconds(0.0125f);
            Managers.TurnManager.Instance.tracker.listSorted = false;
        }

        while (wipeImage.transform.localPosition.x < 1000)
        {
            wipeImage.transform.localPosition += new Vector3(25, 0, 0);
            yield return new WaitForSeconds(0.0125f);
        }

        if (init)
        {
            if (Managers.TurnManager.Instance.firstT1Char.GetComponent<CharData>().isInCombat && !Managers.TurnManager.Instance.firstT1Char.GetComponent<CharData>().hasActed)
                Managers.CombatManager.Instance.StartCombat();
            else
                Managers.TurnManager.Instance.EndRound();
        }
        else
        {
            init = true;
            Managers.TurnManager.Instance.Init();
        }
        yield return null;
    }

    // EXIT SCENE FUNCTION
    public void ExitScene(string scene)
    {
        wipeImage.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        wipeImage.transform.localPosition = new Vector3(-1000, 0, 0);
        StartCoroutine(ExitSceneWipe(scene));
    }
    IEnumerator ExitSceneWipe(string scene)
    {
        yield return new WaitForSeconds(0.1f);
        
        if (scene != "Hub")
            Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.ScreenTransition, 0.1f);
        else
            Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.ScreenTransition2, 0.1f);
        while (wipeImage.transform.localPosition.x < 0)
        {
            wipeImage.transform.localPosition += new Vector3(25, 0, 0);
            yield return new WaitForSeconds(0.0125f);
        }

        Managers.SceneChangeManager.Instance.ChangeScene(scene);
        yield return null;
    }

    public void BlackOut(string scene, Color fadeColor, float fadeSpeed)
    {
        wipeImage.GetComponent<Image>().color = (fadeColor - new Color(0, 0, 0, 1));
        wipeImage.transform.localPosition = new Vector3(0, 0, 0);

        StartCoroutine(ExitSceneBlackOut(scene, fadeSpeed));
    }
    IEnumerator ExitSceneBlackOut(string scene, float speed)
    {
        yield return new WaitForSeconds(0.1f);
        while (wipeImage.GetComponent<Image>().color.a < 1)
        {
            wipeImage.GetComponent<Image>().color += new Color(0, 0, 0, speed);
            yield return new WaitForSeconds(0.0125f);
        }

        Managers.SceneChangeManager.Instance.ChangeScene(scene);
        yield return null;
    }

    public void FadeIn(Color fadeColor, float fadeSpeed)
    {
        wipeImage.GetComponent<Image>().color = fadeColor;
        wipeImage.transform.localPosition = new Vector3(0, 0, 0);

        StartCoroutine(EnterSceneFadeIn(fadeSpeed));
    }
    IEnumerator EnterSceneFadeIn(float speed)
    {
        yield return new WaitForSeconds(0.1f);
        while (wipeImage.GetComponent<Image>().color.a > 0)
        {
            wipeImage.GetComponent<Image>().color -= new Color(0, 0, 0, speed);
            yield return new WaitForSeconds(0.0125f);
        }
        
        yield return null;
    }
}
