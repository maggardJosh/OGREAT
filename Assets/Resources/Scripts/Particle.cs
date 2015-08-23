using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Particle : FAnimatedSprite
{
    Vector2 vel;
    float rot;
    Vector2 accel;
    public bool isActive = false;
    int animRandom = 180;
    int animBaseSpeed = 100;
    protected enum DeathType
    {
        ANIM,
        ALPHA
    }
    protected DeathType deathType = DeathType.ANIM;
    private Particle(string elementName)
        : base(elementName)
    {
        this.isVisible = false;
    }

    public void activate(Vector2 pos, Vector2 vel, Vector2 accel, float rot)
    {
        this.ignoreTransitioning = false;
        this.isVisible = true;
        this.isActive = true;
        this.vel = vel;
        this.SetPosition(pos);
        this.rot = rot;
        this.accel = accel;
        this.play("active", true);
    }

    public override void Update()
    {
        if (!ignoreTransitioning && C.isTransitioning)
            return;
        this.x += vel.x * Time.deltaTime;
        this.y += vel.y * Time.deltaTime;
        vel += accel * Time.deltaTime;

        this.rotation = rot;
        //this.rotation += rot * Time.deltaTime;
        if(this.deathType == DeathType.ANIM && this.IsStopped)
        {
            this.RemoveFromContainer();
            this.isActive = false;
        }

        base.Update();
    }
    public class ParticleOne : Particle
    {

        private static ParticleOne[] particleList;
        const int MAX_PARTICLES = 100;
        public static ParticleOne getParticle(float lifeTime = 1f)
        {
            if (particleList == null)
                particleList = new ParticleOne[MAX_PARTICLES];
            ParticleOne result = particleList[RXRandom.Int(MAX_PARTICLES)];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    ParticleOne p = new ParticleOne();
                    particleList[x] = p;
                    result = p;
                    break;
                }
                else if (!particleList[x].isActive)
                {
                    result = particleList[x];
                    result.RemoveFromContainer();
                }
            }
            result.color = Color.white;
            result.alpha = 1;
            Go.killAllTweensWithTarget(result);
            Go.to(result, lifeTime, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadIn).onComplete(() => { result.RemoveFromContainer(); result.isActive = false; }));
            result.play("active");
            return result;

        }

        private ParticleOne()
            : base("whiteParticle")
        {
            this.deathType = DeathType.ALPHA;
            this.addAnimation(new FAnimation("active", new int[] { 1 }, animBaseSpeed + (int)(RXRandom.Float() * animRandom), false));
        }
    }



    public class ParticleTwo : Particle
    {

        private static ParticleTwo[] particleList;
        const int MAX_PARTICLES = 100;
        public static ParticleTwo getParticle(float lifeTime = 1f)
        {
            if (particleList == null)
                particleList = new ParticleTwo[MAX_PARTICLES];
            ParticleTwo result = particleList[RXRandom.Int(MAX_PARTICLES)];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    ParticleTwo p = new ParticleTwo();
                    particleList[x] = p;
                    result = p;
                    break;
                }
                else if (!particleList[x].isActive)
                {
                    result = particleList[x];
                    result.RemoveFromContainer();
                }
            }
            result.color = Color.white;
            result.alpha = 1;
            Go.killAllTweensWithTarget(result);
            Go.to(result, lifeTime, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadIn).onComplete(() => { result.RemoveFromContainer(); result.isActive = false; }));
            result.play("active");
            return result;

        }

        private ParticleTwo()
            : base("whiteParticle")
        {
            this.deathType = DeathType.ALPHA;
            this.addAnimation(new FAnimation("active", new int[] { 1 }, animBaseSpeed + (int)(RXRandom.Float() * animRandom), false));
        }
    }

}

