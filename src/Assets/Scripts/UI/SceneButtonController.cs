using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSE;
    
    public void SceneChangeTitle()
    {
        audioSE.PlayOneShot(audioSE.clip);
        SceneManager.LoadSceneAsync("NewTitle");
    }

    public void SceneChangeMainGame()
    {
        audioSE.PlayOneShot(audioSE.clip);
        SceneManager.LoadSceneAsync("MainGame");
    }
    public void SceneChangeMainGameWithNoSE()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void SceneRerode()
    {
        audioSE.PlayOneShot(audioSE.clip);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }


    public void FinishGame()
    {
        audioSE.PlayOneShot(audioSE.clip);
        Application.Quit();
    }

}
