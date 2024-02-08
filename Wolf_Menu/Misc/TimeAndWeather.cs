using GlobalSnowEffect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wolf_Menu.Misc
{
    internal class TimeAndWeather
    {
        internal static void SetTime(float time)
        {
            EnviroSkyMgr.instance.SetTimeOfDay(time);

            foreach (BatteryCharger battery in UnityEngine.Object.FindObjectsOfType<BatteryCharger>()) { battery.PassTime(); }
        }

        internal static void SetSeason(int season)
        {
            switch (season)
            {
                case 0:
                    EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years, 0, EnviroSkyMgr.instance.Time.Hours, EnviroSkyMgr.instance.Time.Minutes, 0);
                    break;
                case 1:
                    EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years, 2, EnviroSkyMgr.instance.Time.Hours, EnviroSkyMgr.instance.Time.Minutes, 0);
                    break;
                case 2:
                    EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years, 5, EnviroSkyMgr.instance.Time.Hours, EnviroSkyMgr.instance.Time.Minutes, 0);
                    break;
                case 3:
                    EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years, 12, EnviroSkyMgr.instance.Time.Hours, EnviroSkyMgr.instance.Time.Minutes, 0);
                    break;
            }

            EnviroSkyMgr.instance.GetTimeString();
            
        }

        internal static void SetWeather(int weatherID)
        {
            EnviroSkyMgr.instance.ChangeWeather(weatherID);
        }

        internal static void SetSnow(float snowValue)
        {
            if(snowValue <= 0) 
            {
                ToggleSnow(false);
                return;
            }
            ToggleSnow(true);
            UiHandler.pTools.PlayerCam.GetComponent<GlobalSnow>().snowAmount = snowValue;
        }

        private static void ToggleSnow(bool toggle)
        {

            switch (toggle)
            {
                case true:
                    SetSeason(0);
                    break;
                case false:
                    SetSeason(1);
                    UiHandler.pTools.PlayerCam.GetComponent<GlobalSnow>().snowAmount = 0;
                    break;

            }

            UiHandler.pTools.PlayerCam.GetComponent<GlobalSnow>().enabled = toggle;
        }

        internal static void ResetSpawners()
        {
            UiHandler.pTools.ResetSpawners();
        }

    }
}
