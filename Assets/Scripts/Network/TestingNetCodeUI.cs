using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;

    void Hide()
    {
        gameObject.SetActive(false);
    }
    public void StartHost()
    {
        Debug.Log("Host started");
        Multiplayer.Instance.StartGameHost();
        Hide();
    }

    public void StartClient()
    {
        Debug.Log("client started");
        Multiplayer.Instance.StartGameClient();
        Hide();
    }
}
