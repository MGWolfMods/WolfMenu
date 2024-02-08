using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wolf_Menu.Misc
{
    public class CarListing : MonoBehaviour
    {
        CarManager carManager;
        GameObject player;

        public void Start()
        {
            player = GameObject.Find("Player");
            carManager = player.GetComponent<CarManager>();

            carManager.carObjects.Add(this.gameObject);
        }

        public void OnDestroy()
        {
            try
            {
                carManager.carObjects.Remove(this.gameObject);
            }
            catch {}
        }
    }
}
