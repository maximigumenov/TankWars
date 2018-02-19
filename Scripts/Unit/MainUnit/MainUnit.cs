using UnityEngine;
using System.Collections;
using Unit;
using Fire;
using UnityEngine.Events;

public class MainUnit : MonoBehaviour, IMainUnit, IDataUnit
{

    [SerializeField] CharacterController controller;
    [SerializeField] MainUnitCharacteristic characteristics;
    float speedRun = 0;
    Vector3 startPosition = Vector3.zero;
    [SerializeField] TypeUnit _typeUnit;
    Enemy.FindData findData = new Enemy.FindData();
    enum VectoreMove { none, forward, back }
    VectoreMove vectoreMove = VectoreMove.none;

    bool _isLife = true;

    bool isLoopAction = false;
    UnityAction LoopAction;

    IFindUnit findUnit = new FindUnit();
    SimpleMove simpleMove = new SimpleMove();
    private Vector3 moveDirection = Vector3.zero;


    public Transform transformObject
    {
        get
        {
            return transform;
        }
    }

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





    void Awake()
    {
        UnitsControl.mainUnit = this;
        simpleMove.Initialization(characteristics);
        GameTime.AddEvent(() => FindEnemy(), GameTimeController.GameTimeController.TimeEnum.halfSecond);
        InitFire(0);
        ICharacteristics mainCharacter = characteristics;
        startPosition = mainCharacter.transformObject.position;
        UIControl.SetHp(mainCharacter.hp, mainCharacter.maxHp);
    }

    void Update()
    {
        ExitGame();
        Move();
        Fire();
        SelectWeapon();
        DistanceFromCenter();
    }

    void ExitGame()
    {
        if (InputGame.ExitGame())
        {
            Application.Quit();
        }
    }

    void DistanceFromCenter()
    {
        ICharacteristics character = characteristics;
        IMainUnitCharacteristic mainCharacter = characteristics;
        float dist = Vector3.Distance(Vector3.zero, character.transformObject.position);
        if (dist > mainCharacter.warning)
        {
            UIControl.SetWarningTerritory(mainCharacter.endTerritory - dist);
        }
        else
        {
            UIControl.SetWarningTerritory();
        }

        if (dist > mainCharacter.endTerritory)
        {
            MonitorCamera.SetStartPosition();
            UIControl.SetWarningTerritory();
            Reborn();
        }

    }

    void SelectWeapon()
    {
        if (InputGame.SelectLeft())
        {
            characteristics.nowNumberWeapon--;
            if (characteristics.nowNumberWeapon < 0)
            {
                characteristics.nowNumberWeapon = characteristics.listFireData.Count - 1;
            }
            InitFire(characteristics.nowNumberWeapon);

        }
        else if (InputGame.SelectRight())
        {
            characteristics.nowNumberWeapon++;
            if (characteristics.nowNumberWeapon > characteristics.listFireData.Count - 1)
            {
                characteristics.nowNumberWeapon = 0;
            }
            InitFire(characteristics.nowNumberWeapon);
        }
    }

    public void SetDemage(int demage)
    {
        ICharacteristics myCharacteristic = characteristics;
        myCharacteristic.hp -= Damage.SetDamage(myCharacteristic.defense, demage);
        UIControl.SetHp(myCharacteristic.hp, myCharacteristic.maxHp);
        if (myCharacteristic.hp <= 0)
        {
            UIControl.SetHp(myCharacteristic.maxHp, myCharacteristic.maxHp);
            Reborn();
        }
    }

    void Reborn()
    {
        ICharacteristics myCharacteristic = characteristics;
        myCharacteristic.hp = myCharacteristic.maxHp;
        myCharacteristic.transformObject.position = startPosition;
    }

    void Move()
    {
        vectoreMove = VectoreMove.none;
        ICharacteristics characteristic = characteristics;
        IMainUnitCharacteristic mainCharact = characteristics;

        if (InputGame.Up())
        {
            speedRun -= Time.deltaTime * mainCharact.acceleration;
            if (speedRun > 0)
            {
                speedRun = 0;
            }
            vectoreMove = VectoreMove.forward;

        }

        if (InputGame.Down())
        {
            speedRun += Time.deltaTime * mainCharact.acceleration;
            if (speedRun < 0)
            {
                speedRun = 0;
            }
            vectoreMove = VectoreMove.back;
        }

        if (InputGame.Left())
        {
            if (vectoreMove == VectoreMove.back)
            {
                transform.Rotate(new Vector3(0, mainCharact.headSpeedRotate * Time.deltaTime, 0));
            }
            else
            {
                transform.Rotate(new Vector3(0, -1 * mainCharact.headSpeedRotate * Time.deltaTime, 0));
            }

        }

        if (InputGame.Right())
        {
            if (vectoreMove == VectoreMove.back)
            {
                transform.Rotate(new Vector3(0, -1 * mainCharact.headSpeedRotate * Time.deltaTime, 0));
            }
            else
            {
                transform.Rotate(new Vector3(0, mainCharact.headSpeedRotate * Time.deltaTime, 0));
            }
        }

        if (vectoreMove == VectoreMove.none)
        {
            if (speedRun > 0)
            {
                speedRun -= Time.deltaTime * mainCharact.acceleration;
            }
            else if (speedRun < 0)
            {
                speedRun += Time.deltaTime * mainCharact.acceleration;
            }
        }

        if (Mathf.Abs(speedRun) > mainCharact.maxSpeedAcceleration)
        {
            if (speedRun > 0)
            {
                speedRun = mainCharact.maxSpeedAcceleration;
            }
            else
            {
                speedRun = mainCharact.maxSpeedAcceleration * -1;
            }
        }


        moveDirection = new Vector3(0, 0, speedRun);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= characteristic.speedMove;

        moveDirection.y -= mainCharact.gravity;
        controller.Move(moveDirection * Time.deltaTime);
        Vector3 vector = Vector3.zero;

        Transform headTarget = mainCharact.standartTarget;
        if (mainCharact.enemyTarget != null)
        {
            headTarget = mainCharact.enemyTarget;
        }
        simpleMove.TurnToTarget(mainCharact.head, headTarget);
        mainCharact.head.localRotation = Quaternion.EulerAngles(0, mainCharact.head.localRotation.ToEuler().y, 0);


    }

