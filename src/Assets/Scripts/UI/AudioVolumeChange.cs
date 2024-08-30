using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class AudioVolumeChange : SelectButtonScript
{
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private float volume_Max = 10;
    [SerializeField] static public float audioVolume;
  
    private void Start()
    {
        audioVolume = volume_Max;
        SetAudioVolume();
        currentButtonIndex = Buttons.Length -1;
    }





    public void SetAudioVolume()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.volume = audioVolume / volume_Max;
        }
    }
    public override void PushOnlyButtonEvent(int buttonPos)
    {
        if (buttonPos < 0 || buttonPos >= Buttons.Length) return;
        if (buttonPos == currentButtonIndex) return;
        currentButtonIndex = buttonPos;
        cursol.transform.localPosition = Buttons[currentButtonIndex].transform.localPosition;

        if (Buttons.Length - 1 > 0)
            audioVolume = currentButtonIndex * (volume_Max / (float)(Buttons.Length - 1));

        SetAudioVolume();
        audioSE.PlayOneShot(audioSE.clip);
    }
    protected override void SetNextSelect(int next)
    {
        if (next == 0) return;
        currentButtonIndex += next;
        buttonType = next < 0 ? ButtonType.Up : ButtonType.Down;
        cursol.transform.localPosition = Buttons[currentButtonIndex].transform.localPosition;

        if (Buttons.Length - 1 > 0)
            audioVolume = currentButtonIndex * (volume_Max / (float)(Buttons.Length - 1));
       

        SetAudioVolume();
        audioSE.PlayOneShot(audioSE.clip);
    }
}
