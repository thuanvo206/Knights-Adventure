using Fusion;
using UnityEngine;

public class LobbyRunner : MonoBehaviour
{
    public BasicSpawner _BasicSpawner;

    async void Start()
    {
        // Khởi tạo Lobby
        await _BasicSpawner.StartLobbyAndRunner();
    }

    public async void CreateRoom(string roomName)
    {
        Debug.Log($" >>>>>>> Creating room: {roomName}");
        
        // SỬA TẠI ĐÂY: Đổi thành số 1
        // Vì hiện tại Scenes/GameScene của bạn đang ở Index 1 trong Build Settings
        var scene = SceneRef.FromIndex(1); 
        
        await _BasicSpawner.StartHost(roomName, scene);
    }
    
    public async void JoinRoom(SessionInfo sessionInfo)
    {
        Debug.Log($" >>>>>>> Joining room: {sessionInfo.Name}");
        await _BasicSpawner.StartClient(sessionInfo.Name);
    }
}