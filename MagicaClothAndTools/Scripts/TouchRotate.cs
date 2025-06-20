using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchRotate : MonoBehaviour, IDragHandler
{
    public Transform Target;

    public void OnDrag(PointerEventData eventData)
    {
        Target.localEulerAngles = Target.localEulerAngles + new Vector3(0, eventData.delta.x / MathF.PI, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
