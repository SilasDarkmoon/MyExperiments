using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransparentLuaControl : TransparentControl<CapsUnityLuaBehav>
{
    public override void OnPointerDown(Vector2 pos)
    {
        _Target.CallLuaFunc("onPointerDown", pos, _CanvasCamera);
    }
    public override void OnPointerMove(Vector2 pos)
    {
        _Target.CallLuaFunc("onPointerMove", pos, _CanvasCamera);
    }
    public override void OnPointerUp(Vector2 pos, bool outside)
    { 
        _Target.CallLuaFunc("onPointerUp", pos, outside, _CanvasCamera);
    }
}
