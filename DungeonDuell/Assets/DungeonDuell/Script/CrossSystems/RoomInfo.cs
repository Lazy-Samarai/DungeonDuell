using System.Collections.Generic;

public class RoomInfo
{
    public List<ConnectionDir> AllowedDoors;

    public RoomElement RoommElement = RoomElement.Standard;

    public int RoomOwner; // 0 - no One - else Player x

    public RoomType Roomtype = RoomType.Generic;

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
        Roomtype = newRoomtype;
        RoommElement = newRoomElement;
        AllowedDoors = newAllowedDoors;
    }

    public RoomInfo(int id, List<RoomConnection> newConncection, RoomType newRoomtype, RoomElement newRoomElement,
        List<ConnectionDir> newAllowedDoors, int owner, bool newfirstTimeSpawn)
    {
        RoomID = id;
        Conncection = newConncection;
        Roomtype = newRoomtype;
        RoommElement = newRoomElement;
        AllowedDoors = newAllowedDoors;
        RoomOwner = owner;
        FirstTimeSpawn = newfirstTimeSpawn;
    }

    public int RoomID { get; set; }
    public List<RoomConnection> Conncection { get; set; }
    public bool FirstTimeSpawn { get; set; } = true;
}

public class RoomConnection
{
    public ConnectionDir ConnectionDir;
    public int TargetRoomId;

    public RoomConnection(int id, ConnectionDir dir)
    {
        TargetRoomId = id;
        ConnectionDir = dir;
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
    SpawnPlayer1,
    SpawnPlayer2,
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