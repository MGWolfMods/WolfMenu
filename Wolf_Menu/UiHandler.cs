using EnviroSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wolf_Menu.Misc;
using Wolf_Menu.Tabs;

namespace Wolf_Menu
{
    internal class UiHandler
    { 

        internal static GameObject menuCanvas;
        internal static FirstPersonAIO fpsController;
        internal static GameObject player;
        internal static tools pTools;

        public enum Tabs {Main, EditCar, CarSpawn, ItemSpawn, CarList, Teleport, Player};
      
        private static GameObject eventSystem;

        private static bool _menuState = false;
        internal static bool _forceMenuOpen = false;
        internal static Transform contentParent;


        internal static void GameLoad()
        {


            contentParent = menuCanvas.transform.Find("Body/GridHori/ContentPanel").transform;

            foreach (Transform child in contentParent)
            {
                child.gameObject.SetActive(false);
            }
            contentParent.Find("Main Menu").gameObject.SetActive(true);

            player = GameObject.Find("Player").gameObject;
            pTools = player.GetComponent<tools>();

            menuCanvas.transform.Find("Body/GridHori/MenuBar/Body/EditCar/MenuBtn").GetComponent<Button>().onClick.AddListener(delegate { T_EditCar.LoadCar(null); SwitchTabs(Tabs.EditCar); });
            menuCanvas.transform.Find("Body/GridHori/MenuBar/Body/CarSpawner/MenuBtn").GetComponent<Button>().onClick.AddListener(delegate { SwitchTabs(Tabs.CarSpawn); });
            menuCanvas.transform.Find("Body/GridHori/MenuBar/Body/ItemSpawner/MenuBtn").GetComponent<Button>().onClick.AddListener(delegate { SwitchTabs(Tabs.ItemSpawn); });
            menuCanvas.transform.Find("Body/GridHori/MenuBar/Body/CarList/MenuBtn").GetComponent<Button>().onClick.AddListener(delegate { SwitchTabs(Tabs.CarList); });
            menuCanvas.transform.Find("Body/GridHori/MenuBar/Body/Teleporter/MenuBtn").GetComponent<Button>().onClick.AddListener(delegate { SwitchTabs(Tabs.Teleport); });
            menuCanvas.transform.Find("Body/GridHori/MenuBar/Footer/SteamUser/Settings").GetComponent<Button>().onClick.AddListener(delegate { SwitchTabs(Tabs.Player); });
            T_CarSpawner.Initialize();
            T_ItemSpawner.Initialize();
            T_CarList.Initialize();
            T_Teleport.Initialize();
            T_Player.Initialize();
            T_EditCar.Initialize();
            T_Warning.Initialize();

            JsonParser.Initialize();
            KeybindHandler.Initialize();
        }

        internal static void ChangeState(bool state)
        {
            if (_forceMenuOpen) { return; }
            GetIcon.LoadIcons();
            _menuState = state;
            menuCanvas.SetActive(_menuState);

            switch (_menuState)
            {
                case true:
                    eventSystem = new GameObject("EventSystemTEMP");
                    eventSystem.AddComponent<EventSystem>();
                    eventSystem.AddComponent<StandaloneInputModule>();
                    fpsController.ControllerPause();
                    break;
                case false:
                    UnityEngine.Object.Destroy(eventSystem);
                    fpsController.ControllerUnPause();
                    break;
            }

        }

        internal static void SwitchTabs(Tabs tabs)
        { 

            foreach (Transform child in contentParent)
            {
                child.gameObject.SetActive(false);
            }

            switch (tabs)
            {
                case Tabs.Main:
                    contentParent.Find("Main Menu").gameObject.SetActive(true);
                    break;

                case Tabs.EditCar:
                    contentParent.Find("EditCar").gameObject.SetActive(true);
                    break;

                case Tabs.CarSpawn:
                    contentParent.Find("CarSpawner").gameObject.SetActive(true);
                    T_CarSpawner.LoadList();
                    break;

                case Tabs.ItemSpawn:
                    contentParent.Find("ItemSpawner").gameObject.SetActive(true);
                    T_ItemSpawner.SetupCategories();
                    break;
                case Tabs.CarList:
                    contentParent.Find("CarList").gameObject.SetActive(true);
                    T_CarList.LoadList();
                    break;
                case Tabs.Teleport:
                    contentParent.Find("Teleport").gameObject.SetActive(true);
                    T_Teleport.LoadTeleportList();
                    break;
                case Tabs.Player:
                    contentParent.Find("PlayerMenu").gameObject.SetActive(true);
                    break;
            }
        }
    }
}
