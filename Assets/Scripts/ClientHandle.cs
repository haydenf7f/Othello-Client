using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
