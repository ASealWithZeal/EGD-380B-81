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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetUpTrackers(List<GameObject> l, int j)
    {
        if (t1.Count == 0)
            t1.Add(null);

        if (j == 1)
            StartCoroutine(CreateT1TrackerUI(l));

        else if (j == 2)
            StartCoroutine(CreateT2TrackerUI(l));
    }

    // Creates a tracker at a specific index
    //  - Used to create the beginning turn order
    //  - Used to slide enemies into the turn order
    public void CreateT1Tracker(GameObject theImage, int i)
    {
        GameObject g = Instantiate(image, t1Storage);
        //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
        g.GetComponent<Image>().color = theImage.GetComponent<SpriteRenderer>().color;
        g.transform.position = t1Tracker[i + 1].rectTransform.position;
        g.transform.localScale = t1Tracker[i + 1].rectTransform.localScale;

        t1.Add(g);
    }

    public void CreateT2Tracker(GameObject theImage, int i)
    {
        GameObject g = Instantiate(image, t2Storage);
        //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
        g.GetComponent<Image>().color = theImage.GetComponent<SpriteRenderer>().color;
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
    IEnumerator CreateT1TrackerUI(List<GameObject> l)
    {
        float incs = (250 / seconds) * timeIncrements;

        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = Instantiate(image, t1Storage);
            //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
            g.GetComponent<Image>().color = l[i].GetComponent<SpriteRenderer>().color;
            g.GetComponent<Image>().color -= new Color(0, 0, 0, 1);

            g.transform.position = t1Tracker[i + 1].rectTransform.position;
            g.transform.localScale = t1Tracker[i + 1].rectTransform.localScale * 2;

            t1.Add(g);
        }

        while (t1[1].GetComponent<Image>().color.a < 1 && t1[1].transform.localScale.x > t1Tracker[1].rectTransform.localScale.x)
        {
            for (int i = 1; i < t1.Count; ++i)
            {
                t1[i].GetComponent<Image>().color += new Color(0, 0, 0, incs * 0.025f);
                t1[i].transform.localScale -= new Vector3(incs * 0.025f, incs * 0.025f, incs * 0.025f);
            }

            yield return new WaitForSeconds(timeIncrements);
        }

        for (int i = 1; i < t1.Count; ++i)
        {
            t1[i].GetComponent<Image>().color += new Color(0, 0, 0, 1);
            t1[i].transform.localScale = t1Tracker[i].rectTransform.localScale;
        }

        yield return null;
    }

    // Sets up the T2 tracker UI at the beginning of each turn
    IEnumerator CreateT2TrackerUI(List<GameObject> l)
    {
        float incs = (250 / seconds) * timeIncrements;

        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = Instantiate(image, t2Storage);
            //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
            g.GetComponent<Image>().color = l[i].GetComponent<SpriteRenderer>().color;
            g.GetComponent<Image>().color -= new Color(0, 0, 0, 1);

            g.transform.position = t2Tracker[i].rectTransform.position;
            g.transform.localScale = t2Tracker[i].rectTransform.localScale * 2;

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

        Managers.TurnManager.Instance.StartRound();

        yield return null;
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

            else if (t1.Count <= 1 && t1[0].GetComponent<Image>().color.a <= 0.1f)
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
            t2Storage.transform.GetChild(0).parent = t1Storage.transform;
            t2.Remove(t2[0]);
        }

        // Starts the next global turn of gameplay, which sets up t2 and moves the turn again
        Managers.TurnManager.Instance.StartGlobalTurn();

        yield return null;
    }
}
