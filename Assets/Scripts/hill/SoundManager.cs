//@
// Sound Manager.

// Updated 2015.11.21
// Hill Lu
//-----------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;


public enum  BGMStage
{
    TITLE = 0,
    OPENING = 1,
    FIGHT_STAGE_1 = 2,
    //FIGHT_EVOLUTION = 3,
    FIGHT_STAGE_2 = 4,
    END_WIN = 5,
    END_LOSE = 6 
}

public enum SFXType
{
    ROBOT_PUNCH = 0,
    ROBOT_PUNCH_FAKE = 1,
    ROBOT_JET = 2,
    ROBOT_HURT = 3,
    ROBOT_CONNECT = 4,
    ROBOT_DISCONNECT = 5,
    ROBOT_POSE_HIT = 6,
    ROBOT_POSE_COMPLETE = 7,
    ROBOT_SPECIAL_ATK = 8,
    BUILDING_EXPLOSION = 9,
    MONSTER_ATK = 10,
    MONSTER_SPATK = 11,
    MONSTER_SPATK_SPIN = 12,
    ROBOT_BROKEN = 13
}

public enum DIALOGRole
{
    COMMANDER = 0
}

public enum DIALOG
{
    COMMANDER_001 = 0,
    COMMANDER_002 = 1,
    COMMANDER_003 = 2,
    COMMANDER_004 = 3,
    COMMANDER_005 = 4
}

[System.Serializable]
public class SoundManager : GenericSingleton<SoundManager>
{

    protected SoundManager() { }

    public Transform soundManager;
    public Transform sfxManager;
    public GameObject sfxExplosionGO;

    public AudioMixerSnapshot amsNormal;
    public AudioMixerSnapshot amsDialogSpeaking;

    public AudioSource bgmAS;
    public AudioSource[] dialogAS;

    public AudioClip bgmTitleC;
    public AudioClip bgmOpeningC;
    public AudioClip bgmFightStage1C;
    public AudioClip bgmFightStage2C;
    public AudioClip bgmEndingWinC;
    public AudioClip bgmEndingLoseC;

    public AudioClip sfxPunchC;
    public AudioClip sfxPunchFakeC;
    public AudioClip[] sfxJetC;
    public AudioClip sfxHurtC;
    public AudioClip sfxConnectC;
    public AudioClip sfxDisconnectC;
    public AudioClip sfxExplosionC;
    public AudioClip sfxPoseHitC;
    public AudioClip sfxLaserC;
    public AudioClip sfxMonsterATKC;
    public AudioClip sfxMonsterSPATKC;
    public AudioClip sfxMonsterSPATKSpinC;
    public AudioClip sfxRobotBrokenC;

    public AudioClip[] dialogC;

    //test
    public Transform testGO;

    const int bgmAmount = 7;//setnum
    const int sfxAmount = 14;//setnum
    const int dialogRoleAmount = 1; //setnum
    SoundParam[] bgm = new SoundParam[bgmAmount];   //setnum
    SoundParam[][] sfx = new SoundParam[sfxAmount][];   //setnum

    SoundParam currentBGMsp;
    bool startFallingPitch = false;
    bool startRisingPitch = false;
    bool isInSlowMotion;
    bool canRemoveNow = true;
    bool soundManagerEffective = true;

    List<GameObject> sfxToBeRemove = new List<GameObject>();
    

    class SoundParam
    {
        public AudioClip clip;
        public int length;
        public float volumn;
        public float pitch;
        public float pitchUpperBound; //used for random pitch
        public bool loop;

        public SoundParam(AudioClip clip, float volumn, float pitch, float pitchUpperBound,bool loop)
        {
            this.clip = clip;
            this.volumn = volumn;
            this.pitch = pitch;
            this.pitchUpperBound = pitchUpperBound;
            this.loop = loop;
        }

        public SoundParam(AudioClip clip, float volumn, float pitch,float pitchUpperBound)
            : this(clip, volumn, pitch, pitchUpperBound,true)
        {        }

