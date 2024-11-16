using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverController : MonoBehaviour
{
   
    [SerializeField] private MikoshiCollisionDetection mikosiCollision;
    [SerializeField] private FirstHumanRagdollController[] ragdoll;
    [SerializeField] private MikosiRagdollController mikosiRagdoll;

    [SerializeField] private GameObject ResultUI;

    [SerializeField] private AudioSource MainBGMAudio;
    [SerializeField] private AudioSource GameoverBGMAudio;
    [SerializeField] private AudioSource mikosiAudioSource;
    [SerializeField] private AudioClip LittleFailureSound, BigFailureSound;
    
    private bool IsGameover;
    public bool IsGameoverFlag => IsGameover;
    public bool FailureTurnCheck = false;
    private void Start()
    {
        IsGameover = false;
    }
    public void FailureTurn()
    {
        FailureTurnCheck = true;
    }
    /// <summary>
    /// �l��������Ȃ����ɋN��
    /// </summary>
    public void Disapper()
    {
        IsGameover = true;
        foreach (FirstHumanRagdollController r in ragdoll) 
        {
            r.SetRagDoll();
        }
        mikosiRagdoll.SetPhysics();
        mikosiAudioSource.PlayOneShot(LittleFailureSound);
    }
    /// <summary>
    /// ��]�o���Ȃ����ɋN��
    /// </summary>
    public void TurnGameover()
    {
        IsGameover = true;

        mikosiRagdoll.SetRagDoll();
        mikosiAudioSource.PlayOneShot(BigFailureSound);      
    }
    /// <summary>
    /// �Ԃɂ����������ɋN��
    /// </summary>
    public void CarHit()
    {
        IsGameover = true;
        foreach (FirstHumanRagdollController r in ragdoll)
        {
            r.SetRagDoll();
        }
        mikosiRagdoll.SetRagDoll();
        mikosiAudioSource.PlayOneShot(BigFailureSound);
    }

    public void Gameover()
    {
        MainBGMAudio.Stop();
        IsGameover = true;
        StartCoroutine(nameof(GameEnd));
    }

    private IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(3.5f);
        ResultUI.SetActive(true);
        GameoverBGMAudio.Play();
    }
}
        
