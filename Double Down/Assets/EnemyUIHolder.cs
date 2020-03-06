using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIHolder : MonoBehaviour
{
    private Transform enemyList;
    public GameObject charUI;

    // Start is called before the first frame update
    void Start()
    {
        enemyList = GameObject.Find("EnemyChars").transform;

        for (int i = 0; i < enemyList.childCount; ++i)
        {
            GameObject n = Instantiate(charUI, gameObject.transform);
            enemyList.GetChild(i).GetComponent<CharData>().charUI = n;
            enemyList.GetChild(i).GetComponent<CharData>().SetCharUI();
        }
    }
}
