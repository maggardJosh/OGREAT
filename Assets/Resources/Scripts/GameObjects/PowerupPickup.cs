using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PowerupPickup : FutilePlatformerBaseObject
{
    FSprite sprite;
    Player.AttackType pickupType = Player.AttackType.NORMAL;
    private float _isoHeight = 0;
    public float isoHeight { get { return _isoHeight; } set { _isoHeight = value; sprite.y = _isoHeight; } }
    public PowerupPickup(World world,Vector2 pos, Player.AttackType pickupType)
        : base(new RXRect(0, -3, 10, 7), world, "enemy1shadow")
    {
        this.SetPosition(pos);
        this.blocksOtherObjects = false;
        this.pickupType = pickupType;
        sprite = new FSprite(this.pickupType.ToString().ToLower() + "poweruppickup");
        this.AddChild(sprite);
        Go.to(this, 1.0f, new TweenConfig().floatProp("isoHeight", 5).setEaseType(EaseType.QuadOut).setIterations(-1, LoopType.PingPong));
    }

    public override void OnFixedUpdate()
    {
        if (this.isColliding(world.p))
        {
            world.p.Pickup(pickupType);
            world.RemoveObject(this);
        }
        base.OnFixedUpdate();
    }
}
