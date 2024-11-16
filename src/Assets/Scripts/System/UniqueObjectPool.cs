using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UniqueObjectPool : MonoBehaviour
{
    [Header(""),Space]
    [SerializeField] public GameObject PeopleNumUIObj;
    [SerializeField] public TextMeshProUGUI PeopleNumText;
    [SerializeField] public GameObject BeforePlayUIObj;
    [SerializeField] public GameObject WaitUIObj;
    [SerializeField] public MainGameTimeManager timeManager;
    [SerializeField] public AfterPeopleManager afterPeopleManager;
    [SerializeField] public GameoverController gameoverController;
    [SerializeField] public GameObject TimeNumUIObj;
    [SerializeField] public TextMeshProUGUI TimeNumText;
    [SerializeField] public GameObject ClearResultUI;
    [SerializeField] public GameObject GameoverResultUI;
    [SerializeField] public ClearUISetting setting;

    [SerializeField] public UnityEngine.UI.Slider FeverSlider;
    [SerializeField] public GameObject FeverSliderUIObj;
    [SerializeField] public GameObject FeverSliderTextUI;
}
