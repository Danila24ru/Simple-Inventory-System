using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : UnityEvent<GameObject> { }

/// <summary>
/// Helper component to get events from other GameObjects
/// </summary>
public class TriggerHandler : MonoBehaviour
{
    public TriggerEvent enterTriggerEvent;
    public TriggerEvent exitTriggerEvent;

    public UnityEvent onMouseDownEvent;
    public UnityEvent onMouseUpEvent;

    private void Awake()
    {
        enterTriggerEvent = new TriggerEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        enterTriggerEvent?.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        exitTriggerEvent?.Invoke(other.gameObject);
    }

    private void OnMouseDown()
    {
        onMouseDownEvent?.Invoke();
    }

    private void OnMouseUp()
    {
        onMouseUpEvent?.Invoke();
    }
}
