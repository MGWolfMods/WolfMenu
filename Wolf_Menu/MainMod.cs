using EnviroSamples;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Wolf_Menu.Misc;
using Wolf_Menu.Tabs;

namespace Wolf_Menu
{
    public class MainMod : Mod
    {
        public override string ID => "wolf.menu";
        public override string Name => "Wolf Menu";
        public override string Author => "wolfmods";
        public override string Version => "beta";

        //game vars
        private GameObject player;
        private FirstPersonAIO fpsController;
        private CarManager carManager;

        //wolfmenu vars
        private GameObject wolfMenuCanvas;
        private string menuNameString = "WolfMenu";


        private Texture2D steamImage;
        private string steamUsername;

        public MainMod()
        {
            AssetBundle menuBundle = AssetBundle.LoadFromMemory(Properties.Resources.wolfmenu);
            wolfMenuCanvas = menuBundle.LoadAsset<GameObject>(menuNameString);
            menuBundle.Unload(false);
        }

        public override void OnLoad()
        {

            //initzialize vars
            player = GameObject.Find("Player");
            carManager = player.AddComponent<CarManager>();
            fpsController = player.GetComponent<FirstPersonAIO>();
            player.AddComponent<NoClip>();

            wolfMenuCanvas = UnityEngine.Object.Instantiate(wolfMenuCanvas);
            wolfMenuCanvas.name = menuNameString;
            wolfMenuCanvas.SetActive(false);


            UiHandler.menuCanvas = wolfMenuCanvas;
            UiHandler.fpsController = fpsController;
            UiHandler.GameLoad();

            if (steamUsername != null)
            {
                GameObject steamParent = wolfMenuCanvas.transform.Find("Body/GridHori/MenuBar/Footer/SteamUser").gameObject;
                if(steamUsername.Length > 7) { steamUsername = steamUsername.Substring(0, 5) + ".."; }
                steamParent.transform.Find("Settings/SteamUsername").GetComponent<TMPro.TextMeshProUGUI>().text = steamUsername;
                if(steamImage != null) 
                { 
                    steamParent.transform.Find("UserImageMask/UserImage").GetComponent<RawImage>().texture = steamImage;
                    steamParent.transform.Find("UserImageMask/UserImage").transform.localScale = new Vector3(1, -1, 1);
                }      
            }

            //GameObject steamParent2 = wolfMenuCanvas.transform.Find("Body/GridHori/MenuBar/Footer/SteamUser").gameObject;
            //steamParent2.transform.Find("Settings/SteamUsername").GetComponent<TMPro.TextMeshProUGUI>().text = "WolfMods";
        }

        public override void OnMenuLoad()
        {
            if (SteamManager.Initialized)
            {
                steamUsername = SteamFriends.GetPersonaName();
                steamImage = GetSteamImageAsTexture2D(SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID()));
            }

        }

        public static Texture2D GetSteamImageAsTexture2D(int iImage)
        {
            Texture2D ret = null;
            uint ImageWidth;
            uint ImageHeight;
            bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

            if (bIsValid)
            {
                byte[] Image = new byte[ImageWidth * ImageHeight * 4];

                bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
                if (bIsValid)
                {
                    ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                    ret.LoadRawTextureData(Image);
                    ret.Apply();
                }
            }

            return ret;
        }


        public override void Update()
        {
            if (Input.GetKeyDown(KeybindHandler.toggleMenu))
            {
                UiHandler.ChangeState(!wolfMenuCanvas.activeSelf);
            }

            if(Input.GetKeyDown(KeyCode.Escape) && wolfMenuCanvas.activeSelf)
            {
                UiHandler.ChangeState(false);
            }

            T_Teleport.SetActivePlayerPos();
            KeybindHandler.SavingKeystroke();
        }
    }
}