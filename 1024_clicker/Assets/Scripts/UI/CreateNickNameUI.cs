using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNickNameUI : MonoBehaviour
{
    [SerializeField] private GameObject namePanel;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField nicknameInput;
    
    private async UniTaskVoid Start()
    {
        createButton.onClick.AddListener(() => OnCreateButtonClicked().Forget());
    }


    private async UniTaskVoid OnCreateButtonClicked()
    {
        string nickname = nicknameInput.text;

        createButton.interactable = false;
        nicknameInput.interactable = false;

        try
        {
            var (success, error) = await ProfileManager.Instance.SaveProfileAsync(nickname);

            if (success)
            {
                Debug.Log("프로필 생성 성공!");

                namePanel.SetActive(true);
                gameObject.SetActive(false);
            }
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"프로필 생성 중 오류 발생: {ex.Message}");
        }
        finally
        {
            createButton.interactable = true;
            nicknameInput.interactable = true;
        }
    }
}