        public SoundParam(AudioClip clip, float volumn, float pitch)
            : this(clip,volumn,pitch,pitch)
        {        }

        public SoundParam()
        {        }
    }

    void Start()
    {
		// Debug.Log("Sound Manager start working");
        Initialize();//initialize sound param
        StartCoroutine("CoRoutineRemoveSFX");
    }

    void Update()
    {

        //hotkeys for testing
        //if (Input.GetKeyDown("t")) bgmPlay(BGMStage.TITLE);
        //if (Input.GetKeyDown("o")) bgmPlay(BGMStage.OPENING);
        //if (Input.GetKeyDown("1")) bgmPlay(BGMStage.FIGHT_STAGE_1);
        //if (Input.GetKeyDown("2")) bgmPlay(BGMStage.FIGHT_STAGE_2);
        //if (Input.GetKeyDown("w")) bgmPlay(BGMStage.END_WIN);
        //if (Input.GetKeyDown("l")) bgmPlay(BGMStage.END_LOSE);

        //if (Input.GetKeyDown("p")) sfxPlay(SFXType.ROBOT_PUNCH);
        //if (Input.GetKeyDown("f")) sfxPlay(SFXType.ROBOT_PUNCH_FAKE);
        //if (Input.GetKeyDown("j")) sfxPlay(SFXType.ROBOT_JET);
        //if (Input.GetKeyDown("h")) sfxPlay(SFXType.ROBOT_HURT);
        //if (Input.GetKeyDown("c")) sfxPlay(SFXType.ROBOT_CONNECT);
        //if (Input.GetKeyDown("d")) sfxPlay(SFXType.ROBOT_DISCONNECT);
        //if (Input.GetKeyDown("i")) sfxPlay(SFXType.ROBOT_POSE_HIT);
        //if (Input.GetKeyDown("s")) sfxPlay(SFXType.ROBOT_POSE_COMPLETE);
        //if (Input.GetKeyDown("l")) sfxPlay(SFXType.ROBOT_SPECIAL_ATK);
        //if (Input.GetKeyDown("e")) sfxPlay3D(SFXType.BUILDING_EXPLOSION, testGO);
        //if (Input.GetKeyDown("a")) sfxPlay(SFXType.MONSTER_ATK);
        //if (Input.GetKeyDown("t")) sfxPlay(SFXType.MONSTER_SPATK);
        //if (Input.GetKeyDown("k")) sfxPlay(SFXType.MONSTER_SPATK_SPIN);
        //if (Input.GetKeyDown("b")) sfxPlay(SFXType.ROBOT_BROKEN);

        //if (Input.GetKeyDown("5")) DialogPlay(DIALOGRole.COMMANDER, DIALOG.COMMANDER_001, 1f);
        //if (Input.GetKeyDown("6")) DialogPlay(DIALOGRole.COMMANDER, DIALOG.COMMANDER_002, 1f);
        //if (Input.GetKeyDown("7")) DialogPlay(DIALOGRole.COMMANDER, DIALOG.COMMANDER_003, 1f);
        //if (Input.GetKeyDown("8")) DialogPlay(DIALOGRole.COMMANDER, DIALOG.COMMANDER_004, 1f);
        //if (Input.GetKeyDown("9")) DialogPlay(DIALOGRole.COMMANDER, DIALOG.COMMANDER_005, 1f);

        //if (Input.GetKeyDown("s")) EnterSlowMotion();
        //if (Input.GetKeyDown("x")) ExitSlowMotion();
    }

    void RemoveSFX()
    {
        Debug.Log("Destroy");
        foreach (GameObject go in sfxToBeRemove)
        {
            Destroy(go);
        }
        sfxToBeRemove.Clear();
    }

    IEnumerator CoRoutineRemoveSFX()
    {
        Debug.Log("Start coroutine removesfx");
        while (canRemoveNow)
        {
                yield return new WaitForSeconds(2f);
            if (canRemoveNow)
            {
                RemoveSFX();
            }      
        }
        Debug.Log("Exit coroutine removesfx");
    }

