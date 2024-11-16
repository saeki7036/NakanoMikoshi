using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGame: MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Finish"))
        {
            SceneButtonController sceneButtonController = GetComponent<SceneButtonController>();
            sceneButtonController.FinishGame();          
        }
    }
}
