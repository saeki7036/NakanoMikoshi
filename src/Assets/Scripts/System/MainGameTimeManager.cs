using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameTimeManager : MonoBehaviour
{
    [SerializeField] private float BonusTime = 15, ClearWaitTime = 10;
    [SerializeField] private const int MaxCountDownTime = 3;
    [SerializeField] private TextMeshProUGUI WaitUIText,TimeUIText,ResultUIText;
    [SerializeField] private Slider FeverSlider;
    [SerializeField] private GameStateController gameStateController;
    [SerializeField] private AfterPeopleManager afterPeopleManager;
    [SerializeField] private AudioSource SEAudio,MainBGMAudio, FeverBGMAudio,ResultBGMAudio;
    [SerializeField] private AudioClip CountDownClip,StartVoiceClip;

    public void StartCountDown() { StartCoroutine(nameof(WaitGame)); }
    public void StartResultStay() { StartCoroutine(nameof(Result)); }
    private float feverSliderDec;
    private int mainGameSec,mainGameMin, feverElapsedTime;
    private bool endCountDown, clearResult;

    public int GetTimeMin => mainGameMin;
    public bool GetEndCountDown => endCountDown;
    public bool GetClearResult => clearResult ;
    public bool EndFeverTime => FeverSlider.value <= 0 ;

    private string TimeText;
    private IEnumerator WaitGame()
    {
        for (int WaitTime = 0; WaitTime < MaxCountDownTime; WaitTime++)
        {
            SEAudio.PlayOneShot(CountDownClip);
            WaitUIText.text = (MaxCountDownTime - WaitTime).ToString("0");
            yield return new WaitForSeconds(1);
        }
        MainBGMAudio.Play();
        SEAudio.PlayOneShot(StartVoiceClip);
        endCountDown = true;
    }

    private IEnumerator Result()
    {
        FeverBGMAudio.Stop();
        ResultBGMAudio.Play();
        yield return new WaitForSeconds(ClearWaitTime);
        clearResult = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        mainGameSec = 0;
        mainGameMin = 0;
        TimeText = "0";
        feverElapsedTime = 0;
        feverSliderDec = 1 / BonusTime;
        endCountDown = false;
        clearResult = false;
        WaitUIText.text = "0";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeUpdateMainGame();
        SetFeverSlider();
    }

    void TimeUpdateMainGame()
    {
        if (gameStateController.GetState().ToString() != nameof(MainGameState))
            return;
        mainGameSec += 2;


        if (mainGameSec >= 6000)
        {
            mainGameSec = 0;
            mainGameMin++;

        }
        TimeText = "";

        if (mainGameMin > 0)
        {
            TimeText = mainGameMin + ",";                     
        }

        TimeText += (mainGameSec / 100).ToString("00") + "," + (mainGameSec % 100).ToString("00");
        TimeUIText.text = TimeText;
        ResultUIText.text = TimeText;
    }

    void SetFeverSlider()
    {
        string stateName = gameStateController.GetState().ToString();
        if (stateName == nameof(MainGameState))
        {
            FeverSlider.value = (afterPeopleManager.GetPeopleCount - 6) / 100.0f;
        }
        else if (stateName == nameof(FeverTimeState))
        {
            if (MainBGMAudio.isPlaying)
            {
                MainBGMAudio.Stop();
                FeverBGMAudio.Play();
            }
            feverElapsedTime++;
        }
        if (feverElapsedTime == 50)
        {
            FeverSlider.value -= feverSliderDec;
            feverElapsedTime = 0;
        }
    }
}
