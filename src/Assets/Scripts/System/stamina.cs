using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stamina : MonoBehaviour
{
    [SerializeField] public GameStateController gamestateController;
    [SerializeField] int stamina_number_first = 3;
    public float[] stamina_value;
    public int stamina_number_now;
    float slide_sec_value;
    public float slide_value = 1.0f;
    [SerializeField]int stamina_heal_needTime = 5;
    Vector3 stamina_image_pos = new Vector3(800,-400,0);
    public int stamina_rest;
    [SerializeField] Slider stamina_slider;
    public Slider[] slider_clone;

    [SerializeField] Transform canvas;

    int time = 0;

    // Start is called before the first frame update
    void Start()
    {
        //afterPeopleManager = GetComponent<AfterPeopleManager>();
        stamina_value = new float[stamina_number_first + 1] ;
        slider_clone = new Slider[stamina_number_first] ;
        stamina_number_now = stamina_number_first;

        slide_sec_value = 1.0f / stamina_heal_needTime;

       Vector3 vectorCanvas = canvas.localPosition + stamina_image_pos * canvas.localScale.x; //�L�����p�X��Scale�ɍ��킹��B


        foreach (int stamina in stamina_value)
        {
            stamina_value[stamina] = 1;
        }

        for(int i = 0; i < stamina_number_first; i++)
        {

            slider_clone[i] =  Instantiate(stamina_slider, vectorCanvas, Quaternion.identity, canvas);

            vectorCanvas.x -= 100 * canvas.localScale.x;


            Debug.Log(stamina_image_pos);
            stamina_rest = stamina_number_first - 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string state = gamestateController.GetState().ToString();
        if (state == nameof(GameoverState)
            || state == nameof(ClearState))
  
            foreach (int stamina in stamina_value)
            {
                Destroy(GameObject.Find("stamina_slider(Clone)"));
            }

    }

    private void FixedUpdate()
    {
        if(stamina_rest < 3)
        {
            time++;

            //for (int i = 2; i >= stamina_number_now; i--) { Debug.Log(i); slider_clone[i].value = 1; }
            string state = gamestateController.GetState().ToString();
            if ((state == nameof(MainGameState) && time >= 50)||( state == nameof(FeverTimeState) && time >= 2))
            {
                slide_value += slide_sec_value;

                slider_clone[stamina_number_now - 1].value = 1 - slide_value;

                if (slide_value >= 1 && stamina_number_now != 3) { slide_value = 0; }

                if (slider_clone[stamina_number_now - 1].value <= 0)
                {
                    if (stamina_number_now < stamina_number_first)
                    {
                        stamina_number_now++;
                        stamina_rest++;
                    }
                }
                time = 0;
            }
        }
       else
            time = 0;
    }
}
