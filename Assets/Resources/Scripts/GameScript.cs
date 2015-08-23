using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{
    World w;
    FNode tempFollowNode;
    FContainer ogreatContainer;
    FLabel o, g, r, e, a, t;
    FLabel pressStart;
    FLabel madeForLD;
    FLabel twitter;

    private enum State
    {
        TITLE,
        INGAME
    }

    private State state = State.TITLE;
    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);
        futileParams.AddResolutionLevel(640.0f / 4f, 1.0f, 1.0f, "");

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0/ 255.0f, 94 / 255.0f, 38/255.0f);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/inGameAtlas");
        Futile.atlasManager.LoadFont(C.smallFontName, "smallFont_0", "Atlases/smallFont", 0, 0);
        Futile.atlasManager.LoadFont(C.largeFontName, "largeFont_0", "Atlases/largeFont", 0, 0);

        FSoundManager.PlayMusic("OGreatBG");
        C.getCameraInstance().SetPosition(Futile.screen.halfWidth, -Futile.screen.halfHeight);

        FSprite bevelOverlay = new FSprite(Futile.whiteElement);
        bevelOverlay.width = Futile.screen.width;
        bevelOverlay.height = Futile.screen.height;
        bevelOverlay.shader = FShader.Basic_PixelSnap;
        bevelOverlay.sortZ = 100;
        w = new World();
        Futile.stage.AddChild(w);
        twitter = new FLabel(C.smallFontName, "@MaggardJosh");
        twitter.x = Futile.screen.halfWidth - twitter.textRect.width / 2f;
        twitter.y = -Futile.screen.halfHeight + twitter.textRect.height / 2f;
        C.getCameraInstance().AddChild(twitter);

        madeForLD = new FLabel(C.smallFontName, "Made in 48 hours\nfor Ludum Dare 33");
        madeForLD.alignment = FLabelAlignment.Left;
        madeForLD.x = -Futile.screen.halfWidth;
        madeForLD.y = -Futile.screen.halfHeight + madeForLD.textRect.height / 2f;
        C.getCameraInstance().AddChild(madeForLD);

        w.LoadMap("titleMap");
        w.currentState = World.State.IN_GAME;
        
        w.UpdateWorldBounds();

        Futile.stage.AddChild(C.getCameraInstance());
        C.getCameraInstance().SetPosition(w.p.GetPosition());
        C.getCameraInstance().AddChild(bevelOverlay);

        w.p.CurrentState = Player.State.DYING;
        tempFollowNode = new FNode();
        tempFollowNode.SetPosition(w.p.GetPosition());
        C.getCameraInstance().follow(tempFollowNode);
        StartRandomMapMovement();
        w.ui.isVisible = false;
        w.p.SetPosition(-1000, 1000);
        pressStart = new FLabel(C.smallFontName, "PRESS K TO START");
        C.getCameraInstance().AddChild(pressStart);
        pressStart.y = -Futile.screen.halfHeight * .3f;
        pressStart.alpha = 0;
        o = new FLabel(C.largeFontName, "O");
        o.alignment = FLabelAlignment.Left;
        g = new FLabel(C.largeFontName, "G");
        r = new FLabel(C.largeFontName, "R");
        e = new FLabel(C.largeFontName, "E");
        g.alignment = FLabelAlignment.Left;
        r.alignment = FLabelAlignment.Left;
        e.alignment = FLabelAlignment.Left;
        a = new FLabel(C.largeFontName, "A");
        a.alignment = FLabelAlignment.Left;
        t = new FLabel(C.largeFontName, "T");
        t.alignment = FLabelAlignment.Left;
        ogreatContainer = new FContainer();
        ogreatContainer.AddChild(o);
        ogreatContainer.AddChild(g);
        ogreatContainer.AddChild(r);
        ogreatContainer.AddChild(e);
        ogreatContainer.AddChild(a);
        ogreatContainer.AddChild(a);
        ogreatContainer.AddChild(t);
        C.getCameraInstance().AddChild(ogreatContainer);

        e.x = -e.textRect.width / 2f;
        r.x = e.x - r.textRect.width;
        g.x = r.x - g.textRect.width;
        o.x = g.x - o.textRect.width - 10;
        a.x = e.x + e.textRect.width;
        t.x = a.x + a.textRect.width;

        ogreatContainer.y = Futile.screen.height * .3f;
        ogreatContainer.x = 10;
        float duration = 2.0f;
        float initDuration = .8f;
        Color greenColor = new Color(0, .3f, 0);

        o.y = g.y = r.y = e.y = a.y = t.y = Futile.screen.halfHeight;

        Go.to(o, initDuration, new TweenConfig().floatProp("y", 0).setDelay(.5f).setEaseType(EaseType.BackOut));
        Go.to(g, initDuration, new TweenConfig().floatProp("y", 0).setDelay(.6f).setEaseType(EaseType.BackOut));
        Go.to(r, initDuration, new TweenConfig().floatProp("y", 0).setDelay(.7f).setEaseType(EaseType.BackOut));
        Go.to(e, initDuration, new TweenConfig().floatProp("y", 0).setDelay(.8f).setEaseType(EaseType.BackOut));
        Go.to(a, initDuration, new TweenConfig().floatProp("y", 0).setDelay(.9f).setEaseType(EaseType.BackOut));
        Go.to(t, initDuration, new TweenConfig().floatProp("y", 0).setDelay(1.0f).setEaseType(EaseType.BackOut).onComplete(() =>
            {
                Go.to(o, duration, new TweenConfig().floatProp("x", 11, true).colorProp("color", greenColor).setEaseType(EaseType.CircInOut).setIterations(-1, LoopType.PingPong));
                Go.to(g, duration, new TweenConfig().colorProp("color", greenColor).setEaseType(EaseType.CircInOut).setIterations(-1, LoopType.PingPong));
                Go.to(r, duration, new TweenConfig().colorProp("color", greenColor).setEaseType(EaseType.CircInOut).setIterations(-1, LoopType.PingPong));
                Go.to(e, duration, new TweenConfig().colorProp("color", greenColor).setEaseType(EaseType.CircInOut).setIterations(-1, LoopType.PingPong));

                Futile.instance.SignalUpdate += OnUpdate;
                Go.to(pressStart, 1.0f, new TweenConfig().floatProp("alpha", 1.0f).setDelay(.5f).setEaseType(EaseType.QuadIn));
            }));
        w.inDeadMode = true;



    }

    public void OnUpdate()
    {
        switch (state)
        {
            case State.TITLE:
                if (C.getKeyDown(C.ACTION_KEY))
                {
                    FSoundManager.PlaySound("menuSelect");
                    float transOut = .6f;
                    Go.killAllTweensWithTarget(tempFollowNode);
                    Futile.instance.SignalUpdate -= OnUpdate;
                    Go.killAllTweensWithTarget(o);
                    Go.killAllTweensWithTarget(g);
                    Go.killAllTweensWithTarget(r);
                    Go.killAllTweensWithTarget(e);
                    Go.killAllTweensWithTarget(a);
                    Go.killAllTweensWithTarget(t);
                    Go.killAllTweensWithTarget(pressStart);
                    Go.to(pressStart, transOut, new TweenConfig().floatProp("alpha", 0).onComplete(() => { pressStart.RemoveFromContainer(); }));
                    Go.to(o, transOut, new TweenConfig().floatProp("y", 100, true).setDelay(.5f).setEaseType(EaseType.BackIn));
                    Go.to(g, transOut, new TweenConfig().floatProp("y", 100, true).setDelay(.6f).setEaseType(EaseType.BackIn));
                    Go.to(r, transOut, new TweenConfig().floatProp("y", 100, true).setDelay(.7f).setEaseType(EaseType.BackIn));
                    Go.to(e, transOut, new TweenConfig().floatProp("y", 100, true).setDelay(.8f).setEaseType(EaseType.BackIn));
                    Go.to(a, transOut, new TweenConfig().floatProp("y", 100, true).setDelay(.9f).setEaseType(EaseType.BackIn));
                    Go.to(t, transOut, new TweenConfig().floatProp("y", 100, true).setDelay(1.0f).setEaseType(EaseType.BackIn).onComplete(() =>
                    {
                        ogreatContainer.RemoveAllChildren();
                        ogreatContainer.RemoveFromContainer();
                        w.ResetVars();
                        w.LoadNewMap("mapOne");
                        madeForLD.RemoveFromContainer();
                        twitter.RemoveFromContainer();

                    }));
                    Go.killAllTweensWithTarget(madeForLD);
                    Go.killAllTweensWithTarget(twitter);
                    Go.to(madeForLD, transOut, new TweenConfig().floatProp("y", -madeForLD.textRect.height, true).setEaseType(EaseType.BackIn));
                    Go.to(twitter, transOut, new TweenConfig().floatProp("y", -twitter.textRect.height, true).setEaseType(EaseType.BackIn));
                }
                break;
        }
    }

    public void StartRandomMapMovement()
    {
        Go.to(tempFollowNode, 10.0f, new TweenConfig().setDelay(1.0f).floatProp("x", RXRandom.Float() * C.getCameraInstance().getWorldBounds().width + C.getCameraInstance().getWorldBounds().x).floatProp("y", RXRandom.Float() * -C.getCameraInstance().getWorldBounds().height).onComplete(() => { StartRandomMapMovement(); }));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            FSoundManager.isMuted = !FSoundManager.isMuted;
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (((FPixelSnapShader)FShader.Basic_PixelSnap).dotMatrixAmount == 0)
                ((FPixelSnapShader)FShader.Basic_PixelSnap).dotMatrixAmount = 1;
            else
                ((FPixelSnapShader)FShader.Basic_PixelSnap).dotMatrixAmount = 0;
        }

    }
}
