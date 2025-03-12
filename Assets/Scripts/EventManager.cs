using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    private Dictionary<string, UnityEvent<object>> _eventDictionary = new Dictionary<string, UnityEvent<object>>();
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    public static void RegisterEvent(string eventName, UnityAction<object> listener)
    {
        bool isEventAlreadyRegistered = Instance._eventDictionary.TryGetValue(eventName, out var thisEvent);
        if (isEventAlreadyRegistered)
        {
            // add new listener
            thisEvent.AddListener(listener);
        }
        else
        {
            // create new event and add listener
            thisEvent = new UnityEvent<object>();
            thisEvent.AddListener(listener);
            Instance._eventDictionary.Add(eventName, thisEvent);
        }
    }
    
    public static void TriggerEvent(string eventName, object arg0)
    {
        bool hasEvent = Instance._eventDictionary.TryGetValue(eventName, out var thisEvent);
        if (hasEvent)
        {
            thisEvent.Invoke(arg0);
        }
    }
    
}