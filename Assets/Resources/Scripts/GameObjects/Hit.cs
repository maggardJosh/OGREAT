using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Hit : FAnimatedSprite
{
    public Hit()
        : base("hit")
    {
        this.addAnimation(new FAnimation("active", new int[] { 1, 2, 3, 4 }, 50, false));
        this.rotation = RXRandom.Bool() ? 90f : 0;
        this.play("active");
    }
    public override void Update()
    {
        if (this.IsStopped)
            this.RemoveFromContainer();
        base.Update();
    }
}

