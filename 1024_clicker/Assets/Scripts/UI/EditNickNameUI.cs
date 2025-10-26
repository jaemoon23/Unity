using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditNickNameUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject editNicknamePanel;
    [SerializeField] private GameObject ProfilePanel;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;

    [Header("Input Fields & Texts")]
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private TextMeshProUGUI nickNameText;

    [Header("Components")]
    [SerializeField] private ProfileUI profileUI;

    private async UniTaskVoid Start()
    {
        saveButton.onClick.AddListener(() => OnSaveButtonClicked().Forget());
        closeButton.onClick.AddListener(() => OnCloseButtonClicked());
    }

    private async UniTaskVoid OnEnable() 
    {
        UpdateUI().Forget();
    }

    private async UniTaskVoid UpdateUI()
    {
        // 프로필 UI 업데이트
        var (profile, error) = await ProfileManager.Instance.LoadProfileAsync();
        nickNameText.text = $"현재: {profile.nickname}";
    }

    private async UniTaskVoid OnSaveButtonClicked()
    {
        string newNickname = nicknameInput.text;

        saveButton.interactable = false;
        nicknameInput.interactable = false;

        try
        {
            var (success, error) = await ProfileManager.Instance.SaveProfileAsync(newNickname);

            if (success)
            {
                Debug.Log("프로필 수정 성공!");
                UpdateUI().Forget();
                profileUI.UpdateUI().Forget();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"프로필 수정 중 오류 발생: {ex.Message}");
        }
        finally
        {
            saveButton.interactable = true;
            nicknameInput.interactable = true;
        }
    }
    
    private void OnCloseButtonClicked()
    {
        editNicknamePanel.SetActive(false);
        ProfilePanel.SetActive(true);
    }
}
