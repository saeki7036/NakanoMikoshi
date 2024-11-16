using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class WaitState : GameState
{
    StateManager stateManager;
    UniqueObjectPool objectPool;
  
    public WaitState(StateManager stateManager, UniqueObjectPool objectPool)
    {
        this.stateManager = stateManager;
        this.objectPool = objectPool;
    }

    public void Enter()
    {
        objectPool.WaitUIObj.SetActive(true);
        objectPool.PeopleNumUIObj.SetActive(true);
        objectPool.TimeNumUIObj.SetActive(true);
        objectPool.timeManager.StartCountDown();
    }

    public void StateUpdate()
    {
        if(objectPool.timeManager.GetEndCountDown)
            stateManager.ChangeState(new MainGameState(stateManager, objectPool));              
    }

    public void Exit()
    {
        objectPool.WaitUIObj.SetActive(false);
    }
}