    void FindEnemy()
    {

        ICharacteristics charact = characteristics;
        IMainUnitCharacteristic mainCharact = characteristics;

        findData.positiveFind = () => FindTarget();
        findData.negativeFind = () => NotFindTarget();
        findData.radius = charact.radiusAttackLook;
        findData.maxDistance = charact.maxDistanceAttackLook;
        findData.direction = Vector3.back;
        findData.typeUnit = TypeUnit.enemy;

        findUnit.FindObject(findData, mainCharact.standartTarget);
    }

    public int GetDemage()
    {
        IMainUnitCharacteristic mainCharact = characteristics;
        return mainCharact.fireData.demage;
    }

    public void ActionUnit_Start(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public void InitFire(int number)
    {
        IMainUnitCharacteristic mainCharact = characteristics;
        if (isLoopAction)
        {
            GameTime.RemoveEvent(LoopAction, GameTimeController.GameTimeController.TimeEnum.oneTenth);
        }

        DeactiveFireEffect();

        mainCharact.fireData = mainCharact.listFireData[number];
        IFireData fireDataObj = mainCharact.fireData;
        fireDataObj.fireGun = FireGun.CreateFire(mainCharact.fireData.typeFire);
        fireDataObj.fireGun.DataInit(characteristics);

        UIControl.SetWeapon(fireDataObj.icon, fireDataObj.description);
    }

    public void Fire()
    {
        IMainUnitCharacteristic mainCharact = characteristics;
        if (InputGame.FireDown())
        {

            ActiveFireEffect();
            if (mainCharact.fireData.isLoop)
            {
                LoopAction = () => ActiveFireEffect();
                isLoopAction = true;
                GameTime.AddEvent(LoopAction, GameTimeController.GameTimeController.TimeEnum.oneTenth);
            }
        }

        if (InputGame.FireUp())
        {
            if (isLoopAction)
            {
                GameTime.RemoveEvent(LoopAction, GameTimeController.GameTimeController.TimeEnum.oneTenth);
            }

        }
    }

    void FireForTarget()
    {
        if (characteristics.enemyTarget.GetComponent<IReactionShot>() != null)
        {
            IFireGun fireGunObject = characteristics.fireData.fireGun;
            fireGunObject.SetDamage();
        }

    }

    void FireFotNothing()
    {
        IFireGun fireGunObject = characteristics.fireData.fireGun;
        fireGunObject.SetDamageEmpty(CreateBullet(characteristics.fireData.typeFire));

    }



    GameObject CreateBullet(FireGun.TypeFire typeFire)
    {

        if (ControlFireBullet.IsBullet(typeFire))
        {
            return ControlFireBullet.GetBullet(typeFire);
        }
        else
        {
            GameObject obj = Instantiate<GameObject>(characteristics.fireData.prefabExplosion);
            ControlFireBullet.AddBullet(obj, typeFire);
            return obj;
        }
    }

    void ActiveFireEffect()
    {
        IFireGun fireGunObject = characteristics.fireData.fireGun;
        fireGunObject.ActiveFireEffect();
        Invoke("DeactiveFireEffect", 0.05f);
        if (characteristics.enemyTarget)
        {
            FireForTarget();
        }
        else
        {
            FireFotNothing();
        }
    }

    void DeactiveFireEffect()
    {
        IFireGun fireGunObject = characteristics.fireData.fireGun;
        fireGunObject.DeactiveFireEffect();
    }

    void FindTarget()
    {
        IMainUnitCharacteristic mainCharact = characteristics;
        mainCharact.enemyTarget = findData.hits[0].collider.transform;

    }

    public void NotFindTarget()
    {
        IMainUnitCharacteristic mainCharact = characteristics;
        mainCharact.enemyTarget = null;
    }
}
