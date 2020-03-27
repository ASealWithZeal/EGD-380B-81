using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Floor Data")]
    public GameObject floor = null;
    public Material[] floorMats = null;
    public float[] floorMatChances = null;

    [Header("Wall Data")]
    public GameObject wall = null;
    public Material[] wallMats = null;

    [Header("Liquid Data")]
    public GameObject liquid = null;
    public Material[] liquidMats = null;
    public float[] liquidMatChances = null;

    [Header("Bridge Data")]
    public GameObject bridges = null;
    public Material[] bridgeMats = null;

    // Start is called before the first frame update
    void Start()
    {
        CreateEnvironmentMats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEnvironmentMats()
    {
        GenerateTiles(floor.transform, floorMats, floorMatChances);
        GenerateTiles(liquid.transform, liquidMats, liquidMatChances);
    }

    public void GenerateTiles(Transform holder, Material[] holderMats, float[] holderChances)
    {
        List<Material> lastMats = new List<Material>();

        for (int i = 0; i < holder.childCount; ++i)
        {
            float rand = Random.Range(0.0f, 100.0f);
            float chance = 0.0f;
            Debug.Log(rand);
            //holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[0];

            for (int j = 0; j < holderMats.Length; ++j)
            {
                if (rand >= chance && rand <= chance + holderChances[j])
                {
                    //if (lastMats.Count < 2)
                    //{
                    //    lastMats.Add(holderMats[j]);
                    //    holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[j];
                    //}
                    //else if (lastMats.Count == 2 && (lastMats[0] != holderMats[j] || lastMats[1] != holderMats[j]))
                    //{
                    //    lastMats.Remove(lastMats[0]);
                    //    lastMats.Add(holderMats[j]);
                    //    holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[j];
                    //}
                    //else
                    //{
                    //    i = 0;
                    //}
            
                    holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[j];
                }

                chance += holderChances[j];
            }
        }
    }
}
