using UnityEngine;


namespace TiltFive
{
    public class TrackPlayer : MonoBehaviour
    {

        public Transform PlayerPos;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if (PlayerPos != null)
            {
                Debug.Log("Object has been paired");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            transform.position = PlayerPos.position; // Update the camera's position to match the player's position
        }
    }
}

