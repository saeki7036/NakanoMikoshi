using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClearUISetting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HyoukaText, PepoleUIText;
    [SerializeField] private Sprite Clear_Good_Sprite;
    [SerializeField] private Sprite Clear_Bad_Sprite;
    [SerializeField] private int Clear_Good_Time = 2;
    [SerializeField] UnityEngine.UI.Image ClearImage;
    [SerializeField] UnityEngine.UI.Image[] MissionImage;
    [SerializeField] AfterPeopleManager afterPeopleManager;
    [SerializeField] MainGameTimeManager mainGameTimeManager;
    public void Setting()
    {
        PepoleUIText.text = afterPeopleManager.GetPeopleCount.ToString() + "人神輿";
        int good_count = 0;
        if (Clear_Good_Time > mainGameTimeManager.GetTimeMin)
        {
            ClearImage.sprite = Clear_Good_Sprite;
        }
        else
        {
            ClearImage.sprite = Clear_Bad_Sprite;
        }

        if (afterPeopleManager.IsClearFlag)
        {
            MissionImage[0].sprite = Clear_Good_Sprite;
            good_count++;
        }
        else
        {
            MissionImage[0].sprite = Clear_Bad_Sprite;
        }

        if (Clear_Good_Time >= mainGameTimeManager.GetTimeMin)
        {
            MissionImage[1].sprite = Clear_Good_Sprite;
            good_count++;
        }
        else
        {
            MissionImage[1].sprite = Clear_Bad_Sprite;
        }

        if (afterPeopleManager.GetLostFlag == false)
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
        {
            HyoukaText.text = "名人";
        }
    }
}
