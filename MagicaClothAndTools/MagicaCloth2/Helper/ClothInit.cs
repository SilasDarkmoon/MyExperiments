using System.Collections;
using UnityEngine;

public class ClothInit : MonoBehaviour
{
    private int _initCount = 0;
    public IEnumerator InitClothWork(ClothManager clothman)
    {
        ++_initCount;
        yield return null;
        clothman.InitCloth();
        --_initCount;
        if (_initCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
