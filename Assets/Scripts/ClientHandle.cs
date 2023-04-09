using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _clientID = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.clientID = _clientID;
        ClientSend.WelcomeReceived();
    }

    public static void ServerMessage(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _clientID = _packet.ReadInt();
        Debug.Log($"Message from server: {_msg}");
    }

    public static void StartGame(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log($"Received a start game request from server: {_msg}");
        SceneManager.LoadScene("Game");
    }

    public static void GameUpdate(Packet _packet)
    {
        // TODO: Implement
    }
}
