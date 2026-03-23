using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : NetworkBehaviour
{
    public TextMeshProUGUI nameText; // gán text nằm trên đầu nhân vật
    public Slider sliderHp;
    public Slider sliderMp;

    // các model nhân vật tương ứng với các class
    public GameObject[] characterModels; // gán prefab cho từng class (theo thứ tự enum)

    public PlayerDataManager dataManager; // tham chiếu đến PlayerDataManager để lấy thông tin player

    private void Start()
    {
        // ẩn các model nhân vật khi bắt đầu, sẽ hiển thị sau khi nhận RPC
        foreach (var model in characterModels)
        {
            if (model != null) model.SetActive(false);
        }
    }

    // hàm này sẽ được gọi khi object được spawn,
    // có thể dùng để khởi tạo các component hoặc tìm reference
    public override void Spawned()
    {
        // có thể khởi tạo mặc định hoặc để trống, sẽ được set sau khi nhận RPC
        dataManager = FindFirstObjectByType<PlayerDataManager>();
    }

    // hàm này sẽ được gọi mỗi frame để cập nhật hiển thị,
    // có thể dùng để cập nhật tên và model dựa trên PlayerName và Class
    public override void Render()
    {
        if (dataManager == null) return;
        if (dataManager.TryGetMeta(Object.InputAuthority, out var playerMeta))
        {
            nameText.text = playerMeta.Name.ToString();
            
            sliderHp.maxValue = playerMeta.MaxHp;
            sliderMp.maxValue = playerMeta.MaxMp;
            
            sliderHp.value = playerMeta.Hp;
            sliderMp.value = playerMeta.Mp;

            // cập nhật model nhân vật
            for (var i = 0; i < characterModels.Length; i++)
            {
                if (characterModels[i] != null)
                    characterModels[i].SetActive(i == (int)playerMeta.Class);
            }
        }
    }
}