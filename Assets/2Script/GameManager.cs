using Fusion;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
enum PlayingState
{
    Stop,
    Waiting,
    Playing,
    Respawn,
    Death,
}
public class GameManager : NetworkBehaviour
{
    
    public Image fadeImage; // 오브젝트의 Renderer 컴포넌트에 대한 참조
    public static GameManager Instance { get; private set; }
    public TMP_Text timerText;

    //[Networked] private TickTimer gameTimer { get; set; }
    [Networked]
    public TickTimer countdownTimer { get; set; }
    [Networked] public int playingState { get; set; }
    private bool CMISSpawn = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //public bool IsGamePlaying
    //{
    //    get
    //    {
    //        if (Object != null)
    //        {
    //            return isGamePlaying;
    //        }
    //        return false;
    //    }
    //}

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            StartGame();
        }
        CMISSpawn = true;
        FadeOut(1f);

    }

    // 게임 시작을 처리하는 메서드
    public void StartGame()
    {
        if (Object.HasStateAuthority)
        {
            countdownTimer = TickTimer.CreateFromSeconds(Runner, 8f); // 예시로 60초 게임 타임 설정
            playingState = (int)PlayingState.Waiting;
        }

    }

    public int GetPlaying() { return playingState; }


    public override void FixedUpdateNetwork()
    {
        if ((int)PlayingState.Stop != playingState && countdownTimer.ExpiredOrNotRunning(Runner))
        {
            // 타이머가 만료되면 게임 종료 처리
            playingState = (int)PlayingState.Stop;
            EndGame();
        }
    }

    private void Update()
    {

        if (CMISSpawn)
        {
            if (timerText && playingState != (int)PlayingState.Stop)
            {
                // 게임 시간을 표시합니다. 실제 게임에서는 TickTimer의 남은 시간을 계산하여 표시해야 합니다.
                timerText.text = $"{FormatTime((int)countdownTimer.RemainingTime(Runner))}";
            }
        }

    }
    string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // 게임 종료를 처리하는 메서드
    void EndGame()
    {
        // 게임 종료 로직 구현
        FadeIn(1f);

    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeMaterialToFullAlpha(duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeMaterialToZeroAlpha(duration));
    }

    private IEnumerator FadeMaterialToFullAlpha(float duration)
    {
        // Material의 Color의 Alpha 값을 0으로 설정합니다.
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        while (fadeImage.color.a < 1.0f)
        {
            // Alpha 값을 점진적으로 증가시킵니다.
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a + (Time.deltaTime / duration));
            yield return null;
        }
    }

    private IEnumerator FadeMaterialToZeroAlpha(float duration)
    {
        // Material의 Color의 Alpha 값을 1로 설정합니다.
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        while (fadeImage.color.a > 0.0f)
        {
            // Alpha 값을 점진적으로 감소시킵니다.
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (Time.deltaTime / duration));
            yield return null;
        }
    }
}