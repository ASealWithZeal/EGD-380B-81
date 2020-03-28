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
    public float[] wallMatChances = null;

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

    public void CreateEnvironmentMats()
    {
        GenerateTiles(floor.transform, floorMats, floorMatChances);
        GenerateTiles(liquid.transform, liquidMats, liquidMatChances);
        GenerateTiles(wall.transform, wallMats, wallMatChances);
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
                    holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[j];

                chance += holderChances[j];
            }
        }
    }
}
