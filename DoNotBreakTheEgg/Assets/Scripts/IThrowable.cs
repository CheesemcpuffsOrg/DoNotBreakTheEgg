using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IThrowable
{
    void Throw(float power, Transform startPoint);
}
