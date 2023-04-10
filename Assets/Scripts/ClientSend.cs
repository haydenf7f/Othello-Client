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

    public static void PlayerMove(MoveInfo moveInfo)
    {
        using (Packet _packet = new Packet((int)ClientPackets.PlayerMove))
        {
            _packet.Write(moveInfo.Position.Row);
            _packet.Write(moveInfo.Position.Column);
            _packet.Write((int)moveInfo.Player);
            _packet.Write(moveInfo.Outflanked.Count);
            foreach (Position pos in moveInfo.Outflanked)
            {
                _packet.Write(pos.Row);
                _packet.Write(pos.Column);
            }

            SendTCPData(_packet);
        }
    }

    #endregion
}
    
