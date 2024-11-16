using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class MainGameState : GameState
{
    StateManager stateManager;
    UniqueObjectPool objectPool;
    public MainGameState(StateManager stateManager, UniqueObjectPool objectPool)
    {
        this.stateManager = stateManager;
        this.objectPool = objectPool;
    }

    public void Enter()
    {
        
    }

    public void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z))       
            stateManager.ChangeState(new FeverTimeState(stateManager, objectPool));
        
        else if(objectPool.afterPeopleManager.IsClearFlag)
            stateManager.ChangeState(new FeverTimeState(stateManager, objectPool));

        else if (objectPool.gameoverController.IsGameoverFlag)
            stateManager.ChangeState(new GameoverState(stateManager, objectPool));
    }

    public void Exit()
    {
        // gameObject.SetActive(false);
    }
}
