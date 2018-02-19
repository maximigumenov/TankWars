using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine.Events;
using Fire;

public class Zombie : Enemy, IDetectCollisionTarget, IReactionShot
{
    [SerializeField] Animator animator;
    [SerializeField] ZombieCharacteristic characteristics;

    UnityAction findAction;
    UnityAction demageFunc;

    #region ActionUnit

    public void ReactionShot(FireGun.TypeFire typeFire)
    {
        IZombieCharacteristic zombieChar = characteristics;
        ICharacteristics objChar = characteristics;
        switch (typeFire)
        {
            case FireGun.TypeFire.machineGun:
                zombieChar.bloodParticle.transform.position = objChar.transformObject.position;
                zombieChar.bloodParticle.Play();
                break;
            case FireGun.TypeFire.fragmentary:
                zombieChar.bigBoomParticle.transform.position = objChar.transformObject.position;
                zombieChar.bigBoomParticle.Play();
                break;
            case FireGun.TypeFire.armorPiercer:
                zombieChar.boomParticle.transform.position = objChar.transformObject.position;
                zombieChar.boomParticle.Play();
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
        IZombieCharacteristic zombieChar = characteristics;
        zombieChar.boxCollider.enabled = true;
        zombieChar.bloodParticle.transform.parent = null;
        zombieChar.boomParticle.transform.parent = null;
        zombieChar.bigBoomParticle.transform.parent = null;
        demageFunc = () => SetDemage();
    }

    protected override void ActionUnit_Update()
    {
        if (isLife)
        {
            base.ActionUnit_Update();
            moveEnemy.Move(agent, target);
            moveEnemy.TurnToTarget(mainTransform, target);
            animator.transform.localRotation = Quaternion.identity;
            animator.transform.localPosition = Vector3.zero;
            ChangeSpeed();
        }
    }

    protected override void ActionUnit_Attack()
    {
        base.ActionUnit_Attack();

        ICharacteristics myCharacteristic = characteristics;
        IZombieCharacteristic myZombieCharacteristic = characteristics;

        if (myZombieCharacteristic.unitAction != ZombieCharacteristic.ZombieAction.attack)
        {
            animator.SetTrigger("Attack");
            GameTime.AddEvent(demageFunc, GameTimeController.GameTimeController.TimeEnum.second);
            moveEnemy.Stop();
            myZombieCharacteristic.unitAction = ZombieCharacteristic.ZombieAction.attack;
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

    protected override void ActionUnit_Hide()
    {
        base.ActionUnit_Hide();

        animator.SetTrigger("Idle");
    }

    protected override void ActionUnit_Move()
    {
        base.ActionUnit_Move();

        animator.transform.localRotation = Quaternion.identity;
        animator.transform.localPosition = Vector3.zero;
        IZombieCharacteristic myZombieCharacteristic = characteristics;
        myZombieCharacteristic.unitAction = ZombieCharacteristic.ZombieAction.move;
        animator.SetTrigger("Move");
    }

    protected override void ActionUnit_Die()
    {
        if (isLife)
        {
            GameTime.RemoveEvent(demageFunc, GameTimeController.GameTimeController.TimeEnum.second);
            base.ActionUnit_Die();
            animator.SetTrigger("Death");
            IZombieCharacteristic myZombieCharacteristic = characteristics;
            myZombieCharacteristic.boxCollider.enabled = false;
            myZombieCharacteristic.unitAction = ZombieCharacteristic.ZombieAction.death;
            GameTime.RemoveEvent(findAction, GameTimeController.GameTimeController.TimeEnum.oneTenth);
            agent.destination = agent.transform.position;
            UnitsControl.mainUnit.NotFindTarget();
        }
    }

    protected override void ActionUnit_Idle()
    {
        base.ActionUnit_Idle();
        animator.SetTrigger("Idle");
    }

    protected override void ActionUnit_Find(FindData data)
    {
        base.ActionUnit_Find(data);

    }
    #endregion

    #region Functions

    void ChangeSpeed()
    {
        IMainUnit main = UnitsControl.mainUnit;
        ICharacteristics myCharacteristic = characteristics;
        IZombieCharacteristic myZombieCharacteristic = characteristics;

        float dist = Vector3.Distance(main.transformObject.position, myCharacteristic.transformObject.position);
        if (myZombieCharacteristic.unitAction == ZombieCharacteristic.ZombieAction.move)
        {
            if (dist <= myZombieCharacteristic.agresiveLook)
            {
                myCharacteristic.speedMove = myZombieCharacteristic.agresiveSpeed;
                myZombieCharacteristic.unitAction = ZombieCharacteristic.ZombieAction.run;
                animator.SetTrigger("Run");
                UpdateCharacteristick();
            }
        }
        else if (myZombieCharacteristic.unitAction == ZombieCharacteristic.ZombieAction.run)
        {
            if (dist > myZombieCharacteristic.agresiveLook)
            {
                myCharacteristic.speedMove = myZombieCharacteristic.normalSpeed;
                myZombieCharacteristic.unitAction = ZombieCharacteristic.ZombieAction.move;
                animator.SetTrigger("Move");
                UpdateCharacteristick();
            }
        }
    }

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
        IZombieCharacteristic myZombieCharacteristic = characteristics;

        if (myZombieCharacteristic.unitAction == ZombieCharacteristic.ZombieAction.attack)
        {
            GameTime.RemoveEvent(demageFunc, GameTimeController.GameTimeController.TimeEnum.second);
            myCharacteristic.speedMove = myZombieCharacteristic.agresiveSpeed;
            myZombieCharacteristic.unitAction = ZombieCharacteristic.ZombieAction.run;
            animator.SetTrigger("Run");
            animator.transform.localRotation = Quaternion.identity;
            animator.transform.localPosition = Vector3.zero;
            moveEnemy.Start();
            UpdateCharacteristick();
        }
    }

    public void SetDemage()
    {

        ICharacteristics _characteristics = characteristics;
        UnitsControl.mainUnit.SetDemage(_characteristics.damage);
    }

    public void ActiveEvent()
    {
        SetDemage();
        ActionUnit_Die();
    }

    #endregion
}
