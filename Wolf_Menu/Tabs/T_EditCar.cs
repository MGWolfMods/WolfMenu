using FluffyUnderware.DevTools.Extensions;
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
    internal class T_EditCar
    {
        private static GameObject rootBody;
        private static GameObject mainBody;
        private static GameObject errorScreen;

        private static GameObject leftBody;
        private static GameObject rightBody;

        private static Image carImage;
        private static TMPro.TextMeshProUGUI carName;
        private static TMPro.TextMeshProUGUI carInfos;
        private static Button sellButton;
        private static Button teleportButton;
        private static Button deleteButton;

        private static TMPro.TMP_InputField mileageInput;
        private static Button mileageButton;

        private static Slider powerSlider;
        private static TMPro.TextMeshProUGUI powerText;
        private static Button powerButton;

        private static Slider idleRPMSlider;
        private static TMPro.TextMeshProUGUI idleRPMText;
        private static Button idleRPMButton;

        private static Button gearButton;
        private static GameObject gearContainer;

        private static CarManager carManager;

        internal static void Initialize()
        {

            carManager = UiHandler.player.GetComponent<CarManager>();

            rootBody = UiHandler.contentParent.Find("EditCar/Body").gameObject;
            mainBody = rootBody.transform.Find("HorizontalList").gameObject;
            errorScreen = rootBody.transform.Find("NoCar").gameObject;
            leftBody = mainBody.transform.Find("Left/Body").gameObject;
            rightBody = mainBody.transform.Find("Right/Body").gameObject;

            carImage = leftBody.transform.Find("CarInfo/CarImage").GetComponent<Image>();
            carName = leftBody.transform.Find("CarInfo/CarName").GetComponent<TMPro.TextMeshProUGUI>();
            carInfos = leftBody.transform.Find("CarInfo/CarInfos").GetComponent<TMPro.TextMeshProUGUI>();

            sellButton = leftBody.transform.Find("CarInfo/ActionButtons/Sell/Button").GetComponent<Button>();
            teleportButton = leftBody.transform.Find("CarInfo/ActionButtons/Teleport/Button").GetComponent<Button>();
            deleteButton = leftBody.transform.Find("CarInfo/ActionButtons/Delete/Button").GetComponent<Button>();

            mileageInput = leftBody.transform.Find("Mileage/InputField").GetComponent<TMPro.TMP_InputField>();
            mileageButton = leftBody.transform.Find("Mileage/Button").GetComponent <Button>();

            powerSlider = rightBody.transform.Find("Power/Slider").GetComponent<Slider>();
            powerText = rightBody.transform.Find("Power/Text").GetComponent<TMPro.TextMeshProUGUI>();
            powerButton = rightBody.transform.Find("Power/Button").GetComponent<Button>();

            idleRPMSlider = rightBody.transform.Find("IdleRPM/Slider").GetComponent<Slider>();
            idleRPMText = rightBody.transform.Find("IdleRPM/Text").GetComponent<TMPro.TextMeshProUGUI>();
            idleRPMButton = rightBody.transform.Find("IdleRPM/Button").GetComponent<Button>();

            gearButton = rightBody.transform.Find("Transmission/Button").GetComponent<Button>();
            gearContainer = rightBody.transform.Find("GearContainer").gameObject;
        }

        internal static void LoadCar(GameObject car = null)
        {
            if(car == null) 
            {
                car = carManager.getClosestCar();
                if(car == null) { 
                    SwitchView(errorScreen);
                    return; 
                }
            }

            foreach (Transform child in rightBody.transform)
            {
                if (child.name.Contains("gearratio")) { GameObject.Destroy(child.gameObject); }
            }

            
            NWH.VehiclePhysics2.VehicleController vehicleController = car.GetComponent<NWH.VehiclePhysics2.VehicleController>();

            carImage.sprite = Sprite.Create(RuntimePreviewGenerator.GenerateModelPreview(car.transform, 200, 200, false), new Rect(0f, 0f, 200f, 200f), new Vector2(0.5f, 0.5f), 100f);
            carName.text = car.name.Replace("(Clone)", "");
            carInfos.text = GetCarDescription(car);

            mileageButton.onClick.RemoveAllListeners();
            mileageButton.onClick.AddListener(delegate { ChangeMileage(car, mileageInput.text); });
            SetUpButtons(car);


            powerSlider.onValueChanged.RemoveAllListeners();
            powerSlider.onValueChanged.AddListener(delegate { powerText.text = powerSlider.value.ToString("0"); });
            powerSlider.value = GetHorsePower(car);
            powerButton.onClick.RemoveAllListeners();
            powerButton.onClick.AddListener(delegate { ChangeHorsePower(car, powerSlider.value); });

            idleRPMSlider.onValueChanged.RemoveAllListeners();
            idleRPMSlider.onValueChanged.AddListener(delegate { idleRPMText.text = idleRPMSlider.value.ToString("0"); });
            idleRPMSlider.value = vehicleController.powertrain.engine.idleRPM;
            idleRPMButton.onClick.RemoveAllListeners();
            idleRPMButton.onClick.AddListener(delegate { ChangeIdleRPM(car, idleRPMSlider.value); });

            List<Slider> gearRatio = new List<Slider>();
            gearContainer.SetActive(false);

            foreach (float gear in vehicleController.powertrain.transmission.ForwardGears)
            {
                int index = vehicleController.powertrain.transmission.ForwardGears.IndexOf(gear) + 1;
                GameObject temp_container = GameObject.Instantiate(gearContainer);
                temp_container.name = "gearratio " + index;
                temp_container.transform.SetParent(rightBody.transform);
                temp_container.transform.Find("Gear").GetComponent<TMPro.TextMeshProUGUI>().text = index.ToString();
                Slider temp_slider = temp_container.transform.Find("Slider").GetComponent<Slider>();
                TMPro.TextMeshProUGUI temp_text = temp_container.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
                temp_slider.onValueChanged.AddListener(delegate { temp_text.text = temp_slider.value.ToString("0.#"); });
                temp_slider.value = gear;
                temp_container.SetActive(true);
                gearRatio.Add(temp_slider);
            }

            gearButton.onClick.RemoveAllListeners();
            gearButton.onClick.AddListener(delegate { ChangeGearRatio(car, gearRatio); });


            if (vehicleController.powertrain.transmission.ForwardGearCount <= 9) 
            {
                rightBody.transform.Find("Transmission").gameObject.SetActive(false);
            }


            SwitchView(mainBody);
        }

        internal static float GetHorsePower(GameObject car)
        {
            float outputHP = 0f;
            MainCarProperties mainCarProperties = car.GetComponent<MainCarProperties>();
            if(mainCarProperties.EngineBlock) { outputHP = mainCarProperties.EngineBlock.Power; }
            return outputHP;
        }

        internal static void ChangeGearRatio(GameObject car, List<Slider> newValues)
        {

            NWH.VehiclePhysics2.VehicleController vehicleController = car.GetComponent<NWH.VehiclePhysics2.VehicleController>();

            foreach (Slider slider in newValues)
            {
                int index = newValues.IndexOf(slider);
                vehicleController.powertrain.transmission.ForwardGears[index] = slider.value;
            }
            LoadCar(car);
        }

        internal static void ChangeIdleRPM(GameObject car, float newValue)
        {
            NWH.VehiclePhysics2.VehicleController vehicleController = car.GetComponent<NWH.VehiclePhysics2.VehicleController>();
            vehicleController.powertrain.engine.idleRPM = newValue;
            LoadCar(car);
        }

        internal static void ChangeHorsePower(GameObject car, float newValue)
        {
            MainCarProperties mainCarProperties = car.GetComponent<MainCarProperties>();
            if (mainCarProperties.EngineBlock) { mainCarProperties.EngineBlock.Power = newValue; }
            LoadCar(car);
        }

        internal static void ChangeMileage(GameObject car, string newMileage)
        {
            MainCarProperties mainCarProperties = car.GetComponent<MainCarProperties>();

            mainCarProperties.Mileage = float.Parse(newMileage, CultureInfo.InvariantCulture.NumberFormat);
            if (mainCarProperties.Cluster)
            {
                mainCarProperties.Cluster.MileageText.text = newMileage;
                mainCarProperties.Cluster.transform.Find("Canvas/Text").GetComponent<Text>().text = newMileage;
            }
            LoadCar(car);
        }

        private static void SwitchView(GameObject view)
        {
            foreach (Transform child in rootBody.transform)
            {
                child.gameObject.SetActive(false);
            }

            view.SetActive(true);
        }

        private static void SetUpButtons(GameObject car)
        {
            sellButton.onClick.RemoveAllListeners();
            teleportButton.onClick.RemoveAllListeners();
            deleteButton.onClick.RemoveAllListeners();

            teleportButton.onClick.AddListener(delegate { T_CarList.TeleportToCar(car); });
            deleteButton.onClick.AddListener(delegate {
                if(carManager.playerCar != car) { GameObject.Destroy(car); }
            });
        }

        private static string GetCarDescription(GameObject car)
        {
            MainCarProperties mainCarProperties = car.GetComponent<MainCarProperties>();
            NWH.VehiclePhysics2.VehicleController vehicleController = car.GetComponent<NWH.VehiclePhysics2.VehicleController>();

            bool hasEngine = mainCarProperties.EngineBlock ? true : false;
            string carDescription = "";
            string EngineName = "None";
            string HorsePower = "0";
            string CarMass = vehicleController.mass.ToString("N0");
            string Mileage = mainCarProperties.Mileage.ToString("N0");

            if (hasEngine)
            {
                EngineName = mainCarProperties.EngineBlock.PartNameExtension;
                HorsePower = mainCarProperties.EngineBlock.Power.ToString("N0");
            }

            carDescription = "Mileage: " + Mileage + "@Mass: " + CarMass + "@HP: " + HorsePower + "@Engine: " + EngineName;
            carDescription = carDescription.Replace("@", Environment.NewLine);

            return carDescription;
        }

    }
}
