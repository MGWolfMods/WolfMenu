using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wolf_Menu.Misc
{
    public class CarManager : MonoBehaviour
    {

        public List<GameObject> carObjects = new List<GameObject>();
        public GameObject playerCar;
        GameObject playerObject = GameObject.Find("Player");

        public void Start()
        {
            StartCoroutine(LoadCars());
            foreach (GameObject car in GameObject.Find("CarsParent").GetComponent<CarList>().Cars) 
            {
                car.AddComponent<CarListing>();
            }
        }



        IEnumerator LoadCars()
        {
            yield return new WaitForSeconds(5);
            GameObject[] temp_carObjects = GameObject.FindGameObjectsWithTag("Vehicle");
            int listLenght = temp_carObjects.Length;

            if (temp_carObjects != null)
            {
                foreach (GameObject car in temp_carObjects)
                {
                    if(car.name.ToLower().Contains("aitrafficcar")) { continue; }
                    if (car.GetComponent<CarListing>() == null)
                    {
                        car.AddComponent<CarListing>();
                    }
                }
            }
        }


        public void Update()
        {
            if (playerObject.transform.root.name != "Player")
            {
                playerCar = playerObject.transform.root.gameObject;
            }
            else
            {
                playerCar = null;
            }
        }

        public GameObject getClosestCar()
        {
            foreach (GameObject car in carObjects)
            {
                float dist = Vector3.Distance(car.transform.position, playerObject.transform.position);
                if (dist < 3.5f)
                {
                    return car;
                }
            }

            return null;
        }


    }
}