    //Initialize SFX Param (volumn and pitch of each clip)
    void Initialize() {
        Debug.Log("Initialize Sounds");
        int i,j;

        //bgm initiate
        bgm[(int)BGMStage.TITLE] = new SoundParam(bgmTitleC, 1f, 1f,1f,false);
        bgm[(int)BGMStage.OPENING] = new SoundParam(bgmOpeningC, 0.9f, 1f);
        bgm[(int)BGMStage.FIGHT_STAGE_1] = new SoundParam(bgmFightStage1C, 1f, 1f);
        bgm[(int)BGMStage.FIGHT_STAGE_2] = new SoundParam(bgmFightStage2C, 1f, 1.15f);
        bgm[(int)BGMStage.END_WIN] = new SoundParam(bgmEndingWinC, 1f, 1f,1f,false);
        bgm[(int)BGMStage.END_LOSE] = new SoundParam(bgmEndingLoseC, 1f, 1f,1f,false);

        //sfx initiate
        for (j = 0; j <= sfxAmount - 1; j++)
        {
            if (j == (int)SFXType.ROBOT_JET)
            {
                sfx[j] = new SoundParam[2];
            }
            else
            {
                sfx[j] = new SoundParam[1];
            }
            
        }

        sfx[(int)SFXType.ROBOT_PUNCH][0] = new SoundParam(sfxPunchC, 1f, 0.8f, 1.2f);
        sfx[(int)SFXType.ROBOT_PUNCH_FAKE][0] = new SoundParam(sfxPunchFakeC, 1f, 0.8f, 1.2f);
        for (i = 0; i <= sfxJetC.Length - 1; i++)
        {
            sfx[(int)SFXType.ROBOT_JET][i] = new SoundParam(sfxJetC[i], 1f, 0.8f, 1.2f);
        }
        sfx[(int)SFXType.ROBOT_HURT][0] = new SoundParam(sfxHurtC, 1f, 0.8f, 1.2f);
        sfx[(int)SFXType.ROBOT_CONNECT][0] = new SoundParam(sfxConnectC, 1f, 1f);
        sfx[(int)SFXType.ROBOT_DISCONNECT][0] = new SoundParam(sfxDisconnectC, 1f, 1f);
        sfx[(int)SFXType.ROBOT_POSE_HIT][0] = new SoundParam(sfxPoseHitC, 1f, 1f);
        sfx[(int)SFXType.ROBOT_POSE_COMPLETE][0] = new SoundParam(sfxPoseHitC, 1f, 2f);
        sfx[(int)SFXType.ROBOT_SPECIAL_ATK][0] = new SoundParam(sfxLaserC, 1f, 1f);
        sfx[(int)SFXType.BUILDING_EXPLOSION][0] = new SoundParam(sfxExplosionC, 1f, 0.8f, 1.2f);
        sfx[(int)SFXType.MONSTER_ATK][0] = new SoundParam(sfxMonsterATKC, 1f, 1f);
        sfx[(int)SFXType.MONSTER_SPATK][0] = new SoundParam(sfxMonsterSPATKC, 1f, 1f);
        sfx[(int)SFXType.MONSTER_SPATK_SPIN][0] = new SoundParam(sfxMonsterSPATKSpinC, 1f, 1f);
        sfx[(int)SFXType.ROBOT_BROKEN][0] = new SoundParam(sfxRobotBrokenC, 1f, 0.6f);
    }

    //bgm
    public void bgmPlay(BGMStage stage)
    {
        Debug.Log("Play BGM");
        StartCoroutine(CoRoutineBgmPlay(bgmAS, stage, 10f, 10f));
        currentBGMsp = bgm[(int)stage];
    }

