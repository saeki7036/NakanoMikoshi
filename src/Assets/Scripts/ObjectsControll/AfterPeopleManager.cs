using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AfterPeopleManager : MonoBehaviour
{
    [SerializeField] int clearConditions = 100;
    [SerializeField] GameObject AfterPeople;
    [SerializeField] GameObject[] aPeopleParents;
    [SerializeField] GameObject aPeopleParent;
    [SerializeField] GameObject Parents;
    [SerializeField] TextMeshProUGUI PeopleNumText;

    [SerializeField] int touchFoodDecrPeople;
    [SerializeField] int scaleCorrection = 4;

    private bool isSort = false, isFever = false,isLostPeople = false;
    private int peopleCount, behindPeopleCount, behindPeopleRow,
        ColumnCount = 0;

    public int GetColumnCount => ColumnCount;
    public int GetPeopleCount => peopleCount;

    public bool GetLostFlag => isLostPeople;
    public bool IsClearFlag => peopleCount - minMenber >= clearConditions;

    const int behind0Max = 12, minMenber = 6,StartMenber = 18;

    Vector3[] behindMovePoint = new Vector3[3];
    Vector3[] behindMoveAll = new Vector3[9];
    Vector3[] behind0MovePoint = new Vector3[6];

    void Start()
    {
        PeopleNumText.text = 0.ToString() + "人神輿";
        peopleCount = minMenber;

        AfterPeople.transform.localPosition = Vector3.zero;

        //配列に代入
        for (int i = 0; i < behindMovePoint.Length; i++)
        { 
            behindMovePoint[i].x = (1.2f + 0.6f * i) * scaleCorrection; 
        }
        for (int i = 1; i < behindMoveAll.Length; i++)
        {
            if (i % 2 == 1)
            { behindMoveAll[i].x = 0.6f * (i / 2 + 1) * scaleCorrection; }
            else
            { behindMoveAll[i].x = -0.6f * (i / 2) * scaleCorrection; }
        }
        for (int i = 0; i < behind0MovePoint.Length; i++)
        {
            if (i < behind0MovePoint.Length / 2)
            { behind0MovePoint[i].x = 1.6f * scaleCorrection; }
            else
            { behind0MovePoint[i].x = 2.2f * scaleCorrection; }

            if (i % 3 == 0)
            { behind0MovePoint[i].z = 0.5f * scaleCorrection; }
            else if (i % 3 == 1)
            { behind0MovePoint[i].z = -0.1f * scaleCorrection; }
            else
            { behind0MovePoint[i].z = -0.75f * scaleCorrection; }
        }

        behindPeopleCount = 0;
        behindPeopleRow = 0;
        //人の列の親生成
        GenerateParent(0);

        //初期メンバーの生成
        for (int i = 0; i < StartMenber - minMenber; i++)
        {
            AssignMikoshiPeople(transform.position);       
        }

        isSort = false;  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSort)
        {
            Invoke(nameof(Sort), 2);
            isSort = false;
        }
    }

    /// <summary>
    /// 人数を書き込む
    /// </summary>
    void SendPeopleValue()
    {
        PeopleNumText.text = (peopleCount - 6).ToString("") + "人神輿";
    }

    /// <summary>
    /// 神輿に人を追加する
    /// </summary>
    /// <param name="pos">追加した位置</param>
    public void AssignMikoshiPeople(Vector3 pos)
    {
        peopleCount++;
        SendPeopleValue();
       
        behindPeopleCount = peopleCount - StartMenber;
        if (behindPeopleCount % 9 == 1)
        {
            //列に9人いる時、列を増やす
            behindPeopleRow++;
            GenerateParent(1);
        }

        GenerateMikoshiPeople(pos);

        if (IsClearFlag && isFever == false)
        {
            isFever = true;
            //FeverTime();
        }
    }

    //人の生成
    public void GenerateMikoshiPeople(Vector3 PeoplePos)
    {
        Vector3 pos = Vector3.zero;
        
        //神輿の周りの人
        if (peopleCount > minMenber && peopleCount <= StartMenber)
        {
            pos = GetAfterPosition(peopleCount);
        }
        //神輿の後ろの人
        else if (peopleCount > StartMenber)
        {
            behindPeopleCount = peopleCount - StartMenber;          
            float steps = (behindPeopleCount % 9) switch
            {
                1 => 0f,
                2 => 0.6f,
                3 => -0.6f,
                4 => 1.2f,
                5 => -1.2f,
                6 => 1.8f,
                7 => -1.8f,
                8 => 2.4f,
                0 => -2.4f,
                _ => pos.x // 例外
            };        
            pos.x = steps * scaleCorrection;         
        }

        //Debug.Log("aPeopleParents.length:" + aPeopleParents.Length + " behindPeopleRow:" + behindPeopleRow);

        var parent = aPeopleParents[behindPeopleRow].transform;

        AfterPeople.name = peopleCount.ToString();

        GameObject AfterPeoplePre = Instantiate(AfterPeople, parent);
        AfterPeoplePre.transform.position = PeoplePos;

        AfterPeopleMoveScript afterPeopleMoveScript = AfterPeoplePre.GetComponent<AfterPeopleMoveScript>();
        afterPeopleMoveScript.Setpoint(pos);
    }
    //人の削除
    public void EliminateMikoshiPeople()
    {
        isLostPeople = true;
        int childCount = aPeopleParents[behindPeopleRow].transform.childCount;
        Vector3 destroyObj;

        for (int i = 0; i < touchFoodDecrPeople; i++)
        {
            destroyObj = GetDestroyObjectPos(childCount);

            if (peopleCount <= minMenber)
                break;

            for (int j = 0; j < childCount; j++)
            {
                GameObject childVec = aPeopleParents[behindPeopleRow].transform.GetChild(j).gameObject;
                AfterPeopleMoveScript afterPeopleMoveScript = childVec.GetComponent<AfterPeopleMoveScript>();
                Vector3 childObj = afterPeopleMoveScript.GetPoint();

                if (childObj == destroyObj)
                {
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
                if (behindPeopleRow == 0)
                { childCount = behind0Max; }
                else
                { childCount = 9; }
            }

        }

        SendPeopleValue();
        if (peopleCount <= minMenber) 
        { GameOverDirection(); }
        isSort = true;
    }
    //削除する対象の取得
    Vector3 GetDestroyObjectPos(int childCount)
    {
        Vector3 vector3 = Vector3.zero;
        int isEven = childCount % 2 == 0 ? 1 : -1;
      
        if (peopleCount > StartMenber)
        {
            // peopleCount > 18 の場合
            vector3.x = 0.6f * (childCount / 2) * isEven * scaleCorrection;
            vector3.z = 0.0f;
        }
        else
        {
            vector3 = GetAfterPosition(peopleCount);
        }

        return vector3;
    }

    //後ろの人の位置を取得
    Vector3 GetAfterPosition(int count)
    {
        Vector3 vector3 = Vector3.zero;
        // x座標の設定
        if (count > minMenber && count <= 12)
        {
            vector3.x = (count % 2 == 0 ? -1.6f : 1.6f) * scaleCorrection;
        }
        else if (count > 12 && count <= StartMenber)
        {
            vector3.x = (count % 2 == 0 ? -2.2f : 2.2f) * scaleCorrection;
        }

        // z座標の設定
        if (count == 7 || count == 8 || count == 13 || count == 14)
        {
            vector3.z = 0.5f * scaleCorrection;
        }
        else if (count == 9 || count == 10 || count == 15 || count == 16)
        {
            vector3.z = -0.1f * scaleCorrection;
        }
        else
        {
            vector3.z = -0.75f * scaleCorrection;
        }

        return vector3;
    }

    /// <summary>
    /// 親の生成
    /// </summary>
    public void GenerateParent(int initCorre)
    {
        Vector3 parentPos = new Vector3(0.0f, -0.25f, 0.0f);
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

    /// <summary>
    /// ゲームオーバーの呼び出し
    /// </summary>
    private void GameOverDirection()
    {
        GameoverController gameoverController = GetComponent<GameoverController>();
        gameoverController.Disapper();

        //MainBGMAudio.Stop();
        //playerMode = PlayerMode.GameoverDirection;
        //TimeNum.SetActive(false);
    }
    public void CarDecr(bool isR)
    {
        isLostPeople = true;

        int decrCount = 0;
        Debug.Log("a");
        int[] rowDecrCount = new int[behindPeopleRow + 1];
        Debug.Log("b");
        DecrPeople(isR, ref decrCount, ref rowDecrCount);
        Debug.Log("c");
        MovePeople(isR, ref decrCount, ref rowDecrCount);
        Debug.Log("d");
        HitCountTextSet();
    }

    /// <summary>
    /// 全体の人数を書き込む
    /// </summary>
    public void HitCountTextSet()
    {
        int count = minMenber;
        for (int i = 0; i <= behindPeopleRow; i++)
        {
            //Debug.Log(i + ":" + aPeopleParents[i].transform.childCount);
            count += aPeopleParents[i].transform.childCount;
        }  
        
        peopleCount = count;
        SendPeopleValue();
        isSort = true;
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
                //sortRow = 0;
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
                //sortRow = row;
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
                    //sortRow =
                    cMRPeopleCount--;
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
                    //sortRow = cMRPeopleCount;
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
            if (toMoveRow == 0) { ToMoveAssign(ref toMovePoint, behind0MovePoint, 6, ref moveNumber, ref toMoveRow); }
            else { ToMoveAssign(ref toMovePoint, behindMovePoint, 3, ref moveNumber, ref toMoveRow); }

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
        if (isSort == false) return;
        isSort = false;
        
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
                    if (i / 2 >= 6)
                        Debug.Log("childCount = " + childCount);

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
                Debug.Log("A" + i + " sRow:" + sortrow); 
                DestroyParent();
            }
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
        SendPeopleValue();
    }
}
