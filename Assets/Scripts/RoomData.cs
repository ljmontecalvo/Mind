using UnityEngine;

[CreateAssetMenu(menuName = "Mind/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;
    public GameObject roomPrefab;
    public Sprite roomIcon; // Optional for UI
    public AudioClip backgroundMusic; // Optional
}
