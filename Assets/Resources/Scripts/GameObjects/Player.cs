﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutileFourDirectionBaseObject
{
    FAnimatedSprite sprite;
    public enum State
    {
        IDLE,
        MOVING,
        ATTACKING,
        TAKE_DAMAGE,
        DYING
    }
    float moveSpeed = 1;
    bool lastAttackPress = false;
    public float invulnerableCount = 0;
    int health = UI.MAX_HEALTH;
    public enum AttackType
    {
        NORMAL,
        WIDE,
        WIDE_TWO,
        WIDE_360
    }
    public AttackType currentAttack = AttackType.NORMAL;

    private float _isoHeight = 0;
    public float isoHeight { get { return _isoHeight; } set { _isoHeight = value; sprite.y = _isoHeight; } }


    State _state = State.IDLE;
    public State CurrentState
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                stateCount = 0;
                switch (value)
                {
                    case State.ATTACKING:
                        hasAttacked = false;
                        break;
                }
            }
        }
    }
    int idleAnim = 1;
    public Player(World world)
        : base(new RXRect(0, -5, 12, 7), world, "playerShadow")
    {
        int animSpeed = 200;
        int attackSpeed = 100;
        sprite = new FAnimatedSprite("player");
        bounceiness = 0;
        ResetMaxVel();
        handleStateCount = true;
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.IDLE + "1", new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 9, 1 }, animSpeed, false));
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.IDLE + "2", new int[] { 10, 1 }, 1500, false));
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.IDLE + "3", new int[] { 1, 10, 11, 10, 1 }, animSpeed, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.IDLE, new int[] { 4 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.IDLE, new int[] { 7 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.IDLE, new int[] { 7 }, animSpeed, true));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.MOVING, new int[] { 1, 2, 1, 3 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.MOVING, new int[] { 4, 5, 4, 6 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.MOVING, new int[] { 7, 8 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.MOVING, new int[] { 7, 8 }, animSpeed, true));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.ATTACKING, new int[] { 17, 18 }, attackSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.ATTACKING, new int[] { 19, 20 }, attackSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.ATTACKING, new int[] { 21, 22 }, attackSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.ATTACKING, new int[] { 21, 22 }, attackSpeed, true));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.TAKE_DAMAGE, new int[] { 12 }, animSpeed, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.TAKE_DAMAGE, new int[] { 13 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.TAKE_DAMAGE, new int[] { 14 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.TAKE_DAMAGE, new int[] { 14 }, animSpeed, true));


        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.DYING, new int[] { 25 }, animSpeed, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.DYING, new int[] { 25 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.DYING, new int[] { 25 }, animSpeed, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.DYING, new int[] { 25 }, animSpeed, true));


        sprite.play(Direction.UP.ToString());
        this.AddChild(sprite);
    }
    private void ResetMaxVel()
    {
        maxXVel = 1;
        maxYVel = 1;
        minYVel = -1;
    }
    public void Pickup(AttackType type)
    {
        switch (currentAttack)
        {
            case AttackType.NORMAL:
                switch (type)
                {
                    case AttackType.NORMAL:
                        currentAttack = AttackType.WIDE;
                        break;
                }
                break;
            case AttackType.WIDE:
                switch (type)
                {
                    case AttackType.NORMAL:
                        currentAttack = AttackType.WIDE_TWO;
                        break;
                }
                break;
            case AttackType.WIDE_TWO:
                switch (type)
                {
                    case AttackType.NORMAL:
                        currentAttack = AttackType.WIDE_360;
                        break;
                }
                break;
        }
        world.ui.UpdatePowerup(currentAttack);
    }
    protected override void OnUpdate()
    {
        if (invulnerableCount > 0)
            invulnerableCount -= Time.deltaTime;
        else
            invulnerableCount = 0;

        base.OnUpdate();
    }
    public void GetHealth()
    {
        health = Mathf.Clamp(++health, 0, UI.MAX_HEALTH);
        world.ui.UpdateHealth(health);
    }
    public override void OnFixedUpdate()
    {

        if (CurrentState == State.IDLE && sprite.IsStopped)
        {
            float rand = RXRandom.Float();
            if (rand < .6f)
                idleAnim = 1;
            else if (rand < .8f)
            {

                idleAnim = 2;
                world.SpawnWords("...", this.GetPosition(), Color.white);
            }
            else
            {
                idleAnim = 3;
            }

        }
        switch (CurrentState)
        {
            case State.IDLE:
            case State.MOVING:
                if (C.getKey(C.UP_KEY))
                {
                    yAcc = moveSpeed;
                }
                else if (C.getKey(C.DOWN_KEY))
                    yAcc = -moveSpeed;
                if (C.getKey(C.RIGHT_KEY))
                    xAcc = moveSpeed;
                else if (C.getKey(C.LEFT_KEY))
                    xAcc = -moveSpeed;

                if (Input.GetMouseButton(0))
                {
                    CurrentState = State.ATTACKING;
                    FSoundManager.PlaySound("attack");
                    Go.to(this, .1f, new TweenConfig().floatProp("isoHeight", 4).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                    PlayAnim(true);
                    return;
                }
                if (CurrentState == State.IDLE && (xAcc != 0 || yAcc != 0))
                {
                    CurrentState = State.MOVING;
                }
                else if (xAcc == 0 && yAcc == 0)
                    CurrentState = State.IDLE;


                if (xAcc == 0)
                    xVel *= .8f;
                if (yAcc == 0)
                    yVel *= .8f;

                break;
            case State.ATTACKING:
                float attackMoveSpeed = .1f;
                if (C.getKey(C.UP_KEY))
                    yAcc = attackMoveSpeed;
                else if (C.getKey(C.DOWN_KEY))
                    yAcc = -attackMoveSpeed;
                if (C.getKey(C.RIGHT_KEY))
                    xAcc = attackMoveSpeed;
                else if (C.getKey(C.LEFT_KEY))
                    xAcc = -attackMoveSpeed;

                if (!hasAttacked && stateCount > .05f)
                {
                    hasAttacked = true;
                    Attack();

                }
                if (stateCount > .2f)
                    CurrentState = State.IDLE;

                if (xAcc == 0)
                    xVel *= .8f;
                if (yAcc == 0)
                    yVel *= .8f;
                break;
            case State.TAKE_DAMAGE:
                if (stateCount > .7f)
                {
                    if (health <= 0)
                    {
                        SpawnDeathParticles();
                        xAcc = 0;
                        yAcc = 0;
                        xVel = 0;
                        yVel = 0;
                        invulnerableCount = 0;
                        ResetMaxVel();
                        CurrentState = State.DYING;
                        FSoundManager.PlaySound("playerDeath");
                    }
                    else
                    {

                        ResetMaxVel();
                        CurrentState = State.IDLE;
                    }
                }
                xVel *= .9f;
                yVel *= .9f;
                break;
            case State.DYING:
                if (stateCount > .3f)
                    world.PlayerDied();
                break;
        }
        base.OnFixedUpdate();
        PlayAnim();
        lastAttackPress = Input.GetMouseButton(0);
        this.isVisible = invulnerableCount * 1000 % 50 < 30;
    }


    private void SpawnDeathParticles(int numParticles = 30)
    {
        for (int i = 0; i < numParticles; i++)
        {

            Particle p = Particle.ParticleOne.getParticle(1.0f);

            p.activate(this.GetPosition() + new Vector2(hitBox.x + RXRandom.Float() * hitBox.width - hitBox.width / 2f, hitBox.y + RXRandom.Float() * hitBox.height - hitBox.height / 2f),
                new Vector2(RXRandom.Float() * 30 - 15, RXRandom.Float() * 20 + 20), new Vector2(0, 20), 0);
            world.AddChild(p);
        }

    }

    bool hasAttacked = false;

    public void Attack()
    {
        Vector2 dirVect = new Vector2(Input.mousePosition.x - (x - C.getCameraInstance().x) * Futile.displayScale - Futile.screen.pixelWidth / 2f, Input.mousePosition.y - (this.y - C.getCameraInstance().y) * Futile.displayScale - Futile.screen.pixelHeight / 2f).normalized;
        Direction d = Direction.UP;
        if (Mathf.Abs(dirVect.x) > Mathf.Abs(dirVect.y))
        {
            if (dirVect.x < 0)
                d = Direction.LEFT;
            else
                d = Direction.RIGHT;
        }
        else if (dirVect.y < 0)
            d = Direction.DOWN;
        else
            d = Direction.UP;

        float dist = 30;
        Attack attack;
        Attack attack2;
        Attack attack3;
        Attack attack4;
        switch (currentAttack)
        {
            case AttackType.NORMAL:

                attack = new Attack(world, d);
                attack.SetDirection(d);
                attack.SetPosition(this.GetPosition() + dirVect * 5);
                Go.to(attack, .35f, new TweenConfig().floatProp("x", dirVect.x * dist, true).floatProp("y", dirVect.y * dist, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack); }));
                world.AddObject(attack);
                break;
            case AttackType.WIDE:

                attack = new Attack(world, d);
                attack.SetPosition(this.GetPosition() + dirVect * 5);
                float angle = Mathf.Atan2(dirVect.y, dirVect.x);
                dirVect.x = Mathf.Cos(angle - Mathf.PI / 7f);
                dirVect.y = Mathf.Sin(angle - Mathf.PI / 7f);
                Go.to(attack, .3f, new TweenConfig().floatProp("x", dirVect.x * dist, true).floatProp("y", dirVect.y * dist, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack); }));
                world.AddObject(attack);

                attack2 = new Attack(world, d);
                world.AddObject(attack2);
                Vector2 secondAttackDir = new Vector2(Mathf.Cos(angle + Mathf.PI / 7f), Mathf.Sin(angle + Mathf.PI / 7f));
                attack2.SetPosition(this.GetPosition() + secondAttackDir * 5);
                Go.to(attack2, .3f, new TweenConfig().floatProp("x", secondAttackDir.x * dist, true).floatProp("y", secondAttackDir.y * dist, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack2); }));

                break;
            case AttackType.WIDE_TWO:
                attack = new Attack(world, d);
                attack.SetPosition(this.GetPosition() + dirVect * 5);
                angle = Mathf.Atan2(dirVect.y, dirVect.x);
                dirVect.x = Mathf.Cos(angle - Mathf.PI / 10f);
                dirVect.y = Mathf.Sin(angle - Mathf.PI / 10f);
                Go.to(attack, .3f, new TweenConfig().floatProp("x", dirVect.x * dist * 1.5f, true).floatProp("y", dirVect.y * dist * 2f, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack); }));
                world.AddObject(attack);

                attack2 = new Attack(world, d);
                world.AddObject(attack2);
                secondAttackDir = new Vector2(Mathf.Cos(angle + Mathf.PI / 10f), Mathf.Sin(angle + Mathf.PI / 10f));
                attack2.SetPosition(this.GetPosition() + secondAttackDir * 5);
                Go.to(attack2, .3f, new TweenConfig().floatProp("x", secondAttackDir.x * dist * 1.5f, true).floatProp("y", secondAttackDir.y * dist * 2f, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack2); }));

                attack3 = new Attack(world, d);
                secondAttackDir = new Vector2(Mathf.Cos(angle - Mathf.PI / 4f), Mathf.Sin(angle - Mathf.PI / 4f));
                attack3.SetPosition(this.GetPosition() + secondAttackDir * 5);
                Go.to(attack3, .3f, new TweenConfig().floatProp("x", secondAttackDir.x * dist, true).floatProp("y", secondAttackDir.y * dist , true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack3); }));
                world.AddObject(attack3);

                attack4 = new Attack(world, d);
                world.AddObject(attack4);
                secondAttackDir = new Vector2(Mathf.Cos(angle + Mathf.PI / 4f), Mathf.Sin(angle + Mathf.PI / 4f));
                attack4.SetPosition(this.GetPosition() + secondAttackDir * 5);
                Go.to(attack4, .3f, new TweenConfig().floatProp("x", secondAttackDir.x * dist, true).floatProp("y", secondAttackDir.y * dist, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(attack4); }));


                break;
            case AttackType.WIDE_360:

                angle = Mathf.Atan2(dirVect.y, dirVect.x);
                for (int i = 0; i < 8; i++)
                {
                    Direction attackDir = Direction.UP;
                    if (Mathf.Abs(dirVect.x) > Mathf.Abs(dirVect.y))
                    {
                        if (dirVect.x < 0)
                            attackDir = Direction.LEFT;
                        else
                            attackDir = Direction.RIGHT;
                    }
                    else if (dirVect.y < 0)
                        attackDir = Direction.DOWN;
                    else
                        attackDir = Direction.UP;
                    Attack tempAttackWide = new Attack(world, attackDir);
                    Vector2 newDirVect = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    tempAttackWide.SetPosition(this.GetPosition() + newDirVect * 5);
                    tempAttackWide.alpha = 0;
                    Go.to(tempAttackWide, (i + 1) * .03f, new TweenConfig().floatProp("alpha", 1).onComplete(() =>
                        {
                            Go.to(tempAttackWide, .3f, new TweenConfig().floatProp("x", newDirVect.x * dist, true).floatProp("y", newDirVect.y * dist, true).setEaseType(EaseType.QuadOut).onComplete(() => { world.RemoveObject(tempAttackWide); }));
                            world.AddObject(tempAttackWide);
                        }));
                    angle += Mathf.PI / 4;
                }


                break;
        }
    }

    public void TakeDamage(Vector2 vel)
    {
        if (CurrentState == State.DYING)
            return;
        FSoundManager.PlaySound("playerHurt");
        Go.killAllTweensWithTarget(this);
        isoHeight = 0;
        Go.to(this, .2f, new TweenConfig().floatProp("isoHeight", 8, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
        health--;
        world.ui.UpdateHealth(health);
        CurrentState = State.TAKE_DAMAGE;
        invulnerableCount = 3;
        maxXVel = 2;
        maxYVel = 2;
        minYVel = -2;
        xVel = vel.x;
        yVel = vel.y;
        C.getCameraInstance().shake(.7f, .3f);
    }
    public void PlayAnim(bool forced = false)
    {
        string anim = CurrentDirection.ToString() + CurrentState.ToString();
        if (CurrentDirection == Direction.DOWN && CurrentState == State.IDLE)
            anim += idleAnim.ToString();
        sprite.play(anim, forced);
    }
}

