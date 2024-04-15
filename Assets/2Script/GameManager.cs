using Fusion;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    float starttime = 0f;
    public float gamePlayTime = 20f;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public List<PlayerInfo> playersInfo;

    public Image fadeImage; // 오브젝트의 Renderer 컴포넌트에 대한 참조
    public static GameManager Instance { get; private set; }
    public TMP_Text timerText;

    //[Networked] private TickTimer gameTimer { get; set; }
    [Networked]
    public TickTimer countdownTimer { get; set; }
    [Networked(OnChanged = nameof(ChangeplayingState))]
    public int playingState { get; set; }
    private bool CMISSpawn = false;

    [Networked]
    public int playTime { get; set; } = 0;
    LateUpdate lateUpdate = new LateUpdate();

    static void ChangeplayingState(Changed<GameManager> changed)
    {
        int newS = changed.Behaviour.playingState;
        changed.LoadOld();
        int oldS = changed.Behaviour.playingState;
        if (newS != oldS)
        {
            //Debug.Log("AttackCount = " + newS);
            changed.Behaviour.SetPlayingState(newS);
        }
    }

    void SetPlayingState(int _num)
    {
        //FadeIn_Out(3f);

        if (_num == (int)PlayingState.Waiting)
        {
            UIManager.Instance.OnGameWait();
            //FadeIn_Out(1f);
        }
        else if (_num == (int)PlayingState.Playing)
        {
            UIManager.Instance.OnGameStart();
        }
        else if (_num == (int)PlayingState.Stop)
        {
            UIManager.Instance.OnGameEnd();
        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetPlayer(PlayerInfo _playerinfo)
    {
        playersInfo.Add(_playerinfo);
        //playersInfo[_playerinfo.playerNumber-1] = _playerinfo;
    }


    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            playingState = (int)PlayingState.Waiting;
            Debug.Log($"playingState = {playingState}");
            //StartGame();
        }
        UIManager.Instance.OnGameWait();

        CMISSpawn = true;
        //FadeIn_Out(3f);
    }

    
    public void OnClickStartGame()
    {

        if (HasStateAuthority)
        {

            countdownTimer = TickTimer.CreateFromSeconds(Runner, gamePlayTime); // 예시로 60초 게임 타임 설정
            Debug.Log($"countdownTimer = {countdownTimer}");
            StartGame();
            starttime = Time.time;
        }
    }

    public int GetPlaying() { return playingState; }


    public override void FixedUpdateNetwork()
    {
        if (CMISSpawn)
        {
            if (HasStateAuthority)
            {
                if (playingState == (int)PlayingState.Playing)
                {
                    if (countdownTimer.ExpiredOrNotRunning(Runner))
                    {
                        // 타이머가 만료되면 게임 종료 처리
                        Debug.Log("타이머 끝");
                        playingState = (int)PlayingState.Stop;
                        //Debug.Log($"playingState = {playingState}");

                        EndGame();
                    }
                    playTime = (int)countdownTimer.RemainingTime(Runner);

                }
            }
        }

    }

    private void Update()
    {
        //Debug.Log("Update");

        if (CMISSpawn)
        {
            if (timerText && playingState == (int)PlayingState.Playing)
            {
                timerText.text = FormatTime(playTime);
                //Debug.Log("시간 가능중 ");
            }
        }
    }
    string FormatTime(int time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // 게임 시작을 처리하는 메서드

    public void StartGame()
    {
        UIManager.Instance.OnGameStart();

        
        playingState = (int)PlayingState.Playing;
        
        Debug.Log("STARTTTT");
    }
    // 게임 종료를 처리하는 메서드

    void EndGame()
    {
        // 게임 종료 로직 구현
        //FadeIn_Out(2f);
        Debug.Log("EndGame FadeInOut");

        UIManager.Instance.OnGameEnd();
        
        //foreach (var player in playersInfo)
        //{
        //    if (player != null)
        //        player.TriggerGameEnd();
        //}

        playingState = (int)PlayingState.Waiting;

    }

    public void FadeIn(float duration)
    {
        
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        StartCoroutine(FadelToFullAlpha(duration));
    }

    public void FadeOut(float duration)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        StartCoroutine(FadeToZeroAlpha(duration));
    }
    public void FadeIn_Out(float duration)
    {
        
        StartCoroutine(CFadelIn_Out(duration));
    }
    public void FadeIn_OutStop()
    {
        StopCoroutine("CFadelIn_Out");
    }
    private IEnumerator FadelToFullAlpha(float duration)
    {
        // Material의 Color의 Alpha 값을 0으로 설정합니다.
        
        while (fadeImage.color.a < 0.9f)
        {
            // Alpha 값을 점진적으로 증가시킵니다.

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a + (Time.deltaTime / duration));
            yield return lateUpdate;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

    }

    private IEnumerator FadeToZeroAlpha(float duration)
    {
        // Material의 Color의 Alpha 값을 1로 설정합니다.
        while (fadeImage.color.a > 0.0f)
        {
            // Alpha 값을 점진적으로 감소시킵니다.
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (Time.deltaTime / duration));
            yield return lateUpdate;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

    }

    private IEnumerator CFadelIn_Out(float duration)
    {
        // Material의 Color의 Alpha 값을 0으로 설정합니다.
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        while (fadeImage.color.a < 0.9f)
        {
            // Alpha 값을 점진적으로 증가시킵니다.
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a + (Time.deltaTime / 0.1f));
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        while (fadeImage.color.a > 0.1f)
        {
            // Alpha 값을 점진적으로 감소시킵니다.
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (Time.deltaTime / 2f));
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        yield return null;

    }
}