    IEnumerator CoRoutineBgmPlay(AudioSource audioS, BGMStage stage, float fadeOutSpeed, float fadeInSpeed)
    {
        SoundParam sp = new SoundParam();

        StartCoroutine(MusicFadeOut(audioS, fadeOutSpeed, 0.01f, true));
        while (audioS.isPlaying)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        sp = bgm[(int)stage];

        bgmAS.clip = sp.clip;
        bgmAS.pitch = sp.pitch;
        bgmAS.loop = sp.loop;
        StartCoroutine(MusicFadeIn(audioS, fadeInSpeed, sp.volumn, true));
    }


    //sfx
    public void sfxPlay(SFXType type)
    {
        //Debug.Log("Play SFX");
        SoundParam sp = new SoundParam();
        int randomSP;
        float randomPitch;

        randomSP = Random.Range(0, sfx[(int)type].Length);
        sp = sfx[(int)type][randomSP];

        randomPitch = Random.Range(sp.pitch, sp.pitchUpperBound);
        AudioPlay(sp.clip, sfxManager, sp.volumn, randomPitch);
    }

    public void sfxPlay3D(SFXType type, Transform colliderGOT)
    {
        //Debug.Log("Play SFX3D");
        GameObject go;
        AudioSource goAS = new AudioSource();
        SoundParam sp = new SoundParam();
        int randomSP;
        float randomPitch;

        randomSP = Random.Range(0, sfx[(int)type].Length);

        //Debug.Log("start inistantiate 3d sfx");
        go = Instantiate(sfxExplosionGO, colliderGOT.position, colliderGOT.rotation) as GameObject;
        go.transform.parent = sfxManager;
        sp = sfx[(int)type][randomSP];

        goAS = go.GetComponent<AudioSource>();
        randomPitch = Random.Range(sp.pitch, sp.pitchUpperBound);
        goAS.clip = sp.clip;
        goAS.loop = false;
        goAS.Play();

        StartCoroutine(AudioDestory(go,goAS.clip.length));
        //Destroy(go, goAS.clip.length);
    }


    //dialog

    public void DialogPlay(DIALOGRole role,DIALOG dialogID, float volumn)
    {
        Debug.Log("Dialog Play");
        StartCoroutine(CoRoutineDialogPlay((int)role, (int)dialogID, volumn));
    }

    IEnumerator CoRoutineDialogPlay(int RoleID, int dialogID, float volume)
    {
        while (dialogAS[RoleID].isPlaying)
        {
            //Debug.Log("Sound Manager Warning: Previous dialog doesn't finish. COULD CAUSE SOME SOUNDS ERROR.");
            yield return new WaitForSeconds(Time.deltaTime);
        }
        dialogAS[RoleID].clip = dialogC[dialogID];
        dialogAS[RoleID].volume = volume;
        dialogAS[RoleID].Play();
        amsDialogSpeaking.TransitionTo(.01f);
        yield return new WaitForSeconds(dialogAS[RoleID].clip.length);
        amsNormal.TransitionTo(.01f);
        Debug.Log("Dialog Over");
        // Debug.Log (dialogAS[RoleID].clip);
    }

    //special function
    public void EnterSlowMotion()
    {
        Debug.Log("Sound slow motion start");

        canRemoveNow = false;
        isInSlowMotion = true;
        StartCoroutine(PitchChange(bgmAS, -0.5f, 0.5f));
        StartCoroutine(SlowMotionSFXCheck());
    }
    
    public void ExitSlowMotion()
    {
        Debug.Log("Sound slow motion end");
        AudioSource[] sfxASs;

        isInSlowMotion = false;
        StartCoroutine(PitchChange(bgmAS, 0.5f, currentBGMsp.pitch));

        sfxASs = GetComponentsInChildren<AudioSource>();
		for (int i = sfxASs.Length - 1; i >= 0; i--)
        {
            StartCoroutine(PitchChange(sfxASs[i], 0.5f, 1f));
        }
        canRemoveNow = true;
        StartCoroutine("CoRoutineRemoveSFX");
    }

