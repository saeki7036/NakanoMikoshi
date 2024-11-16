using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeverTimeState : GameState
{
    StateManager stateManager;
    UniqueObjectPool objectPool;
    public FeverTimeState(StateManager stateManager, UniqueObjectPool objectPool)
    {
        this.stateManager = stateManager;
        this.objectPool = objectPool;
    }
    public void Enter()
    {
        objectPool.FeverSliderTextUI.SetActive(true);
    }

    public void StateUpdate()
    {
        if (objectPool.timeManager.EndFeverTime)
            stateManager.ChangeState(new ClearState(stateManager, objectPool));
    }

    public void Exit()
    {
        objectPool.FeverSliderTextUI.SetActive(false);
        objectPool.PeopleNumUIObj.SetActive(false);
        objectPool.FeverSliderUIObj.SetActive(false);
        objectPool.TimeNumUIObj.SetActive(false);
    }
}
