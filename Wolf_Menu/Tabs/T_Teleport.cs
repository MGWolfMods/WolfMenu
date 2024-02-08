using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Wolf_Menu.Misc;

namespace Wolf_Menu.Tabs
{
    internal class T_Teleport
    {
        private static GameObject bodyParent;
        private static GameObject teleportSelectView;
        private static GameObject teleportCreateView;
        private static GameObject teleportIconView;

        private static GameObject teleportParent;
        private static GameObject teleportContainer;

        private static GameObject itemParent;
        private static GameObject itemContainer;

        private static GameObject teleportAddCustom;
        private static GameObject teleportFilter;

        private static GameObject teleportCreateName;
        private static Tuple<GameObject, GameObject, GameObject> teleportCreateLocation;
        private static GameObject teleportCreatePlayerPos;
        private static GameObject teleportCreateCustomImage;
        private static GameObject teleportCreateCustomImageRaw;
        private static GameObject teleportCreateChangeImage;
        private static GameObject teleportCreateBtn;

        private static string currentImgName = "";
        private static bool creatorAddedListners = false;


        private static List<TeleportJson> teleportJson = new List<TeleportJson>();

        private static Dictionary<string, Tuple<Vector3, Vector3, string>> teleportPositions = new Dictionary<string, Tuple<Vector3, Vector3, string>>()
        {
            {"Home", Tuple.Create(new Vector3(167.7684f, 63.14f, 543.246f), new Vector3(0f, 326.6771f, 0f), "")},
            {"Car Shop", Tuple.Create(new Vector3(108.0859f, 63.2078f, 640.665f), new Vector3(0f, 234.4753f, 0f), "tire16")},
            {"Junkyard", Tuple.Create(new Vector3(-541.1466f, 53.0532f, -404.4335f), new Vector3(0f, 283.208435f, 0f), "")},
        };

        internal static void Initialize()
        {
            bodyParent = UiHandler.contentParent.Find("Teleport/Body").gameObject;
            teleportSelectView = bodyParent.transform.Find("TeleportSelect").gameObject;
            teleportCreateView = bodyParent.transform.Find("TeleportCreate").gameObject;
            teleportIconView = bodyParent.transform.Find("ItemMenu").gameObject;


            teleportParent = teleportSelectView.transform.Find("TeleportScroll/Viewport/Content").gameObject;
            teleportContainer = teleportParent.transform.Find("TeleportContainer").gameObject;

            teleportAddCustom = teleportSelectView.transform.Find("AddTeleport").gameObject;

            teleportCreateName = teleportCreateView.transform.Find("NameInput/InputField").gameObject;
            teleportCreateLocation = Tuple.Create(
                teleportCreateView.transform.Find("LocationInput/InputFieldX").gameObject,
                teleportCreateView.transform.Find("LocationInput/InputFieldY").gameObject,
                teleportCreateView.transform.Find("LocationInput/InputFieldZ").gameObject);

            teleportCreatePlayerPos = teleportCreateView.transform.Find("PlayerPos/Toggle").gameObject;
            teleportCreateCustomImage = teleportCreateView.transform.Find("CustomImage/Image").gameObject;
            teleportCreateCustomImageRaw = teleportCreateView.transform.Find("CustomImage/ImageRaw").gameObject;
            teleportCreateChangeImage = teleportCreateView.transform.Find("CustomImage/ChangeIcon").gameObject;
            teleportCreateBtn = teleportCreateView.transform.Find("CreateParent/Create").gameObject;


            itemParent = teleportIconView.transform.Find("ItemScroll/Viewport/Content").gameObject;
            itemContainer = itemParent.transform.Find("ItemContainer").gameObject;
            teleportFilter = teleportIconView.transform.Find("Filter").gameObject;


            teleportSelectView.SetActive(false);
            teleportCreateView.SetActive(false);

            teleportContainer.SetActive(false);

            teleportAddCustom.GetComponent<Button>().onClick.AddListener(delegate { LoadCreateTeleport(); });
            teleportFilter.GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(delegate { OnDropdownChanged(); });


            teleportJson = JsonParser.ReadFromJson<TeleportJson>(JsonParser.Json.Teleport);
        }


