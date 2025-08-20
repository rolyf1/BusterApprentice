using StarterAssets;
using TiltFiveCursor;
using Unity.VisualScripting;
using UnityEngine;


namespace TiltFive
{
    public class TrackPlayer : MonoBehaviour
    {

        public Transform PlayerPos;
        public int camYOffset = 10;
        public int camZOffset = -7;
        public int camXOffset = 0;
        private Vector3 camPos;

        // Cutscene variables

        public int cutsceneNum = 0;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            camPos = new Vector3(PlayerPos.position.x + camXOffset, PlayerPos.position.y + camYOffset, PlayerPos.position.z + camZOffset);
            transform.position = Vector3.Lerp(transform.position, camPos, 0.1f);
        }
    }
}

