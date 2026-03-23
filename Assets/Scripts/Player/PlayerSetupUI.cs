using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile
{
    public string playerName;
    public CharacterClass characterClass;
    
    public int Hp;
    public int MaxHp;
    public int Mp;
    public int MaxMp;
}

public class PlayerSetupUI : MonoBehaviour
{
    public GameObject setupPanel;
    public GameObject lobbyPanel;
    public BasicSpawner spawner;
    // tên nhân vật
    public TMP_InputField nameInput;
    // lớp nhân vật được chọn
    public CharacterClass selectedClass;
    // danh sách các Image để hiển thị preview nhân vật (có thể gán trong Inspector)
    public Image[] characterPreviews;
    
    void Start()
    {
        setupPanel.gameObject.SetActive(true);
        lobbyPanel.gameObject.SetActive(false);
        // highlight lớp nhân vật mặc định (ví dụ là Warrior)
        OnClassSelected(0);
    }
    
    // xử lý khi người chơi chọn 1 lớp nhân vật
    public void OnClassSelected(int classIndex)
    {
        selectedClass = (CharacterClass)classIndex;
        Debug.Log($"Selected class: {selectedClass}");
        // highlight nhân vật đã chọn
        characterPreviews[classIndex].color = Color.green; 
        // reset màu các preview khác
        for (var i = 0; i < characterPreviews.Length; i++)
        {
            if (i != classIndex)
                characterPreviews[i].color = Color.white;
        }
    }
    
    // xử lý khi người chơi nhấn nút xác nhận
    public void OnClickConfirm()
    {
        var charName = nameInput.text;
        if (string.IsNullOrWhiteSpace(charName))
            charName = $"Player_{UnityEngine.Random.Range(1000, 9999)}";
        // lưu profile local vào BasicSpawner
        spawner.SetLocalProfile(new PlayerProfile
        {
            playerName = charName,
            characterClass = selectedClass,
            Hp = 30, 
            MaxHp = 100, 
            Mp = 30, 
            MaxMp = 50
        });
        setupPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
}
