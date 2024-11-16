using UnityEngine;

public class BeforePlayState : GameState
{
    GameObject gameObject;
    UniqueObjectPool objectPool;
    StateManager stateManager;
    public BeforePlayState(StateManager stateManager, UniqueObjectPool objectPool)
    {
        this.stateManager = stateManager;
        this.objectPool = objectPool;
    }

    public void Enter()
    {
        return;
    }

    public void StateUpdate()
    {
        if (Input.anyKeyDown)
        {
            stateManager.ChangeState(new WaitState(stateManager, objectPool));
        }
    }

    public void Exit()
    {
        objectPool.BeforePlayUIObj.SetActive(false);
    }
}
