using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using FishNet;

public class ConnectionSetter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _address;

    public void Connect()
    {
        if (SceneHandler.Instance != null)
        {
            InstanceFinder.TransportManager.Transport.SetClientAddress(_address.text);
            SceneHandler.Instance.LoadPlayer();
        }
    }

    public void ConnectServer()
    {
        if (SceneHandler.Instance != null)
        {
            InstanceFinder.TransportManager.Transport.SetClientAddress(_address.text);
            SceneHandler.Instance.LoadServer();
        }
    }
}
