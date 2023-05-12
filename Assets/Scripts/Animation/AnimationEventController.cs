using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    public static Action onAnimationEvent;

    private void InvokeEvent()
    {
        if (GetComponentInParent<PlayerController>() != null)
            onAnimationEvent?.Invoke();
    }
}
