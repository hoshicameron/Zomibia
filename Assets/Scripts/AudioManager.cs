using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Guns Audio Clips")]
    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip aimAudioClip;
    [SerializeField] private AudioClip riffleReloadAmmoLeftAudioClip;
    [SerializeField] private AudioClip riffleReloadOutOFAmmoAudioClip;
    [SerializeField] private AudioClip handGunReloadAmmoLeftAudioClip;
    [SerializeField] private AudioClip handGunReloadOutOFAmmoAudioClip;
    [SerializeField] private AudioClip triggerAudioClip;

    [Header("Player Audio Clips")]
    [SerializeField] private AudioClip walkAudioClip;    //the player walk sound
    [SerializeField] private AudioClip runAudioClip;     //the player run sound
    [SerializeField] private AudioClip jumpAudioClip;     //the player jump sound
    [SerializeField] private AudioClip landAudioClip;     //the player land sound

    [Header("Voices")]
    [SerializeField] private AudioClip deathAudioClip;     //the player death voice

    [Header("Sting Audio Clips")]
    [SerializeField] private AudioClip[] explosionAudioClips;
    [SerializeField] private AudioClip[] casingAudioClips;
    [SerializeField] private AudioClip[] grenadeAudioClips;
    [SerializeField] private AudioClip[] missAudioClips;
    [SerializeField] private AudioClip impactAudioClip;
    [SerializeField] private AudioClip ammoPickupAudioClip;
    [SerializeField] private AudioClip aidPickupAudioClip;

    [Header("Ambient Audio")]
    [SerializeField] private AudioClip ambientAudioClip;
    [SerializeField] private AudioClip musicAudioClip;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup ambientGroup; //The ambient mixer group
    [SerializeField] private AudioMixerGroup musicGroup;  //The music mixer group
    [SerializeField] private AudioMixerGroup stingGroup;  //The sting mixer group
    [SerializeField] private AudioMixerGroup playerGroup; //The player mixer group
    [SerializeField] private AudioMixerGroup effectGroup; //The efect mixer group
    [SerializeField] private AudioMixerGroup impactGroup; //The efect mixer group

    AudioSource ambientSource;			//Reference to the generated ambient Audio Source
    AudioSource musicSource;            //Reference to the generated music Audio Source
    AudioSource stingSource;            //Reference to the generated sting Audio Source
    AudioSource playerSource;           //Reference to the generated player Audio Source
    AudioSource effectSource;           //Reference to the generated effect Audio Source
    AudioSource impactSource;           //Reference to the generated impact Audio Source

    private void Awake()
    {
        //If an AudioManager exists and it is not this...
        if (Instance != null && Instance != this)
        {
            //...destroy this. There can be only one AudioManager
            Destroy(gameObject);
            return;
        }

        //This is the current AudioManager and it should persist between scene loads
        Instance = this;
        DontDestroyOnLoad(gameObject);


        //Generate the Audio Source "channels" for our game's audio
        ambientSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
        musicSource		= gameObject.AddComponent<AudioSource>() as AudioSource;
        stingSource		= gameObject.AddComponent<AudioSource>() as AudioSource;
        playerSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
        playerSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
        effectSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
        impactSource	= gameObject.AddComponent<AudioSource>() as AudioSource;


        //Assign each audio source to its respective mixer group so that it is
        //routed and controlled by the audio mixer
        ambientSource.outputAudioMixerGroup = ambientGroup;
        musicSource.outputAudioMixerGroup	= musicGroup;
        stingSource.outputAudioMixerGroup	= stingGroup;
        playerSource.outputAudioMixerGroup	= playerGroup;
        effectSource.outputAudioMixerGroup	= effectGroup;
        impactSource.outputAudioMixerGroup = impactGroup;

    }

    private void Start()
    {
        musicSource.clip = musicAudioClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public static void PlayWalkAudio()
    {
        if(Instance==null)
            return;

        Instance.playerSource.clip = Instance.walkAudioClip;
        Instance.playerSource.loop = true;
        Instance.playerSource.Play();
    }

    public static void PlayRunAudio()
    {
        if(Instance==null)
            return;

        Instance.playerSource.clip = Instance.runAudioClip;
        Instance.playerSource.loop = true;
        Instance.playerSource.Play();
    }

    public static void PlayShootAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.shootAudioClip;
        Instance.stingSource.Play();
    }

    public static void PlayTriggerAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.triggerAudioClip;
        Instance.stingSource.Play();
    }

    public static void StopPlayerAudio()
    {
        if(Instance==null)
            return;

        Instance.playerSource.Stop();
    }

    public static void PlayerReloadAmmoLeftAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.riffleReloadAmmoLeftAudioClip;
        Instance.stingSource.Play();
    }
    public static void PlayerReloadOutOfAmmoAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.riffleReloadOutOFAmmoAudioClip;
        Instance.stingSource.Play();
    }

    public static void PlayExplosionAudioClip()
    {
        if(Instance==null)
            return;

        var index = Random.Range(0, Instance.explosionAudioClips.Length);
        Instance.effectSource.clip = Instance.explosionAudioClips[index];
        Instance.effectSource.Play();
    }

    public static void PlayCasingAudio()
    {
        if(Instance==null)
            return;
        foreach (var audioClip in Instance.casingAudioClips)
        {
            Instance.effectSource.clip = audioClip;
            Instance.effectSource.Play();
        }
    }

    public static void PlayGrenadeExplosionAudio()
    {
        if(Instance==null)
            return;

        var index = Random.Range(0, Instance.grenadeAudioClips.Length);
        Instance.effectSource.clip = Instance.explosionAudioClips[index];
        Instance.effectSource.Play();
    }

    public static void PlayHitImpactAudioClip()
    {
        if(Instance==null)
            return;

        Instance.impactSource.clip = Instance.impactAudioClip;
        Instance.impactSource.Play();
    }

    public static void PlayMissAudio()
    {
        if(Instance==null)
            return;

        var index = Random.Range(0, Instance.missAudioClips.Length);
        Instance.impactSource.clip = Instance.missAudioClips[index];
        Instance.impactSource.Play();

    }

    public static void PlayJumpAudio()
    {
        if(Instance==null)
            return;

        Instance.playerSource.clip = Instance.jumpAudioClip;
        Instance.playerSource.loop = false;
        Instance.playerSource.Play();

    }

    public static void PlayLandAudio()
    {
        if(Instance==null)
            return;

        Instance.playerSource.clip = Instance.landAudioClip;
        Instance.playerSource.loop = false;
        Instance.playerSource.Play();

    }

    public static void PlayAmmoPickupAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.ammoPickupAudioClip;
        Instance.stingSource.Play();

    }
    public static void PlayAidPickupAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.aidPickupAudioClip;
        Instance.stingSource.Play();
    }

    public static void PlayDeathAudio()
    {
        if(Instance==null)
            return;

        Instance.stingSource.clip = Instance.deathAudioClip;
        Instance.stingSource.Play();
    }






}
