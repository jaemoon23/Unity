using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject ProfilePanel;


    [Header("Buttons")]
    [SerializeField] private Button openProfileButton;
    [SerializeField] private Button editNicknameButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button closeProfileButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI uidText;


    private void UpdateText()
    {
        
    }
}
