using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class MikoshiCollisionDetection : MonoBehaviour
{
    public int clearConditions;
    [SerializeField] GameObject AfterPeople;
    [SerializeField] GameObject[] aPeopleParents;
    [SerializeField] GameObject aPeopleParent;
    [SerializeField] GameObject Parents;

    [SerializeField] int touchFoodDecrPeople;
    public int peopleCount;
    public bool isFever;
    [SerializeField] private float BonusTime;
    [SerializeField] private float ClearWaitTime;

    [SerializeField] int scaleCorrection;
    public int behindPeopleCount;
    public int behindPeopleRow;
    int behindMax;
    int behind0Max;
    int behindMoveCount;
    int behind0MoveCount;
    int sortRow;
    bool isSort;
    bool do_people_lose;
    int game_time_sec;
    int game_time_min;
    int good_count = 0;
    float fever_Slider_dec;
    int fever_elapsed_time;

    Vector3[] behindMovePoint;
    Vector3[] behindMoveAll;
    Vector3[] behind0MovePoint;
    Vector3 pos;
    Vector3 parentPos;

    private AudioSource m_audioSource;
    [SerializeField] private AudioSource MainBGMAudio;
    [SerializeField] private AudioSource BonusBGMAudio;
    [SerializeField] private AudioSource ClearBGMAudio;
    [SerializeField] private AudioSource GameoverBGMAudio;
    public int ColumnCount = 0;

    [SerializeField] private AudioClip PeopleRecoverySound;
    [SerializeField] private AudioClip SubSound;
    [SerializeField] private AudioClip FoodHitSound;
    [SerializeField] private AudioClip StartSound;
    [SerializeField] private AudioClip CarTouch;
    [SerializeField] private AudioClip CarTouchCrush;

    [SerializeField] UnityEngine.UI.Image ClearImage;
    [SerializeField] UnityEngine.UI.Image[] MissionImage;

    [SerializeField] private GameObject ClearResult;
    [SerializeField] private GameObject GameoverResult;
    [SerializeField] private GameObject Wait;
    [SerializeField] private GameObject BeforePlay;
    [SerializeField] private GameObject PeopleNum;
    [SerializeField] private GameObject TimeNum;
    [SerializeField] private GameObject FeverSliderObj;
    [SerializeField] private GameObject FeverSliderText;
    [SerializeField] private UnityEngine.UI.Slider FeverSlider;

    [SerializeField] private TextMeshProUGUI WaitText;
    [SerializeField] private TextMeshProUGUI TimeNumText;
    [SerializeField] public TextMeshProUGUI PeopleNumText;
    [SerializeField] private TextMeshProUGUI ResultTimeText;
    [SerializeField] public TextMeshProUGUI ResultPeoplecountText;
    [SerializeField] private TextMeshProUGUI HyoukaText;
    [SerializeField] private Sprite Clear_Good_Sprite;
    [SerializeField] private Sprite Clear_Bad_Sprite;
    [SerializeField] private int MaxWaitTime;
    [SerializeField] private int Clear_Good_Time = 2;

    private GameoverController gameoverController;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private MikosiAnimationController mikosiAnimationController;
    public enum PlayerMode
    {
        Before,
        Wait,
        Play,
        Bonus,
        Clear,
        Result,
        GameoverDirection,
        Gameover
    }

    public PlayerMode playerMode = PlayerMode.Before;

    enum ColCarMode
    {
        None,
        Center,
        Right,
        Left
    }
    ColCarMode ColCar = ColCarMode.None;


    // Start is called before the first frame update
    void Start()
    {
        PeopleNumText.text = 0.ToString() + "人神輿";
        m_audioSource = GetComponent<AudioSource>();
        peopleCount = 6;
        AfterPeople.transform.localPosition = Vector3.zero;

        isFever = false;
        FeverSliderObj.SetActive(true);
        FeverSlider.value = 0.12f;

        behindPeopleCount = 0;
        behindPeopleRow = 0;
        behindMax = 9;
        behind0Max = 12;
        behindMoveCount = behindMax - 6;
        behind0MoveCount = behind0Max / 2;
        sortRow = 0;
        isSort = false;

        fever_Slider_dec = 1 / BonusTime;

        pos = new Vector3(0.0f, -0.25f, 0.0f);
        parentPos = new Vector3(0.0f, -0.25f, 0.0f);

        behindMovePoint = new Vector3[behindMoveCount];
        for (int i = 0; i < behindMoveCount; i++) { behindMovePoint[i].x = (1.2f + 0.6f * i) * scaleCorrection; }

        behindMoveAll = new Vector3[behindMax];
        for (int i = 1; i < behindMax; i++)
        {
            if (i % 2 == 1) { behindMoveAll[i].x = 0.6f * (i / 2 + 1) * scaleCorrection; }
            else { behindMoveAll[i].x = -1 * 0.6f * (i / 2) * scaleCorrection; }
        }

        behind0MovePoint = new Vector3[behind0MoveCount];
        for (int i = 0; i < behind0MoveCount; i++)
        {
            if (i < behind0MoveCount / 2) { behind0MovePoint[i].x = 1.6f * scaleCorrection; }
            else { behind0MovePoint[i].x = 2.2f * scaleCorrection; }

            if (i == 0 || i == 3) { behind0MovePoint[i].z = 0.5f * scaleCorrection; }
            else if (i == 1 || i == 4) { behind0MovePoint[i].z = -0.1f * scaleCorrection; }
            else { behind0MovePoint[i].z = -0.75f * scaleCorrection; }
        }

        //人の列の親生成
        GenerateParent(0);

        ColCar = ColCarMode.None;

        //デバッグ用

        for (int i = 0; i < 12; i++)
        {
            peopleCount++;
            PeopleNumText.text = (peopleCount - 6).ToString() + "人神輿";
            behindPeopleCount = peopleCount - 18;
            if (behindPeopleCount % 9 == 1)
            {
                //列に9人いる時、列を増やす
                behindPeopleRow++;
                GenerateParent(1);
            }

            Debug.Log(peopleCount);

            //人の生成
            GenerateMikoshiPeople(transform.position);
        }
        gameoverController = GetComponent<GameoverController>();

    }

    private void Update()
    {
        if (Input.anyKeyDown && playerMode == PlayerMode.Before)
            WaitStart();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (playerMode == PlayerMode.Play)
                FeverTime();
        }
    }

    private void FixedUpdate()
    {
        if (isSort == true)
        {
            //関数
            Invoke("Sort", 2);
            isSort = false;
        }


        if (TimeNum.activeInHierarchy == true)
        {

            if (playerMode == PlayerMode.Play)
            {
                game_time_sec += 2;
            }

            if (game_time_sec >= 6000)
            {
                game_time_sec = 0;
                game_time_min++;

            }

            if (game_time_min > 0)
            {
                TimeNumText.text = game_time_min + "," + (game_time_sec / 100).ToString("00") + "," + (game_time_sec % 100).ToString("00");
                ResultTimeText.text = game_time_min + "," + (game_time_sec / 100).ToString("00") + "," + (game_time_sec % 100).ToString("00");
            }
            else
            {
                TimeNumText.text = (game_time_sec / 100).ToString() + "," + (game_time_sec % 100).ToString("00");
                ResultTimeText.text = (game_time_sec / 100).ToString() + "," + (game_time_sec % 100).ToString("00");
            }


        }

        if(playerMode == PlayerMode.Play)
        {
            FeverSlider.value = (peopleCount - 6) /100.0f;
        }
        else if (playerMode == PlayerMode.Bonus)
        {

            fever_elapsed_time++;

            if (fever_elapsed_time == 50)
            {
                FeverSlider.value -= fever_Slider_dec;
                fever_elapsed_time = 0;
            }
        }


    }


    //神輿との判定
    void OnTriggerEnter(Collider other)
    {
        //人との接触
        if (other.gameObject.tag == "People")
        {
            mikosiAnimationController.GetPeople();
            m_audioSource.PlayOneShot(PeopleRecoverySound);
            Debug.Log("People Touch");

            peopleCount++;
            PeopleNumText.text = (peopleCount - 6).ToString() + "人神輿";
            ResultPeoplecountText.text = (peopleCount - 6).ToString("") + "人神輿";
            behindPeopleCount = peopleCount - 18;
            if (behindPeopleCount % 9 == 1)
            {
                //列に9人いる時、列を増やす
                behindPeopleRow++;
                GenerateParent(1);
            }

            Destroy(other.gameObject);

            //Debug.Log(peopleCount);

            //人の生成
            GenerateMikoshiPeople(other.transform.position);

            if (peopleCount - 6 >= clearConditions && isFever == false)
            {
                isFever = true;
                FeverTime();
            }
        }
    }

    public void WaitStart()
    {
        PeopleNum.SetActive(true);
        BeforePlay.SetActive(false);
        playerMode = PlayerMode.Wait;
        Wait.SetActive(true);
        WaitText.text = MaxWaitTime.ToString("0");
        StartCoroutine("WaitGame");
    }
    public void FeverTime()
    {
        Debug.Log("Fever");
        playerMode = PlayerMode.Bonus;
        FeverSliderText.SetActive(true);
        FeverSlider.value = 1;
        MainBGMAudio.Stop();
        BonusBGMAudio.Play();
        StartCoroutine("GameClear");

    }

    public void GameOverDirection()
    {


        MainBGMAudio.Stop();

        playerMode = PlayerMode.GameoverDirection;

        TimeNum.SetActive(false);
    }
    public void Gameover()
    {
        Debug.Log("Game Over");
        FeverSliderObj.SetActive(false);
        GameoverBGMAudio.Play();
        GameoverResult.SetActive(true);
    }
    public void CarHitGameover()
    {
        if (playerMode == PlayerMode.Play)
        {
            m_audioSource.PlayOneShot(CarTouchCrush);
            gameoverController.CarHit();
            ColCar = ColCarMode.Center;
            GameOverDirection();
        }

    }
    public void GameStart()
    {
        MainBGMAudio.Play();
        m_audioSource.PlayOneShot(StartSound);
        playerMode = PlayerMode.Play;
        Wait.SetActive(false);
        TimeNum.SetActive(true);
    }
    private IEnumerator WaitGame()
    {
        Debug.Log("wait");
        for (int WaitTime = 0; WaitTime < MaxWaitTime; WaitTime++)
        {
            m_audioSource.PlayOneShot(SubSound);
            WaitText.text = (MaxWaitTime - WaitTime).ToString("0");
            yield return new WaitForSeconds(1);
        }
        GameStart();
    }
    private IEnumerator GameClear()
    {
        yield return new WaitForSeconds(BonusTime);
        BonusBGMAudio.Stop();
        ClearBGMAudio.Play();
        playerMode = PlayerMode.Clear;
        PeopleNum.SetActive(false);
        TimeNum.SetActive(false);
        FeverSliderObj.SetActive(false);
        if (Clear_Good_Time > game_time_min)
        {
            ClearImage.sprite = Clear_Good_Sprite;
        }
        else
        {
            ClearImage.sprite = Clear_Bad_Sprite;
        }

        if (peopleCount >= clearConditions - 6)
        {
            MissionImage[0].sprite = Clear_Good_Sprite;
            good_count++;
        }
        else
        {
            MissionImage[0].sprite = Clear_Bad_Sprite;
        }

        if (Clear_Good_Time >= game_time_min)
        {
            MissionImage[1].sprite = Clear_Good_Sprite;
            good_count++;
        }
        else
        {
            MissionImage[1].sprite = Clear_Bad_Sprite;
        }

        if (do_people_lose == false)
        {
            MissionImage[2].sprite = Clear_Good_Sprite;
            good_count++;
        }
        else
        {
            MissionImage[2].sprite = Clear_Bad_Sprite;
        }

        if (good_count <= 1)
        {
            HyoukaText.text = "見習い";
        }
        else if (good_count == 3)
        {
            HyoukaText.text = "達人";
        }
        else
        { HyoukaText.text = "名人";
        }

        StartCoroutine("Result");
    }
    private IEnumerator Result()
    {
        yield return new WaitForSeconds(ClearWaitTime);
        ClearResult.SetActive(true);
    }
    public void GenerateParent(float initCorre)
    {
        ColumnCount++;
        int childCount = Parents.transform.childCount;
        Array.Resize(ref aPeopleParents, aPeopleParents.Length + 1);
        parentPos.z = initCorre * (-0.7f - 0.6f * childCount);

        GameObject cloneParent = Instantiate(aPeopleParent, Vector3.zero, Quaternion.identity);
        cloneParent.name = "Parent" + childCount;
        cloneParent.transform.parent = Parents.transform;
        cloneParent.transform.localPosition = parentPos;
        cloneParent.transform.localRotation = Quaternion.identity;

        aPeopleParents[childCount] = cloneParent;
    }

    public void DestroyParent()
    {
        int childCount = behindPeopleRow;
        Debug.Log("parentDestroy:" + childCount);
        ColumnCount--;

        Destroy(Parents.transform.GetChild(childCount).gameObject);

        Array.Resize(ref aPeopleParents, aPeopleParents.Length - 1);

        behindPeopleRow--;

    }

    //人の生成
    public void GenerateMikoshiPeople(Vector3 PeoplePos)
    {
        pos = Vector3.zero;

        //神輿の周りの人
        if (peopleCount > 6 && peopleCount <= 18)
        {
            if (peopleCount > 6 && peopleCount <= 12)
            {
                if (peopleCount % 2 == 0) { pos.x = -1.6f * scaleCorrection; }
                else { pos.x = 1.6f * scaleCorrection; }
            }
            else if (peopleCount > 12 && peopleCount <= 18)
            {
                if (peopleCount % 2 == 0) { pos.x = -2.2f * scaleCorrection; }
                else { pos.x = 2.2f * scaleCorrection; }
            }

            if (peopleCount == 7 || peopleCount == 8 ||
                peopleCount == 13 || peopleCount == 14) { pos.z = 0.5f * scaleCorrection; }
            else if (peopleCount == 9 || peopleCount == 10 ||
                peopleCount == 15 || peopleCount == 16) { pos.z = -0.1f * scaleCorrection; }
            else { pos.z = -0.75f * scaleCorrection; }
        }
        //神輿の後ろの人
        else if (peopleCount > 18)
        {
            behindPeopleCount = peopleCount - 18;

            //behindPeopleRow = (behindPeopleCount - 1) / 9 + 1;

            switch (behindPeopleCount % 9)
            {
                case 1:
                    pos.x = 0f * scaleCorrection;
                    break;

                case 2:
                    pos.x = 0.6f * scaleCorrection;
                    break;

                case 3:
                    pos.x = -0.6f * scaleCorrection;
                    break;

                case 4:
                    pos.x = 1.2f * scaleCorrection;
                    break;

                case 5:
                    pos.x = -1.2f * scaleCorrection;
                    break;

                case 6:
                    pos.x = 1.8f * scaleCorrection;
                    break;

                case 7:
                    pos.x = -1.8f * scaleCorrection;
                    break;

                case 8:
                    pos.x = 2.4f * scaleCorrection;
                    break;

                case 0:
                    pos.x = -2.4f * scaleCorrection;
                    break;
            }
        }
        Debug.Log("aPeopleParents.length:" + aPeopleParents.Length + " behindPeopleRow:" + behindPeopleRow);
        var parent = aPeopleParents[behindPeopleRow].transform;

        AfterPeople.name = peopleCount.ToString();
        GameObject AfterPeoplePre = Instantiate(AfterPeople, parent);
        AfterPeoplePre.transform.position = PeoplePos;
        AfterPeopleMoveScript afterPeopleMoveScript = AfterPeoplePre.GetComponent<AfterPeopleMoveScript>();
        afterPeopleMoveScript.Setpoint(pos);
    }

    //食べ物との衝突
    public void FoodTouch()
    {
        Debug.Log("Food Touch");

        if (playerMode == PlayerMode.Play)
        {
            cameraController.FoodHitCamera();
            m_audioSource.PlayOneShot(FoodHitSound);
            int childCount = aPeopleParents[behindPeopleRow].transform.childCount, rl;
            Vector3 destroyObj = Vector3.zero;

            do_people_lose = true;

            for (int i = 0; i < touchFoodDecrPeople; i++)
            {
                if (childCount % 2 == 0) { rl = 1; }
                else { rl = -1; }

                if (peopleCount > 18)
                {
                    destroyObj.x = 0.6f * (childCount / 2) * rl * scaleCorrection;
                    destroyObj.z = 0.0f;
                }
                else
                {
                    if (peopleCount > 6 && peopleCount <= 12)
                    {
                        if (peopleCount % 2 == 0) { destroyObj.x = -1.6f * scaleCorrection; }
                        else { destroyObj.x = 1.6f * scaleCorrection; }
                    }
                    else if (peopleCount > 12 && peopleCount <= 18)
                    {
                        if (peopleCount % 2 == 0) { destroyObj.x = -2.2f * scaleCorrection; }
                        else { destroyObj.x = 2.2f * scaleCorrection; }
                    }

                    if (peopleCount == 7 || peopleCount == 8 ||
                        peopleCount == 13 || peopleCount == 14) { destroyObj.z = 0.5f * scaleCorrection; }
                    else if (peopleCount == 9 || peopleCount == 10 ||
                        peopleCount == 15 || peopleCount == 16) { destroyObj.z = -0.1f * scaleCorrection; }
                    else { destroyObj.z = -0.75f * scaleCorrection; }
                }

                if (peopleCount > 6)
                {
                    for (int j = 0; j < childCount; j++)
                    {
                        GameObject childVec = aPeopleParents[behindPeopleRow].transform.GetChild(j).gameObject;
                        AfterPeopleMoveScript afterPeopleMoveScript = childVec.GetComponent<AfterPeopleMoveScript>();
                        Vector3 childObj = afterPeopleMoveScript.GetPoint();

                        //Debug.Log("destroyObj:" + destroyObj);
                        //Debug.Log("childObj" + childObj);

                        if (childObj == destroyObj)
                        {
                            //  Destroy(aPeopleParents[behindPeopleRow].transform.GetChild(j).gameObject);
                            afterPeopleMoveScript.FoodDeath();
                            peopleCount--;
                            childCount--;
                            break;
                        }
                    }

                    //子が0になったら1つ前の親に
                    if (childCount == 0)
                    {
                        DestroyParent();

                        if (behindPeopleRow == 0) { childCount = behind0Max; }
                        else { childCount = behindMax; }
                    }
                }
                else
                {
                    break;
                }
            }
            PeopleNumText.text = (peopleCount - 6).ToString("") + "人神輿";
            //Debug.Log("peopleCount:" + peopleCount);
            if (peopleCount <= 6) { GameOverDirection(); gameoverController.Disapper(); }
            isSort = true;
        }

    }

    public void RightHit()
    {
        if (playerMode == PlayerMode.Play)
        {
            Debug.Log("右側");
            cameraController.RightHitCamer();
            m_audioSource.PlayOneShot(CarTouch);
            if (ColCar == ColCarMode.None)
            {
                ColCar = ColCarMode.Right;
                bool isR = true;

                int decrCount = 0;
                Debug.Log("a");
                int[] rowDecrCount = new int[behindPeopleRow + 1];
                Debug.Log("b");
                DecrPeople(isR, ref decrCount, ref rowDecrCount);
                Debug.Log("c");
                MovePeople(isR, ref decrCount, ref rowDecrCount);
                Debug.Log("d");

                int count = 0;
                for (int i = 0; i <= behindPeopleRow; i++)
                {
                    Debug.Log(i + ":" + aPeopleParents[i].transform.childCount);
                    count += aPeopleParents[i].transform.childCount;
                }
                count += 6;
                Debug.Log("count" + count);
                peopleCount = count;
                PeopleNumText.text = (peopleCount - 6).ToString("") + "人神輿";

                isSort = true;

                ColCar = ColCarMode.None;
            }
        }

    }

    public void LeftHit()
    {
        if (playerMode == PlayerMode.Play)
        {
            Debug.Log("左側");
            cameraController.LeftHitCamer();
            m_audioSource.PlayOneShot(CarTouch);

            if (ColCar == ColCarMode.None)
            {
                ColCar = ColCarMode.Left;
                bool isR = false;

                int decrCount = 0;
                Debug.Log("a");
                int[] rowDecrCount = new int[behindPeopleRow + 1];
                Debug.Log("b");
                DecrPeople(isR, ref decrCount, ref rowDecrCount);
                Debug.Log("c");
                MovePeople(isR, ref decrCount, ref rowDecrCount);
                Debug.Log("d");

                int count = 0;
                for (int i = 0; i <= behindPeopleRow; i++)
                {
                    Debug.Log(i + ":" + aPeopleParents[i].transform.childCount);
                    count += aPeopleParents[i].transform.childCount;
                }
                count += 6;
                Debug.Log("count" + count);
                peopleCount = count;
                PeopleNumText.text = (peopleCount - 6).ToString("") + "人神輿";

                isSort = true;

                ColCar = ColCarMode.None;
            }
        }

    }

    void DecrPeople(bool isR, ref int decrCount, ref int[] rowDecrCount)
    {
        Debug.Log("behind:" + behindPeopleRow);
        for (int i = behindPeopleRow; i >= 0; i--)
        {
            int childCount = aPeopleParents[i].transform.childCount;
            Debug.Log("decrChildCount" + childCount);

            for (int j = 0; j < childCount; j++)
            {
                GameObject childVec = aPeopleParents[i].transform.GetChild(j).gameObject;
                AfterPeopleMoveScript afterPeopleMoveScript = childVec.GetComponent<AfterPeopleMoveScript>();
                Vector3 childObj = afterPeopleMoveScript.GetPoint();

                if ((isR == true) && (childObj.x >= 1.2f * scaleCorrection) ||
                    (isR == false) && (childObj.x <= -1.2f * scaleCorrection))
                {
                    afterPeopleMoveScript.CarHitdeath();
                    //Destroy(aPeopleParents[i].transform.GetChild(j).gameObject);
                    peopleCount--;
                    decrCount++;
                    rowDecrCount[i]++;
                }
            }
        }
        Debug.Log("peopleCount:" + peopleCount);
        Debug.Log("decrCount:" + decrCount);
    }

    void MovePeople(bool isR, ref int decrCount, ref int[] rowDecrCount)
    {
        //減った部分に後ろから人を補充する
        int destroyChildCount = 0;
        int bPRowHold = behindPeopleRow;
        int childCount = aPeopleParents[bPRowHold].transform.childCount;

        int dCHold = decrCount, arrayCount = 0;

        GameObject[] moveObject = new GameObject[decrCount];
        //ここまで確認中
        int[] canMoveRowPeople = new int[behindPeopleRow + 1];
        int cMRPeopleCount = 0;

        for (int c = 0; c < 13; c++)
        {
            //Debug.Log("無限ループ1");
            if (decrCount == 0) { break; }
            Vector3 moveObjPoint = Vector3.zero;
            Vector3 compaObjPoint = Vector3.zero;

            bool isSkip = false, corrSkip;

            for (int i = 0; i < childCount; i++)
            {
                int pC = childCount - rowDecrCount[bPRowHold] - destroyChildCount;
                corrSkip = false;

                if (bPRowHold > 0)
                {
                    switch (pC)
                    {
                        case 1:
                            compaObjPoint.x = 0.0f;
                            corrSkip = true;
                            break;

                        case 2:
                            compaObjPoint.x = 0.6f * scaleCorrection;
                            corrSkip = true;
                            break;

                        case 3:
                            compaObjPoint.x = -0.6f * scaleCorrection;
                            corrSkip = true;
                            break;

                        case 4:
                            compaObjPoint.x = 1.2f * scaleCorrection;
                            break;

                        case 5:
                            compaObjPoint.x = 1.8f * scaleCorrection;
                            break;

                        case 6:
                            compaObjPoint.x = 2.4f * scaleCorrection;
                            break;

                        default:
                            isSkip = true;
                            break;
                    }
                }
                else if (bPRowHold == 0)
                {
                    switch (pC)
                    {
                        case 1:
                            compaObjPoint = behind0MovePoint[0];
                            break;

                        case 2:
                            compaObjPoint = behind0MovePoint[1];
                            break;

                        case 3:
                            compaObjPoint = behind0MovePoint[2];
                            break;

                        case 4:
                            compaObjPoint = behind0MovePoint[3];
                            break;

                        case 5:
                            compaObjPoint = behind0MovePoint[4];
                            break;

                        case 6:
                            compaObjPoint = behind0MovePoint[5];
                            break;

                        default:
                            isSkip = true;
                            break;
                    }
                }

                if (isSkip == true) { break; }

                if ((isR == true) && (corrSkip == false)) { compaObjPoint.x *= -1; }

                //Transform childTransform = aPeopleParents[bPRowHold].transform.GetChild(i);
                //moveObjPoint = childTransform.localPosition;
                GameObject childVec = aPeopleParents[bPRowHold].transform.GetChild(i).gameObject;
                AfterPeopleMoveScript afterPeopleMoveScript = childVec.GetComponent<AfterPeopleMoveScript>();
                moveObjPoint = afterPeopleMoveScript.GetPoint();


                //判定
                if (compaObjPoint == moveObjPoint)
                {
                    Debug.Log("移動できる人:" + childVec.name);
                    moveObject[arrayCount] = aPeopleParents[bPRowHold].transform.GetChild(i).gameObject;

                    arrayCount++;
                    decrCount--;
                    destroyChildCount++;
                    break;
                }
            }

            if (isSkip == true || decrCount == 0)
            {
                //Debug.Log("cMRPC:" + cMRPeopleCount + " childC:" + childCount + " bPRH:" + bPRowHold + " rDC:" + rowDecrCount[bPRowHold]);
                Debug.Log("childCount:" + childCount + " /その列で居なくなった人:" + rowDecrCount[bPRowHold]);
                canMoveRowPeople[cMRPeopleCount] = childCount - rowDecrCount[bPRowHold];//その列で移動できる人
                //Debug.Log("canMovePeople:" + canMoveRowPeople[cMRPeopleCount]);

                cMRPeopleCount++;
                destroyChildCount = 0;
                bPRowHold--;

                if (bPRowHold < 0) { break; }
                childCount = aPeopleParents[bPRowHold].transform.childCount;
            }
        }

        int moveNumber = 0, toMoveRow = 0, behindRowHold = behindPeopleRow;
        int a = 0, row = 0;
        cMRPeopleCount = 0;
        arrayCount = 0;
        decrCount = dCHold;
        //for (int i = 0; i < canMoveRowPeople.Length; i++) { Debug.Log("canMoveRowPeople[" + i + "]:" + canMoveRowPeople[i]); }
        for (int b = 0; b < 13; b++)
        {
            //Debug.Log("無限ループ2");
            if (decrCount == 0)
            {
                sortRow = 0;
                isSort = true;
                break;
            }

            Debug.Log("Row:" + cMRPeopleCount + " canMovePeople:" + canMoveRowPeople[cMRPeopleCount] + "behindRow:" + behindPeopleRow);
            Debug.Log(" row:" + row + " Count:" + rowDecrCount[row]);
            int z = behindRowHold - cMRPeopleCount;
            Debug.Log("チェック:" + z + " <= " + row);
            if (behindRowHold - cMRPeopleCount <= row)
            {
                //移動できる人の列と補充しないといけない列が同じなら、ループを終わらせてその列だけ並び替えをさせる
                Debug.Log("ソートON");
                sortRow = row;
                isSort = true;
                break;
            }

            a = canMoveRowPeople[cMRPeopleCount] - rowDecrCount[row];//各列で移動できる人-各列で補充しないといけない人数
            if (a >= 0)
            {
                //rowDecrCount[i]
                ToMove(ref moveObject, isR, ref arrayCount, rowDecrCount[row], ref moveNumber, ref toMoveRow);
                decrCount -= rowDecrCount[row];

                row++;
                if (a == 0)
                {
                    cMRPeopleCount++;
                }
                else
                {
                    canMoveRowPeople[cMRPeopleCount] = a;
                }
                if (aPeopleParents[cMRPeopleCount].transform.childCount <= 0)
                {
                    Debug.Log("B");
                    //DestroyParent();
                    Debug.Log("ソートON");
                    sortRow = cMRPeopleCount--;
                    isSort = true;
                    break;
                }
            }
            else
            {
                //canMoveRowPeople[cMRPCount]

                ToMove(ref moveObject, isR, ref arrayCount, canMoveRowPeople[cMRPeopleCount], ref moveNumber, ref toMoveRow);
                decrCount -= canMoveRowPeople[cMRPeopleCount];
                if (aPeopleParents[cMRPeopleCount].transform.childCount <= 0)
                {
                    Debug.Log("C");
                    //DestroyParent();
                    Debug.Log("ソートON");
                    sortRow = cMRPeopleCount;
                    isSort = true;
                    break;
                }
                cMRPeopleCount++;
            }
        }
    }

    void ToMove(ref GameObject[] moveObject, bool isR, ref int arrayCount, int numOfRoop, ref int moveNumber, ref int toMoveRow)
    {
        Vector3 toMovePoint = Vector3.zero;

        for (int i = 0; i < numOfRoop; i++)
        {
            AfterPeopleMoveScript afterPeopleMoveScript = moveObject[arrayCount].GetComponent<AfterPeopleMoveScript>();
            moveObject[arrayCount].transform.SetParent(aPeopleParents[toMoveRow].transform);
            //Debug.Log("moveObject:" + moveObject[arrayCount]);

            //移動先の座標を求める
            if (toMoveRow == 0) { ToMoveAssign(ref toMovePoint, behind0MovePoint, behind0MoveCount, ref moveNumber, ref toMoveRow); }
            else { ToMoveAssign(ref toMovePoint, behindMovePoint, behindMoveCount, ref moveNumber, ref toMoveRow); }

            if (isR == false) { toMovePoint.x *= -1; }

            arrayCount++;

            afterPeopleMoveScript.Setpoint(toMovePoint);
        }
    }


    void ToMoveAssign(ref Vector3 toMovePoint, Vector3[] movePoint, int bMoveCount, ref int moveNumber, ref int toMoveRow)
    {
        toMovePoint = movePoint[moveNumber];

        moveNumber++;

        if (moveNumber == bMoveCount)
        {
            moveNumber = 0;
            toMoveRow++;
        }
    }

    void Sort()
    {
        int sortrow = 0;
        while (true)
        {
            Debug.Log("Sort");
            Vector3 movePoint = Vector3.zero;
            int childCount = aPeopleParents[sortrow].transform.childCount;
            Debug.Log("sRow:" + sortrow + " childCount:" + childCount);
            for (int i = 0; i < childCount; i++)
            {
                GameObject child = aPeopleParents[sortrow].transform.GetChild(i).gameObject;
                AfterPeopleMoveScript afterPeopleMoveScript = child.GetComponent<AfterPeopleMoveScript>();
                if (sortrow == 0)
                {
                    movePoint = behind0MovePoint[i / 2];
                    if (i % 2 == 1) { movePoint.x *= -1; }
                }
                else { movePoint = behindMoveAll[i]; }

                afterPeopleMoveScript.Setpoint(movePoint);
            }
            sortrow++;
            if (sortrow > behindPeopleRow || aPeopleParents[sortrow].transform.childCount <= 0) { sortrow--; break; }
        }

        Debug.Log("Sort終了: 列数:" + behindPeopleRow + " sortRow:" + sortrow);
        //Sortより後ろの列があれば消す
        for (int i = behindPeopleRow; i > sortrow; i--)
        {
            if (aPeopleParents[i].transform.childCount <= 0)
            {
                Debug.Log("A" + i + " sRow:" + sortrow); DestroyParent(); }
        }

        int count = 0;
        for (int i = 0; i <= behindPeopleRow; i++)
        {
            Debug.Log(i + ":" + aPeopleParents[i].transform.childCount);
            count += aPeopleParents[i].transform.childCount;
        }
        count += 6;
        Debug.Log("count" + count);
        peopleCount = count;
        PeopleNumText.text = (peopleCount - 6).ToString("") + "人神輿";
    }
}

