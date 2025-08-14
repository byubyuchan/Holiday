using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
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
    public float SFXVolume;
    public int channels;
    AudioSource[] SFXPlayers;
    int channelIndex;

    [Header("# Volume UI")]
    public Scrollbar BGMScrollbar;
    public Scrollbar SFXScrollbar;

    [System.Serializable]
    public class SFXPair
    {
        public string key;      // 효과음 이름
        public AudioClip clip;  // 실제 클립
    }

    public List<SFXPair> SFXPairs = new List<SFXPair>();
    Dictionary<string, AudioClip> SFXDict;

    void Awake()
    {
        Debug.Assert(channels > 0, "Channels를 1 이상으로 설정하세요.");
        instance = this;
        Init();
        Debug.Assert(SFXPlayers != null, "SFXPlayers 배열이 null입니다!");
        foreach (var sfx in SFXPlayers)
            Debug.Assert(sfx != null, "SFXPlayers 배열에 null AudioSource가 있습니다!");

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

        SFXDict = new Dictionary<string, AudioClip>();
        foreach (var pair in SFXPairs)
        {
            if (!SFXDict.ContainsKey(pair.key))
                SFXDict.Add(pair.key, pair.clip);
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

        SFXPlayers = new AudioSource[channels];
        for (int i = 0; i < channels; i++)
        {
            GameObject SFXObject = new GameObject("SFXPlayer");
            // 자식 트랜스폼으로 지정
            SFXObject.transform.parent = transform;
            SFXPlayers[i] = SFXObject.AddComponent<AudioSource>();
            SFXPlayers[i].playOnAwake = false;
            SFXPlayers[i].volume = SFXVolume;
            // BGM만 하이패스 이펙트가 적용됨
            SFXPlayers[i].bypassListenerEffects = true;
        }
    }

    public void PlaySFX(string key)
    {
        if (SFXPlayers == null || SFXPlayers.Length == 0)
        {
            Debug.LogWarning("SFXPlayers가 초기화되지 않았거나 채널이 0입니다.");
            return;
        }
        if (SFXDict == null)
        {
            Debug.LogWarning("SFXDict가 null입니다. 초기화 확인 필요!");
            return;
        }
        if (!SFXDict.ContainsKey(key))
        {
            Debug.LogWarning($"SFXDict에 키 '{key}'가 없습니다.");
            return;
        }

        // 일반적으로 사용: PlaySFX("Enemy_Die");
        for (int i = 0; i < SFXPlayers.Length; i++)
        {
            int idx = (i + channelIndex) % SFXPlayers.Length;
            if (SFXPlayers[idx].isPlaying) continue;

            if (SFXDict.TryGetValue(key, out AudioClip clip))
            {
                SFXPlayers[idx].clip = clip;
                SFXPlayers[idx].Play();
            }
            channelIndex = (idx + 1) % SFXPlayers.Length;
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
