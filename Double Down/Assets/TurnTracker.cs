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
    [HideInInspector] public List<GameObject> t2;
    public GameObject image;

    public float seconds = 3.0f;
    public float timeIncrements = 0.005f;

    public bool bigButton = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bigButton)
        {
            bigButton = false;

            if (t1.Count < 1)
            {
                StartCoroutine(MoveNextTurnTrackerUI());
            }

            else
            {
                StartCoroutine(MoveTurnTrackerUI());
            }
        }
    }

    public void SetUpTrackers(List<GameObject> l1, List<GameObject> l2)
    {
        t1.Add(null);

        for (int i = 0; i < l1.Count; ++i)
            CreateT1Tracker(l1[i], i);

        for (int i = 0; i < l2.Count; ++i)
            CreateT2Tracker(l2[i], i);
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
        if (i == 0)
            StartCoroutine(MoveTurnTrackerUI());
        else
            StartCoroutine(MoveNextTurnTrackerUI());
    }

    // Moves the turn order to the LEFT whenever a character performs an action
    IEnumerator MoveTurnTrackerUI()
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
                    t1[0].GetComponent<Image>().color -= new Color(0, 0, 0, 0.01f * mainInc);
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

        yield return null;
    }

    // Moves the turn order to the LEFT whenever a character performs an action
    IEnumerator MoveNextTurnTrackerUI()
    {
        bool moving = true;

        float incs = (800 / seconds) * timeIncrements;

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
            }

            yield return new WaitForSeconds(timeIncrements);

            if (t2.Count > 1 && t2[0].transform.position.x <= t1Tracker[1].transform.position.x)
            {
                for (int i = 0; i < t2.Count; ++i)
                {
                    t2[i].transform.position = t1Tracker[i + 1].transform.position;
                }
                moving = false;
            }
        }
        
        t1.Add(null);

        for (int i = 0; i < t2.Count; ++i)
            t1.Add(t2[i]);

        t2.Clear();

        yield return null;
    }
}
