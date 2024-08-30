using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutScript : MonoBehaviour
{
    bool toStage = false;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float displayImageDuration = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AudioSource audioSE;

    float m_timer;

    void Update()
    {
        if (toStage)
        {
            FadeOut();
        }
    }

    public void ClickStartButton()
    {
        toStage = true;
        audioSE.PlayOneShot(audioSE.clip);
    }

    void FadeOut()
    {
        m_timer += Time.deltaTime;

        canvasGroup.alpha = m_timer / fadeDuration;

        if (m_timer > fadeDuration + displayImageDuration)
        {
            SceneButtonController sceneButtonController = GetComponent<SceneButtonController>();
            sceneButtonController.SceneChangeMainGameWithNoSE();
        }
    }
}
