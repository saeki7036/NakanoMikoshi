using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{
   private AfterPeopleManager afterPeopleManager;
    [SerializeField] private Animator[] HumanAnimator;
    [SerializeField] private Animator MikoshiAnimator;
    private int ReColum = 0;

    void Start()
    {
        afterPeopleManager = GetComponent<AfterPeopleManager>();
    }
    // Update is called once per frame
    void Update()
    {
        int Colum = afterPeopleManager.GetColumnCount;
        if (Colum == ReColum) return;

        foreach (Animator Human in HumanAnimator)
        {
            Human.SetFloat("speed", 2 + (Colum * 0.2f + 1));
        }
        MikoshiAnimator.SetFloat("speed", 1 + (Colum * 0.2f + 1));

        ReColum = Colum;
    }
}
