using System;
using System.Collections.Generic;

namespace New.Utils
{
// Generic event manager for game-wide events
    public static class EventManager
    {
        private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
        private static Dictionary<string, Delegate> eventDictionaryParams = new Dictionary<string, Delegate>();
        
        // Add a listener to an event
        public static void AddListener(string eventName, Action listener)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] += listener;
            }
            else
            {
                eventDictionary.Add(eventName, listener);
            }
        }
        
        // Add a listener with parameters to an event
        public static void AddListener<T>(string eventName, Action<T> listener)
        {
            if (eventDictionaryParams.ContainsKey(eventName))
            {
                eventDictionaryParams[eventName] = Delegate.Combine(eventDictionaryParams[eventName], listener);
            }
            else
            {
                eventDictionaryParams.Add(eventName, listener);
            }
        }
        
        // Remove a listener from an event
        public static void RemoveListener(string eventName, Action listener)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] -= listener;
                
                if (eventDictionary[eventName] == null)
                {
                    eventDictionary.Remove(eventName);
                }
            }
        }
        
        // Remove a listener with parameters from an event
        public static void RemoveListener<T>(string eventName, Action<T> listener)
        {
            if (eventDictionaryParams.ContainsKey(eventName))
            {
                eventDictionaryParams[eventName] = Delegate.Remove(eventDictionaryParams[eventName], listener);
                
                if (eventDictionaryParams[eventName] == null)
                {
                    eventDictionaryParams.Remove(eventName);
                }
            }
        }
        
        // Trigger an event
        public static void TriggerEvent(string eventName)
        {
            if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
            {
                thisEvent?.Invoke();
            }
        }
        
        // Trigger an event with parameters
        public static void TriggerEvent<T>(string eventName, T param)
        {
            if (eventDictionaryParams.TryGetValue(eventName, out Delegate thisEvent))
            {
                if (thisEvent is Action<T> typedEvent)
                {
                    typedEvent(param);
                }
            }
        }
    }
}