using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private PlayerStatus player;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text keyText;

    void Update()
    {
        if (player == null)
        {
            return;
        }
        staminaSlider.maxValue = player.MaxStamina;
        staminaSlider.value = player.CurrentStamina;
        hpText.text = $"HP:{player.CurrentHp}/{player.MaxHp}";
        keyText.text = $"Key{player.KeyCount}";
    }
}
