using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using FishNet;
using FishNet.Utility;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using GameKit.Utilities;
using GameKit.Utilities.Types;
using FishNet.Managing.Transporting;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    private ConnectionSetter _connectionSetter;

    public Coroutine tryConnect {private set; get;}

    public float timeoutTimer {private set; get;}

    [SerializeField, Scene]
    private string _onlineScene;
    public void SetOnlineScene(string sceneName) => _onlineScene = sceneName;

    [SerializeField, Scene]
    private string _aRScene;
    public void SetARScene(string sceneName) => _aRScene = sceneName;

    [SerializeField, Scene]
    private string _offlineScene;
    public void SetOfflineScene(string sceneName) => _offlineScene = sceneName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);

            return;
        }

        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (_connectionSetter != null) return;

        _connectionSetter = FindObjectOfType<ConnectionSetter>();
    }

    public void LoadPlayer()
    {
        tryConnect = StartCoroutine(TryConnect(false, false));
    }

    public void LoadPlayerAR()
    {
        tryConnect = StartCoroutine(TryConnect(false, true));
    }

    public void LoadServer()
    {
        tryConnect = StartCoroutine(TryConnect(true, false));
    }

    private IEnumerator TryConnect(bool asServer, bool asAR)
    {
        if (!asAR)
            SceneManager.LoadScene(_onlineScene);
        else
            SceneManager.LoadScene(_aRScene);

        Tugboat transport = (Tugboat)InstanceFinder.TransportManager.Transport;

        if (transport.GetClientAddress() == "localhost" && transport.GetConnectionState(true) != LocalConnectionState.Started)
            transport.StartConnection(true); // try start server

        if (!asServer)
            transport.StartConnection(false); // try start client
        
        timeoutTimer = 0f;

        do
        {
            // Do another safety check here to see if he can connect
            if (transport.GetConnectionState(false) == LocalConnectionState.Started || transport.GetConnectionState(true) == LocalConnectionState.Started)
            {
                Debug.Log("Connected");
                timeoutTimer = transport.GetTimeout(false);
            }
            else
            {
                timeoutTimer += Time.unscaledDeltaTime;
                Debug.Log("Timeout timer: " + timeoutTimer);
            }
            
            yield return null;
        }
        while (timeoutTimer < transport.GetTimeout(false));

        if (!asServer)
        {
            // load offline scene
            if (transport.GetConnectionState(false) == LocalConnectionState.Stopped)
            {
                Debug.Log("Server is invalid, try another address");
                SceneManager.LoadScene(_offlineScene);
            }
        }
    }
}
