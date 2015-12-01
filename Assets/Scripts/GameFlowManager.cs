using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFlowManager : MonoBehaviour
{
    public float titleTime;
    public float endTime;

    public Camera flowCamera;
    public RawImage img;

    public MovieTexture startCutScene;
    public AudioSource startCutSceneAudio;
    //public AudioClip startCutSceneAudio;
    public KeyCode skipKey;

    public Texture2D title;
    public Texture2D goodEnd;
    public Texture2D badEnd;
    public Texture2D credit;

    

	// Use this for initialization
	private void Start ()
    {
        StartCoroutine(IntroCoroutine());
	}

    private IEnumerator IntroCoroutine()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.bgmPlay(BGMStage.TITLE);

        img.texture = title;
        yield return new WaitForSeconds(titleTime);

        //if (SoundManager.Instance != null)
        //    SoundManager.Instance.bgmPlay(BGMStage.OPENING);

        if (startCutScene != null)
        {
            img.texture = startCutScene;
            startCutScene.Play();
            startCutSceneAudio.Play();
            while (startCutScene.isPlaying && !Input.GetKeyDown(skipKey))
                yield return null;
            startCutScene.Stop();
            startCutSceneAudio.Stop();
        }

        flowCamera.gameObject.SetActive(false);

        if (GameController.Instance != null)
            GameController.Instance.GameStart();

        yield return null;
    }

    public void ActivateEnd(bool good)
    {
        StartCoroutine(EndCoroutine(good));
    }

    private IEnumerator EndCoroutine(bool good)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.bgmPlay(good ? BGMStage.END_WIN : BGMStage.END_LOSE);

        yield return new WaitForSeconds(3f);
        img.texture = good ? goodEnd : badEnd;
        flowCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(endTime);
        img.texture = credit;
        //reload scene??
    }
}
