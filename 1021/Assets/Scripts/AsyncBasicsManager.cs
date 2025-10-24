using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AsyncBasicsManager : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI section1StatusText;
    public TextMeshProUGUI section2StatusText;
    public TextMeshProUGUI section3StatusText;
    public TextMeshProUGUI section4StatusText;
    public TextMeshProUGUI section5StatusText;
    public TextMeshProUGUI section6StatusText;
    public TextMeshProUGUI timeOutValueText;

    [Header("Sliders")]
    public Slider progressBar1;
    public Slider progressBar2;
    public Slider progressBar3;
    public Slider progressBar4;
    public Slider timeOutSlider;

    [Header("Objects")]
    public Transform cubeObjTr;

    private void Start()
    {
        InitTimeOutSlider();
    }

    private void OnDestroy()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();

        cancelCts?.Cancel();
        cancelCts?.Dispose();
    }

    #region Section1
    public void OnSyncDownloadButtonClicked()
    {
        UpdateSection1Text("OnSyncDownloadButtonClicked: Start");

        Thread.Sleep(3000);

        UpdateSection1Text("OnSyncDownloadButtonClicked: End");
    }
    public async void OnAsyncDownloadButtonClicked()
    {
        UpdateSection1Text("OnAsyncDownloadButtonClicked: Start");

        await Task.Delay(3000);

        UpdateSection1Text("OnAsyncDownloadButtonClicked: End");
    }

    private void UpdateSection1Text(string msg)
    {
        section1StatusText.text = msg;
        Debug.Log($"[Section 1] {msg}");
    }
    #endregion

    #region Section2
    private CancellationTokenSource delayCts = new CancellationTokenSource();

    /// <summary>
    /// 지정 초 대기
    /// </summary>
    public async void OnDelayClicked(int seconds)
    {
        UpdateSection2Text($"대기중: {seconds} 초...");

        for (int i = seconds; i > 0; i--)
        {
            delayCts.Token.ThrowIfCancellationRequested();  // 취소 요청이 있으면 예외 발생

            UpdateSection2Text($"남은 시간: {i} 초...");
            await Task.Delay(1000);
        }

        UpdateSection2Text($"대기 완료");
    }

    /// <summary>
    /// 취소 가능한 10초 대기
    /// </summary>
    public async void OnCancellableDelayClicked()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();
        delayCts = new CancellationTokenSource();

        try
        {
            for (int i = 10; i > 0; i--)
            {
                delayCts.Token.ThrowIfCancellationRequested();  // 취소 요청이 있으면 예외 발생

                UpdateSection2Text($"남은 시간: {i} 초...");
                await Task.Delay(1000, delayCts.Token); // 취소 토큰 전달
            }

            UpdateSection2Text($"10초 대기 완료");
        }
        catch (OperationCanceledException)
        {
            UpdateSection2Text("10초 대기 취소");
        }
    }

    /// <summary>
    ///  10초 대기 취소
    /// </summary>
    public void OnCancelDelayClicked()
    {
        delayCts?.Cancel();
        delayCts?.Dispose();
        delayCts = new CancellationTokenSource();
    }

    /// <summary>
    /// 섹션2 상태 텍스트 업데이트
    /// </summary>
    private void UpdateSection2Text(string msg)
    {
        section2StatusText.text = msg;
        Debug.Log($"[Section 2] {msg}");
    }
    #endregion

    #region Section3
    private void ResetProgressBar()
    {
        progressBar1.value = 0;
        progressBar2.value = 0;
        progressBar3.value = 0;
    }

    /// <summary>
    /// 순차 다운로드 시뮬레이션
    /// </summary>
    public async void OnSequencialDownloadClicked()
    {
        ResetProgressBar();
        UpdateSection3Text("순차 다운로드 시작");

        float startTime = Time.time;

        await FakeDownloadAsync(progressBar1, 1, 2000);
        await FakeDownloadAsync(progressBar2, 2, 2000);
        await FakeDownloadAsync(progressBar3, 3, 2000);

        float elapsed = Time.time - startTime;
        UpdateSection3Text($"순차 다운로드 끝 {elapsed} 초");
    }

    /// <summary>
    /// 병렬 다운로드 시뮬레이션
    /// </summary>
    public async void OnParallelDownloadClicked()
    {
        ResetProgressBar();
        UpdateSection3Text("병렬 다운로드 시작");

        float startTime = Time.time;

        // 병렬로 다운로드 작업 시작
        Task download1 = FakeDownloadAsync(progressBar1, 1, 2000);
        Task download2 = FakeDownloadAsync(progressBar2, 2, 2000);
        Task download3 = FakeDownloadAsync(progressBar3, 3, 2000);

        await Task.WhenAll(download1, download2, download3);

        float elapsed = Time.time - startTime;
        UpdateSection3Text($"병렬 다운로드 끝 {elapsed} 초");
    }

    /// <summary>
    /// 가짜 다운로드 작업 시뮬레이션
    /// </summary>
    private async Task FakeDownloadAsync(Slider progressBar, int index, int durationMs)
    {
        int steps = 20;                         // 진행 단계를 20단계로 나눔
        int delayPerStep = durationMs / steps;  // 각 단계당 대기 시간 계산

        for (int i = 1; i < steps; i++)
        {
            progressBar.value = (float)i / steps;
            await Task.Delay(delayPerStep);
        }
        progressBar.value = 1f; // 완료
        Debug.Log($"[Section 3] 파일 {index}");
    }


    private void UpdateSection3Text(string msg)
    {
        section3StatusText.text = msg;
        Debug.Log($"[Section 3] {msg}");
    }
    #endregion

    #region Selection4
    /// <summary>
    /// 타임아웃 슬라이더 초기화
    /// </summary>
    private void InitTimeOutSlider()
    {
        timeOutSlider.minValue = 1f;
        timeOutSlider.maxValue = 5f;
        timeOutSlider.value = 3f;
        OnTimeOutSliderValueChanged(timeOutSlider.value);

        timeOutSlider.onValueChanged.AddListener(OnTimeOutSliderValueChanged);
    }

    /// <summary>
    /// 타임아웃 슬라이더 값 변경 이벤트
    /// </summary>
    private void OnTimeOutSliderValueChanged(float value)
    {
        timeOutValueText.text = $"{value} 초";
    }

    private void UpdateSection4Text(string msg)
    {
        section4StatusText.text = msg;
        Debug.Log($"[Section 4] {msg}");
    }

    /// <summary>
    /// 타임아웃 다운로드 시뮬레이션
    /// </summary>
    public async void OnTimeOutDownloadClicked()
    {
        UpdateSection4Text("다운로드 시작");
        Task downLoadTask = Task.Delay(4000); // 4초 다운로드 작업
        Task timeOutTask = Task.Delay((int)(timeOutSlider.value * 1000)); // 슬라이더 값에 따른 타임아웃 작업


        Task completedTask = await Task.WhenAny(downLoadTask, timeOutTask); // 먼저 완료되는 작업 대기

        // 어떤 작업이 완료되었는지에 따라 결과 처리
        if (completedTask == downLoadTask)
        {
            UpdateSection4Text("다운로드 완료");
        }
        else
        {
            UpdateSection4Text("타임 아웃");
        }
    }
    #endregion

    #region Section5

    /// <summary>
    /// 안전 코드 실행
    /// </summary>
    public async void OnSafeCodeClicked()
    {
        UpdateSection5Text("안전 코드 실행 시작");

        await Task.Delay(1000);

        cubeObjTr.position += Vector3.up * 0.5f;

        UpdateSection5Text("큐브 이동 완료");
    }

    /// <summary>
    /// 위험 코드 실행
    /// </summary>
    public async void OnUnsafeCodeClicked()
    {
        UpdateSection5Text("위험 코드 실행 시작");

        await Task.Run(() =>
            {
                Thread.Sleep(1000); // 메인 스레드가 아닌 별도의 스레드에서 실행됨
                cubeObjTr.position += Vector3.up * 0.5f;    // Unity 오브젝트에 접근 시도 (안전하지 않음)
            }
        );

        UpdateSection5Text("위험 코드 실행 완료");
    }

    private void UpdateSection5Text(string msg)
    {
        section5StatusText.text = msg;
        Debug.Log($"[Section 5] {msg}");
    }
    #endregion

    #region Section6
    private CancellationTokenSource cancelCts = new CancellationTokenSource();

    /// <summary>
    /// 작업 시작
    /// </summary>
    public async void OnStartTaskClicked()
    {
        UpdateSection6Text("시작");

        try
        {
            await UpdateProgressBar4Async(progressBar4);

        }
        catch (OperationCanceledException)
        {
            return;
        }
        UpdateSection6Text("정상적으로 완료");
    }

    /// <summary>
    /// 작업 취소 버튼
    /// </summary>
    public void OnCancelTaskClicked()
    {
        ProgressCancel();
    }
    
    /// <summary>
    /// 작업 취소
    /// </summary>
    public void ProgressCancel()
    {
        cancelCts.Cancel();
        cancelCts.Dispose();
        cancelCts = new CancellationTokenSource();
        UpdateSection6Text("캔슬");
    }

    /// <summary>
    /// 진행 작업
    /// </summary>
    private async Task UpdateProgressBar4Async(Slider progressBar)
    {
        int totalSteps = 20;
        for (int i = 0; i < totalSteps; i++)
        {
            progressBar.value = (float)i / totalSteps;
            await Task.Delay(200, cancelCts.Token);
        }
        progressBar.value = 1f;
    }
    

    private void UpdateSection6Text(string msg)
    {
        section6StatusText.text = msg;
        Debug.Log($"[Section 6] {msg}");
    }
    #endregion
}
