using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UniTaskManager : MonoBehaviour
{
    [Header("Section 1")]
    public Button delayButton;
    public Button delayFrameButton;
    public Button yieldButton;
    public Button nextFrameButton;

    [Header("Section 2")]
    public Button sequentialButton;
    public Button whenAllButton;
    public Button whenAnyButton;
    public Slider progressBar1;
    public Slider progressBar2;
    public Slider progressBar3;

    [Header("Section 3")]
    public Button loadResourceButton;
    public Button loadingWithProgressButton;
    public Button cancelButton;
    public Slider loadingProgressBar;

    [Header("Section 4")]
    public Button updateButton;
    public Button fixedUpdateButton;
    public Button lateUpdateButton;

    [Header("Section 5")]
    public Button destroyTokenButton;
    public Button timeOutTokenButton;
    public Button linkedTokenButton;
    public Button cancelSection5Button;
    public Slider section5ProgressBar;
    private CancellationTokenSource section5Cts = new CancellationTokenSource();

    [Header("Section 6")]
    public Button fadeInButton;
    public Button fadeOutButton;
    public Button animationButton;
    public Button waitForInputButton;
    public CanvasGroup fadePanel;
    public Transform animatedCubeTr;


    [Header("Texts")]
    public TextMeshProUGUI[] sectionText;

    private void Start()
    {
        // Section 1
        delayButton.onClick.AddListener(() => OnDelayClicked().Forget());
        delayFrameButton.onClick.AddListener(() => OnDelayFrameClicked().Forget());
        yieldButton.onClick.AddListener(() => OnDelayYieldClicked().Forget());
        nextFrameButton.onClick.AddListener(() => OnNextFrameClicked().Forget());

        // Section 2
        sequentialButton.onClick.AddListener(() => OnSequencialClicked().Forget());
        whenAllButton.onClick.AddListener(() => OnWhenAllClicked().Forget());
        whenAnyButton.onClick.AddListener(() => OnWhenAnyClicked().Forget());

        // Section 3
        loadResourceButton.onClick.AddListener(() => OnResourceLoadClicked().Forget());
        loadingWithProgressButton.onClick.AddListener(() => OnLoadWithProgressBarClicked().Forget());
        cancelButton.onClick.AddListener(() => OnCancelLoadingClicked());

        // section 4
        updateButton.onClick.AddListener(() => OnUpdateClicked().Forget());
        fixedUpdateButton.onClick.AddListener(() => OnFixedUpdateClicked().Forget());
        lateUpdateButton.onClick.AddListener(() => OnLateUpdateClicked().Forget());

        // Section 5
        destroyTokenButton.onClick.AddListener(() => OnDestroyTokenClicked().Forget());
        timeOutTokenButton.onClick.AddListener(() => OnTimeOutTokenClicked().Forget());
        linkedTokenButton.onClick.AddListener(() => OnClickLinkedCtsClicked().Forget());
        cancelSection5Button.onClick.AddListener(() => OnCancelSection5Clicked());

        // section 6
        fadeInButton.onClick.AddListener(() => OnFadeInClicked().Forget());
        fadeOutButton.onClick.AddListener(() => OnFadeOutClicked().Forget());
        animationButton.onClick.AddListener(() => OnAnimationClicked().Forget());
        waitForInputButton.onClick.AddListener(() => OnWaitKeyClicked().Forget());

        // Progress Bars Reset
        ResetProgressBar();
    }

    private void OnDestroy()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
    }

    private void UpdateSectionText(int section, string msg)
    {
        var log = $"Section {section}: {msg}";
        sectionText[section - 1].text = log;
        Debug.Log(log);
    }

    private void ResetProgressBar()
    {
        progressBar1.value = 0f;
        progressBar2.value = 0f;
        progressBar3.value = 0f;
        loadingProgressBar.value = 0f;
    }

    private async UniTask FakeLoadAsync(Slider progressBar, int ms)
    {
        int steps = 20;
        int delayPerStep = ms / steps;

        for (int i = 0; i < steps; i++)
        {
            progressBar.value = (float)i / steps;
            await UniTask.Delay(delayPerStep);
        }
        progressBar.value = 1f;
    }

    #region Section1
    private async UniTaskVoid OnDelayClicked()
    {
        UpdateSectionText(1, "OnDelayClicked");

        await UniTask.Delay(2000);

        UpdateSectionText(1, "2초 대기 완료");
    }
    private async UniTaskVoid OnDelayFrameClicked()
    {
        UpdateSectionText(1, "OnDelayFrameClicked");

        int startFrame = Time.frameCount;
        await UniTask.DelayFrame(60);
        int endFrame = Time.frameCount;
        UpdateSectionText(1, $"{endFrame - startFrame} 프레임 대기 완료");
    }
    private async UniTaskVoid OnDelayYieldClicked()
    {
        UpdateSectionText(1, "OnDelayYieldClicked");

        int startFrame = Time.frameCount;

        await UniTask.Yield();

        int endFrame = Time.frameCount;

        UpdateSectionText(1, $"Yield 완료:  시작{startFrame} 끝{endFrame}");
    }
    private async UniTaskVoid OnNextFrameClicked()
    {
        UpdateSectionText(1, "OnNextFrameClicked");
        int startFrame = Time.frameCount;
        await UniTask.NextFrame();
        int endFrame = Time.frameCount;
        UpdateSectionText(1, $"NextFrame 완료:  시작{startFrame} 끝{endFrame}");
    }
    #endregion

    #region Section2
    private async UniTaskVoid OnSequencialClicked()
    {
        ResetProgressBar();
        UpdateSectionText(2, "OnSequencialClicked");

        float startTime = Time.time;

        await FakeLoadAsync(progressBar1, 2000);
        await FakeLoadAsync(progressBar2, 2500);
        await FakeLoadAsync(progressBar3, 1500);

        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"순차 완료: {elapsed} 초");
    }

    private async UniTaskVoid OnWhenAllClicked()
    {
        ResetProgressBar();
        UpdateSectionText(2, "OnWhenAllClicked");

        float startTime = Time.time;

        await UniTask.WhenAll(
            FakeLoadAsync(progressBar1, 2000),
            FakeLoadAsync(progressBar2, 2500),
            FakeLoadAsync(progressBar3, 1500)
        );

        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"WhenAll 실행 완료: {elapsed} 초");
    }

    private async UniTaskVoid OnWhenAnyClicked()
    {
        ResetProgressBar();
        UpdateSectionText(2, "OnWhenAnyClicked");

        float startTime = Time.time;

        int index = await UniTask.WhenAny(
            FakeLoadAsync(progressBar1, 2000),
            FakeLoadAsync(progressBar2, 2500),
            FakeLoadAsync(progressBar3, 1500)
        );


        float elapsed = Time.time - startTime;

        UpdateSectionText(2, $"WhenAny 실행 완료 {index}: {elapsed} 초");
    }
    #endregion

    #region Section3
    private async UniTaskVoid OnResourceLoadClicked()
    {
        loadingProgressBar.value = 0f;
        UpdateSectionText(3, "OnResourceLoadClicked");

        var prefab = await Resources.LoadAsync<GameObject>("RotatingCube").ToUniTask() as GameObject;
        loadingProgressBar.value = 1f;
        UpdateSectionText(3, "리소스 로딩 완료");
        Instantiate(prefab);
    }

    private CancellationTokenSource loadCts = new CancellationTokenSource();

    private async UniTask OnLoadWithProgressBarClicked()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();

        try
        {
            UpdateSectionText(3, "로딩 시작!");
            loadingProgressBar.value = 0f;

            for (int i = 0; i < 100; i++)
            {
                loadCts.Token.ThrowIfCancellationRequested();
                loadingProgressBar.value = i / 100f;

                UpdateSectionText(3, $"로딩 중... {i}% ");

                await UniTask.Delay(50, cancellationToken: loadCts.Token);  // 취소 토큰 전달
            }
            loadingProgressBar.value = 1f;
            UpdateSectionText(3, "로딩 완료!");
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(3, "로딩 취소!");
        }

    }

    private void OnCancelLoadingClicked()
    {
        if (loadCts != null && !loadCts.IsCancellationRequested)
        {
            loadCts?.Cancel();
            loadCts?.Dispose();
            loadCts = null;
        }
        else
        {
            UpdateSectionText(3, "취소할 로딩 작업이 없습니다.");
        }

    }
    #endregion

    #region Section4
    private async UniTaskVoid OnUpdateClicked()
    {
        UpdateSectionText(4, "OnUpdateClicked");

        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            UpdateSectionText(4, $"업데이트 프레임 {Time.frameCount}");
        }

        UpdateSectionText(4, "업데이트 타이밍 테스트 끝");
    }
    private async UniTaskVoid OnFixedUpdateClicked()
    {
        UpdateSectionText(4, "OnFixedUpdateClicked");

        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            UpdateSectionText(4, $"픽스드 업데이트 프레임 {Time.time}");
        }

        UpdateSectionText(4, "픽스드 업데이트 타이밍 테스트 끝");

    }
    private async UniTaskVoid OnLateUpdateClicked()
    {
        UpdateSectionText(4, "OnLateUpdateClicked");
        for (int i = 0; i < 3; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);
            UpdateSectionText(4, $"레이트 업데이트 프레임 {Time.frameCount}");
        }

        UpdateSectionText(4, "레이트 업데이트 타이밍 테스트 끝");

    }
    #endregion

    #region Section5
    public async UniTask LongTaskAsync(CancellationToken ct)
    {
        UpdateSectionText(5, "Long task started... (10s)");
        section5ProgressBar.value = 0;

        for (int i = 0; i <= 100; i++)
        {
            ct.ThrowIfCancellationRequested();
            section5ProgressBar.value = i / 100f;

            UpdateSectionText(5, $"Progress: {i}% (Cancellable)");

            await UniTask.Delay(100, cancellationToken: ct);
        }

        UpdateSectionText(5, "Task complete! (100%)");
    }

    private async UniTaskVoid OnDestroyTokenClicked()
    {
        UpdateSectionText(5, "OnDestroyTokenClicked");
        try
        {
            await LongTaskAsync(this.GetCancellationTokenOnDestroy());
            UpdateSectionText(5, "테스크 완료");
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(5, "취소!");
        }
    }

    private async UniTaskVoid OnTimeOutTokenClicked()
    {
        UpdateSectionText(5, "OnTimeOutTokenClicked");

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(3));
        try
        {
            await LongTaskAsync(cts.Token);
            UpdateSectionText(5, "테스크 완료");
        }
        catch (OperationCanceledException)
        {
            UpdateSectionText(5, "타임 아웃 취소!");
        }
    }

    private async UniTaskVoid OnClickLinkedCtsClicked()
    {
        UpdateSectionText(5, "OnClickLinkedCtsClicked");

        section5Cts?.Cancel();
        section5Cts?.Dispose();
        section5Cts = new CancellationTokenSource();
            
        var timeOutCts = new CancellationTokenSource();
        timeOutCts.CancelAfterSlim(TimeSpan.FromSeconds(3));

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            section5Cts.Token,
            timeOutCts.Token,
            this.GetCancellationTokenOnDestroy()
        );

        try
        {
            await LongTaskAsync(linkedCts.Token);
            UpdateSectionText(5, "테스크 완료");
        }
        catch (OperationCanceledException)
        {
            if (timeOutCts.IsCancellationRequested)
            {
                UpdateSectionText(5, "타임 아웃 취소!");
            }
            else if (section5Cts.IsCancellationRequested)
            {
                UpdateSectionText(5, "사용자 취소!");
            }
            else
            {
                UpdateSectionText(5, "오브젝트 파괴로 인한 취소!");
            }
        }
        finally
        {
            timeOutCts.Dispose();
            section5Cts.Dispose();
            linkedCts.Dispose();

            section5Cts = null;
        }
    }

    private void OnCancelSection5Clicked()
    {
        if (section5Cts != null && !section5Cts.IsCancellationRequested)
        {
            section5Cts?.Cancel();
        }
        else
        {
            UpdateSectionText(5, "취소할 작업이 없습니다.");
        }
    }
    #endregion

    #region Section6
    private async UniTask FadeAsync(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        canvasGroup.alpha = from;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            await UniTask.Yield();
        }

        canvasGroup.alpha = to;
    }

    private async UniTask MoveToAsync(RectTransform target, Vector2 to, float duration)
    {
        Vector2 from = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.anchoredPosition = Vector2.Lerp(from, to, t);
            await UniTask.Yield();
        }

        target.anchoredPosition = to;
    }

    private async UniTask RotateToAsync(Transform target, float speed, float duration)
    {
        float from = target.eulerAngles.z;
        float elapsed = 0f;
        float currentAngle = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentAngle += speed * Time.deltaTime;
            target.rotation = Quaternion.Euler(0, 0, currentAngle);
            await UniTask.Yield();
        }
        target.rotation = Quaternion.Euler(0, 0, from + speed * duration);
    }

    private async UniTaskVoid OnFadeInClicked()
    {
        UpdateSectionText(6, "OnFadeInClicked");
        await FadeAsync(fadePanel, 0f, 1f, 0.5f);
        UpdateSectionText(6, "페이드 인 완료");
    }

    private async UniTaskVoid OnFadeOutClicked()
    {
        UpdateSectionText(6, "OnFadeOutClicked");
        await FadeAsync(fadePanel, 1f, 0f, 0.5f);
        UpdateSectionText(6, "페이드 아웃 완료");
    }

    private async UniTaskVoid OnAnimationClicked()
    {
        UpdateSectionText(6, "OnAnimationClicked");

        var rectTr = animatedCubeTr as RectTransform;
        var originalPos = rectTr.anchoredPosition;

        await MoveToAsync(rectTr, originalPos + Vector2.up * 50f, 0.5f);
        UpdateSectionText(6, "1. 위로 이동 완료");

        await RotateToAsync(animatedCubeTr, 360f, 0.5f);
        UpdateSectionText(6, "2. 회전 완료");

        await MoveToAsync(rectTr, originalPos, 0.5f);
        UpdateSectionText(6, "3. 원위치 이동 완료");
    }

    private async UniTaskVoid OnWaitKeyClicked()
    {
        UpdateSectionText(6, "OnWaitKeyClicked");

        await UniTask.WaitUntil(() => Input.anyKey);

        UpdateSectionText(6, "키 입력");
    }
    #endregion
}
