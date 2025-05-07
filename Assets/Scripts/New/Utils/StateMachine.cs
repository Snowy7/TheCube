using System;
using System.Collections.Generic;

namespace New.Utils
{
// Simple state machine for managing states
    public class StateMachine<T>
    {
        private T currentState;
        private Dictionary<T, List<Transition<T>>> transitions = new Dictionary<T, List<Transition<T>>>();
        private List<Transition<T>> currentTransitions = new List<Transition<T>>();
        private List<Transition<T>> anyTransitions = new List<Transition<T>>();
        
        private static List<Transition<T>> EmptyTransitions = new List<Transition<T>>(0);
        
        public T CurrentState => currentState;
        
        public void SetState(T state)
        {
            if (state.Equals(currentState))
                return;
            
            currentState = state;
            transitions.TryGetValue(currentState, out currentTransitions);
            
            if (currentTransitions == null)
                currentTransitions = EmptyTransitions;
        }
        
        public void AddTransition(T from, T to, Func<bool> condition)
        {
            if (!transitions.TryGetValue(from, out var transitionList))
            {
                transitionList = new List<Transition<T>>();
                transitions[from] = transitionList;
            }
            
            transitionList.Add(new Transition<T>(to, condition));
        }
        
        public void AddAnyTransition(T to, Func<bool> condition)
        {
            anyTransitions.Add(new Transition<T>(to, condition));
        }
        
        public void Tick()
        {
            // Check for transitions from any state
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition())
                {
                    SetState(transition.To);
                    return;
                }
            }
            
            // Check for transitions from current state
            foreach (var transition in currentTransitions)
            {
                if (transition.Condition())
                {
                    SetState(transition.To);
                    return;
                }
            }
        }
        
        private class Transition<TState>
        {
            public TState To { get; }
            public Func<bool> Condition { get; }
            
            public Transition(TState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }
    }
}