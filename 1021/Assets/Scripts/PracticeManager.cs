using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class PracticeManager : MonoBehaviour
{
    [Header("Practice 1")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private int time = 90;
    bool isStop = false;
    bool isRunning = false;
    CancellationTokenSource p1Cts = new CancellationTokenSource();

    [Header("Practice 2")]
    [SerializeField] private Button SceneChangeButton;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject canvas;

    [Header("Practice 3")]
    [SerializeField] private Button Start3Button;
    [SerializeField] private Button Stop3Button;
    [SerializeField] private Slider slider3_1;
    [SerializeField] private Slider slider3_2;
    [SerializeField] private Slider slider3_3;
    [SerializeField] private Slider totalSlider;
    [SerializeField] private TextMeshProUGUI text3;
    private CancellationTokenSource Cts3 = new CancellationTokenSource();

    [Header("Practice 4")]
    [SerializeField] private Button start4Button;
    [SerializeField] private Button stop4Button;
    [SerializeField] private Button reset4Button;
    [SerializeField] private GameObject go4;
    Vector2 originalPos;
    private CancellationTokenSource Cts4 = new CancellationTokenSource();

    [Header("Practice 5")]
    [SerializeField] private Button save5Button;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private TMP_InputField inputField5;
    private CancellationTokenSource Cts5 = new CancellationTokenSource();
    private CancellationTokenSource inputCts = new CancellationTokenSource();
    bool isAutoSaving5 = false;

    private void Awake()
    {
        DontDestroyOnLoad(canvas);
        //DontDestroyOnLoad(loadingPanel);
    }
    private void Start()
    {
        // Practice 1
        startButton.onClick.AddListener(() => StartButtonClicked().Forget());
        stopButton.onClick.AddListener(() => StopButtonClicked());
        resumeButton.onClick.AddListener(() => ResumeButtonClicked());
        resetButton.onClick.AddListener(() => ResetButtonClicked());

        // Practice 2
        SceneChangeButton.onClick.AddListener(() => OnSceneChangeButtonClicked().Forget());

        // Practice 3
        Start3Button.onClick.AddListener(() => OnStart3ButtonClicked().Forget());
        Stop3Button.onClick.AddListener(() => OnStop3ButtonClicked());

        // Practice 4
        start4Button.onClick.AddListener(() => OnStart4ButtonClicked().Forget());
        stop4Button.onClick.AddListener(() => OnStop4ButtonClicked());
        reset4Button.onClick.AddListener(() => OnReset4ButtonClicked());

        // Practice 5
        save5Button.onClick.AddListener(() => OnSave5ButtonClicked());
        inputField5.onValueChanged.AddListener((value) => OnInputFieldChanged().Forget());
        AutoSaveAsync().Forget();

        
    }

    private void OnDestroy()
    {
        p1Cts?.Cancel();
        p1Cts?.Dispose();

        Cts3?.Cancel();
        Cts3?.Dispose();

        Cts4?.Cancel();
        Cts4?.Dispose();

        Cts5?.Cancel();
        Cts5?.Dispose();

        inputCts?.Cancel();
        inputCts?.Dispose();
    }

    #region Practice 1
    private async UniTaskVoid StartButtonClicked()
    {

        if (isRunning)
        {
            Debug.Log("타이머가 이미 실행 중");
            return;
        }
        isRunning = true;
        for (int i = time; i >= 0; i--)
        {
            int mi;
            int se;
            try
            {
                await UniTask.WaitWhile(() => isStop, cancellationToken: p1Cts.Token);

                mi = i / 60;
                se = i % 60;
                UpdateText1($"{mi:00}:{se:00}");

                await UniTask.Delay(1000, cancellationToken: p1Cts.Token);
            }
            catch (OperationCanceledException)
            {
                mi = time / 60;
                se = time % 60;
                UpdateText1($"{mi:00}:{se:00}");
                Debug.Log("취소 완료");
                isRunning = false;
                return;
            }
        }
        UpdateText1($"Time's Up!");
        isRunning = false;
    }
    private void StopButtonClicked()
    {
        isStop = true;
    }

    private void ResumeButtonClicked()
    {
        isStop = false;
    }

    private void ResetButtonClicked()
    {
        if (p1Cts != null && !p1Cts.IsCancellationRequested)
        {
            p1Cts?.Cancel();
            p1Cts?.Dispose();
            p1Cts = new CancellationTokenSource();
            Debug.Log("취소 요청");
        }
    }

    private void UpdateText1(string str)
    {
        countText.text = str;
    }
    #endregion

    #region Practice 2

    private void InitSlider(Slider slider)
    {
        slider.value = 0;
    }
    private async UniTaskVoid OnSceneChangeButtonClicked()
    {
        
        loadingPanel.SetActive(true);
        InitSlider(loadingBar);

        await FadeInAsync(loadingPanel, 2.0f);

        var scene = SceneManager.LoadSceneAsync("New Scene 1");
        scene.allowSceneActivation = false;

        float timer = 0f;
        float duration = 2.0f;

        while (timer < duration)
        {
            float targetProgress = scene.progress >= 0.9f ? 1f : scene.progress;

            timer += Time.deltaTime;
            loadingBar.value = Mathf.Lerp(loadingBar.value, targetProgress, timer / duration);
            loadingText.text = $"Loading... {(int)(loadingBar.value * 100)}%";
            await UniTask.Yield();
        }

        loadingBar.value = 1f;
        foreach (Transform child in canvas.transform)
        {
            if (child.gameObject != loadingPanel)
            {
                child.gameObject.SetActive(false);
            }
        }

        scene.allowSceneActivation = true;

        await scene.ToUniTask();
        await FadeOutAsync(loadingPanel, 2.0f);
        await UniTask.Yield();

        loadingPanel.SetActive(false);

    }

    private async UniTask FadeInAsync(GameObject obj, float duration, CancellationToken token = default)
    {
        var can = obj.GetComponent<CanvasGroup>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            can.alpha = Mathf.Lerp(0, 1, t);
            await UniTask.Yield(cancellationToken: token);
        }
        can.alpha = 1f;
    }
    private async UniTask FadeOutAsync(GameObject obj, float duration, CancellationToken token = default)
    {
        var can = obj.GetComponent<CanvasGroup>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            can.alpha = Mathf.Lerp(1, 0, t);
            await UniTask.Yield(cancellationToken: token);
        }
        can.alpha = 0f;
    }

    #endregion

    #region Parctice 3
    private void InitSlider3()
    {
        slider3_1.value = 0f;
        slider3_2.value = 0f;
        slider3_3.value = 0f;
    }
    private async UniTaskVoid OnStart3ButtonClicked()
    {
        Debug.Log("시작 버튼 누름");
        Cts3 = new CancellationTokenSource();

        var timeOutCts3 = new CancellationTokenSource();
        timeOutCts3.CancelAfterSlim(TimeSpan.FromSeconds(10));

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            Cts3.Token,
            timeOutCts3.Token,
            this.GetCancellationTokenOnDestroy()
        );
        
        text3.text = $"작업 진행중 ";
        InitSlider3();

        try
        {
            await UniTask.WhenAll(
                LoadRecource("1", slider3_1, UnityEngine.Random.Range(1, 11), linkedCts),
                LoadRecource("2", slider3_2, UnityEngine.Random.Range(1, 11), linkedCts),
                LoadRecource("3", slider3_3, UnityEngine.Random.Range(1, 11), linkedCts)
            );
            Debug.Log("작업 완료");
            text3.text = "All resources loaded!";
        }
        catch (OperationCanceledException)
        {
            if (timeOutCts3.IsCancellationRequested)
            {
                text3.text = "Loading timeout!";
            }
            else if (Cts3.IsCancellationRequested)
            {
                text3.text = "Loading cancelled";
            }
            else
            {
                text3.text = "오브젝트 파괴";
            }
        }
        finally
        {
            timeOutCts3.Cancel();
            timeOutCts3.Dispose();
            Cts3.Dispose();
            linkedCts.Dispose();

            Cts3 = null;
        }
    }

    private void OnStop3ButtonClicked()
    {
        if (Cts3 != null && !Cts3.IsCancellationRequested)
        {
            Cts3?.Cancel();
        }
    }

    private async UniTask LoadRecource(string path, Slider slider, float duration, CancellationTokenSource ct)
    {
        var sprite = Resources.LoadAsync<Sprite>(path);

        float elapsed = 0f;
        float startValue = slider.value;

        while (!sprite.isDone || elapsed < duration)
        {
            elapsed += Time.deltaTime;

            slider.value = Mathf.Lerp(startValue, 1, elapsed / duration);
            TotalSlider();
            await UniTask.Yield(cancellationToken: ct.Token);
        }
        slider.value = 1f;
        TotalSlider();
    }

    private void TotalSlider()
    {
        float total = (slider3_1.value + slider3_2.value + slider3_3.value) / 3f;

        totalSlider.value = total;

    }
    #endregion

    #region Parctice 4
    private async UniTaskVoid OnStart4ButtonClicked()
    {
        try
        {
            go4.SetActive(true);

            var rectTr = go4.GetComponent<RectTransform>();
            originalPos = rectTr.anchoredPosition;

            await FadeInAsync(go4, 2.0f, Cts4.Token);

            await MoveObjAsync(go4, 0.5f);

            await ScaleUpAsync(rectTr, 0.5f);

            await RotateObjAsync(rectTr, 360f, 1f);

            await FadeOutAsync(go4, 2.0f, Cts4.Token);

            rectTr.anchoredPosition = originalPos;
            rectTr.localScale = new Vector2(1f, 1f);
            go4.SetActive(false);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("취소");
        }
        
    }
    private void OnReset4ButtonClicked()
    {
        Cancel4();
        var rectTr = go4.GetComponent<RectTransform>();
        rectTr.anchoredPosition = originalPos;
        rectTr.localScale = new Vector2(1f, 1f);
        rectTr.rotation = Quaternion.Euler(0, 0, 0);
        go4.SetActive(false);
    }

    private void OnStop4ButtonClicked()
    {
        Cancel4();
    }
    
    private void Cancel4()
    {
        if (Cts4 != null && !Cts4.IsCancellationRequested)
        {
            Cts4?.Cancel();
            Cts4?.Dispose();
            Cts4 = new CancellationTokenSource();
        }
    }

    private async UniTask MoveObjAsync(GameObject go, float duration)
    {
        var rectTransform = go.GetComponent<RectTransform>();

        Vector2 from = rectTransform.anchoredPosition; // 현재 위치
        Vector2 to = from + new Vector2(200, 0);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(from, to, t);
            await UniTask.Yield(cancellationToken: Cts4.Token);
        }
    }

    private async UniTask ScaleUpAsync(RectTransform tr, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            tr.localScale = Vector2.Lerp(tr.localScale, new Vector2(1.5f, 1.5f), t);
            await UniTask.Yield(cancellationToken: Cts4.Token);
        }
    }

    private async UniTask RotateObjAsync(RectTransform tr, float speed, float duration)
    {
        float from = tr.eulerAngles.z;
        float elapsed = 0f;
        float currentAngle = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentAngle += speed * Time.deltaTime;
            tr.rotation = Quaternion.Euler(0, 0, currentAngle);
            await UniTask.Yield(cancellationToken: Cts4.Token);
        }
        tr.rotation = Quaternion.Euler(0, 0, from + speed * duration);
    }
    #endregion

    #region Parctice 5
    
    private async UniTaskVoid OnInputFieldChanged()
    {
        Debug.Log("입력 변경 감지");
       
        // 기존 타이머 취소
        inputCts?.Cancel();
        inputCts?.Dispose();
        inputCts = new CancellationTokenSource();

        // 새로운 타이머 시작
        while (true)
        {
            try
            {
                await UniTask.Delay(3000, cancellationToken: inputCts.Token);
                Save5();
                Debug.Log("3초 동안 입력 없음 저장 실행");
                break; // 3초 동안 입력이 없으면 루프 종료
            }
            catch (OperationCanceledException)
            {
                Debug.Log("타이머 취소 및 초기화");
                break;
            }
        }
    }

    private async UniTask AutoSaveAsync()
    {
        try
        {
            while (!Cts5.IsCancellationRequested)
            {

                await UniTask.Delay(30000, cancellationToken: Cts5.Token); // 30초 대기
                text5.text = $"Auto saved at {DateTime.Now:HH:mm:ss}";
                isAutoSaving5 = true;
                Debug.Log($"자동 저장 완료");

            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("AutoSave 중단됨");
        }
    }
    private void OnSave5ButtonClicked()
    {
        inputCts?.Cancel();
        if (isAutoSaving5)
        {
            isAutoSaving5 = false;
            return;
        }
        Debug.Log("수동 저장 버튼 클릭");
        Save5();
    }
    private void Save5()
    {
        text5.text = $"Saved at {DateTime.Now:HH:mm:ss}";
    }
    #endregion
}
