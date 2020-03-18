using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionCanvas : MonoBehaviour
{
    public GameObject wipeImage = null;
    public bool init = false;

    // ENTER SCENE FUNCTION
    public void EnterScene(SceneType type)
    {
        StartCoroutine(EnterSceneWipe(type));
    }
    IEnumerator EnterSceneWipe(SceneType type)
    {
        Debug.Log(type.ToString());
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

        //if (type == SceneType.Combat)
        //    Managers.TurnManager.Instance.FillCombatTurns();

        if (init)
        {
            Debug.Log(Managers.TurnManager.Instance.t1.Count);
            if (Managers.TurnManager.Instance.t1[0].GetComponent<CharData>().isInCombat && !Managers.TurnManager.Instance.t1[0].GetComponent<CharData>().hasActed)
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
        wipeImage.transform.localPosition = new Vector3(-1000, 0, 0);
        StartCoroutine(ExitSceneWipe(scene));
    }
    IEnumerator ExitSceneWipe(string scene)
    {
        yield return new WaitForSeconds(0.1f);
        while (wipeImage.transform.localPosition.x < 0)
        {
            wipeImage.transform.localPosition += new Vector3(25, 0, 0);
            yield return new WaitForSeconds(0.0125f);
        }

        Managers.SceneChangeManager.Instance.ChangeScene(scene);
        yield return null;
    }
}
