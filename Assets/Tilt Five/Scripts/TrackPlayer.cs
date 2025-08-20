using StarterAssets;
using TiltFiveCursor;
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
        public Vector3 camRotation = new Vector3(0, 0, 0);

        // Cutscene variables
        private CursorBehaviour _cursor;
        private PlayerController _controller;
        public int cutsceneNum = 0;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (!_cursor.inCutscene && !_controller.camShift) // If player not in cutscene, track player
            {
                camPos = new Vector3(PlayerPos.position.x + camXOffset, PlayerPos.position.y + camYOffset, PlayerPos.position.z + camZOffset);
                transform.position = Vector3.Lerp(transform.position, camPos, 0.1f);
            }
            
        }
    }
}

