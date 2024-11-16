using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class GameStateController : MonoBehaviour
{
    private StateManager stateManager;
    [SerializeField]
    UniqueObjectPool objectPool;

    public GameState GetState() { return stateManager.GetState(); }
    // Start is called before the first frame update
    void Start()
    {
        stateManager = new();
        stateManager.ChangeState(new BeforePlayState(stateManager, objectPool));
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.StateUpdate();

        GameStateTriggered();
    }
    void GameStateTriggered()
    {
        Debug.Log(GetState().ToString());
    }
}
public class StateManager
{
    private GameState currentState;

    public GameState GetState() => currentState;
    public void ChangeState(GameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void StateUpdate()
    {
        currentState?.StateUpdate();
    }
}
