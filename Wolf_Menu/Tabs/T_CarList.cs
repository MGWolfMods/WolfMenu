using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wolf_Menu.Misc;


namespace Wolf_Menu.Tabs
{
    internal class T_CarList
    {

        private static CarManager carManager;

        private static GameObject carListParent;
        private static GameObject carListContainer;
        private static GameObject noCarFound;

        private static string[] blackList = { "CarContainer", "NoCar" };


        internal static void Initialize()
        {
            carManager = GameObject.Find("Player").GetComponent<CarManager>();

            carListParent = UiHandler.contentParent.transform.Find("CarList/Body/CarList/Viewport/Content").gameObject;
            carListContainer = carListParent.transform.Find("CarContainer").gameObject;
            noCarFound = carListParent.transform.Find("NoCar").gameObject;
            carListContainer.SetActive(false);
            noCarFound.SetActive(false);

            UiHandler.contentParent.transform.Find("CarList/Header/Reload").GetComponent<Button>().onClick.AddListener(LoadList);
        }

        internal static void LoadList()
        {
            foreach (Transform child in carListParent.transform)
            {
                if (!blackList.Contains(child.name)) { GameObject.Destroy(child.gameObject); }
            }

            noCarFound.SetActive(true);

            foreach (GameObject car in carManager.carObjects)
            {
                if(car.GetComponent<MainCarProperties>().Owner != "Player") { continue; }

                GameObject tempContainer = UnityEngine.Object.Instantiate(carListContainer);
                tempContainer.transform.SetParent(carListParent.transform);
                tempContainer.name = car.name.Replace("(Clone)", "");
                tempContainer.transform.Find("EditCar/CarName").GetComponent<TMPro.TextMeshProUGUI>().text = tempContainer.name;
                tempContainer.SetActive(true);
                RuntimePreviewGenerator.BackgroundColor = new Color(0f, 0f, 0f, 0f);
                tempContainer.transform.Find("CarImg").GetComponent<Image>().sprite = Sprite.Create(RuntimePreviewGenerator.GenerateModelPreview(car.transform, 200, 200, false), new Rect(0f, 0f, 200f, 200f), new Vector2(0.5f, 0.5f), 100f);
                tempContainer.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(delegate { DeleteCar(car); });
                tempContainer.transform.Find("Teleport").GetComponent<Button>().onClick.AddListener(delegate { TeleportToCar(car); });
                tempContainer.transform.Find("CarDesc").GetComponent<TMPro.TextMeshProUGUI>().text = GetCarDescription(car);
                tempContainer.transform.Find("EditCar").GetComponent<Button>().onClick.AddListener(delegate { LoadEditCar(car); });
                noCarFound.SetActive(false);
            }

        }

        private static void LoadEditCar(GameObject car)
        {
            T_EditCar.LoadCar(car);
            UiHandler.SwitchTabs(UiHandler.Tabs.EditCar);
        }


        internal static string GetCarDescription(GameObject car)
        {
            string description = "";
            string engineName = "Engine: None";
            string horsePower = "HP: 0";
            if (car.GetComponent<MainCarProperties>().EngineBlock)
            {
                engineName = "Engine: " + car.GetComponent<MainCarProperties>().EngineBlock.PartNameExtension;
                horsePower = "HP: " + car.GetComponent<MainCarProperties>().EnginePower;
            }
            description = engineName + "@" + horsePower;
            description = description.Replace("@", Environment.NewLine);

            return description;
        }

        internal static void DeleteCar(GameObject car)
        {
            if(car!= null && carManager.playerCar != car) 
            {
                GameObject.Destroy(car);
                carManager.carObjects.Remove(car);
                LoadList(); 
            }
        }
        internal static void TeleportToCar(GameObject car)
        {
            GameObject.Find("Player").transform.position = new Vector3(car.transform.position.x, car.transform.position.y + 5f, car.transform.position.z);
            UiHandler.ChangeState(false);
        }


    }
}
