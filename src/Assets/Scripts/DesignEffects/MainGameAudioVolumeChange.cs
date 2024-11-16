using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameAudioVolumeChange : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources;
    // Start is called before the first frame update
    private void Start()
    {
        //Debug.Log(AudioVolumeChange.IsInitialized);
        if (AudioVolumeChange.IsInitialized == false) return;

        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.volume = AudioVolumeChange.audioVolume;
        }
    }
}
