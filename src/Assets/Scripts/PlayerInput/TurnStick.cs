using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class TurnStick : MonoBehaviour
{
    private float Input_Horizontal = 0f,Input_Vertical = 0f;
    bool stick_right = true, stick_left = false, stick_up = false,stick_down = false;
    bool button_right = false, button_left = false;
    public bool in_corner = false;
    public int turn_times = 0;
    string state;
    [SerializeField] private bool RendaInput = true;
    [SerializeField] private TurnSlider turnSlider;
    private GameStateController gameStateController;
    private AudioSource m_audioSource;
    [SerializeField] private AudioClip StickSound;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Manager = GameObject.Find("GameManager");
        GameObject Player = GameObject.Find("Player");
        m_audioSource = Player.GetComponent<AudioSource>();
        gameStateController = Manager.GetComponent<GameStateController>();
        turnSlider = GameObject.Find("UICanvas").GetComponent<TurnSlider>();   
    }

    // Update is called once per frame
    void Update()
    {
        state = gameStateController.GetState().ToString();
        if (state == nameof(FeverTimeState)
            || state == nameof(ClearState))
            turn_times = turnSlider.TurnTime_CompleteEnd;
        
        if (in_corner)
        {
            turnStick(tag == "R");        
        }
       // Debug.Log(turn_times);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("trigger");
            in_corner = true;
            if (state != nameof(MainGameState))
                turn_times = turnSlider.TurnTime_CompleteEnd;
            else
            {
                turn_times = 0;
            }
        }
    }

    void turnStick(bool RorL)
    {
        Input_Horizontal = Input.GetAxis("Submit1");
        Input_Vertical = Input.GetAxis("Submit2");

        if (RorL)
        {
            Right_corner();
            if (RendaInput)
                Right_Renda();
        }
        else
        {
            Left_corner();
            if (RendaInput)
                Left_Renda();
        }
    }

    void RightOneCount()
    {
        m_audioSource.PlayOneShot(StickSound);
        turnSlider.RightSliser.value = ++turn_times;
    }

    void LeftOneCount()
    {
        m_audioSource.PlayOneShot(StickSound);
        turnSlider.LeftSliser.value = ++turn_times;
    }

    void Right_corner()
    {
        if (Input_Horizontal > 0 && stick_right)
        {
            stick_right = false;
            stick_down = true;
        }
        else if (Input_Vertical < 0 && stick_down)
        {
            stick_down = false;
            stick_left = true;
        }
        else if (Input_Horizontal < 0 && stick_left)
        {
            stick_left = false;
            stick_up = true;
        }
        else if (Input_Vertical > 0 && stick_up)
        {
            stick_up = false;
            stick_right = true;
            RightOneCount();
        }
    }

    void Right_Renda()
    {
        if (Input_Horizontal > 0 && !button_right)
        {
            button_right = true;
        }

        if (Input_Horizontal <= 0 && button_right)
        {
            button_right = false;
            RightOneCount();
        }
    }

    void Left_corner()
    {     
        if (Input_Horizontal < 0 && stick_left)
        {
            stick_left = false;
            stick_down = true;
        }
        else if (Input_Vertical < 0 && stick_down)
        {
            stick_down = false;
            stick_right = true;
        }
        else if (Input_Horizontal > 0 && stick_right)
        {
            stick_right = false;
            stick_up = true;
        }
        else if (Input_Vertical > 0 && stick_up)
        {
            stick_up = false;
            stick_left = true;

            LeftOneCount();
        }
    }

    void Left_Renda()
    {
        if (Input_Horizontal < 0 && !button_left)
        {
            button_left = true;
        }

        if (Input_Horizontal >= 0 && button_left)
        {
            button_left = false;
            LeftOneCount();
        }
    }
}
