using FluffyUnderware.DevTools.Extensions;
using Steamworks;
using System;
using System.Collections;
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
    internal class T_ItemSpawner
    {

        internal static List<GameObject> partList = new List<GameObject>();
        private static List<GameObject> fluidList = new List<GameObject>();
        private static List<GameObject> toolsList = new List<GameObject>();
        private static List<GameObject> wheelsList = new List<GameObject>();
        private static List<GameObject> rimList = new List<GameObject>();

        private static GameObject itemSpawnerParent;

        private static GameObject categorieParent;
        private static GameObject categorieContainer;

        private static GameObject itemParent;
        private static GameObject itemContainer;

        private static GameObject carPartParent;
        private static GameObject carPartContainer;

        private static GameObject enginePartParent;
        private static GameObject enginePartContainer;

        private static GameObject carMenuView;
        private static GameObject categorieView;
        private static GameObject itemView;
        private static GameObject engineMenuView;

        private static bool firstSetup = false;

        private static string[] carBlacklist = { "LADcabrio", "WolfCabrio", "WolfCoupe", "WolfWagon", "LAD1", "LAD2" };

        private static Dictionary<string, string> categories = new Dictionary<string, string>() 
        {
            {"All Items", "TrailerCar" },
            {"Car parts", "L500" },
            {"Engine parts", "CylinderBlock"},
            {"Tools", "Wrench"},
            {"Rims", "Rim16St3" },
            {"Tires", "tire16" },
            {"Fluids", "Brake Fluid"}

        };

        private static Dictionary<string, string> engineTypes = new Dictionary<string, string>()
        {
            {"I4", "CylinderBlock" },
            {"V8", "CylinderBlockV8"},
            {"I6", "CylinderBlockI6"},
            {"I6D", "CylinderBlockI6D" },
            {"500cc", "Cylinder00" },
            {"V2", "Cylinder10A"}

        };

        internal static void Initialize()
        {
            LoopShopList();
            itemSpawnerParent = UiHandler.contentParent.Find("ItemSpawner/Body").gameObject;
            categorieView = itemSpawnerParent.transform.Find("Categories").gameObject;
            itemView = itemSpawnerParent.transform.Find("ItemMenu").gameObject;
            carMenuView = itemSpawnerParent.transform.Find("CarParts").gameObject;
            engineMenuView = itemSpawnerParent.transform.Find("EngineParts").gameObject;

            categorieParent = itemSpawnerParent.transform.Find("Categories/Viewport/Content").gameObject;
            categorieContainer = categorieParent.transform.Find("CategorieContainer").gameObject;

            itemParent = itemSpawnerParent.transform.Find("ItemMenu/ItemScroll/Viewport/Content").gameObject;
            itemContainer = itemParent.transform.Find("ItemContainer").gameObject;

            carPartParent = itemSpawnerParent.transform.Find("CarParts/Viewport/Content").gameObject;
            carPartContainer = carPartParent.transform.Find("CategorieContainer").gameObject;

            enginePartParent = itemSpawnerParent.transform.Find("EngineParts/Viewport/Content").gameObject;
            enginePartContainer = enginePartParent.transform.Find("CategorieContainer").gameObject;

            categorieContainer.SetActive(false);
            itemContainer.SetActive(false);
            carPartContainer.SetActive(false);
            enginePartContainer.SetActive(false);
        }

        internal static void SetupCategories()
        {

            categorieView.SetActive(true);
            itemView.SetActive(false);
            carMenuView.SetActive(false);
            engineMenuView.SetActive(false);

            if (firstSetup) return;


            foreach (KeyValuePair<string, string> kvp in categories) 
            {

                Sprite itemImage = GetIcon.itemFromString(kvp.Value);
                GameObject temp_cat = UnityEngine.Object.Instantiate(categorieContainer);
                temp_cat.name = kvp.Key;
                temp_cat.transform.Find("CatName").GetComponent<TMPro.TextMeshProUGUI>().text = kvp.Key;
                if(itemImage != null) { temp_cat.transform.Find("CatImg").GetComponent<Image>().sprite = itemImage; }
                temp_cat.transform.Find("CatSelect").GetComponent<Button>().onClick.AddListener(delegate { LoadItemMenu(kvp.Key); });
                temp_cat.transform.SetParent(categorieParent.transform);
                temp_cat.SetActive(true);
            }

            foreach (KeyValuePair<string, string> kvp in engineTypes)
            {

                Sprite itemImage = GetIcon.itemFromString(kvp.Value);
                GameObject temp_cat = UnityEngine.Object.Instantiate(enginePartContainer);
                temp_cat.name = kvp.Key;
                temp_cat.transform.Find("CatName").GetComponent<TMPro.TextMeshProUGUI>().text = kvp.Key;
                if (itemImage != null) { temp_cat.transform.Find("CatImg").GetComponent<Image>().sprite = itemImage; }
                temp_cat.transform.Find("CatSelect").GetComponent<Button>().onClick.AddListener(delegate { LoadItemMenu("engineparts", null, kvp.Key); });
                temp_cat.transform.SetParent(enginePartParent.transform);
                temp_cat.SetActive(true);
            }


            foreach (GameObject carObjct in T_CarSpawner.cars)
            {
                if(carBlacklist.Contains(carObjct.name)) { continue; }
                Sprite itemImage = GetIcon.carFromString(carObjct.name);
                GameObject tempContainer = UnityEngine.Object.Instantiate(carPartContainer);
                tempContainer.transform.SetParent(carPartParent.transform);
                tempContainer.name = carObjct.name;
                tempContainer.transform.Find("CatName").GetComponent<TMPro.TextMeshProUGUI>().text = carObjct.name;
                tempContainer.SetActive(true);
                if (itemImage != null) { tempContainer.transform.Find("CatImg").GetComponent<Image>().sprite = itemImage; }
                tempContainer.transform.Find("CatSelect").GetComponent<Button>().onClick.AddListener(delegate { LoadItemMenu("carparts", carObjct); });
            }

            firstSetup = true;
        }

        

        private static void LoadItemMenu(string menuName, GameObject extraObject = null, string extrastring = null)
        {

            foreach (Transform child in itemParent.transform)
            {
                if(child.name != "ItemContainer") { GameObject.Destroy(child.gameObject); }
            }

            if (menuName == "All Items")
            {
                foreach (GameObject tool in partList)
                {
                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }


            if (menuName == "Tools")
            {
                foreach(GameObject tool in toolsList)
                {
                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }

            if (menuName == "Fluids")
            {
                foreach (GameObject tool in fluidList)
                {
                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }

            if (menuName == "Rims")
            {
                foreach (GameObject tool in rimList)
                {
                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }

            if (menuName == "Tires")
            {
                foreach (GameObject tool in wheelsList)
                {
                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }

            if(menuName == "carparts")
            {

                foreach (GameObject tool in partList)
                {
                    if (!tool.GetComponent<Partinfo>() || !tool.GetComponent<Partinfo>().FitsToCar.Contains(extraObject.name)) { continue; }
                    if (tool.GetComponent<Partinfo>().Engine || tool.GetComponent<Partinfo>().FitsToEngine.Length>0) { continue; }

                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }

            if (menuName == "engineparts")
            {

                foreach (GameObject tool in partList)
                {
                    if (!tool.GetComponent<Partinfo>() || !tool.GetComponent<Partinfo>().FitsToEngine.Contains(extrastring) || !tool.GetComponent<Partinfo>().Engine) { continue; }

                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    temp_item.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SpawnItem(tool); });
                    temp_item.SetActive(true);
                }
            }

            itemView.SetActive(true);
            categorieView.SetActive(false);
            carMenuView.SetActive(false);
            engineMenuView.SetActive(false);

            if (menuName == "Car parts")
            {
                carMenuView.SetActive(true);
                itemView.SetActive(false);
                return;
            }

            if (menuName == "Engine parts")
            {
                engineMenuView.SetActive(true);
                itemView.SetActive(false);
                return;
            }

        }

        private static void SpawnItem(GameObject objectName)
        {
            GameObject newObject = GameObject.Instantiate(objectName, Camera.main.transform.position + Camera.main.transform.forward * 1f, Quaternion.identity);
            newObject.name = objectName.name.Replace("(Clone)", "");
        }

        private static void LoopShopList()
        {
            foreach (Transform part in GameObject.Find("SHOPITEMS").GetComponentsInChildren(typeof(Transform)))
            {
                if (part.GetComponent<SaleItem>() != null)
                {
                    partList.Add(part.GetComponent<SaleItem>().Item);

                    GameObject temp_item = part.GetComponent<SaleItem>().Item;

                    if (temp_item.GetComponent<CarProperties>() == null)
                    {
                        if (temp_item.GetComponent<PickupTool>() != null && temp_item.GetComponent<PickupTool>().FluidContainer)
                        {
                            fluidList.Add(temp_item);
                            continue;
                        }
                        if (temp_item.GetComponent<PickupTool>() != null && !temp_item.GetComponent<PickupTool>().paint)
                        {
                            toolsList.Add(temp_item);
                        }
                    }
                    else
                    {
                        if (temp_item.GetComponent<Partinfo>() != null && temp_item.GetComponent<Partinfo>().Rim)
                        {
                            rimList.Add(temp_item);
                        }
                        if (temp_item.GetComponent<Partinfo>() != null && temp_item.GetComponent<Partinfo>().Tire)
                        {
                            wheelsList.Add(temp_item);
                        }
                    }
                }
            }
            foreach (GameObject part in GameObject.Find("PartsParent").GetComponent<JunkPartsList>().Parts)
            {
                if (!partList.Contains(part))
                {
                    if (part.GetComponent<Partinfo>() != null)
                    {
                        partList.Add(part);
                    }
                }

            }
        }

    }
}
