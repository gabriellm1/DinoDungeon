using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_background : MonoBehaviour
{

    public GameObject player;
    public GameObject jungle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (player != null)
        {
            if (player.transform.position.y < 154)
            {
                jungle.transform.position = new Vector3(-0.024f, 0.012f, 7.0f);
            }
        }

    }
}
