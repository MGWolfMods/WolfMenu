using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Wolf_Menu.Tabs
{
    internal class T_Warning
    {
        private static GameObject popUp;
        private static GameObject mainBody;
        private static TMPro.TextMeshProUGUI headline;
        private static TMPro.TextMeshProUGUI content;

        private static GameObject buttonParent;
        private static GameObject buttonContainer;

        internal static void Initialize()
        {
            popUp = UiHandler.menuCanvas.transform.Find("Popup").gameObject;
            mainBody = UiHandler.menuCanvas.transform.Find("Body").gameObject;
            headline = popUp.transform.Find("Grid/Headline/Title").GetComponent<TMPro.TextMeshProUGUI>();
            content = popUp.transform.Find("Grid/Body/Parent/Textbox/Text").GetComponent <TMPro.TextMeshProUGUI>();

            buttonParent = popUp.transform.Find("Grid/Body/Parent/Buttons").gameObject;
            buttonContainer = buttonParent.transform.Find("ButtonContainer").gameObject;
            buttonParent.SetActive(false);
            buttonContainer.SetActive(false);
            popUp.SetActive(false);
        }

        internal static void ShowWarning(string head, string text, bool needButtons, string[] buttonText = null,UnityAction[] buttonaction = null)
        {
            popUp.SetActive(true);
            mainBody.SetActive(false);
            headline.text = head;
            content.text = text;

            if(needButtons)
            {
                foreach (Transform child in buttonParent.transform)
                {
                    if (child.name != "ButtonContainer") { GameObject.Destroy(child.gameObject); }
                }

                for (int i = 0; i < buttonaction.Length; i++)
                {
                    GameObject tempButton = GameObject.Instantiate(buttonContainer);
                    tempButton.transform.SetParent(buttonParent.transform);
                    tempButton.SetActive(true);
                    TMPro.TextMeshProUGUI tempText = tempButton.transform.Find("Button/Text").GetComponent<TMPro.TextMeshProUGUI>();
                    Button pressButton = tempButton.transform.Find("Button").GetComponent<Button>();
                    tempText.text = buttonText[i];
                    pressButton.onClick.AddListener(buttonaction[i]);
                    tempButton.SetActive(true);
                }

                buttonParent.SetActive(true);
            }
        }

        internal static void CloseWarning()
        {
            popUp.SetActive(false);
            mainBody.SetActive(true);
        }

    }
}
