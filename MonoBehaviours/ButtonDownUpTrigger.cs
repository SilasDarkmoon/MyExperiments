using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonDownUpTrigger : MonoBehaviour
{
    public Selectable TriggerButton;
    public UnityEvent OnButtonDown;
    public UnityEvent OnButtonUp;

    private Func<bool> get_isPointerDown;
    private static MethodInfo method_isPointerDown;

    public void Rebind()
    {
        if (TriggerButton)
        {
            if (method_isPointerDown == null)
            {
                var prop = typeof(Selectable).GetProperty("isPointerDown", BindingFlags.Instance | BindingFlags.NonPublic);
                method_isPointerDown = prop.GetGetMethod(true);
            }
            if (method_isPointerDown != null)
            {
                get_isPointerDown = (Func<bool>)method_isPointerDown.CreateDelegate(typeof(Func<bool>), TriggerButton);
            }
        }
        else
        {
            get_isPointerDown = null;
        }
    }

    private void Awake()
    {
        if (!TriggerButton)
        {
            TriggerButton = GetComponent<Selectable>();
        }
        Rebind();
    }
    private void Update()
    {
        IsButtonDown = get_isPointerDown?.Invoke() ?? false;
    }

    private bool _isButtonDown = false;
    public bool IsButtonDown
    {
        get => _isButtonDown;
        private set
        {
            if (value != _isButtonDown)
            {
                _isButtonDown = value;
                if (value)
                {
                    OnButtonDown?.Invoke();
                }
                else
                {
                    OnButtonUp?.Invoke();
                }
            }
        }
    }
}