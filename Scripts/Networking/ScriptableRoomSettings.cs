using UnityEngine;

[CreateAssetMenu(fileName = "Room Settings", menuName = "Scriptable Objects/Room Settings", order = 1)]
public class ScriptableRoomSettings : ScriptableObject
{
	public string RoomName = "Default Server";

	public byte MaxPlayers = 4;

	public string Player_Name = "";
}
