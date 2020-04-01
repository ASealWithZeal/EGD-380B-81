using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTrackerObj : MonoBehaviour
{
    public GameObject obj;
    public GameObject selector;
    public Image portrait;
    public CharData objData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        selector.SetActive(!selector.activeSelf);
    }
}
