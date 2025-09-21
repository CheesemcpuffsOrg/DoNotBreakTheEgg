using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEmission
{
    Type EmissionType { get; }
    public void Emit(object obj);
}
