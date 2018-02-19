using UnityEngine;
using System.Collections;
using Unit;

public class TerrainObject : MonoBehaviour, IDataUnit
{
    bool _isLife = false;
    [SerializeField] TypeUnit _typeUnit;

    public TypeUnit typeUnit
    {
        get
        {
            return _typeUnit;
        }
    }

    public bool isLife
    {
        get
        {
            return _isLife;
        }
        set
        {
            _isLife = value;
        }
    }

    public void ActionUnit_Start(Vector3 position)
    {
        throw new System.NotImplementedException();
    }
}
