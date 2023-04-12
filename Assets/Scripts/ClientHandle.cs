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
        Debug.Log($"Receiving a game update from server...");

        int row = _packet.ReadInt();
        int column = _packet.ReadInt();
        Position position = new Position(row, column);
        int player = _packet.ReadInt();
        int outflankedCount = _packet.ReadInt();
        List<Position> outflanked = new List<Position>();

        for (int i = 0; i < outflankedCount; i++)
        {
            int outflankedRow = _packet.ReadInt();
            int outflankedColumn = _packet.ReadInt();
            outflanked.Add(new Position(outflankedRow, outflankedColumn));
        }

        MoveInfo moveInfo = new MoveInfo { Player = (Player)player, Position = position, Outflanked = outflanked };

        Debug.Log($"The other player made a move at ({position.Row}, {position.Column})! (Player: {player})");
        
        GameManager.instance.UpdateBoardWithMove(position);   
    }
}

