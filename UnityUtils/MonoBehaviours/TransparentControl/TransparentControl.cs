using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TransparentControl<T> : UIBehaviour, ICanvasRaycastFilter where T : MonoBehaviour
{
    protected T _Target;
    protected RectTransform _Trans;
    protected Canvas _ParentCanvas;
    protected Camera _CanvasCamera;
    protected List<Mask> _ParentMasks = new List<Mask>();

    protected int? _PointerDownTouchId;

    protected override void Awake()
    {
        _Target = GetComponent<T>();
        _Trans = GetComponent<RectTransform>();

        GetParentCanvasAndCamera();
    }
    protected void GetParentCanvasAndCamera()
    {
        if (_Trans)
        { // may call this (through SetParent) before Awake.
            var canvas = GetComponentInParent<Canvas>();
            while (canvas != null && !canvas.isRootCanvas && canvas.transform.parent != null)
            {
                canvas = canvas.transform.parent.GetComponentInParent<Canvas>();
            }
            _ParentCanvas = canvas;
            _CanvasCamera = null;
            if (canvas != null)
            {
                _CanvasCamera = canvas.worldCamera;
            }

            _ParentMasks.Clear();
            var trans = _Trans.parent;
            while (trans != null)
            {
                var mask = trans.GetComponent<Mask>();
                if (mask)
                {
                    _ParentMasks.Add(mask);
                }
                if (trans == _ParentCanvas.transform)
                {
                    break;
                }
                trans = trans.parent;
            }
        }
    }

    protected virtual void Update()
    {
        if (_Target != null && _Trans != null && _ParentCanvas != null)
        {
            if (_PointerDownTouchId == null)
            {
                var touches = Input.touches;
                if (touches != null && touches.Length > 0)
                {
                    foreach (var touch in touches)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            var pos = touch.position;
                            if (IsRaycastLocationInControl(pos))
                            {
                                _PointerDownTouchId = touch.fingerId;
                                OnPointerDown(pos);
                                break;
                            }
                        }
                    }
                }
                if (_PointerDownTouchId == null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var pos = Input.mousePosition;
                        if (IsRaycastLocationInControl(pos))
                        {
                            _PointerDownTouchId = int.MinValue;
                            OnPointerDown(pos);
                        }
                    }
                }
            }
            else
            {
                int id = (int)_PointerDownTouchId;
                if (id == int.MinValue)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        _PointerDownTouchId = null;
                        var pos = Input.mousePosition;
                        bool inside = IsRaycastLocationInControl(pos);
                        OnPointerUp(pos, !inside);
                    }
                    else if (!Input.GetMouseButton(0))
                    {
                        _PointerDownTouchId = null;
                        var pos = Input.mousePosition;
                        OnPointerUp(pos, true);
                    }
                    else
                    {
                        var pos = Input.mousePosition;
                        OnPointerMove(pos);
                    }
                }
                else
                {
                    Touch? downtouch = null;
                    var touches = Input.touches;
                    if (touches != null && touches.Length > 0)
                    {
                        foreach (var touch in touches)
                        {
                            if (touch.fingerId == id)
                            {
                                downtouch = touch;
                                break;
                            }
                        }
                    }

                    if (downtouch == null)
                    {
                        _PointerDownTouchId = null;
                        OnPointerUp(new Vector2(float.NaN, float.NaN), true);
                    }
                    else
                    {
                        var touch = (Touch)downtouch;
                        if (touch.phase == TouchPhase.Moved)
                        {
                            var pos = touch.position;
                            OnPointerMove(pos);
                        }
                        else if (touch.phase != TouchPhase.Stationary)
                        {
                            _PointerDownTouchId = null;
                            var pos = touch.position;
                            bool inside = touch.phase == TouchPhase.Ended && IsRaycastLocationInControl(pos);
                            OnPointerUp(pos, !inside);
                        }
                    }
                }
            }
        }
    }

    public bool IsRaycastLocationInControl(Vector2 pos)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(_Trans, pos, _CanvasCamera))
        {
            return false;
        }
        for (int i = 0; i < _ParentMasks.Count; ++i)
        {
            var mask = _ParentMasks[i];
            if (!mask.IsRaycastLocationValid(pos, _CanvasCamera))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return false;
    }

    protected override void OnTransformParentChanged()
    {
        GetParentCanvasAndCamera();
    }

    public abstract void OnPointerDown(Vector2 pos);
    public abstract void OnPointerUp(Vector2 pos, bool outside);
    public abstract void OnPointerMove(Vector2 pos);
    // TODO: OnScroll (mouse scroll)
}
