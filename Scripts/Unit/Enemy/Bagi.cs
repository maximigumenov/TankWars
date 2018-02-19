using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine.Events;
using Fire;

public class Bagi : Enemy, IDetectCollisionTarget, IReactionShot
{

    [SerializeField] BagiCharacteristic characteristics;

    UnityAction findAction;
    UnityAction demageFunc;

    #region ActionUnit

    public void ReactionShot(FireGun.TypeFire typeFire)
    {
        IBagiCharacteristic bagiChar = characteristics;
        ICharacteristics objChar = characteristics;
        switch (typeFire)
        {
            case FireGun.TypeFire.fragmentary:
                bagiChar.boomParticle.transform.position = objChar.transformObject.position;
                bagiChar.boomParticle.Play();
                break;
            case FireGun.TypeFire.armorPiercer:
                bagiChar.boomParticle.transform.position = objChar.transformObject.position;
                bagiChar.boomParticle.Play();
                break;
            default:
                break;
        }

        ActionUnit_GetDemage(UnitsControl.mainUnit.GetDemage());
    }

    public override void ActionUnit_Start(Vector3 position)
    {
        base.ActionUnit_Start(position);

        UpdateCharacteristick();
        ActionUnit_Move();
        IMainUnit mainUnit = UnitsControl.mainUnit;
        ICharacteristics character = characteristics;
        character.hp = character.maxHp;
        target = mainUnit.transformObject;
        FindData findData = new FindData();
        findData.positiveFind = () => ActionUnit_Attack();
        findData.negativeFind = () => FindAttack();
        findData.maxDistance = character.maxDistanceAttackLook;
        findData.radius = character.radiusAttackLook;
        findData.typeUnit = TypeUnit.main;
        findData.direction = Vector3.forward;
        findAction = () => ActionUnit_Find(findData);
        moveEnemy.Start();
        GameTime.AddEvent(findAction, GameTimeController.GameTimeController.TimeEnum.oneTenth);
        IBagiCharacteristic bagiChar = characteristics;
        bagiChar.boxCollider.enabled = true;
        bagiChar.boomParticle.transform.parent = null;
        demageFunc = () => SetDemage();

    }

    protected override void ActionUnit_Update()
    {
        if (isLife)
        {
            base.ActionUnit_Update();
            moveEnemy.Move(agent, target);
            moveEnemy.TurnToTarget(mainTransform, target);
        }
    }

    protected override void ActionUnit_Attack()
    {
        base.ActionUnit_Attack();

        ICharacteristics myCharacteristic = characteristics;
        IBagiCharacteristic bagiCharacteristic = characteristics;

        if (bagiCharacteristic.unitAction != BagiCharacteristic.BagiAction.attack)
        {
            GameTime.AddEvent(demageFunc, GameTimeController.GameTimeController.TimeEnum.second);
            moveEnemy.Stop();
            bagiCharacteristic.unitAction = BagiCharacteristic.BagiAction.attack;
        }
    }

    protected override void ActionUnit_GetDemage(int hitDemage)
    {
        base.ActionUnit_GetDemage(hitDemage);

        ICharacteristics myCharacteristic = characteristics;
        myCharacteristic.hp -= Damage.SetDamage(myCharacteristic.defense, hitDemage);
        if (myCharacteristic.hp <= 0)
        {
            myCharacteristic.hp = 0;
            ActionUnit_Die();
        }
    }

    protected override void ActionUnit_Move()
    {
        base.ActionUnit_Move();
        IBagiCharacteristic enemChar = characteristics;
        enemChar.unitAction = BagiCharacteristic.BagiAction.move;
    }

    protected override void ActionUnit_Die()
    {
        if (isLife)
        {
            GameTime.RemoveEvent(demageFunc, GameTimeController.GameTimeController.TimeEnum.second);
            base.ActionUnit_Die();
            ICharacteristics myCharacteristics = characteristics;
            IBagiCharacteristic bagiCharacteristic = characteristics;
            bagiCharacteristic.boomParticle.transform.position = myCharacteristics.transformObject.position;
            bagiCharacteristic.boomParticle.Play();
            bagiCharacteristic.boxCollider.enabled = false;
            bagiCharacteristic.unitAction = BagiCharacteristic.BagiAction.death;
            GameTime.RemoveEvent(findAction, GameTimeController.GameTimeController.TimeEnum.oneTenth);
            agent.destination = agent.transform.position;
            UnitsControl.mainUnit.NotFindTarget();
            myCharacteristics.transformObject.gameObject.SetActive(false);
        }
    }

    protected override void ActionUnit_Find(FindData data)
    {
        base.ActionUnit_Find(data);
    }
    #endregion

    #region Functions

    void UpdateCharacteristick()
    {
        ICharacteristics characteristic = characteristics;
        moveEnemy.Initialization(characteristics);
        agent.speed = characteristic.speedMove;
    }

    void FindAttack()
    {

        IMainUnit main = UnitsControl.mainUnit;
        ICharacteristics myCharacteristic = characteristics;
        IBagiCharacteristic bagiCharacteristic = characteristics;

        if (bagiCharacteristic.unitAction == BagiCharacteristic.BagiAction.attack)
        {
            GameTime.RemoveEvent(demageFunc, GameTimeController.GameTimeController.TimeEnum.second);
            bagiCharacteristic.unitAction = BagiCharacteristic.BagiAction.run;
            moveEnemy.Start();
            UpdateCharacteristick();
        }
    }

    public void SetDemage()
    {
        IBagiCharacteristic bagiCharacteristic = characteristics;
        ICharacteristics _characteristics = characteristics;
        bagiCharacteristic.fireObject.SetActive(true);
        UnitsControl.mainUnit.SetDemage(_characteristics.damage);

        Invoke("DeactiveFire", 0.1f);
    }

    void DeactiveFire()
    {
        IBagiCharacteristic bagiCharacteristic = characteristics;
        bagiCharacteristic.fireObject.SetActive(false);
    }

    public void ActiveEvent()
    {
        SetDemage();
        IBagiCharacteristic bagiChar = characteristics;
        ICharacteristics objChar = characteristics;

        bagiChar.boomParticle.transform.position = objChar.transformObject.position;
        bagiChar.boomParticle.Play();
    }

    #endregion
}
