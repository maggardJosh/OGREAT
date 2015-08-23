using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FutileFourDirectionBaseObject : FutilePlatformerBaseObject
{
    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }
    protected Direction _direction = Direction.RIGHT;
    public Direction CurrentDirection { get { return _direction; } }
    public void SetDirection(Direction newDirection)
    {
        _direction = newDirection;
    }

    public FutileFourDirectionBaseObject(RXRect hitBox, World world, string shadow = ""): base(hitBox, world, shadow)
    {

    }

    public override void OnFixedUpdate()
    {
        if(Mathf.Abs(yAcc) > Mathf.Abs(xAcc))
        {
        
        if (yAcc > 0)
            _direction = Direction.UP;
        else if (yAcc < 0)
            _direction = Direction.DOWN;
        }
        else if (xAcc > 0)
            _direction = Direction.RIGHT;
        else if (xAcc < 0)
            _direction = Direction.LEFT;
        base.OnFixedUpdate();
        if (_direction == Direction.UP || _direction == Direction.DOWN)
            scaleX = 1;
    }

    public static Vector2 GetUnitVectorDirection(Direction d)
    {
        switch(d)
        {
            case Direction.UP: return Vector2.up;
            case Direction.RIGHT: return Vector2.right;
            case Direction.DOWN: return -Vector2.up;
            case Direction.LEFT: return -Vector2.right;
            default: return Vector2.zero;
        }
    }
}
