using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface CommandsInterface : IEventSystemHandler
{
    void SelectUnit();
}
