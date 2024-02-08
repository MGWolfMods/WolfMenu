using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Wolf_Menu.Misc;

namespace Wolf_Menu.Tabs
{
    internal class T_Player
    {
        private static GameObject menuParent;
        private static Button menuHotkey;
        private static Button noclipHotkey;

        private static Toggle noClipToggle;

        private static Slider moneySlider;
        private static TMPro.TMP_InputField moneyInput;
        private static Button moneyAdd;
        private static Button moneyRemove;

        private static TMPro.TextMeshProUGUI timeText;
        private static Slider timeSlider;
        private static Button timeButton;

        private static Button resetSpawner;

        private static TMPro.TMP_Dropdown weatherDropdown;
        private static Button weatherButton;

        private static TMPro.TMP_Dropdown seasonDropdown;
        private static Button seasonButton;

        private static Slider snowSlider;
        private static Button snowButton;

        private static Button openCustomFolder;

        internal static void Initialize()
        {
            menuParent = UiHandler.contentParent.Find("PlayerMenu/Body/PlayerMenu").gameObject;

            menuHotkey = menuParent.transform.Find("Menu/Hotkey").GetComponent<Button>();
            menuHotkey.onClick.AddListener(delegate { KeybindHandler.changeKeybind("toggleMenu", RefreshHotkeys); });

            noclipHotkey = menuParent.transform.Find("Noclip/Hotkey").GetComponent<Button>();
            noclipHotkey.onClick.AddListener(delegate { KeybindHandler.changeKeybind("noClip", RefreshHotkeys); });
            noClipToggle = menuParent.transform.Find("Noclip/Toggle").GetComponent<Toggle>();
            noClipToggle.onValueChanged.AddListener(delegate { NoClip.ToggleNoClip(noClipToggle.isOn); });

            moneySlider = menuParent.transform.Find("Money/Slider").GetComponent<Slider>();
            moneyInput = menuParent.transform.Find("Money/InputField").GetComponent<TMPro.TMP_InputField>();
            moneyAdd = menuParent.transform.Find("Money/Add").GetComponent<Button>();
            moneyRemove = menuParent.transform.Find("Money/Remove").GetComponent<Button>();
            moneySlider.onValueChanged.AddListener(delegate { moneyInput.text = moneySlider.value.ToString(); });
            moneyAdd.onClick.AddListener(delegate { ChangeMoney(moneyInput.text); });
            moneyRemove.onClick.AddListener(delegate { ChangeMoney("-"+moneyInput.text); });
            moneyInput.onValueChanged.AddListener(delegate { moneySlider.value = Int32.Parse(moneyInput.text); });


            timeSlider = menuParent.transform.Find("TimeWeather/SetTime/Slider").GetComponent<Slider>();
            timeText = timeSlider.transform.Find("TimeText").GetComponent<TMPro.TextMeshProUGUI>();
            timeButton = menuParent.transform.Find("TimeWeather/SetTime/Button").GetComponent<Button>();
            timeSlider.onValueChanged.AddListener(delegate { timeText.text = GetTimeFromSlider(timeSlider.value); });
            timeButton.onClick.AddListener(delegate { TimeAndWeather.SetTime(timeSlider.value); });

            resetSpawner = menuParent.transform.Find("TimeWeather/ResetSpawner/Reset").GetComponent<Button>();
            resetSpawner.onClick.AddListener(delegate { TimeAndWeather.ResetSpawners(); });

            weatherDropdown = menuParent.transform.Find("TimeWeather/SetWeather/Filter").GetComponent<TMPro.TMP_Dropdown>();
            weatherButton = menuParent.transform.Find("TimeWeather/SetWeather/Button").GetComponent<Button>();
            weatherButton.onClick.AddListener(delegate { TimeAndWeather.SetWeather(weatherDropdown.value); });

            seasonDropdown = menuParent.transform.Find("TimeWeather/SetSeason/Filter").GetComponent<TMPro.TMP_Dropdown>();
            seasonButton = menuParent.transform.Find("TimeWeather/SetSeason/Button").GetComponent<Button>();
            seasonButton.onClick.AddListener(delegate { TimeAndWeather.SetWeather(seasonDropdown.value); });

            snowSlider = menuParent.transform.Find("TimeWeather/SetSnow/Slider").GetComponent<Slider>();
            snowButton = menuParent.transform.Find("TimeWeather/SetSnow/Button").GetComponent<Button>();
            snowButton.onClick.AddListener(delegate { TimeAndWeather.SetSnow(snowSlider.value); });

            openCustomFolder = menuParent.transform.Find("TimeWeather/ShowCustom/Show").GetComponent<Button>();
            openCustomFolder.onClick.AddListener(delegate { Process.Start(JsonParser.imageFoler); });

            RefreshHotkeys();
        }

        private static void RefreshHotkeys()
        {
            menuHotkey.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = KeybindHandler.toggleMenu.ToString().Replace("Alpha", "");
            noclipHotkey.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = KeybindHandler.noClip.ToString().Replace("Alpha", "");
        }

        private static string GetTimeFromSlider(float value)
        {
            if (value<=0)
            {
                return "TIME: 00:00";
            }
            string str_value = value.ToString("0.00");
            string[] array = str_value.ToString().Split(',');
            int hrs = Int32.Parse(array[0]);
            double mins = Int32.Parse(array[1]) / 1.6666;
            string finalTime = "TIME: " + hrs.ToString("00") + ":" + mins.ToString("00");
            return finalTime;
        }

        private static void ChangeMoney(string amount)
        {
            int intAmount = Int32.Parse(amount);
            tools.money += intAmount;
        }

        internal static void ChangeNoClipToggle(bool toggle)
        {
            noClipToggle.isOn = toggle;
        }

    }
}
