using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }
    
    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            _packet.Write(Client.instance.clientID);
            _packet.Write(MenuManager.instance.usernameField.text);
            
            SendTCPData(_packet);
        }
    }
    
    #endregion
}
    
