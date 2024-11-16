using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    
    private Vector3 force = Vector3.zero;
    private Transform my_Transform;

    private float Input_Horizontal,old_Horizontal;
    private float Input_Vertical,old_Vertical;
    private float Input_Jump,old_Jump;
    private float turn_times = 0f;
    private float Input_Jump_once;

    private float forward_or_back_speed;

    public float GetFowardSpeed => forward_or_back_speed;

    [SerializeField] private float my_Thrust = 4000f;
    [SerializeField] private float my_Thrust_Max = 70f;
    [SerializeField] private float my_forward_speed = 1.5f;
    [SerializeField] private float jumpVector = 20f;
    [SerializeField] private float gravity = 1000f;
    [SerializeField] private float PlusSpeed = 0.1f;
    [SerializeField] private float BonusMagnification = 1.2f;
    [SerializeField] private float slide_power = 2f;

    [SerializeField] private float BendSpeed = 60f;
    [SerializeField] private float ReturnSpeed = 30f;
    [SerializeField] int Turn_speed = 1;

    [SerializeField] private Rigidbody my_Rigidbody;
    [SerializeField] private GameObject WholeObject;
    [SerializeField] private Animator[] HumansAnimation;

    [SerializeField] private AudioSource PlayerSEAudio;
    [SerializeField] private AudioClip[] JumpSounds;
    [SerializeField] private AudioClip TurnSound;

    [SerializeField] private SEController seController;
    [SerializeField] private stamina stamina_script;
    [SerializeField] private TurnSlider turnSlider;
    [SerializeField] private GameStateController gameStateController;
    [SerializeField] private AfterPeopleManager afterPeopleManager;
    [SerializeField] private MikosiAnimationController MikosiAnimation;

    public bool turn_complete_R = false;
    public bool turn_complete_L = false;

    private bool turnSoundCheck = false;
    public enum playerType
    {
        Up,
        Right,
        Down,
        Left,
    }
    private playerType Angle;

    public void SetType(playerType type){ Angle = type; }
    public playerType GetAngle => Angle;

    public enum MoveControll
    {
        forwardmove,
        rightmove,    
        backmove,
        leftmove,
        dontmove,
        nullmove,
    }

    public MoveControll Horizontal_move, Vertical_move;

    void Start()
    {
        my_Transform = GetComponent<Transform>();
        stamina_script = GetComponent<stamina>();
    }

    void Update()
    {   
        Input_Horizontal = Input.GetAxis("Horizontal");
        Input_Vertical = Input.GetAxis("Vertical");
        Input_Jump = Input.GetAxis("Jump");

        Input_Jump_once = old_Jump - Input_Jump;

        Horizontal_move = Horizontal_Controll(old_Horizontal, Input_Horizontal);
        Vertical_move = Vertical_Controll(old_Vertical, Input_Vertical);

        if(Vertical_move == MoveControll.forwardmove)
        { forward_or_back_speed = 1.25f;}
        else if(Vertical_move == MoveControll.backmove)
        {forward_or_back_speed = 0.8f;}
        else 
        { forward_or_back_speed = 1.0f; }

        //Debug.Log(forward_or_back_speed);

        if(!turn_complete_R&&!turn_complete_L)
        {
            switch(GetAngle)
            {
                case playerType.Up:
                    {
                        if (Horizontal_move == MoveControll.leftmove)
                        {
                            if (my_Rigidbody.velocity.x > -my_Thrust_Max)
                            {
                                force = new Vector3(-my_Thrust, 0, 0);
                            }
                            else
                            {
                                force = Vector3.zero;
                            }                         
                        }
                        else if (Horizontal_move == MoveControll.rightmove)
                        {
                            if (my_Rigidbody.velocity.x < my_Thrust_Max)
                            {
                                force = new Vector3(my_Thrust, 0, 0);
                            }
                            else
                            {
                                force = Vector3.zero;
                            }                           
                        }
                        else
                        {
                            if (my_Rigidbody.velocity.x < 3 && my_Rigidbody.velocity.x > -3)
                            {
                                Vector3 now_velocity = my_Rigidbody.velocity;
                                now_velocity.x = 0f;                         
                                my_Rigidbody.velocity = now_velocity;
                            }
                            else
                            {
                                force = new Vector3(my_Rigidbody.velocity.x * -slide_power, 0, 0);
                            }                         
                        }
                        break;
                    }

                case playerType.Down:
                    {
                        if (Horizontal_move == MoveControll.leftmove)
                        {
                            if (my_Rigidbody.velocity.x < my_Thrust_Max)
                            {
                                force = new Vector3(my_Thrust, 0, 0);
                            }
                            else
                            {
                                force = new Vector3(0, 0, 0);
                            }
                            
                        }
                        else if (Horizontal_move == MoveControll.rightmove)
                        {
                            if (my_Rigidbody.velocity.x > -my_Thrust_Max)
                            {
                                force = new Vector3(-my_Thrust, 0, 0);
                            }
                            else
                            {
                                force = new Vector3(0, 0, 0);
                            }                         
                        }
                        else
                        {
                            if (my_Rigidbody.velocity.x < 3 && my_Rigidbody.velocity.x > -3)
                            {
                                Vector3 now_velocity = my_Rigidbody.velocity;
                                now_velocity.x = 0f;
                                my_Rigidbody.velocity = now_velocity;
                            }
                            else
                            {
                                force = new Vector3(my_Rigidbody.velocity.x * -slide_power, 0, 0);
                            }                          
                        }
                        break;
                    }

                case playerType.Left:
                    {

                        if (Horizontal_move == MoveControll.leftmove)
                        {

                            if (my_Rigidbody.velocity.z > -my_Thrust_Max)
                            {
                                force = new Vector3(0, 0, -my_Thrust);
                            }
                            else
                            {
                                force = new Vector3(0, 0, 0);

                            }
                           
                        }
                        else if (Horizontal_move == MoveControll.rightmove)
                        {


                            if (my_Rigidbody.velocity.z < my_Thrust_Max)
                            {
                                force = new Vector3(0, 0, my_Thrust);
                            }
                            else
                            {
                                force = new Vector3(0, 0, 0);
                            }
                          
                        }
                        else
                        {
                            if (my_Rigidbody.velocity.z < 3 && my_Rigidbody.velocity.z > -3)
                            {
                                float now_velocity_y = my_Rigidbody.velocity.y;
                                float now_velocity_x = my_Rigidbody.velocity.x;

                                my_Rigidbody.velocity = new Vector3(now_velocity_x, now_velocity_y, 0);
                            }
                            else
                            {
                                force = new Vector3(0, 0, my_Rigidbody.velocity.z * -slide_power);
                            }
                           
                        }

                        break;
                    }

                case playerType.Right:
                    {

                        if (Horizontal_move == MoveControll.leftmove)
                        {
                            if (my_Rigidbody.velocity.z < my_Thrust_Max)
                            {
                                force = new Vector3(0, 0, my_Thrust);
                            }
                            else
                            {
                                force = Vector3.zero;
                            }
                        }
                        else if (Horizontal_move == MoveControll.rightmove)
                        {
                            if (my_Rigidbody.velocity.z > -my_Thrust_Max)
                            {
                                force = new Vector3(0, 0, -my_Thrust);
                            }
                            else
                            {
                                force = Vector3.zero;
                            }
                        }
                        else
                        {
                            if (my_Rigidbody.velocity.z < 3 && my_Rigidbody.velocity.z > -3)
                            {
                                Vector3 now_velocity = my_Rigidbody.velocity;
                                now_velocity.z = 0; 
                                my_Rigidbody.velocity = now_velocity;
                            }
                            else
                            {
                                force = new Vector3(0, 0, my_Rigidbody.velocity.z * -slide_power);
                            }
                        }
                        break;
                    }
            }                        
        }

        if(WholeObject!=null)
        {
            if (Horizontal_move == MoveControll.leftmove)
                WholeObject.transform.localRotation = Quaternion.RotateTowards(WholeObject.transform.localRotation, Quaternion.Euler(0, -25, 0), BendSpeed * Time.deltaTime);
            else if (Horizontal_move == MoveControll.rightmove)
                WholeObject.transform.localRotation = Quaternion.RotateTowards(WholeObject.transform.localRotation, Quaternion.Euler(0, 25, 0), BendSpeed * Time.deltaTime);
            else
                WholeObject.transform.localRotation = Quaternion.RotateTowards(WholeObject.transform.localRotation, Quaternion.Euler(0, 0, 0), ReturnSpeed * Time.deltaTime);
        }     

        force.y = -gravity;
        string state = gameStateController.GetState().ToString();
        if (state == nameof(MainGameState) || state == nameof(FeverTimeState))
        {
            if (Input_Jump_once == -1 && transform.position.y <= 2 && stamina_script.stamina_rest > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    PlayerSEAudio.PlayOneShot(JumpSounds[i]);
                }

                float now_velocity_x = my_Rigidbody.velocity.x;
                float now_velocity_z = my_Rigidbody.velocity.z;

                my_Rigidbody.velocity = new Vector3(now_velocity_x, jumpVector, now_velocity_z);

                stamina_script.slider_clone[stamina_script.stamina_number_now - 1].value = 1 - stamina_script.slide_value;
                stamina_script.stamina_rest--;
               
                if (stamina_script.stamina_number_now != 1) 
                { stamina_script.stamina_number_now--; }
            }
           
            force *= Time.deltaTime;
            my_Rigidbody.AddForce(force, ForceMode.Acceleration);

            OldInput();
        }
    }

    void OldInput()
    {
        old_Horizontal = Input_Horizontal;
        old_Vertical = Input_Vertical;
        old_Jump = Input_Jump;
    }

    private void FixedUpdate()
    {
        string state = gameStateController.GetState().ToString();
        if (turn_complete_R)
        {
            if(!turnSoundCheck)
            {
                MikosiAnimation.RightTurn();
                seController.StopSound();
                turnSoundCheck = true;
                PlayerSEAudio.PlayOneShot(TurnSound);
                foreach (Animator Human in HumansAnimation)
                {
                    Human.SetBool("RightTurn", true);
                }
            }
            turn_times += Turn_speed;

            transform.rotation = Quaternion.Euler(0, turn_times, 0);

            if (turn_times % 90 == 0)
            {
                MikosiAnimation.FinishRightTurn();
                turn_complete_R = false;
                turnSoundCheck = false;
                turnSlider.RightTurnEnd();
                foreach (Animator Human in HumansAnimation)
                {
                    Human.SetBool("RightTurn", false);
                }
            }

           
            if (state == nameof(FeverTimeState))
                my_Transform.position += transform.forward * (my_forward_speed  * 0.5f);
            else if (state == nameof(MainGameState))
                my_Transform.position += transform.forward * (my_forward_speed * 0.5f);
        }
        else if (turn_complete_L)
        {
            if (!turnSoundCheck)
            {
                MikosiAnimation.LeftTurn();
                seController.StopSound();
                turnSoundCheck = true;
                PlayerSEAudio.PlayOneShot(TurnSound);
                foreach (Animator Human in HumansAnimation)
                {
                    Human.SetBool("LeftTurn", true);
                    Debug.Log("true");
                }
            }
            turn_times -= Turn_speed;

            transform.rotation = Quaternion.Euler(0, turn_times, 0);

            if (turn_times % 90 == 0)
            {
                MikosiAnimation.FinishLeftTurn();
                turn_complete_L = false;
                turnSoundCheck = false;
                turnSlider.LeftTurnEnd();
                foreach (Animator Human in HumansAnimation)
                {
                    Human.SetBool("LeftTurn", false);
                }
            }
            if (state == nameof(FeverTimeState))
                my_Transform.position += transform.forward * (my_forward_speed * 0.4f);
            else if (state == nameof(MainGameState))
                my_Transform.position += transform.forward * (my_forward_speed * 0.5f);
        }
        else if (state == nameof(FeverTimeState))
            my_Transform.position += transform.forward * (my_forward_speed * forward_or_back_speed *(afterPeopleManager.GetColumnCount * PlusSpeed + 1.5f)*BonusMagnification);
        else if (state == nameof(MainGameState))
            my_Transform.position += transform.forward * (my_forward_speed * forward_or_back_speed * afterPeopleManager.GetColumnCount * PlusSpeed + 1.5f);
    }

    MoveControll Horizontal_Controll(float old, float input)
    {
        MoveControll Controll = MoveControll.nullmove;

        if (old > input)
        {
            Controll = input > 0 ? MoveControll.dontmove : MoveControll.leftmove;
        }
        else if (old < input)
        {
            Controll = input < 0 ? MoveControll.dontmove : MoveControll.rightmove;
        }
        else if (old == input)
        {
            if (input == 0)
                Controll = MoveControll.dontmove;
            else
                Controll = input < 0 ? MoveControll.leftmove : MoveControll.rightmove;
        }

        return Controll;
    }

    MoveControll Vertical_Controll(float old, float input)
    {
        MoveControll Controll = MoveControll.nullmove;

        if (old > input)
        {
            Controll = input > 0 ? MoveControll.dontmove : MoveControll.backmove;      
        }
        else if (old < input)
        {
            Controll = input < 0 ? MoveControll.dontmove : MoveControll.forwardmove;          
        }
        else if (old == input)
        {
            if (input == 0) 
                Controll = MoveControll.dontmove; 
            else          
                Controll = input < 0 ? MoveControll.backmove : MoveControll.forwardmove;           
        }

        return Controll;
    }
}
