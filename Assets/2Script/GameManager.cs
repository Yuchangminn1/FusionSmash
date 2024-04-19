using Fusion;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
enum EPlayingState
{
    Stop,
    Waiting,
    Playing,
    Respawn,
    Death,
    None,
}
enum ERoomState
{
    Waiting,
    Playing,
    End,

}
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    float starttime = 0f;
    public float gamePlayTime = 20f;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    public Image fadeImage; // 오브젝트의 Renderer 컴포넌트에 대한 참조
    public TMP_Text timerText;

    public GameObject startButton;

    //[Networked] private TickTimer gameTimer { get; set; }
    [Networked]
    public TickTimer countdownTimer { get; set; }
    [Networked(OnChanged = nameof(RoomStateChanged))]
    public int roomState { get; set; }
    private bool CMISSpawn = false;

    [Networked]
    public int playTime { get; set; } = 0;
    LateUpdate lateUpdate = new LateUpdate();

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();
    [Networked]

    public int playingPlayerNum { get; set; } = 0;

    float endingTime = 10f;

    [Networked(OnChanged = nameof(RoomStateChanged))]
    public NetworkString<_16> winnerName { get; set; } = "";

    static void WinnerNameChanged(Changed<GameManager> changed)
    {
        string newS = changed.Behaviour.winnerName.ToString();
        changed.Behaviour.SetWinnerText(newS);
    }
    void SetWinnerText(string _winnerName)
    {
        UIManager.Instance.winnerText.text = _winnerName;
    }
    static void RoomStateChanged(Changed<GameManager> changed)
    {
        int newS = changed.Behaviour.roomState;
        changed.LoadOld();
        int oldS = changed.Behaviour.roomState;
        if (newS != oldS)
        {
            //Debug.Log("AttackCount = " + newS);
            changed.Behaviour.SetRoomState(newS);
        }
    }

    void SetRoomState(int _num)
    {
        //FadeIn_Out(3f);

        //if (_num == (int)ERoomState.Waiting)
        //{
        //    UIManager.Instance.OnGameWait();
        //    //FadeIn_Out(1f);
        //}
        //else if (_num == (int)ERoomState.Playing)
        //{
        //    UIManager.Instance.OnGameStart();
        //}
        //else if (_num == (int)ERoomState.End)
        //{
        //    UIManager.Instance.OnGameEnd();
        //}

    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetPlayer(PlayerInfo _playerinfo)
    {
        if (roomState == (int)ERoomState.Playing)
        {
            playerInfos.Add(_playerinfo);
        }
    }


    public override void Spawned()
    {


        if (HasStateAuthority)
        {

            if (roomState == (int)ERoomState.Playing)
            {
                ;
            }
            else
            {
                roomState = (int)ERoomState.Waiting;
                Debug.Log($"playingState = {roomState}");
            }
        }
        else
        {
            if (startButton)
            {
                startButton.SetActive(false);
            }
        }
        //UIManager.Instance.OnGameWait();

        CMISSpawn = true;
        //FadeIn_Out(3f);


    }


    public void OnClickStartGame()
    {

        if (HasStateAuthority)
        {
            countdownTimer = TickTimer.CreateFromSeconds(Runner, gamePlayTime); // 예시로 60초 게임 타임 설정
            Debug.Log($"countdownTimer = {countdownTimer}");
            playerInfos.AddRange(FindObjectsOfType<PlayerInfo>());
            playingPlayerNum = playerInfos.Count;
            Debug.Log($"playingPlayerNum = {playingPlayerNum}");
            foreach (PlayerInfo playerinfo in playerInfos)
            {
                if (playerinfo.isSpawned)
                {
                    playerinfo.FadeIN = true;
                }

            }
            Invoke("StartGame", 0.5f);
            //StartGame();
        }
    }

    public int GetRoomState() { return roomState; }


    public override void FixedUpdateNetwork()
    {
        if (CMISSpawn)
        {
            if (HasStateAuthority || HasInputAuthority)
            {
                if (roomState == (int)ERoomState.Playing)
                {

                    if (countdownTimer.ExpiredOrNotRunning(Runner) || playingPlayerNum == 1)
                    {
                        // 타이머가 만료되면 게임 종료 처리
                        Debug.Log("타이머 끝");
                        roomState = (int)ERoomState.End;
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
            if (timerText && roomState == (int)ERoomState.Playing)
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
        int i = 1;

        foreach (PlayerInfo playerinfo in playerInfos)
        {
            playerinfo.PlayerNumber = i;
            ++i;
            playerinfo.PlayingState = (int)EPlayingState.Playing;
            playerinfo.TriggerInit();
        }

        roomState = (int)ERoomState.Playing;
        starttime = Time.time;

        Debug.Log("STARTTTT");

    }
    
    // 게임 종료를 처리하는 메서드

    void EndGame()
    {
        // 게임 종료 로직 구현
        foreach (PlayerInfo playerinfo in playerInfos)
        {
            if (playerinfo.isSpawned)
            {
                playerinfo.GameEndToWaiting(endingTime);
            }
        }
        //UIManager.Instance.OnGameEnd();
        roomState = (int)ERoomState.Waiting;
        playerInfos.Clear();

    }
    void QWEQWE()
    {

    }
    public void FadeIn(float duration)
    {

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        StartCoroutine(FadelToFullAlpha(duration));
    }

    public void FadeOut(float duration, float _waitTime = 0f)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        StartCoroutine(FadeToZeroAlpha(duration, _waitTime));
    }
    public void FadeIn_Out(float duration, float _waitTime = 0f)
    {

        StartCoroutine(CFadelIn_Out(duration, _waitTime));
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

    private IEnumerator FadeToZeroAlpha(float duration, float _waitTime)
    {
        if (_waitTime > 0.01f)
        {
            yield return new WaitForSeconds(_waitTime);
        }
        // Material의 Color의 Alpha 값을 1로 설정합니다.
        while (fadeImage.color.a > 0.0f)
        {
            // Alpha 값을 점진적으로 감소시킵니다.
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (Time.deltaTime / duration));
            yield return lateUpdate;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

    }

    private IEnumerator CFadelIn_Out(float duration, float _waitTime)
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
        if (_waitTime > 0.01f)
        {
            yield return new WaitForSeconds(_waitTime);
        }
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