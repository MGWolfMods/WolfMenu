using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using FluffyUnderware.Curvy.Generator;
using System.ComponentModel;
using Wolf_Menu.Misc;

namespace Wolf_Menu.Tabs
{
    internal class T_CarSpawner
    {

        internal static GameObject[] cars;
        private static GameObject emptyContainer;
        private static GameObject containerParent;

        private static bool _firstLoad = false;

        internal static void Initialize()
        {
            containerParent = UiHandler.contentParent.Find("CarSpawner/Body/CarList/Viewport/Content").gameObject;
            emptyContainer = containerParent.transform.Find("CarContainer").gameObject;
            emptyContainer.SetActive(false);

            cars = GameObject.Find("CarsParent").GetComponent<CarList>().Cars;
        }

        internal static void LoadList()
        {
            if (_firstLoad) return;
            
            foreach (GameObject carObjct in cars)
            {
                Sprite carImg = GetIcon.carFromString(carObjct.name);
                GameObject tempContainer = UnityEngine.Object.Instantiate(emptyContainer);
                tempContainer.transform.SetParent(containerParent.transform);
                tempContainer.name = carObjct.name;
                tempContainer.transform.Find("CarName").GetComponent<TMPro.TextMeshProUGUI>().text = carObjct.name;
                tempContainer.SetActive(true);
                if (carImg != null) { tempContainer.transform.Find("CarImg").GetComponent<Image>().sprite = carImg; }
                tempContainer.transform.Find("SpawnBtn").GetComponent<Button>().onClick.AddListener(delegate { SpawnCar(carObjct.name); });
            }

            _firstLoad = true;
        }

            
        internal static void SpawnCar(string carName)
        {

            GameObject carToSpawn = null;
            foreach(GameObject carObjct in cars)
            {
                if(carObjct.name == carName)
                {
                    carToSpawn = carObjct;
                    break;
                }
            }
            if(carToSpawn == null) { return; }

            TMPro.TMP_Dropdown carDropdown = containerParent.transform.Find(carName + "/CarStatus").GetComponent<TMPro.TMP_Dropdown>();

            GameObject spawnPos = new GameObject("SpawnPos");
            spawnPos.transform.SetParent(GameObject.Find("Player").transform);
            spawnPos.transform.localPosition = new Vector3(0f, 1.5f, 3f);
            spawnPos.transform.rotation = GameObject.Find("Player").transform.rotation;
            spawnPos.transform.SetParent(null);

            GameObject carSpawn = UnityEngine.Object.Instantiate<GameObject>(carToSpawn, spawnPos.transform.position, spawnPos.transform.rotation);
            carSpawn.GetComponent<MainCarProperties>().SpawnPoint = spawnPos.transform.position;
            switch (carDropdown.value)
            {
                case 0:
                    carSpawn.GetComponent<MainCarProperties>().CreatingStock(0);
				    break;
                case 1:
                    carSpawn.GetComponent<MainCarProperties>().CreatingUsed();
                    break;
                case 2:
                    carSpawn.GetComponent<MainCarProperties>().CreatingStockCrached();
                    break;
                case 3:
                    carSpawn.GetComponent<MainCarProperties>().CreatingJunkyard();
                    break;
                case 4:
                    carSpawn.GetComponent<MainCarProperties>().CreatingStartOldCar();
                    break;
                case 5:
                    carSpawn.GetComponent<MainCarProperties>().CreatingRuinedFind();
                    break;
                case 6:
                    carSpawn.GetComponent<MainCarProperties>().CreatingBarnFind();
                    break;
            }

            UnityEngine.Object.Destroy(spawnPos);
            carSpawn.GetComponent<MainCarProperties>().Owner = "Player";
            CarProperties[] componentsInChildren = carSpawn.GetComponentsInChildren<CarProperties>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].Owner = "Player";
            }
            carSpawn.GetComponent<MainCarProperties>().SetOwnerPlayer();
        }

    }
}
