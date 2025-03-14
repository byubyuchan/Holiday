using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip BGMClip;
    public float BGMVolume;
    AudioSource BGMPlayer;
    AudioHighPassFilter BGMEffet;

    [Header("#SFX")]
    public AudioClip[] SFXClips;
    public float SFXVolume;
    public int channels;
    AudioSource[] SFXPlayers;
    int channelIndex;

    public enum SFX { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }
    [Header("# Volume UI")]
    public Scrollbar BGMScrollbar;
    public Scrollbar SFXScrollbar;

    void Awake()
    {
        instance = this;
        Init();

        // 스크롤바 초기값 설정 및 이벤트 연결
        if (BGMScrollbar != null)
        {
            BGMScrollbar.value = BGMVolume;
            BGMScrollbar.onValueChanged.AddListener(SetBGMVolume);
        }
        if (SFXScrollbar != null)
        {
            SFXScrollbar.value = SFXVolume;
            SFXScrollbar.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    void Init()
    {
        GameObject BGMObject = new GameObject("BGMPlayer");
        // 자식 트랜스폼으로 지정
        BGMObject.transform.parent = transform;
        BGMPlayer = BGMObject.AddComponent<AudioSource>();
        BGMPlayer.playOnAwake = false;
        BGMPlayer.loop = true;
        BGMPlayer.volume = BGMVolume;
        BGMPlayer.clip = BGMClip;
        BGMEffet = Camera.main.GetComponent<AudioHighPassFilter>();

        GameObject SFXObject = new GameObject("SFXPlayer");
        // 자식 트랜스폼으로 지정
        SFXObject.transform.parent = transform;
        SFXPlayers = new AudioSource[channels];
        for (int i = 0; i < channels; i++)
        {
            SFXPlayers[i] = SFXObject.AddComponent<AudioSource>();
            SFXPlayers[i].playOnAwake = false;
            SFXPlayers[i].volume = SFXVolume;
            // BGM만 하이패스 이펙트가 적용됨
            SFXPlayers[i].bypassListenerEffects = true;
        }
    }

    public void PlaySFX(SFX sfx)
    {
        for (int i = 0; i < SFXPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % SFXPlayers.Length;

            if (SFXPlayers[loopIndex].isPlaying) continue;

            int ranIndex = 0;
            if (sfx == SFX.Hit || sfx == SFX.Melee) ranIndex = Random.Range(0, 2);

            SFXPlayers[0].clip = SFXClips[(int)sfx + ranIndex];
            SFXPlayers[0].Play();
            break;

        }
    }

    public void PlayBGM(bool isPlay)
    {
        if (isPlay) BGMPlayer.Play();
        else BGMPlayer.Stop();
    }

    public void EffectBGM(bool isPlay)
    {
        BGMEffet.enabled = isPlay;
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;
        BGMPlayer.volume = BGMVolume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        foreach (AudioSource sfx in SFXPlayers)
        {
            sfx.volume = SFXVolume;
        }
    }
}