        private static void LoadCreateTeleport()
        {
            TMPro.TMP_InputField inputName = teleportCreateName.GetComponent<TMPro.TMP_InputField>();
            TMPro.TMP_InputField inputX = teleportCreateLocation.Item1.GetComponent<TMPro.TMP_InputField>();
            TMPro.TMP_InputField inputY = teleportCreateLocation.Item2.GetComponent<TMPro.TMP_InputField>();
            TMPro.TMP_InputField inputZ = teleportCreateLocation.Item3.GetComponent<TMPro.TMP_InputField>();

            if (!creatorAddedListners)
            {
                teleportCreateChangeImage.GetComponent<Button>().onClick.AddListener(delegate { LoadIconList(); });
                teleportCreateBtn.GetComponent<Button>().onClick.AddListener(delegate { ValidateSpawnpoint(inputName.text, inputX.text, inputY.text, inputZ.text); });
                creatorAddedListners = true;
            }

            SwitchView(teleportCreateView);
        }

        private static void LoadIconList()
        {

            foreach (Transform child in itemParent.transform)
            {
                if (child.name != "ItemContainer") { GameObject.Destroy(child.gameObject); }
            }

            itemContainer.SetActive(false);


            int dropdown = teleportFilter.GetComponent<TMPro.TMP_Dropdown>().value;

            if (dropdown == 0)
            {
                foreach (GameObject tool in T_ItemSpawner.partList)
                {
                    Sprite itemImage = GetIcon.itemFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SelectIcon(itemImage, tool.name); });
                    temp_item.SetActive(true);
                }
            }

