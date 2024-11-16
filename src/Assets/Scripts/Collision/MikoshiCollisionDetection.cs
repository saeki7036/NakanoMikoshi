using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class MikoshiCollisionDetection : MonoBehaviour
{
    [SerializeField] private AudioSource PlayerSEAudio;

    [SerializeField] private AudioClip CarTouch;
    [SerializeField] private AudioClip PeopleRecoverySound;
    [SerializeField] private AudioClip FoodHitSound;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private MikosiAnimationController mikosiAnimationController;
    [SerializeField] private AfterPeopleManager afterPeopleManager;
    [SerializeField] private GameStateController gamestateController;
    [SerializeField] private GameoverController gameoverController;
    private bool IsPlayState => gamestateController.GetState().ToString() == nameof(MainGameState);
    enum ColCarMode
    {
        None,
        Center,
        Right,
        Left
    }
    ColCarMode ColCar = ColCarMode.None;

    private void FixedUpdate()
    {     
      
    }

    //神輿との判定
    void OnTriggerEnter(Collider other)
    {
        //人との接触
        if (other.gameObject.tag == "People")
        {
            mikosiAnimationController.GetPeople();
            PlayerSEAudio.PlayOneShot(PeopleRecoverySound);
            Debug.Log("People Touch");

            //人の生成
            afterPeopleManager.AssignMikoshiPeople(other.transform.position);
            Destroy(other.gameObject);
        }
    }

   
    //食べ物との衝突
    public void FoodTouch()
    {
        Debug.Log("Food Touch");

        if (IsPlayState)
        {
            cameraController.FoodHitCamera();
            PlayerSEAudio.PlayOneShot(FoodHitSound);
            afterPeopleManager.EliminateMikoshiPeople();
        }
    }
    
    public void RightHit()
    {
        if (!IsPlayState)
            return;

        Debug.Log("右側");
        cameraController.RightHitCamer();
        PlayerSEAudio.PlayOneShot(CarTouch);

        if (ColCar != ColCarMode.None)
            return;

        ColCar = ColCarMode.Right;
        afterPeopleManager.CarDecr(true);
        ColCar = ColCarMode.None;       
    }

    public void LeftHit()
    {
        if (!IsPlayState)
            return;

        Debug.Log("左側");
        cameraController.LeftHitCamer();
        PlayerSEAudio.PlayOneShot(CarTouch);

        if (ColCar != ColCarMode.None)
            return;

        ColCar = ColCarMode.Left;
        afterPeopleManager.CarDecr(false);
        ColCar = ColCarMode.None;
    }

    public void CenterHit()
    {
        if (!IsPlayState)
            return;
        Debug.Log("正面衝突");

        ColCar = ColCarMode.Center;
        gameoverController.CarHit();
    }
}

