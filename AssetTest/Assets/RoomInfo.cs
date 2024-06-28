using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo {

    public int RoomID { get; set; }
    public List<RoomConnection> Conncection { get; set; }
    
    public RoomInfo(int id, List<RoomConnection> Conncection)
    {
        RoomID = id;
        Conncection = new List<RoomConnection>();
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
public enum ConnectionDir // structed llike this so the list can be inverted and you have the other dir of a door  (top right conncet botton left of the conncected hexagon)
{
    TopLeft,      // rev bottonright
    TopRight,     // rev bottonleft
    Left,         // rev right
    Right,        // rev left
    BottonLeft,   // rev TopRight
    BottonRight,  // rev TopLeft


}
