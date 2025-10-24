using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;

    // 버튼
    [SerializeField] private Button loginButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button guestButton;
    

    // 인풋필드
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    // 에러 텍스트
    [SerializeField] private TextMeshProUGUI errorText;


    [SerializeField] private Button profileButton;
    [SerializeField] private TextMeshProUGUI profileText;

    private async UniTaskVoid Start()
    {
        SetButtonInteractivity(false);
        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        loginButton.onClick.AddListener(() => OnLoginButtonClicked().Forget());
        createButton.onClick.AddListener(() => OnCreateButtonClicked().Forget());
        guestButton.onClick.AddListener(() => OnGuestButtonButtonClicked().Forget());
        profileButton.onClick.AddListener(() =>
            {
                AuthManager.Instance.SignOut();
                UpdateUI().Forget();
            }
        );

        SetButtonInteractivity(true);

        UpdateUI().Forget();
    }

    public async UniTaskVoid UpdateUI()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsInitialized)
            return;

        // 로그인 상태에 따라 UI 업데이트
        bool IsLoggedIn = AuthManager.Instance.IsLoggedIn;
        loginPanel.SetActive(!IsLoggedIn);

        if (IsLoggedIn)
        {
            string userId = AuthManager.Instance.UserId;
            profileText.text = userId;
        }
        else
        {
            profileText.text = string.Empty;
        }
        errorText.text = string.Empty;
    }

    private async UniTaskVoid OnLoginButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        SetButtonInteractivity(false);
        var (success, error) = await AuthManager.Instance.SignInWithEmailAsync(email, password);

        if (success)
        {
            Debug.Log("[LoginUI] 로그인 성공");
        }
        else
        {
            ShowError(error);
        }
        SetButtonInteractivity(true);
        UpdateUI().Forget();
    }

    private async UniTaskVoid OnCreateButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        SetButtonInteractivity(false);
        var (success, error) = await AuthManager.Instance.CreateUserWithEmailAsync(email, password);

        if (success)
        {
            Debug.Log("[LoginUI] 회원가입 성공");
        }
        else
        {
            ShowError(error);
        }
        SetButtonInteractivity(true);
        UpdateUI().Forget();
    }

    private async UniTaskVoid OnGuestButtonButtonClicked()
    {
        SetButtonInteractivity(false);
        var (success, error) = await AuthManager.Instance.SignInAnonymouslyAsync();

        if (success)
        {
            Debug.Log("[LoginUI] 익명 로그인 성공");
        }
        else
        {
            ShowError(error);
        }
        SetButtonInteractivity(true);
        UpdateUI().Forget();
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.color = Color.red;
    }


    private void SetButtonInteractivity(bool isInteractable)
    {
        loginButton.interactable = isInteractable;
        createButton.interactable = isInteractable;
        guestButton.interactable = isInteractable;
    }
}
