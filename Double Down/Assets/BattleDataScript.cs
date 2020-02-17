using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class BattleDataScript : Singleton<BattleDataScript>
    {
        [Header("Player Character Data")]
        public List<Stats> cStats;
        public List<CharData> cCharData;

        public bool valuesExist = false;
        private static BattleDataScript inst;

        // Start is called before the first frame update
        void Awake()
        {
            GameObject obj = GameObject.Find("BattleData");

            if (inst != null)
                Destroy(gameObject);
            else
            {
                inst = this;
                DontDestroyOnLoad(gameObject);
            }

            CreateData();
        }

        public void CopyCharData(Stats s, CharData c, int i)
        {
            cStats[i].CopyStats(s);
            cCharData[i].CopyData(c);
        }

        private void CreateData()
        {
            for (int i = 0; i < 2; ++i)
            {
                Stats s = gameObject.AddComponent<Stats>();
                CharData c = gameObject.AddComponent<CharData>();
                cStats.Add(s);
                cCharData.Add(c);
            }
        }

        // Populates the data in the fields
        public void PopulateData()
        {
            // Populate the data of both player characters at the beginning of each scene
            if (valuesExist)
            {
                string name = "Char";
                for (int i = 0; i < 2; ++i)
                {
                    GameObject temp = GameObject.Find(name + (i + 1));
                    temp.GetComponent<Stats>().CopyStats(cStats[i]);
                    temp.GetComponent<CharData>().CopyData(cCharData[i]);
                }
            }

            // Copy the data of each player character for future use
            SetCharValues();

            // With the data set, all values now read true
            valuesExist = true;
        }

        // Copy the data of each player character for future use
        public void SetCharValues()
        {
            GameObject temp = GameObject.Find("Char1");
            CopyCharData(temp.GetComponent<Stats>(), temp.GetComponent<CharData>(), 0);
            temp.GetComponent<CharData>().SetCharUI();
            temp = GameObject.Find("Char2");
            CopyCharData(temp.GetComponent<Stats>(), temp.GetComponent<CharData>(), 1);
            temp.GetComponent<CharData>().SetCharUI();
        }

        // Copy the data of each player character for future use, with maxed-out HP and TP
        public void SetMaxCharValues()
        {
            for (int i = 0; i < TurnManager.Instance.playerCharsList.Count; ++i)
            {
                GameObject temp = TurnManager.Instance.playerCharsList[i];
                CopyCharData(temp.GetComponent<Stats>(), temp.GetComponent<CharData>(), temp.GetComponent<CharData>().charNum);
            }

            for (int i = 0; i < 2; ++i)
            {
                cStats[i].currentHP = cStats[i].maxHP;
                cStats[i].currentTP = cStats[i].maxTP;
            }
        }
    }
}