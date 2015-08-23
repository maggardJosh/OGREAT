using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnemySpawn
{
    Vector2 pos;
    
    public EnemySpawn(Vector2 pos)
    {
        this.pos = pos;
    }
    public void SpawnEnemy(FutilePlatformerBaseObject e)
    {
        e.SetPosition(pos);
    }
}

