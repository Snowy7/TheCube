using System;
using System.Collections;
using UnityEngine;

public class TabUI : MonoBehaviour
{
    [Title("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float animationDuration = 0.5f;


    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    [ContextMenu("Show")]
    public void Show()
    {
        gameObject.SetActive(true);
        if (animator) animator.SetBool("Shown", true);
    }
    
    [ContextMenu("Hide")]
    public void Hide()
    {
        if (animator) animator.SetBool("Shown", false);
        Invoke(nameof(Disable), animationDuration);
    }
    
    private void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Switch(bool state, Action callback = null)
    {
        if (state) Show();
        else Hide();
    }
    
    public float GetAnimationDuration()
    {
        return animationDuration;
    }
}
