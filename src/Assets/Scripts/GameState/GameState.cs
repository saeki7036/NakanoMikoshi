using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameState
{
    void Enter();
    void StateUpdate();
    void Exit();
}
