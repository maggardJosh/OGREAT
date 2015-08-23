using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Attack : FutileFourDirectionBaseObject
{
    FAnimatedSprite sprite;
    List<FutilePlatformerBaseObject> hits = new List<FutilePlatformerBaseObject>();
    public Attack(World world,Direction d)
        : base(new RXRect(0, 0, 16, 16), world)
    {
        this._direction = d;
        this.blocksOtherObjects = false;
        sprite = new FAnimatedSprite("attack");
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString(), new int[] { 1, 2, 3, 4, 5, 6, 7 }, 50, false));
        this.AddChild(sprite);
        PlayAnim();
    }
    
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        PlayAnim();
        
    }

    public void CheckObjectCollision(List<FutilePlatformerBaseObject> objects)
    {
        foreach (FutilePlatformerBaseObject o in objects)
        {
            if (o is Enemy)
            {
                Enemy e = (Enemy)o;
                if (!hits.Contains(e) && e.isColliding(this))
                {
                    hits.Add(e);
                    e.TakeDamage(GetUnitVectorDirection(CurrentDirection) * 2);
                    Hit h = new Hit();
                    h.SetPosition((this.GetPosition() + e.GetPosition()) / 2f);
                    world.AddForegroundSprite(h);
                }
            }
            if (o is Ninja)
            {
                Ninja n = (Ninja)o;
                RXDebug.Log(n.isRetreating, n.isoHeight);
                if (!n.isRetreating && !hits.Contains(n) && n.isoHeight < 6 && n.isColliding(this))
                {
                    hits.Add(n);
                    n.TakeDamage(GetUnitVectorDirection(CurrentDirection) * 2);
                    Hit h = new Hit();
                    h.SetPosition((this.GetPosition() + n.GetPosition()) / 2f);
                    world.AddForegroundSprite(h);
                }

            }
        }
    }

    public void PlayAnim()
    {
        switch (_direction)
        {
            case Direction.DOWN: 
                sprite.rotation = 0;
                hitBox.x = 0;
                hitBox.y = -3;
                hitBox.width = 14;
                hitBox.height = 5;
                break;
            case Direction.UP: sprite.rotation = 180;
                 hitBox.x = 0;
                hitBox.y = 3;
                hitBox.width = 14;
                hitBox.height = 5;
                break;
            case Direction.RIGHT: sprite.rotation = -90;
                 hitBox.x = 3;
                hitBox.y = 0;
                hitBox.width = 5;
                hitBox.height = 14;
                break;
            case Direction.LEFT: sprite.rotation = 90;
                hitBox.x = -3;
                hitBox.y = 0;
                hitBox.width = 5;
                hitBox.height = 14;
                break;
        }
        
        UpdateHitBoxSprite();
        sprite.play("attack");
    }
}

