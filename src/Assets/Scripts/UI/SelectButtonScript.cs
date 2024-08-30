using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonScript : MonoBehaviour
{

    [SerializeField] protected Button[] Buttons;
    [SerializeField] protected GameObject cursol;
    [SerializeField] protected AudioSource audioSE;
    [SerializeField] private input_or input = input_or.Vertical;

    protected enum ButtonType
    {
        Normal,
        Up,
        Down
    }
    private enum input_or
    {
        Vertical,
        Horizontal
    }

    protected ButtonType buttonType = ButtonType.Normal;
    protected int currentButtonIndex = 0;

    public virtual void PushOnlyButtonEvent(int buttonPos)
    {
        if (buttonPos < 0 || buttonPos >= Buttons.Length) return;
        if (buttonPos == currentButtonIndex) return;
        currentButtonIndex = buttonPos;
        cursol.transform.localPosition = Buttons[currentButtonIndex].transform.localPosition;
        audioSE.PlayOneShot(audioSE.clip);       
    }


    // Update is called once per frame
    void Update()
    {
        float inputValue = GetInputAxis();
        int NextSelect = GetNextSelect(inputValue);
        SetNextSelect(NextSelect);
        buttonType = ResetButtonType(inputValue);
        EnterButton();
    }

    float GetInputAxis()
    {
        return input == input_or.Vertical ? Input.GetAxis("Vertical") : -Input.GetAxis("Horizontal");
    }

    int GetNextSelect(float value)
    {
        if (Math.Abs(value) < 0.7) return 0;
        if (buttonType != ButtonType.Normal) return 0;

        int NextButtonIndex = value < 0 ? 1 : -1;

        if(NextButtonIndex + currentButtonIndex >= Buttons.Length || 
           NextButtonIndex + currentButtonIndex < 0) return 0;

        return NextButtonIndex;

    }

    protected virtual void SetNextSelect(int next)
    {
        if(next == 0) return;
        currentButtonIndex += next;
        buttonType = next < 0 ? ButtonType.Up : ButtonType.Down;
        cursol.transform.localPosition = Buttons[currentButtonIndex].transform.localPosition;
        audioSE.PlayOneShot(audioSE.clip);
    }

    ButtonType ResetButtonType(float value)
    {
        if (Math.Abs(value) < 0.3)
            if (buttonType != ButtonType.Normal)
                return ButtonType.Normal;

        return buttonType;

    }

    void EnterButton()
    {
        if (Input.GetKeyDown("joystick button 0")|| Input.GetKeyDown("space"))
        {
            Buttons[currentButtonIndex].onClick.Invoke();
        }
    }
}
