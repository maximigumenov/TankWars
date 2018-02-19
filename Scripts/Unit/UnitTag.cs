using UnityEngine;
using System.Collections;
using Unit;

public class UnitTag : MonoBehaviour, IUnitTag
{
    [SerializeField] TypeUnit _typeUnit;
    public TypeUnit typeUnit { get { return _typeUnit; } }
}
