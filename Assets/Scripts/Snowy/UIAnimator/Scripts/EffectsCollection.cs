﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snowy.UI.Effects
{
    [Serializable]
    public class EffectsCollection : IEffect
    {
        [BeginGroup]
        public bool isApplyParallel;
        public bool isCancelParallel;
        [EndGroup]
        [SerializeReference, ReorderableList] private List<SnEffect> effects = new ();
        
        public bool IsPlaying { get; private set; }
        
        public void Initialize(IEffectsManager manager)
        {
            foreach (var effect in effects)
            {
                if (effect == null) continue;
                effect.Initialize(manager);
            }
        }
        
        public EffectsCollection()
        {
            effects = new List<SnEffect>();
            isApplyParallel = true;
        }

        public EffectsCollection(bool isApplyParallel)
        {
            this.isApplyParallel = isApplyParallel;
            effects = new List<SnEffect>();
        }

        public IEnumerator Apply(IEffectsManager manager)
        {
            IsPlaying = true;
            if (isApplyParallel)
            {
                foreach (var effect in effects)
                {
                    try
                    {
                        manager.Mono.StartCoroutine(effect.Apply(manager));
                    } catch (Exception e)
                    {
                        Debug.LogError($"{manager.Transform.name} - {e}");
                    }
                }

                yield return new WaitUntil(() => effects.All(effect => !effect.IsPlaying));
            }
            else
            {
                foreach (var effect in effects)
                {
                    yield return effect.Apply(manager);
                }
            }

            IsPlaying = false;
        }

        public IEnumerator Cancel(IEffectsManager manager)
        {
            foreach (var eff in effects)
            {
                if (eff.IsPlaying)
                {
                    manager.Mono.StopCoroutine(eff.Apply(manager));
                }
            }

            IsPlaying = true;
            if (isCancelParallel)
            {
                foreach (var effect in effects)
                {
                    manager.Mono.StartCoroutine(effect.Cancel(manager));
                }

                yield return new WaitUntil(() => effects.All(effect => !effect.IsPlaying));
            }
            else
            {
                foreach (var effect in effects)
                {
                    yield return effect.Cancel(manager);
                }
            }

            IsPlaying = false;
        }

        public void ImmediateCancel(IEffectsManager manager)
        {
            foreach (var effect in effects)
            {
                effect?.ImmediateCancel(manager);
            }
        }
        
        public void ImmediateCancelWithout(IEffectsManager manager, SnEffect[] skipEffects)
        {
            foreach (var effect in effects)
            {
                // check if any contain the same effect type
                if (skipEffects.Any(skipEffect => effect.GetType() == skipEffect.GetType())) continue;
                effect.ImmediateCancel(manager);
            }
        }

        public SnEffect[] GetEffects()
        {
            return effects.ToArray();
        }

        public SnEffect AddEffect(SnEffect effect)
        {
            effects.Add(effect);
            return effect;
        }

        public void RemoveEffect(SnEffect effect)
        {
            effects = effects.Where(e => e != effect).ToList();
        }

        public void ClearEffects()
        {
            effects.Clear();
        }
    }
}