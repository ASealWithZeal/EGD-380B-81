using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBob : MonoBehaviour
{
    float bobSpeed = 0;
    float defaultY = 0;
    float bobVal = 0.1f;

    bool canBob = true;

    // Update is called once per frame
    void Update()
    {
        if (canBob)
        {
            canBob = false;
            StartCoroutine(Bob());
        }
    }

    private void Start()
    {
        bobSpeed = 0.001f;
        bobVal = 0.03f;
        defaultY = transform.position.y;
        transform.position += new Vector3(0, Random.Range(-0.025f, 0.025f), 0);
    }

    IEnumerator Bob()
    {
        Transform t = transform;

        while (t.position.y > defaultY - bobVal)
        {
            t.position -= new Vector3(0, bobSpeed, 0);
            yield return new WaitForSeconds(0.05f);
        }
        t.position = new Vector3(t.position.x, defaultY - bobVal, t.position.z);
        
        yield return new WaitForSeconds(0.05f);

        while (t.position.y < defaultY + bobVal)
        {
            t.position += new Vector3(0, bobSpeed, 0);
            yield return new WaitForSeconds(0.05f);
        }
        t.position = new Vector3(t.position.x, defaultY + bobVal, t.position.z);
        
        yield return new WaitForSeconds(0.05f);
        canBob = true;
        yield return null;
    }
}
