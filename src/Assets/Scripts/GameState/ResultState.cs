using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultState : GameState
{
    StateManager stateManager;
    UniqueObjectPool objectPool;

    public ResultState(StateManager stateManager, UniqueObjectPool objectPool)
    {
        this.stateManager = stateManager;
        this.objectPool = objectPool;
    }

    public void Enter()
    {
        objectPool.setting.Setting();
        objectPool.ClearResultUI.SetActive(true);
    }

    public void StateUpdate()
    {
        return;
    }

    public void Exit()
    {
        return;
    }
}
