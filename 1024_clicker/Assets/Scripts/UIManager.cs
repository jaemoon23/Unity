using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    // Record Panel & Leaderboard Panel
    [SerializeField] private GameObject recordPanel;
    [SerializeField] private GameObject leaderboardPanel;


    [Header("Buttons")]
    // Record Panel
    [SerializeField] private Button openRecordButton;
    [SerializeField] private Button closeRecordButton;

    // Leaderboard Panel
    [SerializeField] private Button openLeaderboardButton;
    [SerializeField] private Button closeLeaderboardButton;


    private void Start()
    {
        // Record Panel
        openRecordButton.onClick.AddListener(() => recordPanel.SetActive(true));
        closeRecordButton.onClick.AddListener(() => recordPanel.SetActive(false));

        // Leaderboard Panel
        openLeaderboardButton.onClick.AddListener(() => leaderboardPanel.SetActive(true));
        closeLeaderboardButton.onClick.AddListener(() => leaderboardPanel.SetActive(false));
    }
}
