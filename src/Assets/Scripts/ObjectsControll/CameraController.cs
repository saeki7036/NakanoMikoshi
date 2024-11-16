using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject PlayerObj;
    [SerializeField] private player Player;
    [SerializeField] private MikosiAnimationController MikosiAnimator;
    [SerializeField] private Vector3 FrontPos;
    [SerializeField] private Vector3 RightPos;
    [SerializeField] private Vector3 LeftPos;
    [SerializeField] private Quaternion FrontAngle;
    [SerializeField] private Quaternion RightAngle;
    [SerializeField] private Quaternion LeftAngle;
    [SerializeField] private float speed;
    [SerializeField] private float Anglespeed;
    [SerializeField] private AfterPeopleManager afterPeopleManager;
    [SerializeField] private float adjustment;
    [SerializeField] private int ColumLimit;
    [SerializeField] private GameStateController gameStateController;
    [SerializeField] private Vector3 BeforePos;
    [SerializeField] private Quaternion BeforeAngle;
    [SerializeField] private Vector3 ClearPos;
    [SerializeField] private Quaternion ClearAngle;
    [SerializeField] private float ClearSpeed;
    [SerializeField] private float ClearAngleSpeed;
    private bool Clear = false;
    private bool FoodHit = false;
    [SerializeField] private float HitNum;
    [SerializeField] private Vector3 LeftCarHitPos;
    [SerializeField] private Quaternion LeftCarHitAngle;
    [SerializeField] private Vector3 RightCarHitPos;
    [SerializeField] private Quaternion RightCarHitAngle;
    private bool LeftHit = false;
    private bool RightHit = false;
    // Start is called before the first frame update

    void LerpTransform(Transform CameraTransform ,Vector3 Pos, Quaternion Angle)
    {
        CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, Pos, speed);
        CameraTransform.localRotation = Quaternion.Lerp(CameraTransform.localRotation, Angle, Anglespeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        string currentState = gameStateController.GetState().ToString();
        //Debug.Log(currentState == nameof(BeforePlayState));
        if (currentState == nameof(BeforePlayState) || Clear == true)
        {
            LerpTransform(MainCamera.transform, BeforePos, BeforeAngle);
        }
        else if (LeftHit)
        {
            LerpTransform(MainCamera.transform, LeftCarHitPos, LeftCarHitAngle);
        }
        else if (RightHit)
        {
            LerpTransform(MainCamera.transform, RightCarHitPos, RightCarHitAngle);
        }

        else
        {
            float Playerforward = Player.GetFowardSpeed - 1;

            if (Player.turn_complete_R || Player.turn_complete_L || MikosiAnimator.JumpCheck || FoodHit)
            {
                int Colum = ColumLimit < afterPeopleManager.GetColumnCount ? ColumLimit : afterPeopleManager.GetColumnCount;

                switch (Player.GetAngle)
                {
                    case player.playerType.Right:
                        LerpTransform(MainCamera.transform, RightPos - new Vector3(0, 0, (Colum * adjustment) + Playerforward + 20), RightAngle);
                        break;
                    case player.playerType.Left:
                        LerpTransform(MainCamera.transform, LeftPos - new Vector3(0, 0, (Colum * adjustment) + Playerforward + 20), LeftAngle);
                        break;
                    default:
                        LerpTransform(MainCamera.transform, FrontPos - new Vector3(0, 0, (Colum * adjustment) + Playerforward + 20), FrontAngle);
                        break;
                }
            }      
            else
            {
                switch (Player.GetAngle)
                {
                    case player.playerType.Right:
                        LerpTransform(MainCamera.transform, RightPos - new Vector3(0, 0, Playerforward), RightAngle);
                        break;
                    case player.playerType.Left:
                        LerpTransform(MainCamera.transform, LeftPos - new Vector3(0, 0, Playerforward), LeftAngle);
                        break;
                    default:
                        LerpTransform(MainCamera.transform, FrontPos - new Vector3(0, 0, Playerforward), FrontAngle);
                        break;
                }
            }

            if (currentState == nameof(ClearState))
                Clear = true;                          
        }
    }

    public void FoodHitCamera()
    {
        FoodHit = true;
        Invoke("FinishFoodHitCamera", HitNum);
    }
    private void FinishFoodHitCamera()
    {
        FoodHit = false;
    }
    public void LeftHitCamer()
    {
        LeftHit = true;
        Invoke("FinisLeftHitCamera", HitNum);
    }
    private void FinisLeftHitCamera()
    {
        LeftHit = false;
    }
    public void RightHitCamer()
    {
        RightHit = true;
        Invoke("FinisRightHitCamera", HitNum);
    }
    private void FinisRightHitCamera()
    {
        RightHit = false;
    }
}