            if (dropdown == 1)
            {
                foreach (GameObject tool in GameObject.Find("CarsParent").GetComponent<CarList>().Cars)
                {
                    Sprite itemImage = GetIcon.carFromString(tool.name);
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    temp_item.name = tool.name;
                    if (itemImage != null) { temp_item.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SelectIcon(itemImage, tool.name); });
                    temp_item.SetActive(true);
                }
            }

            if (dropdown == 2)
            {
                List<Texture2D> customTextures = GetIcon.customTextures();

                foreach(Texture2D texture in customTextures)
                {
                    GameObject temp_item = UnityEngine.Object.Instantiate(itemContainer);
                    if (texture != null) {
                        temp_item.transform.Find("ItemImg").gameObject.SetActive(false);
                        temp_item.transform.Find("ItemImgRaw").gameObject.SetActive(true);
                        temp_item.transform.Find("ItemImgRaw").GetComponent<RawImage>().texture = texture;
                        temp_item.GetComponent<Button>().targetGraphic = temp_item.transform.Find("ItemImgRaw").GetComponent<Graphic>();
                    }

                    temp_item.transform.SetParent(itemParent.transform);
                    temp_item.GetComponent<Button>().onClick.AddListener(delegate { SelectIcon(null, texture.name, texture); });
                    temp_item.SetActive(true);
                }
            }


            SwitchView(teleportIconView);
        }

        private static void SelectIcon(Sprite iconImg, string imgName, Texture2D texture = null)
        {
            currentImgName = imgName;
            if(iconImg == null && texture != null)
            {
                teleportCreateCustomImageRaw.SetActive(true);
                teleportCreateCustomImage.SetActive(false);
                teleportCreateCustomImageRaw.GetComponent<RawImage>().texture = texture;
            }
            if(iconImg != null) { teleportCreateCustomImage.GetComponent<Image>().sprite = iconImg; }
            LoadCreateTeleport();
        }

        private static void ValidateSpawnpoint(string name, string x, string y, string z)
        {
            if(string.IsNullOrEmpty(name)) return;
            if (string.IsNullOrEmpty(x)) x = "0";
            if (string.IsNullOrEmpty(x)) y = "0";
            if (string.IsNullOrEmpty(x)) z = "0";

            float xValue = float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
            float yValue = float.Parse(y.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
            float zValue = float.Parse(z.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);

            TMPro.TMP_Dropdown dropdown = teleportFilter.GetComponent<TMPro.TMP_Dropdown>();

            Vector3 rot = UiHandler.player.transform.rotation.eulerAngles;

            Tuple<float, float, float> position = Tuple.Create(xValue, yValue, zValue);
            Tuple<float, float, float> roation = Tuple.Create(rot.x, rot.y, rot.z);

            TeleportJson newJson = new TeleportJson(name, position, roation, dropdown.value, currentImgName);

            teleportJson.Add(newJson);
            JsonParser.SaveToJson(teleportJson, JsonParser.Json.Teleport);
            LoadTeleportList();
        }

        internal static void LoadTeleportList()
        {

            foreach(Transform child in teleportParent.transform)
            {
                if(child.name != "TeleportContainer") { GameObject.Destroy(child.gameObject); }
            }

           
            foreach (KeyValuePair<string, Tuple<Vector3, Vector3, string>> kvp in teleportPositions)
            {
                Sprite itemImage = GetIcon.itemFromString(kvp.Value.Item3);
                GameObject temp_cat = UnityEngine.Object.Instantiate(teleportContainer);
                temp_cat.name = kvp.Key;
                temp_cat.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = kvp.Key;
                if (itemImage != null) { temp_cat.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                temp_cat.GetComponent<Button>().onClick.AddListener(delegate { TeleportPlayer(kvp.Value); });
                temp_cat.transform.SetParent(teleportParent.transform);
                temp_cat.SetActive(true);
            }

            foreach(TeleportJson spot in teleportJson)
            {
                Tuple<Vector3, Vector3, string> positions = Tuple.Create(new Vector3(
                    spot.position.Item1,
                    spot.position.Item2,
                    spot.position.Item3), new Vector3(
                        spot.rotation.Item1,
                        spot.rotation.Item2,
                        spot.rotation.Item3), "Test");

                GameObject temp_cat = UnityEngine.Object.Instantiate(teleportContainer);

                if (spot.imageType == 2)
                {
                    temp_cat.transform.Find("ItemImg").gameObject.SetActive(false);
                    temp_cat.transform.Find("ItemImgRaw").gameObject.SetActive(true);
                    Texture2D texture = GetIcon.customFromString(spot.imageName);
                    if(texture != null) { temp_cat.transform.Find("ItemImgRaw").GetComponent<RawImage>().texture = texture; }
                }
                else
                {
                    Sprite itemImage = GetIcon.allFromString(spot.imageName);
                    if (itemImage != null) { temp_cat.transform.Find("ItemImg").GetComponent<Image>().sprite = itemImage; }
                }

                
                
                temp_cat.name = spot.name;
                temp_cat.transform.Find("ItemText").GetComponent<TMPro.TextMeshProUGUI>().text = spot.name;
                temp_cat.GetComponent<Button>().onClick.AddListener(delegate { TeleportPlayer(positions); });
                temp_cat.transform.SetParent(teleportParent.transform);
                temp_cat.transform.Find("Remove").gameObject.SetActive(true);
                temp_cat.transform.Find("Remove").GetComponent<Button>().onClick.AddListener(delegate { RemoveTeleportPoint(spot); });
                temp_cat.SetActive(true);
            }

            SwitchView(teleportSelectView);
        }

        private static void RemoveTeleportPoint(TeleportJson spot)
        {
            teleportJson.Remove(spot);
            JsonParser.SaveToJson(teleportJson, JsonParser.Json.Teleport);
            LoadTeleportList();
        }

        private static void TeleportPlayer(Tuple<Vector3, Vector3, string> tuple)
        {
            GameObject.Find("Player").transform.position = tuple.Item1;
            GameObject.Find("Player").transform.rotation = Quaternion.Euler(tuple.Item2);
        }

        internal static void SetActivePlayerPos()
        {
            if(!teleportCreatePlayerPos.GetComponent<Toggle>().isOn || !teleportCreateView.activeSelf) { return; }

            Vector3 playerPos = UiHandler.player.transform.position;

            teleportCreateLocation.Item1.GetComponent<TMPro.TMP_InputField>().text = playerPos.x.ToString();
            teleportCreateLocation.Item2.GetComponent<TMPro.TMP_InputField>().text = playerPos.y.ToString();
            teleportCreateLocation.Item3.GetComponent<TMPro.TMP_InputField>().text = playerPos.z.ToString();
        }

        private static void OnDropdownChanged()
        {
            LoadIconList();
        }

        private static void SwitchView(GameObject view)
        {
            foreach(Transform child in bodyParent.transform)
            {
                child.gameObject.SetActive(false);
            }

            view.SetActive(true);
        }

    }
}
