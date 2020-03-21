using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTracker : MonoBehaviour
{
    public Transform t1Storage;
    public Transform t2Storage;
    public List<Image> t1Tracker;
    public List<Image> t2Tracker;
    //[HideInInspector]
    public List<GameObject> t1;
    //[HideInInspector]
    public List<GameObject> t2;
    public GameObject image;

    public float seconds = 3.0f;
    public float timeIncrements = 0.005f;
    public bool listSorted = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetUpTrackers(List<GameObject> l, int j, bool addNull)
    {
        if (addNull)
            t1.Add(null);

        if (j == 1)
            StartCoroutine(CreateT1TrackerUI(l, true));

        else if (j == 2)
            StartCoroutine(CreateT2TrackerUI(l, true));
    }

    public void HighlightSelectedTrackers(GameObject obj)
    {
        for (int i = 0; i < t1.Count; ++i)
            if (t1[i].GetComponent<TurnTrackerObj>().obj == obj)
                t1[i].GetComponent<TurnTrackerObj>().Select();
        for (int i = 0; i < t2.Count; ++i)
            if (t2[i].GetComponent<TurnTrackerObj>().obj == obj)
                t2[i].GetComponent<TurnTrackerObj>().Select();
    }

    public void ResetTrackers()
    {
        //int c = t1.Count;
        //if (l1.Count > 1 && l1[0] != null)
        //{
        //    for (int i = 0; i < c; ++i)
        //    {
        //        if (t1[0] != null)
        //            Destroy(t1[0]);
        //        t1.Remove(t1[0]);
        //    }
        //    SetUpTrackers(l1, 1, true);
        //}
        //
        //c = t2.Count;
        //for (int i = 0; i < c; ++i)
        //{
        //    if (t2[0] != null)
        //        Destroy(t2[0]);
        //    t2.Remove(t2[0]);
        //}
        
        DestroyEmptyTrackers(true);

        //SetUpTrackers(l2, 2, false);
    }

    public void DestroyEmptyTrackers(bool end)
    {
        // Destroy all T1 trackers
        for (int i = 0; i < t1.Count; ++i)
        {
            if (t1[i].GetComponent<TurnTrackerObj>().objData.dead)
                StartCoroutine(DestroyTracker(t1[i]));
        }

        // Destroy all T2 trackers
        for (int i = 0; i < t2.Count; ++i)
        {
            if (t2[i].GetComponent<TurnTrackerObj>().objData.dead)
                StartCoroutine(DestroyTracker(t2[i]));
        }

        StartCoroutine(ShiftNonDestroyedTrackers(end));
    }

    public void DestroyNonCombatTrackers(bool end, CharData objData)
    {
        // Destroy all T1 trackers
        for (int i = 0; i < t1.Count; ++i)
        {
            if (!t1[i].GetComponent<TurnTrackerObj>().objData.isInCombat || objData.combatInst != t1[i].GetComponent<TurnTrackerObj>().objData.combatInst)
                StartCoroutine(DestroyTracker(t1[i]));
        }
        
        // Destroy all T2 trackers
        for (int i = 0; i < t2.Count; ++i)
        {
            if (!t2[i].GetComponent<TurnTrackerObj>().objData.isInCombat || objData.combatInst != t2[i].GetComponent<TurnTrackerObj>().objData.combatInst)
                StartCoroutine(DestroyTracker(t2[i]));
        }

        StartCoroutine(ShiftNonDestroyedTrackers(end));
    }

    public void DestroyNonPlayerTrackers(bool end)
    {
        // Destroy all T1 trackers
        for (int i = 0; i < t1.Count; ++i)
        {
            if (t1[i].GetComponent<TurnTrackerObj>().obj == null || t1[i].GetComponent<TurnTrackerObj>().obj.tag != "Player")
                StartCoroutine(DestroyTracker(t1[i]));
        }

        // Destroy all T2 trackers
        for (int i = 0; i < t2.Count; ++i)
        {
            if (t2[i].GetComponent<TurnTrackerObj>().obj == null || t2[i].GetComponent<TurnTrackerObj>().obj.tag != "Player")
                StartCoroutine(DestroyTracker(t2[i]));
        }

        StartCoroutine(ShiftNonDestroyedTrackers(end));
    }

    public void ReorderTrackers(bool wait)
    {
        bool sorted = false;
        GameObject temp;
        Vector3 origPos = new Vector3();
        Vector3 origScale = new Vector3();
        while (!sorted)
        {
            sorted = true;

            for (int i = 0; i < t1.Count; ++i)
            {
                if (t1[0] != null && t1.Count > 0 && i > 0 && t1[i].GetComponent<TurnTrackerObj>().objData.t1Pos < t1[i - 1].GetComponent<TurnTrackerObj>().objData.t1Pos)
                {
                    sorted = false;

                    origPos = t1[i].transform.position;
                    origScale = t1[i].transform.localScale;
                    temp = t1[i];

                    t1[i].transform.position = t1[i - 1].transform.position;
                    t1[i].transform.localScale = t1[i - 1].transform.localScale;
                    t1[i] = t1[i - 1];
                    
                    t1[i - 1].transform.position = origPos;
                    t1[i - 1].transform.localScale = origScale;
                    t1[i - 1] = temp;
                }
            }

            for (int i = 0; i < t2.Count; ++i)
            {
                if (t2.Count > 0 && i != 0 && t2[i].GetComponent<TurnTrackerObj>().objData.t2Pos < t2[i - 1].GetComponent<TurnTrackerObj>().objData.t2Pos)
                {
                    sorted = false;

                    origPos = t2[i].transform.position;
                    origScale = t2[i].transform.localScale;
                    temp = t2[i];
                    t2[i].transform.position = t2[i - 1].transform.position;
                    t2[i].transform.localScale = t2[i - 1].transform.localScale;
                    t2[i] = t2[i - 1];
                    t2[i - 1].transform.position = origPos;
                    t2[i - 1].transform.localScale = origScale;
                    t2[i - 1] = temp;
                }
            }
        }

        if (wait)
            listSorted = true;
    }

    // Add turn trackers between screen resets
    public void AddTurnTrackers(List<GameObject> g1, List<GameObject> g2, GameObject player)
    {
        bool @bool = false;

        List<GameObject> l1 = new List<GameObject>();
        for (int i = 0; i < g1.Count; ++i)
        {
            @bool = true;
            for (int j = 0; j < t1.Count; ++j)
            {
                if (g1[i] == t1[j].GetComponent<TurnTrackerObj>().obj)
                    @bool = false;
            }
            
            if (@bool)
                l1.Add(g1[i]);
        }

        List<GameObject> l2 = new List<GameObject>();
        for (int i = 0; i < g2.Count; ++i)
        {
            @bool = true;
            for (int j = 0; j < t2.Count; ++j)
            {
                if (g2[i] == t2[j].GetComponent<TurnTrackerObj>().obj)
                    @bool = false;
            }
            
            if (@bool)
                l2.Add(g2[i]);
        }

        StartCoroutine(CreateT1TrackerUI(l1, false));
        StartCoroutine(CreateT2TrackerUI(l2, false));
    }

    // Creates a tracker at a specific index
    //  - Used to create the beginning turn order
    //  - Used to slide enemies into the turn order
    public void CreateT1Tracker(GameObject theImage, int i)
    {
        GameObject g = Instantiate(image, t1Storage);
        //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
        g.GetComponent<Image>().color = theImage.GetComponent<SpriteRenderer>().color;
        g.GetComponent<TurnTrackerObj>().obj = theImage;
        g.transform.position = t1Tracker[i + 1].rectTransform.position;
        g.transform.localScale = t1Tracker[i + 1].rectTransform.localScale;

        t1.Add(g);
    }

    public void CreateT2Tracker(GameObject theImage, int i)
    {
        GameObject g = Instantiate(image, t2Storage);
        //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
        g.GetComponent<Image>().color = theImage.GetComponent<SpriteRenderer>().color;
        g.GetComponent<TurnTrackerObj>().obj = theImage;
        g.transform.position = t2Tracker[i].rectTransform.position;
        g.transform.localScale = t2Tracker[i].rectTransform.localScale;

        t2.Add(g);
    }

    public void MoveTracker(int i)
    {
        StartCoroutine(MoveTurnTrackerUI(i));
    }

    private void MoveNextTurnTracker()
    {
        StartCoroutine(MoveNextTurnTrackerUI());
    }

    // Sets up the T1TrackerUI at the beginning of the game
    IEnumerator CreateT1TrackerUI(List<GameObject> l, bool end)
    {
        float incs = (250 / seconds) * timeIncrements;

        int j = 0;
        if (end)
            j = 1;

        int posAdjust = 0;
        if (t1[0] != null && t1.Count > 0)
            posAdjust = t1.Count;

        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = Instantiate(image, t1Storage);
            //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
            g.GetComponent<Image>().color = l[i].GetComponent<SpriteRenderer>().color;
            g.GetComponent<Image>().color -= new Color(0, 0, 0, 1);
            g.GetComponent<TurnTrackerObj>().obj = l[i];
            g.GetComponent<TurnTrackerObj>().objData = l[i].GetComponent<CharData>();

            g.transform.position = t1Tracker[i + j + posAdjust].rectTransform.position;
            g.transform.localScale = t1Tracker[i + j + posAdjust].rectTransform.localScale * 2;

            t1.Add(g);
        }

        while (t1[j].GetComponent<Image>().color.a < 1 && t1[j].transform.localScale.x > t1Tracker[1].rectTransform.localScale.x)
        {
            for (int i = j; i < t1.Count; ++i)
            {
                t1[i].GetComponent<Image>().color += new Color(0, 0, 0, incs * 0.025f);
                t1[i].transform.localScale -= new Vector3(incs * 0.025f, incs * 0.025f, incs * 0.025f);
            }

            yield return new WaitForSeconds(timeIncrements);
        }

        for (int i = j; i < t1.Count; ++i)
        {
            t1[i].GetComponent<Image>().color += new Color(0, 0, 0, 1);
            t1[i].transform.localScale = t1Tracker[i].rectTransform.localScale;
        }

        yield return null;
    }

    // Sets up the T2 tracker UI at the beginning of each turn
    IEnumerator CreateT2TrackerUI(List<GameObject> l, bool end)
    {
        float incs = (250 / seconds) * timeIncrements;

        int posAdjust = 0;
        if (t2.Count > 0)
            posAdjust = t2.Count;

        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = Instantiate(image, t2Storage);
            //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
            g.GetComponent<Image>().color = l[i].GetComponent<SpriteRenderer>().color;
            g.GetComponent<Image>().color -= new Color(0, 0, 0, 1);
            g.GetComponent<TurnTrackerObj>().obj = l[i];
            g.GetComponent<TurnTrackerObj>().objData = l[i].GetComponent<CharData>();

            g.transform.position = t2Tracker[i + posAdjust].rectTransform.position;
            g.transform.localScale = t2Tracker[i + posAdjust].rectTransform.localScale * 2;

            t2.Add(g);
        }

        while (t2[0].GetComponent<Image>().color.a < 1 && t2[0].transform.localScale.x > t2Tracker[0].rectTransform.localScale.x)
        {
            for (int i = 0; i < t2.Count; ++i)
            {
                t2[i].GetComponent<Image>().color += new Color(0, 0, 0, incs * 0.025f);
                t2[i].transform.localScale -= new Vector3(incs * 0.025f, incs * 0.025f, incs * 0.025f);
            }

            yield return new WaitForSeconds(timeIncrements);
        }

        for (int i = 0; i < t2.Count; ++i)
        {
            t2[i].GetComponent<Image>().color += new Color(0, 0, 0, 1);
            t2[i].transform.localScale = t2Tracker[i].rectTransform.localScale;
        }

        if (end)
        {
            if (t1.Count > 0)
                Managers.TurnManager.Instance.StartRound();
            else
                Managers.TurnManager.Instance.EndRound();
        }
        else
            Managers.TurnManager.Instance.SetTurnOrder(2);

        yield return null;
    }

    // Moves the turn order to the LEFT whenever a character performs an action
    IEnumerator ShiftNonDestroyedTrackers(bool endRound)
    {
        yield return new WaitForSeconds(0.1f + (1 / 0.1f) * timeIncrements);

        float mainInc = (240 / seconds) * timeIncrements;
        float incs = (200 / seconds) * timeIncrements;
        
        CheckTs(t1);
        CheckTs(t2);

        bool moving = true;
        while (moving)
        {
            // Moves T1 into position
            for (int i = 0; i < t1.Count; ++i)
            {
                if (i > 0 && t1[i].transform.position.x > t1Tracker[i].transform.position.x)
                {
                    t1[i].transform.position -= new Vector3(incs, 0);
                }

                else if (i == 0 && t1[0].transform.position.x > t1Tracker[0].transform.position.x + 0.01f)
                {
                    t1[0].transform.position -= new Vector3(mainInc, 0);
                    if (t1[0].transform.localScale.x < 1.5f)
                        t1[0].transform.localScale += new Vector3(0.005f * mainInc, 0.005f * mainInc, 0.005f * mainInc);
                }
            }

            // Moves T2 into position
            for (int i = 0; i < t2.Count; ++i)
            {
                if (t2[i].transform.position.x > t2Tracker[i].transform.position.x)
                {
                    t2[i].transform.position -= new Vector3(incs, 0);
                }
            }

            yield return new WaitForSeconds(timeIncrements);

            // Sets T1 and T2 to make sure the new positions are accurate
            if (t1[t1.Count - 1].transform.position.x <= t1Tracker[t1.Count - 1].transform.position.x + 0.01f &&
                t2[t2.Count - 1].transform.position.x <= t2Tracker[t2.Count - 1].transform.position.x + 0.01f)
            {
                for (int i = 0; i < t1.Count; ++i)
                {
                    t1[i].transform.position = t1Tracker[i].transform.position;
                }

                for (int i = 0; i < t2.Count; ++i)
                {
                    t2[i].transform.position = t2Tracker[i].transform.position;
                }
                moving = false;
            }
        }

        // Ends the round of combat
        if (endRound)
            Managers.TurnManager.Instance.EndRound();

        yield return null;
    }

    IEnumerator DestroyTracker(GameObject t)
    {
        //Image i = t.GetComponent<Image>();
        //while (i.color.a > 0)
        //{
        //    i.color -= new Color(0, 0, 0, 0.5f);
        //    yield return new WaitForSeconds(timeIncrements);
        //}

        Destroy(t);

        yield return null;
    }

    // Checks and shifts several values over
    private void CheckTs(List<GameObject> t)
    {
        bool checkT = false;
        while (!checkT)
        {
            checkT = true;

            for (int i = 0; i < t.Count; ++i)
            {
                if (t[i] == null && i != t.Count - 1 && t[i + 1] != null)
                {
                    t[i] = t[i + 1];
                    t[i + 1] = null;
                    checkT = false;
                }

                else if (t[i] == null && i == t.Count - 1)
                {
                    t.Remove(t[i]);
                    checkT = false;
                }
            }
        }

        //for (int i = 0; i < t.Count; ++i)
        //{
        //    if (t[i] == null)
        //    {
        //        t.Remove(t[i]);
        //    }
        //}
    }

    // Moves the turn order to the LEFT whenever a character performs an action
    IEnumerator MoveTurnTrackerUI(int j)
    {
        bool moving = true;
        
        yield return new WaitForSeconds(0.1f);

        float mainInc = (240 / seconds) * timeIncrements;
        float incs = (200 / seconds) * timeIncrements;

        while (moving)
        {
            for (int i = 0; i < t1.Count; ++i)
            {
                if (i > 1 && t1[i].transform.position.x > t1Tracker[i - 1].transform.position.x)
                {
                    t1[i].transform.position -= new Vector3(incs, 0);
                }

                else if (i == 1 && t1[1].transform.position.x > t1Tracker[0].transform.position.x)
                {
                    t1[1].transform.position -= new Vector3(mainInc, 0);
                    if (t1[1].transform.localScale.x < 1.5f)
                        t1[1].transform.localScale += new Vector3(0.005f * mainInc, 0.005f * mainInc, 0.005f * mainInc);
                }

                else if (i == 0 && t1[0] != null)
                {
                    t1[0].transform.position -= new Vector3(mainInc, 0);
                    t1[0].GetComponent<Image>().color -= new Color(0, 0, 0, 0.02f * mainInc);
                }
            }

            yield return new WaitForSeconds(timeIncrements);

            if (t1.Count > 1 && t1[1].transform.position.x <= t1Tracker[0].transform.position.x)
            {
                for (int i = 1; i < t1.Count; ++i)
                {
                    t1[i].transform.position = t1Tracker[i - 1].transform.position;
                }
                moving = false;
            }

            else if (t1.Count == 1 && t1[0].GetComponent<Image>().color.a <= 0.1f)
            {
                moving = false;
            }
        }
        
        Destroy(t1[0]);
        t1.Remove(t1[0]);

        // Depending on the input, either starts the next round of gameplay
        // OR waits for the tracker to finish moving
        if (j == 0)
            Managers.TurnManager.Instance.CheckForRoundType();
        else
            MoveNextTurnTracker();

        yield return null;
    }

    // Moves the turn order to the LEFT whenever a character performs an action
    IEnumerator MoveNextTurnTrackerUI()
    {
        bool moving = true;

        float incs = (1000 / seconds) * timeIncrements;

        while (moving)
        {
            for (int i = 0; i < t2.Count; ++i)
            {
                if (t2[i].transform.position.x > t1Tracker[i + 1].transform.position.x)
                {
                    t2[i].transform.position -= new Vector3(incs, 0);
                    if (t2[i].transform.localScale.x < 1.0f)
                        t2[i].transform.localScale += new Vector3(0.005f * incs, 0.005f * incs, 0.005f * incs);
                }

                if (t2[i].transform.position.x <= t1Tracker[i + 1].transform.position.x)
                    t2[i].transform.position = t1Tracker[i + 1].transform.position;
            }
            
            yield return new WaitForSeconds(timeIncrements);

            if (t2.Count > 1 && t2[0].transform.position.x <= t1Tracker[1].transform.position.x)
            {
                for (int i = 0; i < t2.Count; ++i)
                {
                    t2[i].transform.position = t1Tracker[i + 1].transform.position;
                    t2[i].transform.localScale = t1Tracker[i + 1].transform.localScale;
                }
                moving = false;
            }
        }

        // Sets up t1 for the next global turn
        t1.Add(null);
        for (int i = 0; i < t2.Count; ++i)
            t1.Add(t2[i]);

        // Removes all current things from t2
        int c = t2.Count;
        for (int i = 0; i < c; ++i)
        {
            t2Storage.transform.GetChild(0).SetParent(t1Storage.transform);
            t2.Remove(t2[0]);
        }

        // Starts the next global turn of gameplay, which sets up t2 and moves the turn again
        Managers.TurnManager.Instance.StartGlobalTurn();

        yield return null;
    }
}
