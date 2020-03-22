using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RevivalText : MonoBehaviour
{
    public TextMeshProUGUI t;
    public bool done = false;
    private RectTransform objTransform;

    // Start is called before the first frame update
    void Start()
    {
        objTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowRevivalText(CharData data)
    {
        done = false;

        // Sets up the text to display in the center of the screen
        string s = "";
        if (data.deathTurns != 1)
            s = "s";
        t.SetText("<size=56>" + data.deathTurns + " Turn" + s + "\n</size><color=\"white\">Until Revival</color>");

        StartCoroutine(DisplayText());
    }

    IEnumerator DisplayText()
    {
        yield return new WaitForSeconds(0.1f);

        // Moves in from the left
        while (objTransform.localPosition.x < -50)
        {
            objTransform.localPosition += new Vector3(15, 0, 0);
            yield return new WaitForSeconds(0.0125f);
        }
        objTransform.localPosition = new Vector3(-50, 0, 0);

        // Stays on-screen for half a second
        yield return new WaitForSeconds(0.75f);

        // Moves out to the right
        while (objTransform.localPosition.x < 500)
        {
            objTransform.localPosition += new Vector3(15, 0, 0);
            yield return new WaitForSeconds(0.0125f);
        }
        objTransform.localPosition = new Vector3(-600, 0, 0);

        done = true;
        yield return null;
    }
}
