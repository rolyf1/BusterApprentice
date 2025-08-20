using UnityEngine;
using UnityEngine.UIElements;


namespace TiltFive
{
    public class TrackPlayer : MonoBehaviour
    {

        public Transform PlayerPos;
        private Vector3 _cameraPos = new Vector3();
        public
            float _lerpFactor = 0.1f;

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

            _cameraPos = new Vector3 (PlayerPos.position.x, PlayerPos.position.y + 12, PlayerPos.position.z - 6);
            transform.position = Vector3.Lerp(transform.position, _cameraPos, _lerpFactor);
        }
    }
}

