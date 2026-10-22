using UnityEngine;

public class RoomChanger : MonoBehaviour
{
    public enum Room
    {
        Library,
        Outside,
        Home
    }
    Room _currentRoom = Room.Library;
    public Room CurrentRoom => _currentRoom;

    GameManager _gameManager;

    // 現在いる部屋以外へは、どの部屋からでも移動できる
    public void MoveLibrary()
    {
        ChangeRoom(Room.Library, new Vector2(0, 0));
    }

    public void MoveHome()
    {
        ChangeRoom(Room.Home, new Vector2(-1370, 0));
    }

    public void MoveOutside()
    {
        ChangeRoom(Room.Outside, new Vector2(1230, 0));
    }

    void ChangeRoom(Room room, Vector2 panelPosition)
    {
        if (_currentRoom == room)
        {
            return;
        }
        this.transform.localPosition = panelPosition;
        _currentRoom = room;

        // GameManager側の状態も更新する（出現する選択肢の判定に使う）
        if (_gameManager == null)
        {
            _gameManager = GetComponent<GameManager>();
        }
        if (_gameManager != null)
        {
            _gameManager.SetRoom(room);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
