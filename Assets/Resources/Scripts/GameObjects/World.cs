using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    FTmxMap map;
    FTmxMap loadMap;

    List<FutilePlatformerBaseObject> objects = new List<FutilePlatformerBaseObject>();
    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    List<EnemySpawn> enemySpawns = new List<EnemySpawn>();

    public UI ui;

    FTilemap wallCollisionTilemap;
    FTilemap collisionTilemap;
    FTilemap backgroundTilemap;

    FTilemap foregroundTilemap;
    FContainer backgroundLayer = new FContainer();
    FContainer foregroundLayer = new FContainer();
    FContainer playerLayer = new FContainer();
    public static int POWERUP_KILLS = 100;
    public int killCount = 0;
    public const int MAX_ENEMIES_AT_ONCE = 100;

    public enum State
    {
        TRANS,
        IN_GAME
    }

    public State currentState = State.TRANS;

    public enum EnemyTypes
    {
        NORMAL,
        BIG,
        NINJA
    }

    public Player p;

    public float gravity = 0;
    Dictionary<EnemyTypes, int> spawnCounts = new Dictionary<EnemyTypes, int>();
    int wave = 0;

    public float TileSize
    {
        get
        {
            if (collisionTilemap != null)
                return Mathf.Max(collisionTilemap.tileWidth, collisionTilemap.tileHeight);
            else
                return 0;
        }
    }
    public World()
    {
        loadMap = new FTmxMap();
        loadMap.LoadTMX("Maps/loadMap");
        FTilemap loadMapBG = ((FTilemap)loadMap.getLayerNamed("background"));
        loadMapBG.clipNode = C.getCameraInstance();
        loadMapBG.repeatX = true;
        loadMapBG.repeatY = true;
        this.AddChild(loadMap);
        this.AddChild(backgroundLayer);
        this.AddChild(playerLayer);
        this.AddChild(foregroundLayer);
        playerLayer.shouldSortByZ = true;
        Futile.instance.SignalUpdate += OnUpdate;
    }

    private string GetWaveMap()
    {
        switch (wave)
        {
            case 1:
            case 2:
            case 3:
                return "mapOne";
            case 4:
            case 5:
            case 6:
                return "mapTwo";
            case 7:
            case 8:
            case 9:
                return "mapThree";
            case 10:
            case 11:
            case 12:
                return "mapFour";
            case 13:
            case 14:
            default:
                return "mapFive";
        }
    }
    private void GetEnemyCounts()
    {
        switch (wave)
        {
            case 0: //Title screen
                spawnCounts[EnemyTypes.NORMAL] = 50;
                spawnCounts[EnemyTypes.BIG] = 20;
                spawnCounts[EnemyTypes.NINJA] = 5;
                break;
            case 1:
                spawnCounts[EnemyTypes.NORMAL] = 10;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 0;
                break;
            case 2: //10
                spawnCounts[EnemyTypes.NORMAL] = 40;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 0;
                break;
            case 3://50
                spawnCounts[EnemyTypes.NORMAL] = 0;
                spawnCounts[EnemyTypes.BIG] = 5;
                spawnCounts[EnemyTypes.NINJA] = 0;
                break;
            case 4: //55
                spawnCounts[EnemyTypes.NORMAL] = 20;
                spawnCounts[EnemyTypes.BIG] = 10;
                spawnCounts[EnemyTypes.NINJA] = 0;
                break;
            case 5: //85
                spawnCounts[EnemyTypes.NORMAL] = 155;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 0;
                break;
            case 6: //240   (Wide)
                spawnCounts[EnemyTypes.NORMAL] = 80;
                spawnCounts[EnemyTypes.BIG] = 30;
                spawnCounts[EnemyTypes.NINJA] = 0;
                break;
            case 7://350
                spawnCounts[EnemyTypes.NORMAL] = 0;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 5;
                break;
            case 8://355
                spawnCounts[EnemyTypes.NORMAL] = 130;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 15;
                break;
            case 9://500
                spawnCounts[EnemyTypes.NORMAL] = 200;
                spawnCounts[EnemyTypes.BIG] = 50;
                spawnCounts[EnemyTypes.NINJA] = 10;
                break;
            case 10: //760
                spawnCounts[EnemyTypes.NORMAL] = 0;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 50;
                break;
            case 11:  //810
                spawnCounts[EnemyTypes.NORMAL] = 260;
                spawnCounts[EnemyTypes.BIG] = 0;
                spawnCounts[EnemyTypes.NINJA] = 50;
                break;
            case 12://1120
                spawnCounts[EnemyTypes.NORMAL] = 300;
                spawnCounts[EnemyTypes.BIG] = 100;
                spawnCounts[EnemyTypes.NINJA] = 50;
                break;
            case 13: //1570
                spawnCounts[EnemyTypes.NORMAL] = 100;
                spawnCounts[EnemyTypes.BIG] = 100;
                spawnCounts[EnemyTypes.NINJA] = 75;
                break;
            case 14: //1925 
                spawnCounts[EnemyTypes.NORMAL] = 500;
                spawnCounts[EnemyTypes.BIG] = 300;
                spawnCounts[EnemyTypes.NINJA] = 200;
                break;
            default:    // > 2000 (four side)
                spawnCounts[EnemyTypes.NORMAL] = int.MaxValue;
                spawnCounts[EnemyTypes.BIG] = int.MaxValue;
                spawnCounts[EnemyTypes.NINJA] = int.MaxValue;
                break;
        }
    }


    public void UpdatePowerupKills()
    {
        switch (POWERUP_KILLS)
        {
            case 100:   //Wide
                POWERUP_KILLS = 550;
                break;
            case 550:  //Front and back
                POWERUP_KILLS = 1500;
                break;
            case 1500:
                POWERUP_KILLS = int.MaxValue;
                break;
        }
    }


    public void LoadMap(string mapName)
    {
        GetEnemyCounts();
        map = new FTmxMap();
        objects.Clear();
        spawnPoints.Clear();
        enemySpawns.Clear();
        backgroundLayer.RemoveAllChildren();
        foregroundLayer.RemoveAllChildren();
        playerLayer.RemoveAllChildren();
        map.LoadTMX("Maps/" + mapName);
        backgroundTilemap = (FTilemap)map.getLayerNamed("background");
        collisionTilemap = (FTilemap)map.getLayerNamed("collision");
        foregroundTilemap = (FTilemap)map.getLayerNamed("foreground");

        backgroundTilemap.clipNode = C.getCameraInstance();
        collisionTilemap.clipNode = C.getCameraInstance();
        foregroundTilemap.clipNode = C.getCameraInstance();

        if (p == null)
        {
            p = new Player(this);
            ui = new UI(this);
            C.getCameraInstance().AddChild(ui);
        }

        backgroundLayer.AddChild(backgroundTilemap);
        backgroundLayer.AddChild(collisionTilemap);
        foregroundLayer.AddChild(foregroundTilemap);
        playerLayer.AddChild(p);

        MapLoader.loadObjects(this, map.objects);

        SpawnPlayer(p);

    }
    public void SpawnWords(string message, Vector2 pos, Color color)
    {
        FLabel test = new FLabel(C.smallFontName, message);
        foregroundLayer.AddChild(test);
        test.SetPosition(pos);
        test.color = color;
        test.alpha = 0;
        Go.to(test, .5f, new TweenConfig().floatProp("y", 15, true).floatProp("alpha", 1.0f).setEaseType(EaseType.QuadOut).onComplete(() =>
        {

            Go.to(test, .5f, new TweenConfig().floatProp("y", 10, true));
            Go.to(test, .5f, new TweenConfig().floatProp("alpha", 0).setDelay(.1f).setEaseType(EaseType.QuadOut).onComplete(() => { test.RemoveFromContainer(); }));
        }));
    }
    public void AddForegroundSprite(FSprite s)
    {
        foregroundLayer.AddChild(s);
    }
    public void SpawnPlayer(Player p)
    {
        if (spawnPoints.Count == 0)
            p.SetPosition(0, 0);
        else
            p.SetPosition(spawnPoints[RXRandom.Int(spawnPoints.Count)].pos);
    }
    public void addSpawn(SpawnPoint s)
    {
        spawnPoints.Add(s);
    }
    public void addEnemySpawn(EnemySpawn s)
    {
        enemySpawns.Add(s);
    }
    public void AddObject(FutilePlatformerBaseObject o)
    {
        objects.Add(o);
        playerLayer.AddChild(o);
    }

    public void RemoveObject(FutilePlatformerBaseObject o)
    {
        objects.Remove(o);
        playerLayer.RemoveChild(o);
    }
    RXRect worldPos = new RXRect();
    public RXRect CheckObjectCollision(FutilePlatformerBaseObject self, float x, float y)
    {
        foreach (FutilePlatformerBaseObject o in objects)
        {
            if (!self.collidesWithOtherObjects)
                continue;
            if (!o.blocksOtherObjects)
                continue;
            if (o == self)
                continue;
            if ((self is Player && o is Enemy) ||
                (self is Enemy && o is Player) ||
                (self is Player && o is Ninja) ||
                (self is Ninja && o is Player))
                continue;
            worldPos.x = o.x + o.hitBox.x - o.hitBox.width / 2;
            worldPos.y = o.y + o.hitBox.y - o.hitBox.height / 2;
            worldPos.width = o.hitBox.width;
            worldPos.height = o.hitBox.height;
            if (worldPos.Contains(x, y))
            {
                return worldPos;
            }
        }
        return null;
    }

    public bool isAllPassable(FutilePlatformerBaseObject self, float x, float y, bool isMovingHorizontal = true)
    {

        float xPos = x + self.hitBox.x + (isMovingHorizontal ? 0 : -1);
        float yPos = y + self.hitBox.y + (isMovingHorizontal ? -1 : 0);
        float width = self.hitBox.width + (isMovingHorizontal ? 0 : -4);
        float height = self.hitBox.height + (isMovingHorizontal ? -4 : 0);
        return
            isWallPassable(xPos - width / 2f, yPos - height / 2f) &&
            isWallPassable(xPos + width / 2f, yPos - height / 2f) &&
            isWallPassable(xPos - width / 2f, yPos + height / 2f) &&
            isWallPassable(xPos + width / 2f, yPos + height / 2f) &&
            isPassable(xPos - width / 2f, yPos - height / 2f) &&
            isPassable(xPos + width / 2f, yPos - height / 2f) &&
            isPassable(xPos - width / 2f, yPos + height / 2f) &&
            isPassable(xPos + width / 2f, yPos + height / 2f);
    }
    public bool isAllPassable(float x, float y)
    {
        return isWallPassable(x, y) && isPassable(x, y);
    }
    public bool isWallPassable(float x, float y)
    {
        return true;
        //        return wallCollisionTilemap.IsPassable(x, y);
    }
    public bool isPassable(float x, float y)
    {
        return collisionTilemap.IsPassable(x, y);
    }

    public void UpdateWorldBounds()
    {

        C.getCameraInstance().setWorldBounds(new Rect(0, -collisionTilemap.height, collisionTilemap.width, collisionTilemap.height));
    }

    public void LoadNewMap(string mapName)
    {
        currentState = State.TRANS;
        C.getCameraInstance().setWorldBounds(new Rect(0, -loadMap.height, 0, 0));
        FNode followNode = new FNode();
        followNode.SetPosition(C.getCameraInstance().GetPosition());
        C.getCameraInstance().follow(followNode);

        Vector2 dir = RXRandom.Vector2Normalized();
        float dist = 600;

        followNode.alpha = 0;
        Go.to(followNode, .3f, new TweenConfig().floatProp("alpha", 1).onComplete(() => { FSoundManager.PlaySound("transition"); }));


        Go.to(followNode, 1.0f, new TweenConfig().floatProp("x", dir.x * dist, true).floatProp("y", dir.y * dist, true).setEaseType(EaseType.BackIn).onComplete(() =>
        {
            LoadMap(mapName);
            followNode.x = p.x - dir.x * dist;
            followNode.y = p.y - dir.y * dist;
            C.getCameraInstance().x = p.x - dir.x * dist;
            C.getCameraInstance().y = p.y - dir.y * dist;
            if (!ui.isVisible)
            {
                ui.y = 50;
                ui.isVisible = true;
                Go.to(ui, 2.0f, new TweenConfig().floatProp("y", 0).setEaseType(EaseType.BackOut));
            }
            Go.to(followNode, 1.0f, new TweenConfig().floatProp("x", p.x).floatProp("y", p.y).setEaseType(EaseType.BackOut).onComplete(() =>
            {
                UpdateWorldBounds();
                C.getCameraInstance().follow(p);
                p.CurrentState = Player.State.IDLE;
                inDeadMode = false;
                NextWave();
            }));
        }));

    }

    public void NextWave()
    {
        currentState = State.TRANS;
        wave++;
        GetEnemyCounts();
        string waveMessage = wave == 15 ? "INFINITE WAVE\nThanks for playing!" : "WAVE " + wave.ToString();
        FLabel waveLabel = new FLabel(C.largeFontName, waveMessage);
        waveLabel.y = Futile.screen.halfHeight * .5f;
        waveLabel.x = Futile.screen.width;
        C.getCameraInstance().AddChild(waveLabel);
        FSoundManager.PlaySound("transitionIn");
        Go.to(waveLabel, 1.0f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete(() =>
        {

            Go.to(waveLabel, .7f, new TweenConfig().onStart((t) => { FSoundManager.PlaySound("transitionIn"); }).floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).setDelay(1.0f).onComplete(() => { waveLabel.RemoveFromContainer(); currentState = State.IN_GAME; }));
        }));
    }

    public void ResetVars()
    {
        p.currentAttack = Player.AttackType.NORMAL;
        ui.UpdatePowerup(p.currentAttack);
        numEnemiesSpawned = 0;
        killCount = 0;
        POWERUP_KILLS = 100;
        wave = 0;

    }

    float lastSpawnTime = 0;
    float SPAWN_INTERVAL = 3.0f;
    int spawnPerWave = 5;
    int numEnemiesSpawned = 0;
    float count = 0;
    public void OnUpdate()
    {
        count += Time.deltaTime;
        ui.UpdateKillCount(killCount);
        if (currentState == State.TRANS)
        {
            if (count > .4f)
            {
                count = 0;
                p.GetHealth();
            }
            return;
        }
        bool noMoreSpawns = true;
        for (int enemyType = 0; enemyType < Enum.GetValues(typeof(EnemyTypes)).Length; enemyType++)
            if (spawnCounts[(EnemyTypes)enemyType] > 0)
            {
                noMoreSpawns = false;
                break;
            }
        if (noMoreSpawns)
        {
            if (numEnemiesSpawned == killCount)
            {
                wave++;
                string nextMap = GetWaveMap();
                wave--;
                if ("Maps/" + nextMap != map.actualMapName)
                    LoadNewMap(nextMap);
                else
                    NextWave();
            }
        }
        lastSpawnTime += Time.deltaTime;
        if (lastSpawnTime > SPAWN_INTERVAL)
        {
            lastSpawnTime = 0;
            for (int i = 0; i < Mathf.Min(50, spawnPerWave * (wave > 5 ? wave - 4 : 1)); i++)
            {
                if (numEnemiesSpawned - killCount > MAX_ENEMIES_AT_ONCE)
                    break;
                noMoreSpawns = true;
                for (int enemyType = 0; enemyType < Enum.GetValues(typeof(EnemyTypes)).Length; enemyType++)
                    if (spawnCounts[(EnemyTypes)enemyType] > 0)
                    {
                        noMoreSpawns = false;
                        break;
                    }
                if (noMoreSpawns)
                {
                    if (numEnemiesSpawned == killCount)
                    {
                        wave++;
                        string nextMap = GetWaveMap();
                        wave--;
                        if ("Maps/" + nextMap != map.actualMapName)
                            LoadNewMap(nextMap);
                        else
                            NextWave();
                    }
                    break;
                }

                int j = RXRandom.Int(Enum.GetValues(typeof(EnemyTypes)).Length);
                while (spawnCounts[(EnemyTypes)j] <= 0)
                    j = RXRandom.Int(Enum.GetValues(typeof(EnemyTypes)).Length);
                switch ((EnemyTypes)j)
                {
                    case EnemyTypes.NORMAL:
                        Enemy e = new Enemy(this, 1);
                        SpawnEnemy(e);
                        break;
                    case EnemyTypes.BIG:
                        Enemy e2 = new Enemy(this, 2);
                        SpawnEnemy(e2);
                        break;
                    case EnemyTypes.NINJA:
                        Ninja n = new Ninja(this);
                        SpawnEnemy(n);
                        break;
                }
                numEnemiesSpawned++;
                spawnCounts[(EnemyTypes)j]--;

            }
        }
        foreach (FutilePlatformerBaseObject o in objects)
        {
            if (o is Enemy)
            {
                ((Enemy)o).CheckPlayerCollision(p);
            }
            if (o is Attack)
            {
                ((Attack)o).CheckObjectCollision(objects);
            }
            if (o is AttackWide)
            {
                ((AttackWide)o).CheckObjectCollision(objects);
            }
            if (o is Ninja)
            {
                ((Ninja)o).CheckPlayerCollision(p);
            }
        }
    }

    private void SpawnEnemy(FutilePlatformerBaseObject enemy)
    {
        enemySpawns[RXRandom.Int(enemySpawns.Count())].SpawnEnemy(enemy);
        AddObject(enemy);
        if (enemy.y > -collisionTilemap.tileHeight)    //Top
            EnemyTransition(FutileFourDirectionBaseObject.Direction.DOWN, enemy);
        else if (enemy.y < -collisionTilemap.height + collisionTilemap.tileHeight)   //bottom
            EnemyTransition(FutileFourDirectionBaseObject.Direction.UP, enemy);
        else if (enemy.x < collisionTilemap.tileWidth)   //left
            EnemyTransition(FutileFourDirectionBaseObject.Direction.RIGHT, enemy);
        else
            EnemyTransition(FutileFourDirectionBaseObject.Direction.LEFT, enemy);
    }
    FLabel died;
    FLabel pressK;
    public bool inDeadMode = false;
    public void PlayerDied()
    {
        if (inDeadMode)
            return;
        inDeadMode = true;
        died = new FLabel(C.largeFontName, "YOU DEAD");
        C.getCameraInstance().AddChild(died);
        died.x = Futile.screen.width;
        died.y = Futile.screen.halfHeight * .5f;

        pressK = new FLabel(C.smallFontName, "CLICK TO TRY AGAIN");
        C.getCameraInstance().AddChild(pressK);
        pressK.alpha = 0;
        pressK.y = -Futile.screen.halfHeight * .5f;

        FSoundManager.PlaySound("transitionIn");

        Go.to(died, 1.0f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete(() => { Futile.instance.SignalUpdate += ListenForRestart; }));
        Go.to(pressK, .5f, new TweenConfig().floatProp("alpha", 1));

    }

    private void ListenForRestart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FSoundManager.PlaySound("menuSelect");
            Go.killAllTweensWithTarget(died);
            Go.killAllTweensWithTarget(pressK);
            Go.to(died, .5f, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete(() => { died.RemoveFromContainer(); }));
            Go.to(pressK, .5f, new TweenConfig().floatProp("alpha", 0).onComplete(() => { pressK.RemoveFromContainer(); }));
            Futile.instance.SignalUpdate -= ListenForRestart;
            ResetVars();
            LoadNewMap("mapOne");
        }
    }

    private void EnemyTransition(FutileFourDirectionBaseObject.Direction dir, FutilePlatformerBaseObject enemy)
    {
        if (enemy is Enemy)
        {
            Enemy e = ((Enemy)enemy);
            e.SetDirection(dir);
            e.PlayAnim();
        }
        else if (enemy is Ninja)
        {

            Ninja n = ((Ninja)enemy);
            n.SetDirection(dir);
            n.PlayAnim();
        }

        Tween t = null;
        float randAmount = 8.0f;
        float minAmount = 1.0f;
        switch (dir)
        {
            case FutileFourDirectionBaseObject.Direction.DOWN:
                enemy.y += collisionTilemap.tileHeight * 2f;
                enemy.x += RXRandom.Float() * 2 * collisionTilemap.tileWidth - collisionTilemap.tileWidth;
                t = Go.to(enemy, minAmount, new TweenConfig().setEaseType(EaseType.QuadOut).setDelay(RXRandom.Float() * randAmount).floatProp("y", -collisionTilemap.tileHeight * 2f, true));
                break;
            case FutileFourDirectionBaseObject.Direction.UP:
                enemy.y -= collisionTilemap.tileHeight * 2f;
                enemy.x += RXRandom.Float() * 2 * collisionTilemap.tileWidth - collisionTilemap.tileWidth;
                t = Go.to(enemy, minAmount, new TweenConfig().setEaseType(EaseType.QuadOut).setDelay(RXRandom.Float() * randAmount).floatProp("y", collisionTilemap.tileHeight * 2f, true));
                break;
            case FutileFourDirectionBaseObject.Direction.RIGHT:
                enemy.x -= collisionTilemap.tileWidth * 2f;
                enemy.y += RXRandom.Float() * 2 * collisionTilemap.tileHeight - collisionTilemap.tileHeight;
                t = Go.to(enemy, minAmount, new TweenConfig().setEaseType(EaseType.QuadOut).setDelay(RXRandom.Float() * randAmount).floatProp("x", collisionTilemap.tileWidth * 2f, true));
                break;
            case FutileFourDirectionBaseObject.Direction.LEFT:
                enemy.x += collisionTilemap.tileWidth * 2f;
                enemy.y += RXRandom.Float() * 2 * collisionTilemap.tileHeight - collisionTilemap.tileHeight;
                t = Go.to(enemy, minAmount, new TweenConfig().setEaseType(EaseType.QuadOut).setDelay(RXRandom.Float() * randAmount).floatProp("x", -collisionTilemap.tileWidth * 2f, true));
                break;
        }
        t.setOnCompleteHandler((AbstractTween tw) =>
        {
            if (enemy is Enemy)
            {
                Enemy e = ((Enemy)enemy);
                e.currentState = Enemy.State.IDLE;
                e.StateCount = 1;

            }
            else if (enemy is Ninja)
            {
                Ninja n = ((Ninja)enemy);
                n.currentState = Ninja.State.IDLE;
                n.StateCount = 1;
            }
        });
    }

}

