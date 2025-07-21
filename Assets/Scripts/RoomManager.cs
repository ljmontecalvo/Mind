using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Room Data")]
    public RoomData currentRoomData;
    private GameObject activeRoom;

    [Header("References")]
    public Transform roomParent;
    public Transform playerSpawn;
    public GameObject playerPrefab;

    public void LoadRoom()
    {
        if (currentRoomData == null || currentRoomData.roomPrefab == null)
        {
            Debug.LogError("RoomData not assigned!");
            return;
        }

        // Instantiate Room
        activeRoom = Instantiate(currentRoomData.roomPrefab, roomParent.position, Quaternion.identity, roomParent);

        // Instantiate Player
        Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
    }
}
