using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private Canvas _menuCanvas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuCanvas.gameObject.SetActive(!_menuCanvas.isActiveAndEnabled);
        }
    }

    public void Disconnect()
    {
        if (SceneHandler.Instance == null)
            return;

        StopCoroutine(SceneHandler.Instance.tryConnect);

        InstanceFinder.ClientManager.StopConnection();
        Debug.Log("Disconnected");

        SceneManager.LoadScene("Menu");
    }
}
