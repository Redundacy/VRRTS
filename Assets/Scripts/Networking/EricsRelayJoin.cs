using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EricsRelayJoin : MonoBehaviour
{
    public TMP_Text JoinCodeText;
    public TMP_InputField JoinCodeInputField;
    public UnityTransport transport;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void OnHost()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        JoinCodeText.text = newJoinCode;

        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    //public async void OnHost()
    //{
    //    await StartHostWithRelay();
    //    NetworkManager.Singleton.SceneManager.LoadScene("Pond", LoadSceneMode.Single);
    //}
    //public async Task<string> StartHostWithRelay(int maxConnections = 5)
    //{
    //    await UnityServices.InitializeAsync();
    //    if (!AuthenticationService.Instance.IsSignedIn)
    //    {
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    }
    //    Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
    //    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
    //    var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    //    JoinCodeText.text = joinCode;
    //    return NetworkManager.Singleton.StartHost() ? joinCode : null;
    //}

    public async void OnJoin()
    {
        Debug.Log(JoinCodeInputField.text);
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(JoinCodeInputField.text);
        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }

    //public async void OnJoin()
    //{
    //    await StartClientWithRelay(JoinCodeInputField.text);
    //}
    //public async Task<bool> StartClientWithRelay(string joinCode)
    //{
    //    await UnityServices.InitializeAsync();
    //    if (!AuthenticationService.Instance.IsSignedIn)
    //    {
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    }

    //    var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
    //    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
    //    return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    //}
}
