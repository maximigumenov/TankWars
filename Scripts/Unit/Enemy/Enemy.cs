using UnityEngine;
using System.Collections;
using Unit;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDataUnit
{
    [SerializeField] TypeUnit _typeUnit;

    [SerializeField] protected NavMeshAgent agent;
    protected SimpleMove moveEnemy = new SimpleMove();
    protected UnitEvents unitEvents = new UnitEvents();
    [SerializeField] protected Transform mainTransform;
    [SerializeField] protected Transform target;
    protected bool _isLife = false;
    protected FindUnit findUnit = new FindUnit();

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

    private void Start()
    {

    }

    void Update()
    {
        ActionUnit_Update();
    }

    #region ActionUnit

    public virtual void ActionUnit_Start(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;
        isLife = true;
    }

    protected virtual void ActionUnit_GetDemage(int hitDemage)
    {

    }

    protected virtual void ActionUnit_Update()
    {

    }

    protected virtual void ActionUnit_Attack()
    {

    }



    protected virtual void ActionUnit_Find(FindData data)
    {
        Find(data);
    }

    protected virtual void ActionUnit_Hide()
    {

    }

    protected virtual void ActionUnit_Move()
    {

    }

    protected virtual void ActionUnit_Die()
    {
        if (isLife)
        {
            isLife = false;
            ObjectGenerator.nowObject--;
        }
    }

    protected virtual void ActionUnit_Idle()
    {

    }

    #endregion

    #region Functions
    protected void Find(FindData data)
    {
        IFindUnit _findUnit = findUnit;
        _findUnit.FindObject(data, mainTransform);

    }
    #endregion

    public class FindData
    {
        public System.Action positiveFind;
        public System.Action negativeFind;
        public RaycastHit[] hits;
        public TypeUnit typeUnit;
        public float maxDistance;
        public float radius;
        public Vector3 direction;
    }
}
