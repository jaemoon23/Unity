using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordUI : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;    
    [SerializeField] private GameObject recordTextPrefab;
    [SerializeField] private Transform recordContentParent;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Button RefreshButton;
    [SerializeField] private Button closeButton;

    List<(GameObject, TextMeshProUGUI)> recordTextList = new List<(GameObject, TextMeshProUGUI)>();

    private int idx = 0;
    private int maxIdx = 10;
    private void Start()
    {
        for (int i = 0; i < maxIdx; i++)
        {
            var recordObj = Instantiate(recordTextPrefab, recordContentParent);
            var tmp = recordObj.GetComponentInChildren<TextMeshProUGUI>();
            recordObj.SetActive(false);
            recordTextList.Add((recordObj, tmp));
        }

        RefreshButton.onClick.AddListener(() => UpdateUI().Forget());
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            gameUI.SetActive(true);
        });
    }
    
    private void OnEnable()
    {
        
        UpdateUI().Forget();
    }


    public async UniTaskVoid UpdateUI()
    {
        await UniTask.WaitUntil(() => ScoreManager.Instance != null);

        // 최고 기록 로드
        await ScoreManager.Instance.LoadBestScoreAsync();
        bestScoreText.text = $"최고 기록: {ScoreManager.Instance.CachedBestScore}점";
        // 기존 항목 비활성화
        foreach (var (obj, _) in recordTextList)
        {
            obj.SetActive(false);
        }

        idx = 0;

        var history = await ScoreManager.Instance.LoadHistoryAsync();

        // 기록 항목 업데이트
        foreach (var scoreData in history)
        {
            Debug.Log($"[RecordUI] {idx}번째 항목 설정: {scoreData.score}점");
            recordTextList[idx].Item1.SetActive(true);
            recordTextList[idx].Item2.text = $"{scoreData.score}점 - {scoreData.GetDateString()}";
            idx++;
        }
    }

    
}
