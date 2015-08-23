using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Enemy : FutileFourDirectionBaseObject
{
    FAnimatedSprite sprite;
    public enum State
    {
        SPAWN,
        IDLE,
        MOVING,
        START_ATTACK,
        ATTACKING,
        TAKE_DAMAGE,
        BOUNCE
    }
    int health = 3;
    State _state = State.IDLE;
    public State currentState
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                switch (value)
                {
                    case State.START_ATTACK:
                        FSoundManager.PlaySound("notice");
                        break;
                    case State.ATTACKING:
                        
                        break;
                }
                _state = value;
                stateCount = 0;
            }
        }
    }

    private float _isoHeight = 0;
    public float isoHeight { get { return _isoHeight; } set { _isoHeight = value; sprite.y = _isoHeight; } }

    public Enemy(World world, int type)
        : base(new RXRect(-1, -3, 6, 5), world, "enemy" + type.ToString() + "shadow")
    {
        currentState = State.SPAWN;
        switch (type)
        {
            case 1:
                hitBox.x = -1;
                hitBox.y = -3;
                hitBox.width = 6;
                hitBox.height = 5;
                break;
            case 2:
                hitBox.x = -1;
                hitBox.y = -5;
                hitBox.width = 9;
                hitBox.height = 6;
                break;
        }
        UpdateHitBoxSprite();
        stateCount = RXRandom.Float() * minStateCount;
        handleStateCount = true;
        useActualMaxVel = true;
        maxVel = 1;
        switch (type)
        {
            case 1:
                health = 1;
                break;
            case 2:
                health = 3;
                break;
        }
        sprite = new FAnimatedSprite("enemy" + type.ToString());
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.IDLE, new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.IDLE, new int[] { 5 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.IDLE, new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.IDLE, new int[] { 1 }, 150, false));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.BOUNCE, new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.BOUNCE, new int[] { 5 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.BOUNCE, new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.BOUNCE, new int[] { 1 }, 150, false));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.MOVING, new int[] { 1, 2, 1, 3 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.MOVING, new int[] { 5, 6, 5, 7 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.MOVING, new int[] { 1, 2, 1, 3 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.MOVING, new int[] { 1, 2, 1, 3 }, 150, true));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.SPAWN, new int[] { 1, 2, 1, 3 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.SPAWN, new int[] { 5, 6, 5, 7 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.SPAWN, new int[] { 1, 2, 1, 3 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.SPAWN, new int[] { 1, 2, 1, 3 }, 150, true));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.START_ATTACK, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.START_ATTACK, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.START_ATTACK, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.START_ATTACK, new int[] { 9 }, 150, true));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.ATTACKING, new int[] { 9, 10, 9, 11 }, 100, true));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.ATTACKING, new int[] { 5, 6, 5, 7 }, 100, true));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.ATTACKING, new int[] { 9, 10, 9, 11 }, 100, true));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.ATTACKING, new int[] { 9, 10, 9, 11 }, 100, true));


        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.TAKE_DAMAGE, new int[] { 4 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.TAKE_DAMAGE, new int[] { 8 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.TAKE_DAMAGE, new int[] { 4 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.TAKE_DAMAGE, new int[] { 4 }, 150, false));
        this.AddChild(sprite);
    }
    float minStateCount = 2.0f;
    public override void OnFixedUpdate()
    {
        switch (currentState)
        {
            case State.SPAWN:
                return;
            case State.IDLE:
            case State.MOVING:
                if (this.x < 0)
                    this.x = +hitBox.width / 2f;
                if (this.x > C.getCameraInstance().getWorldBounds().width)
                    this.x = C.getCameraInstance().getWorldBounds().width - hitBox.width;
                if (this.y + hitBox.y > 0)
                    this.y = hitBox.y-hitBox.width / 2f;
                if (this.y +hitBox.y < -C.getCameraInstance().getWorldBounds().height)
                    this.y = hitBox.y-C.getCameraInstance().getWorldBounds().height + hitBox.height;


                this.isVisible = true;
                if (HitSomething)
                {
                    if (hitLeft || hitRight)
                        xAcc *= -1;
                    if (hitUp || hitDown)
                        yAcc *= -1;
                }
                if (stateCount > minStateCount)
                {
                    if (RXRandom.Float() < .1f)
                    {
                        if (currentState == State.IDLE)
                        {
                            currentState = State.MOVING;
                            float randAngle = RXRandom.Float() * Mathf.PI * 2;
                            if (RXRandom.Float() < .5f)
                            {
                                Vector2 diff = world.p.GetPosition() - this.GetPosition();
                                randAngle = Mathf.Atan2(diff.y, diff.x);
                            }
                            xAcc = Mathf.Cos(randAngle) * maxVel * .3f;
                            yAcc = Mathf.Sin(randAngle) * maxVel * .3f;
                            clearAcc = false;
                            maxVel = .5f;
                        }
                        else
                        {
                            currentState = State.IDLE;
                            clearAcc = true;

                        }
                    }
                }
                break;
            case State.ATTACKING:
                if (RXRandom.Float() < .1f)
                    SpawnParticles();

                if (stateCount > 1.0f)
                {
                    currentState = State.IDLE;
                    xAcc = 0;
                    yAcc = 0;
                    xVel = 0;
                    yVel = 0;
                }
                break;
            case State.START_ATTACK:
                x += RXRandom.Float() - .5f;
                if (stateCount > .7f)
                {
                    currentState = State.ATTACKING;
                    maxVel = 1;
                    clearAcc = false;
                    Vector2 diff = (world.p.GetPosition() - this.GetPosition());
                    diff.Normalize();
                    xAcc = diff.x * 1;
                    yAcc = diff.y * 1;
                    SpawnParticles(10);
                }
                break;
            case State.TAKE_DAMAGE:
                this.isVisible = stateCount * 1000 % 50 < 30;
                if (stateCount > .8f)
                {
                    if (this.health <= 0)
                    {
                        die();
                        return;
                    }
                    this.collidesWithOtherObjects = true;
                    this.blocksOtherObjects = true;
                    currentState = State.IDLE;
                    bounceiness = .8f;
                    maxVel = 1;
                }
                break;
            case State.BOUNCE:
                this.isVisible = true;
                if (stateCount > .5f)
                {

                    this.collidesWithOtherObjects = true;
                    this.blocksOtherObjects = true;
                    currentState = State.IDLE;
                    bounceiness = .8f;
                    maxVel = 1;
                }
                break;
        }
        CheckPlayerInSight();
        xVel *= .9f;
        yVel *= .9f;
        base.OnFixedUpdate();
        if (currentState == State.MOVING)
            if (yVel > 0)
                _direction = Direction.UP;
            else if (yVel < 0)
                _direction = Direction.DOWN;
        PlayAnim();
    }

    private void die()
    {
        FSoundManager.PlaySound("enemyDeath");
        if (++world.killCount == World.POWERUP_KILLS)
        {
            world.AddObject(new PowerupPickup(world, this.GetPosition(), Player.AttackType.NORMAL));

        }
        SpawnDeathParticles(20);
        world.RemoveObject(this);
    }

    private const float SEE_DIST_SQUARED = (16 * 3) * (16 * 3);
    private void CheckPlayerInSight()
    {
        if (stateCount > .5f)
        {

            Vector2 diff = (world.p.GetPosition() - this.GetPosition());
            if (diff.SqrMagnitude() <= SEE_DIST_SQUARED)
            {
                if (currentState == State.MOVING || currentState == State.IDLE)
                {
                    xVel = 0;
                    yVel = 0;
                    xAcc = 0;
                    yAcc = 0;
                    currentState = State.START_ATTACK;
                    Go.killAllTweensWithTarget(this);
                    isoHeight = 0;

                    TrySpawnWords(meanWords);
                    Go.to(this, .1f, new TweenConfig().floatProp("isoHeight", 5, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                }
            }
        }
    }
    
    private string[] victoryWords = { "AHA!", "GOT YOU!", "TAKE THAT!", "TOO EASY!", "HAHAHA!" };
    private string[] hurtWords = { "OUCH!", "OWWWW!", "HELP!", "BWAHH!", "ARGH!", "UGH" };
    private string[] meanWords = { "GET EM!", "MONSTER!", "DIE!", "GROSS!", "BWAHH!", "WHOA!", "EWWW!", "OGRE!", "GET BACK!" };
    private void TrySpawnWords(string[] wordArray, float chance= .2f)
    {
        if (RXRandom.Float() < chance)
        {
            world.SpawnWords(wordArray[RXRandom.Int(wordArray.Length)], this.GetPosition(), new Color(.7f, 0, 0));
        }
    }

    public void PlayAnim()
    {
        sprite.play(CurrentDirection.ToString() + currentState);
    }

    public void CheckPlayerCollision(Player p)
    {
        if ((this.currentState != State.BOUNCE && this.currentState != State.TAKE_DAMAGE))
        {

            if (p.invulnerableCount <= 0 && p.isColliding(this))
            {

                p.TakeDamage(new Vector2(xVel * 3, yVel * 3));
                TrySpawnWords(victoryWords, p.CurrentState == Player.State.DYING ? .2f : 1);
                maxVel = 3;
                xAcc = 0;
                yAcc = 0;
                this.collidesWithOtherObjects = false;
                this.blocksOtherObjects = false;
                this.xVel *= -2;
                this.yVel *= -2;
                currentState = State.BOUNCE;
            }
            else
            {
                if (p.isColliding(this))
                {
                    maxVel = 3;
                    xAcc = 0;
                    yAcc = 0;
                    this.collidesWithOtherObjects = false;
                    this.blocksOtherObjects = false;
                    Vector2 diff = (p.GetPosition() - this.GetPosition()).normalized;
                    this.xVel = -diff.x * 2;
                    this.yVel = -diff.y * 2;
                    Go.killAllTweensWithTarget(this);
                    shadow.y = 0;
                    Go.to(this, .1f, new TweenConfig().floatProp("isoHeight", 5, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                    currentState = State.BOUNCE;
                }
            }
        }
    }

    private void SpawnParticles(int numParticles = 1)
    {
        for (int i = 0; i < numParticles; i++)
        {

            Particle p = Particle.ParticleOne.getParticle(1.0f);

            p.activate(this.GetPosition() + new Vector2(hitBox.x + RXRandom.Float() * hitBox.width - hitBox.width / 2f, hitBox.y + RXRandom.Float() * -hitBox.height / 2f),
                new Vector2(-xVel * 10, -yVel * 10), new Vector2(0, 20), 0);
            world.AddChild(p);
        }

    }

    private void SpawnDeathParticles(int numParticles = 1)
    {
        for (int i = 0; i < numParticles; i++)
        {

            Particle p = Particle.ParticleOne.getParticle(1.0f);

            p.activate(this.GetPosition() + new Vector2(hitBox.x + RXRandom.Float() * hitBox.width - hitBox.width / 2f, hitBox.y + RXRandom.Float() * hitBox.height - hitBox.height / 2f),
                new Vector2(-xVel * 10, -yVel * 10), new Vector2(0, 20), 0);
            world.AddChild(p);
        }

    }
    public void TakeDamage(Vector2 vel)
    {
        FSoundManager.PlaySound("enemyHurt");
        TrySpawnWords(hurtWords);
        Go.killAllTweensWithTarget(this);
        this.shadow.y = 0;
        Go.to(this, .1f, new TweenConfig().floatProp("isoHeight", 5, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
        maxVel = 3;
        this.health--;
        xAcc = 0;
        yAcc = 0;
        this.collidesWithOtherObjects = false;
        this.blocksOtherObjects = false;
        this.xVel = vel.x;
        this.yVel = vel.y;
        this.bounceiness = -.8f;
        currentState = State.TAKE_DAMAGE;

    }

}
