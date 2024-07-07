using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo {

    public int RoomID { get; set; }
    public List<RoomConnection> Conncection { get; set; }

    public RoomType roomtype = RoomType.Generic;

    public RoomInfo(int id, List<RoomConnection> newConncection)
    {
        RoomID = id;
        Conncection = newConncection;
    }
    public RoomInfo(int id, List<RoomConnection> newConncection, RoomType newRoomtype)
    {
        RoomID = id;
        Conncection = newConncection;
        roomtype = newRoomtype;
    }

}
public class RoomConnection
{
    public ConnectionDir connectionDir;
    public int targetRoomId;

    public RoomConnection(int id, ConnectionDir dir)
    {
        targetRoomId = id;
        connectionDir = dir;

    }
}
public enum ConnectionDir // structed llike this so the list can be inverted and you have the other dir of a door  (top right conncet botton left of the  hexagon)
{
    TopLeft,      // rev bottonright
    TopRight,     // rev bottonleft
    Left,         // rev right
    Right,        // rev left
    BottonLeft,   // rev TopRight
    BottonRight // rev TopLeft
}
static class ConnectionDirExtension
{
    public static ConnectionDir GetInvert(this ConnectionDir s1)
    {
        ConnectionDir[] values = (ConnectionDir[])ConnectionDir.GetValues(typeof(ConnectionDir));
        int invertedIndex = values.Length - 1 - (int)s1;
        return values[invertedIndex];
    }
}
public enum RoomType
{
    Generic,
    CenterLoot,
    NormalLott,
    Enemy,
    Spawn

}
