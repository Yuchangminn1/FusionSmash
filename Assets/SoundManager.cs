using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ESound
{
    BGM1,
    BGM2,

}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip[] audioClips; // 오디오 클립을 저장할 배열
    private AudioSource audioSource; // 오디오 소스 컴포넌트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정
            audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 컴포넌트 추가
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlaySound(0);
    }
    public void StopSound()
    {
        audioSource.Stop();
    }

    // 오디오 클립 재생 메소드. 오디오 클립 ID를 매개변수로 받습니다.
    public void PlaySound(int clipId)
    {
        if (audioClips[clipId])
        {
            if (clipId >= 0 && clipId < audioClips.Length)
            {
                audioSource.clip = audioClips[clipId]; // 지정된 오디오 클립으로 설정
                audioSource.Play(); // 재생
            }
            else
            {
                Debug.LogWarning("PlaySound: clipId out of range");
            }
        }
        
    }
}
