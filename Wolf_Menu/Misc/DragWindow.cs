using NWH.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wolf_Menu.Misc
{
    public class DragWindow : MonoBehaviour
    {

        private RectTransform bodyTransform;
        private Canvas canvas;

        void Start()
        {
            bodyTransform = transform.parent.GetComponent<RectTransform>();
            canvas = transform.root.GetComponent<Canvas>();
        }


        public void DragHandler(BaseEventData data)
        {
            
        }
    }
}
