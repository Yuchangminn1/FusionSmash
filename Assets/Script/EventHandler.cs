using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public event Action Jump;
    public event Action Attack;
    public event Action<CharacterHandler> CharacterUpdate;
    public event Action<CharacterHandler> Respawn;

    public event Action<Vector2> Move;
}
