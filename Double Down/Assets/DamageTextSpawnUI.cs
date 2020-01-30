using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawnUI : MonoBehaviour
{
    public float height = 20.0f;
    public bool landed = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnNumber());
    }

    IEnumerator SpawnNumber()
    {
        bool temp = false;
        float startPos = gameObject.transform.position.y;
        float targetPos = gameObject.transform.position.y + height;
        
        while (!temp)
        {
            gameObject.transform.position += new Vector3(0, 4, 0);
            yield return new WaitForSeconds(0.005f);

            if (gameObject.transform.position.y >= targetPos)
                temp = true;
        }

        while (temp)
        {
            gameObject.transform.position -= new Vector3(0, 4, 0);
            yield return new WaitForSeconds(0.005f);

            if (gameObject.transform.position.y <= startPos)
                temp = false;
        }

        landed = true;
        yield return null;
    }
}
