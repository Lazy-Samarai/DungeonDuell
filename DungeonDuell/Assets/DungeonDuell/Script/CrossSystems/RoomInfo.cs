using System.Collections.Generic;

public class RoomInfo
{
    public List<ConnectionDir> allowedDoors;

    public RoomElement roommElement = RoomElement.Standard;

    public int roomOwner; // 0 - no One - else Player x

    public RoomType roomtype = RoomType.Generic;

    public RoomInfo(int id, List<RoomConnection> newConncection)
    {
        RoomID = id;
        Conncection = newConncection;
    }

    public RoomInfo(int id, List<RoomConnection> newConncection, RoomType newRoomtype, RoomElement newRoomElement,
        List<ConnectionDir> newAllowedDoors)
    {
        RoomID = id;
        Conncection = newConncection;
        roomtype = newRoomtype;
        roommElement = newRoomElement;
        allowedDoors = newAllowedDoors;
    }

    public RoomInfo(int id, List<RoomConnection> newConncection, RoomType newRoomtype, RoomElement newRoomElement,
        List<ConnectionDir> newAllowedDoors, int owner, bool newfirstTimeSpawn)
    {
        RoomID = id;
        Conncection = newConncection;
        roomtype = newRoomtype;
        roommElement = newRoomElement;
        allowedDoors = newAllowedDoors;
        roomOwner = owner;
        FirstTimeSpawn = newfirstTimeSpawn;
    }

    public int RoomID { get; set; }
    public List<RoomConnection> Conncection { get; set; }
    public bool FirstTimeSpawn { get; set; } = true;
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

public enum
    ConnectionDir // structed llike this so the list can be inverted and you have the other dir of a door  (top right conncet botton left of the  hexagon)
{
    TopLeft, // rev bottonright
    TopRight, // rev bottonleft
    Left, // rev right
    Right, // rev left
    BottonLeft, // rev TopRight
    BottonRight // rev TopLeft
}

internal static class ConnectionDirExtension
{
    public static ConnectionDir GetInvert(this ConnectionDir s1)
    {
        var values = (ConnectionDir[])ConnectionDir.GetValues(typeof(ConnectionDir));
        var invertedIndex = values.Length - 1 - (int)s1;
        return values[invertedIndex];
    }
}

public enum RoomType
{
    Generic,
    PreSetLoot,
    NormalLott,
    Enemy,
    Spawn_Player1,
    Spawn_Player2,
    Trap
}

public enum RoomElement
{
    Standard,
    Ice,
    Fire,
    Water,
    Hole
}