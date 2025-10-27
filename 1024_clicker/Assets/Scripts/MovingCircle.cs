using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SocialPlatforms.Impl;

public class MovingCircle : MonoBehaviour
{
    public Button Circle;
    public Button backgroundCircle;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private RectTransform circleRect;

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private int timer = 10;
    private int currentScore;
    public int CurrentScore => currentScore;

    public GameObject startUI;
    public Button startButton;

    public GameObject endUI;
    public Button endButton;
    public TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        circleRect = Circle.GetComponent<RectTransform>();
        Circle.onClick.AddListener(AddScore);
        backgroundCircle.onClick.AddListener(AddScore);
        startButton.onClick.AddListener(() =>
        {
            startUI.SetActive(false);
            UpdateMove().Forget();
        });
        endButton.onClick.AddListener(() =>
        {
            endUI.SetActive(false);
            startUI.SetActive(true);
            timer = 10;
            currentScore = 0;
            scoreText.text = "점수 : " + currentScore.ToString();
            timerText.text = "시간 : " + timer.ToString();
        });
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    private void MoveCircle()
    {
        var parent = circleRect.parent as RectTransform;
        var parentRect = parent.rect;           
        var circleSize = circleRect.rect.size;     

        float maxX = (parentRect.width - circleSize.x) * 0.5f;
        float maxY = (parentRect.height - circleSize.y) * 0.5f;

        float x = Random.Range(-maxX, maxX);
        float y = Random.Range(-maxY, maxY);

        circleRect.anchoredPosition = new Vector2(x, y);
    }

    private void AddScore()
    {
        if (timer <= 0) return;
        currentScore++;
        scoreText.text = "점수 : " + currentScore.ToString();
    }

    private async UniTask UpdateMove()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            MoveCircle();
            await UniTask.Delay(1000);
            timer--;
            timerText.text = "시간 : " + timer.ToString();
            if (timer <= 0)
            {
                cancellationTokenSource.Cancel();
                timerText.text = "시간 종료!";
                finalScoreText.text = "점수 : " + currentScore.ToString();
            
                // ScoreManager 초기화 대기
                await UniTask.WaitUntil(() => ScoreManager.Instance != null);
                
                if (AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn)
                {
                    await ScoreManager.Instance.SaveScoreAsync(currentScore);
                }
                else
                {
                    Debug.LogWarning("[MovingCircle] 로그인되지 않음 - 점수 저장 안함");
                }
                
                endUI.SetActive(true);
            }
        }
    }

}
