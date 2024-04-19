using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum ESound
{
    BGM1, BGM2,
    Hit1, Hit2,
    Jump1, Jump2,
    NomalAttack, SmashAttack,
    GameStart, GameEnd,
    Victoty1,
}
enum EAudio
{
    audioSourceBGM,
    audioSourceGameSet,
    audioSourceHit,
    audioSourceCharacter,

}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip[] audioClips; // 오디오 클립을 저장할 배열
    private AudioSource[] audioSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정
            audioSource = new AudioSource[4];

            audioSource[0] = gameObject.AddComponent<AudioSource>();
            audioSource[1] = gameObject.AddComponent<AudioSource>();
            audioSource[2] = gameObject.AddComponent<AudioSource>();
            audioSource[3] = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlaySound((int)EAudio.audioSourceBGM,(int)ESound.BGM1);
    }
    public void StopSound(int _audioSource)
    {
        audioSource[_audioSource].Stop();
    }

    // 오디오 클립 재생 메소드. 오디오 클립 ID를 매개변수로 받습니다.
    public void PlaySound(int _audioSource, int clipId ,float delay = 0f)
    {
        if (audioClips[clipId] && audioSource[_audioSource])
        {
            StartCoroutine(CPlaySound(_audioSource, clipId, delay));
        }

    }
    IEnumerator CPlaySound(int _audioSource, int clipId, float delay)
    {
        if (delay > 0.1f)
        {
            yield return new WaitForSeconds(delay);
        }
        audioSource[_audioSource].Stop();
        audioSource[_audioSource].clip = audioClips[clipId]; // 지정된 오디오 클립으로 설정
        audioSource[_audioSource].Play(); // 재생
    }
    
}
