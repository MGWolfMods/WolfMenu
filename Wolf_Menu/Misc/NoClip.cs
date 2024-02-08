using UnityEngine;
using Rewired;
using Wolf_Menu.Tabs;

namespace Wolf_Menu.Misc
{

    public class NoClip : MonoBehaviour
    {
        private static GameObject player;

        private static Rigidbody playerRB;
        private static Collider collider1;
        private static Collider collider2;
        private static FirstPersonAIO playerController;
        private static Player playerRewired;
        private static Crouch playerCrouch;


        private static bool noClipEnabled = false;

        void Start ()
        {
            player = transform.gameObject;

            playerRB = player.GetComponent<Rigidbody>();
            collider1 = player.GetComponent<CapsuleCollider>();
            collider2 = player.transform.Find("Capsule").GetComponent<CapsuleCollider>();
            playerController = player.GetComponent<FirstPersonAIO>();
            playerRewired = ReInput.players.GetPlayer(0);
            playerCrouch = player.GetComponent<Crouch>();
        }

        void Update ()
        {
            if (Input.GetKeyDown(KeybindHandler.noClip))
            {
                T_Player.ChangeNoClipToggle(!noClipEnabled);
            }
        }
        void FixedUpdate()
        {
            if (noClipEnabled)
            {

                float speed = Input.GetKey(playerController.sprintKey) ? 40f : 15f;
                float horizontal = playerRewired.GetAxis("Horizontal");
                float vertical = playerRewired.GetAxis("Vertical");
                player.transform.position += Camera.main.transform.forward * vertical * speed * Time.deltaTime + Camera.main.transform.right * horizontal * speed * Time.deltaTime;
            }
        }

        public static void ToggleNoClip(bool state)
        {
            noClipEnabled = state;
            playerController.playerCanMove = !noClipEnabled;
            playerRB.isKinematic = noClipEnabled;
            collider1.isTrigger = noClipEnabled;
            collider2.isTrigger = noClipEnabled;
            playerCrouch.enabled = !noClipEnabled;
        }
    }
}
