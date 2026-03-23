using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion; // Bắt buộc phải có dòng này

public class Portal : MonoBehaviour
{
    // Chúng ta dùng [Rpc] để đảm bảo khi 1 người chạm vào, 
    // lệnh chuyển cảnh hoặc kết thúc sẽ chạy chính xác trên máy chủ (Host)
    void OnTriggerEnter2D(Collider2D collider)
    {
        // Kiểm tra xem có đúng là Player chạm vào không
        if (collider.CompareTag("Player"))
        {
            NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();

            // Chỉ Server (Host) mới có quyền đổi màn chơi
            if (runner != null && runner.IsServer)
            {
                // Lấy index của Scene hiện tại
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                if (currentSceneIndex == 1) 
                {
                    // Chuyển sang màn tiếp theo (Index 2)
                    // Cách viết này cực kỳ an toàn cho mọi phiên bản Fusion
                    SceneManager.LoadScene(2); 
                }
                else 
                {
                    // Nếu không phải màn 1, mặc định là màn cuối -> Hiện bảng kết thúc
                    RPC_ShowEnding();
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowEnding()
    {
        // Tìm GameManager trong màn chơi để bật UI kết thúc
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null && gm.endingGame != null)
        {
            gm.endingGame.SetActive(true);
            // Trong mạng, không nên dùng Time.timeScale = 0 vì sẽ làm lag kết nối
        }
    }
}