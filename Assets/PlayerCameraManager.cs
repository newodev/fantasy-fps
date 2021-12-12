using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCameraManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject cam;

    void Start()
    {
        // If we are a client, only keep our own camera
        if (!isLocalPlayer)
        {
            Debug.Log("not local!!");
            cam.SetActive(false);
            return;
        }
        // If we are a server, disable all cameras
        if (isServer && !isLocalPlayer)
        {
            Debug.Log("ams erverr!!!");
            cam.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
