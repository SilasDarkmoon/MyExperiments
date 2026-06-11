using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransparentButton : TransparentControl<Button>
{
    public override void OnPointerDown(Vector2 pos)
    { // TODO: make a C# event to this?
        _Target.OnPointerDown(new UnityEngine.EventSystems.PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
    }
    public override void OnPointerMove(Vector2 pos)
    {
    }
    public override void OnPointerUp(Vector2 pos, bool outside)
    {
        _Target.OnPointerUp(new UnityEngine.EventSystems.PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
        if (!outside)
        {
            _Target.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
        }
    }
}
