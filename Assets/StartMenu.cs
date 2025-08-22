using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private float _Yoffset = 0;
    private float _Zoffset = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + _Yoffset, transform.position.z + _Zoffset);
        _Yoffset += 0.0003f;
        _Zoffset += 0.0003f;
    }
}
