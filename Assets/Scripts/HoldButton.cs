using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Action down;
    public Action up;
    bool held;
    public void OnPointerDown(PointerEventData e) { held=true; down?.Invoke(); }
    public void OnPointerUp(PointerEventData e) { Release(); }
    public void OnPointerExit(PointerEventData e) { Release(); }
    void OnDisable() { Release(); }
    void Release() { if(!held)return; held=false; up?.Invoke(); }
}
