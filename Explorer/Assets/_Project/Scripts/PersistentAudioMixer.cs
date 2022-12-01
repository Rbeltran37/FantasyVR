using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PersistentAudioMixer : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource startSceneSource;
    public AudioSource startMenuSource;

    /*public Dictionary<String, List<AudioClip>> sceneSongs =
        new Dictionary<String, List<AudioClip>>();*/


    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneUnloaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneUnloaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        startMenuSource.Play();
        Fade();
    }

    [Button]
    public void Fade()
    {
        //StartCoroutine(StartFade(audioMixer, "StartScene", 5f, 0f));
        StartCoroutine(StartFade(audioMixer, "StartMenu", 5f, 80f));
    }
    
    
    private static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }
}
