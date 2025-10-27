using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject ProfilePanel;
    [SerializeField] private GameObject EditNicknamePanel;
    [SerializeField] private GameObject createNicknamePanel;
    [SerializeField] private GameObject loginPanel;


    [Header("Buttons")]
    [SerializeField] private Button openProfileButton;
    [SerializeField] private Button editNicknameButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button closeProfileButton;

    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private TextMeshProUGUI UidText;


    private async UniTaskVoid Start()
    {
        openProfileButton.onClick.AddListener(() => OnOpenProfileButtonClicked().Forget());
        editNicknameButton.onClick.AddListener(() => OnEditNicknameButtonClicked());
        logoutButton.onClick.AddListener(() => OnLogoutButtonClicked());
        closeProfileButton.onClick.AddListener(() => ProfilePanel.SetActive(false));
    }

    public async UniTaskVoid UpdateUI()
    {
        // 프로필 UI 업데이트
        var (profile, error) = await ProfileManager.Instance.LoadProfileAsync();
        nickNameText.text = profile.nickname;
        UidText.text = $"UID: {AuthManager.Instance.UserId}";
    }

    private async UniTaskVoid OnOpenProfileButtonClicked()
    {
        if (await ProfileManager.Instance.ProfileExisAsync())
        {
            UpdateUI().Forget();
        }
        else
        {
            nickNameText.text = "(없음)";
        }
        ProfilePanel.SetActive(true);
    }

    private async UniTaskVoid OnEditNicknameButtonClicked()
    {
        if (await ProfileManager.Instance.ProfileExisAsync())
        {
            // 프로필이 있으면 편집 패널로
            EditNicknamePanel.SetActive(true);
        }
        else
        {
            // 프로필이 없으면 생성 패널로
            createNicknamePanel.SetActive(true);
        }
        ProfilePanel.SetActive(false);
    }
    
    private void OnLogoutButtonClicked()
    {
        AuthManager.Instance.SignOut();
        ProfilePanel.SetActive(false);
        loginPanel.SetActive(true);
    }


}
