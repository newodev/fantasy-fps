using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCameraManager : MonoBehaviour
{
    [SerializeField]
    private GameObject cam;

    void Start()
    {
        NetworkIdentity id = transform.parent.GetComponent<NetworkIdentity>();

        // If we are a client, only keep our own camera
        if (!id.isLocalPlayer)
        {
            cam.SetActive(false);
            return;
        }
        // If we are a server, disable all cameras
        if (id.isServer && !id.isLocalPlayer)
        {
            cam.SetActive(false);
        }
    }
}
