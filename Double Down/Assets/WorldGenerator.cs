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

    [Header("Rock Data")]
    public GameObject rock = null;
    public GameObject rockPrefab = null;
    public Material[] rockMats = null;
    public float[] rockMatChances = null;
    public float rockSpawnChance = 10;


    // Start is called before the first frame update
    void Start()
    {
        //CreateEnvironmentMats();
    }

    public void CreateEnvironmentMats()
    {
        GenerateTiles(floor.transform, floorMats, floorMatChances);
        GenerateTiles(liquid.transform, liquidMats, liquidMatChances);
        GenerateTiles(wall.transform, wallMats, wallMatChances);

        GenerateEnvironmentPieces(rock.transform, liquid.transform, rockPrefab, rockMats, rockMatChances, rockSpawnChance);
    }

    public void GenerateTiles(Transform holder, Material[] holderMats, float[] holderChances)
    {
        List<Material> lastMats = new List<Material>();

        for (int i = 0; i < holder.childCount; ++i)
        {
            float rand = Random.Range(0.0f, 100.0f);
            float chance = 0.0f;
            //holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[0];

            for (int j = 0; j < holderMats.Length; ++j)
            {
                if (rand >= chance && rand <= chance + holderChances[j])
                    holder.GetChild(i).GetComponent<MeshRenderer>().material = holderMats[j];

                chance += holderChances[j];
            }
        }
    }

    public void GenerateEnvironmentPieces(Transform holder, Transform parent, GameObject obj, Material[] holderMats, float[] holderChances, float spawnChance)
    {
        List<Material> lastMats = new List<Material>();
        bool canSpawn = true;

        int holderCount = holder.childCount;
        for (int i = 0; i < holderCount; ++i)
            DestroyImmediate(holder.GetChild(0).gameObject);

        for (int i = 0; i < parent.childCount; ++i)
        {
            float rand = Random.Range(0.0f, 100.0f);
            if (canSpawn && rand >= 0 && rand <= spawnChance)
            {
                GameObject n = Instantiate(obj, parent.GetChild(i).position, obj.transform.rotation * new Quaternion(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), 1), holder);
                
                canSpawn = false;
            }
            else if (!canSpawn)
                canSpawn = true;
        }

        GenerateTiles(holder, holderMats, holderChances);
    }
}