    IEnumerator SlowMotionSFXCheck()
    {
        AudioSource[] sfxASs;
        while (isInSlowMotion)
        {
            sfxASs = GetComponentsInChildren<AudioSource>();
			for (int i = sfxASs.Length - 1; i >= 0; i--)
			{
				StartCoroutine(PitchChange(sfxASs[i], -0.5f, 0.5f));
			}
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    //private function
    IEnumerator MusicFadeIn(AudioSource audios, float fadeInSpeed, float maxVolumn, bool restart)
    {
        //Debug.Log("Start fade in.");
        if (restart)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            audios.Play();
        }
        while (audios.volume < maxVolumn)
        {
            audios.volume += fadeInSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //		Debug.Log("Complete fade in.");
    }

    void MusicFadeInReplay(AudioSource audios, float fadeInSpeed, float maxVolumn)
    {
        audios.Stop();
        audios.volume = 0;
        audios.pitch = 1f;;
        StartCoroutine(MusicFadeIn(audios, fadeInSpeed, maxVolumn, true));
    }

    IEnumerator MusicFadeOut(AudioSource audios, float fadeOutSpeed, float minVolumn,bool stop)
    {
        //		Debug.Log("Start fade out.");
        while (audios.volume >= minVolumn)
        {
            audios.volume -= fadeOutSpeed * Time.deltaTime;
            if (audios.isPlaying)
            {
                //Debug.Log("audios.isPlaying = " + audios.isPlaying);
                yield return new WaitForSeconds(Time.deltaTime);
            }
                
        }
        if (stop)
        {
            audios.Stop();
        }
    }

    IEnumerator PitchChange(AudioSource audios, float changeSpeed, float toPitch)
    {
        if (!audios.isPlaying)
        {
            yield break;
        }
        if (changeSpeed > 0)
        {
            startRisingPitch = true;
            while (audios.pitch < toPitch)
            {
                audios.pitch += changeSpeed * Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            startRisingPitch = false;
        }
        else
        {
            startFallingPitch = true;
            while (audios.pitch >= toPitch)
            {
                audios.pitch += changeSpeed * Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
                if (startRisingPitch)
                {
                    yield break;
                }
            }
            startFallingPitch = false;
        }
    }


    IEnumerator AudioDestory(GameObject go,float t)
    {
        //Debug.Log("Ready to add to list");
        yield return new WaitForSeconds(t);
        sfxToBeRemove.Add(go);
        //Debug.Log("adkd to list");
    }

    /// Plays a sound by creating an empty game object with an AudioSource
    /// and attaching it to the given transform (so it moves with the transform). Destroys it after it finished playing.
    void AudioPlay(AudioClip clip, Transform emitter, float volume, float pitch)
    {
        //Create an empty game object
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;

        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = false;
        source.Play();

        StartCoroutine(AudioDestory(go,source.clip.length));
        //Destroy(go, clip.length+5f);
    }

    void AudioPlay(AudioClip clip)
    {
        AudioPlay(clip, soundManager, 1f, 1f);
    }

    void AudioPlay(AudioClip clip, Transform emitter)
    {
        AudioPlay(clip, emitter, 1f, 1f);
    }

    void AudioPlay(AudioClip clip, Transform emitter, float volume)
    {
        AudioPlay(clip, emitter, volume, 1f);
    }

    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing.
    //void AudioPlay(AudioClip clip, Vector3 point, float volume, float pitch)
    //{
    //    //Create an empty game object
    //    GameObject go = new GameObject("Audio: " + clip.name);
    //    go.transform.position = point;

    //    //Create the source
    //    AudioSource source = go.AddComponent<AudioSource>();
    //    source.clip = clip;
    //    source.volume = volume;
    //    source.pitch = pitch;
    //    source.Play();
    //    Destroy(go, clip.length+2f);
    //}

    //void AudioPlay(AudioClip clip, Vector3 point)
    //{
    //    AudioPlay(clip, point, 1f, 1f);
    //}

    //void AudioPlay(AudioClip clip, Vector3 point, float volume)
    //{
    //    AudioPlay(clip, point, volume, 1f);
    //}


}
