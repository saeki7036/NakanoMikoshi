using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverState : GameState
{
    StateManager stateManager;
    UniqueObjectPool objectPool;
    public GameoverState(StateManager stateManager, UniqueObjectPool objectPool)
    {
        this.stateManager = stateManager;
        this.objectPool = objectPool;
    }
    
    public void Enter() 
    {
        //èIóπÇ∑ÇÈÇÃÇ≈falseÇ≈åƒÇ‘
        objectPool.PeopleNumUIObj.SetActive(false);
        objectPool.FeverSliderUIObj.SetActive(false);
        objectPool.TimeNumUIObj.SetActive(false) ;

        objectPool.gameoverController.Gameover();
    }
    public void StateUpdate() { }
    public void Exit() { }
}
