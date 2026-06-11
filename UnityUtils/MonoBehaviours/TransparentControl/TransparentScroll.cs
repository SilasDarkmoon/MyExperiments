using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransparentScroll : TransparentControl<ScrollRect>
{
    public override void OnPointerDown(Vector2 pos)
    {
        _Target.OnBeginDrag(new UnityEngine.EventSystems.PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left, position = pos });
    }
    public override void OnPointerMove(Vector2 pos)
    {
        _Target.OnDrag(new UnityEngine.EventSystems.PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left, position = pos });
    }
    public override void OnPointerUp(Vector2 pos, bool outside)
    {
        _Target.OnEndDrag(new UnityEngine.EventSystems.PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left, position = pos });
    }
}
