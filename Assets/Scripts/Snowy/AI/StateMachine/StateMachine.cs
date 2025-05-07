using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowy.AI.StateMachine
{
    public class StateMachine
    {
        private StateNode current;
        private Dictionary<Type, StateNode> nodes = new();
        private HashSet<ITransition> anyTransitions = new();
        
        public StateNode CurrentState => current;

        public void OnUpdate()
        {
            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);
            
            current.State?.OnUpdate();
        }
        
        public void OnFixedUpdate() => current.State?.OnFixedUpdate();

        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        void ChangeState(IState state)
        {
            if (state == current.State) return;
            
            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;
            
            previousState?.OnExit();
            nextState?.OnEnter();
            
            current = nodes[state.GetType()];
        }
        
        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            foreach (var transition in current.Transitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }
            
            return null;
        }
        
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }
        
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }
        
        StateNode GetOrAddNode(IState state)
        {
            if (nodes.TryGetValue(state.GetType(), out var node))
                return node;
            
            var newNode = new StateNode(state);
            nodes[state.GetType()] = newNode;
            return newNode;
        }

        public class StateNode
        {
            public IState State { get;}
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            
            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}