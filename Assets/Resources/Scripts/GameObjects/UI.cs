using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UI : FContainer
{
    public const int MAX_HEALTH = 5;
    private FSprite[] hearts = new FSprite[MAX_HEALTH];
    int health = MAX_HEALTH;
    float uiSideMargin = 3;
    float heartSpace = 1;
    World w;
    FLabel k;
    FLabel killCount;

    FSprite selectedPowerupBG;
    FSprite selectedPowerup;

    public UI(World w)
    {
        this.w = w;
        for (int i = 0; i < MAX_HEALTH; i++)
        {
            hearts[i] = new FSprite("heart");
            hearts[i].x = Futile.screen.halfWidth - hearts[i].width / 2f - uiSideMargin - (i * (hearts[i].width + heartSpace));
            hearts[i].y = Futile.screen.halfHeight - hearts[i].height / 2f - uiSideMargin;
            this.AddChild(hearts[i]);

        }
        selectedPowerupBG = new FSprite("powerupSelect");
        selectedPowerupBG.x = -Futile.screen.halfWidth + selectedPowerupBG.width / 2f + uiSideMargin + 20;
        selectedPowerupBG.y = hearts[0].y;
        this.AddChild(selectedPowerupBG);

        selectedPowerup = new FSprite("normalpowerup");
        selectedPowerup.SetPosition(selectedPowerupBG.GetPosition());
        this.AddChild(selectedPowerup);

        k = new FLabel(C.smallFontName, "K");
        k.SetPosition(selectedPowerupBG.GetPosition() - Vector2.up * 7);
        this.AddChild(k);


        killCount = new FLabel(C.smallFontName, "0");
        killCount.y = Futile.screen.halfHeight - killCount.textRect.height / 2f - uiSideMargin;
        this.AddChild(killCount);
    }

    public void UpdateKillCount(int killCount)
    {
        this.killCount.text = killCount.ToString();
    }

    public void UpdateHealth(int newHealth)
    {
        newHealth = Mathf.Clamp(newHealth, 0, MAX_HEALTH);
        if (newHealth == health)
            return;
        if (newHealth < health)
            TakeDamage(newHealth);
        else
            GetHealth(newHealth);
        health = newHealth;
    }

    public void UpdatePowerup(Player.AttackType type)
    {
        if (selectedPowerup.element.name != (type.ToString().ToLower() + "powerup"))
        {
            FSoundManager.PlaySound("powerup");
            selectedPowerup.SetElementByName(type.ToString().ToLower() + "powerup");
            Go.killAllTweensWithTarget(selectedPowerup);
            selectedPowerup.SetPosition(Futile.stage.GetPosition() + w.p.GetPosition());
            Go.to(selectedPowerup, .6f, new TweenConfig().floatProp("x", selectedPowerupBG.x).floatProp("y", selectedPowerupBG.y).setEaseType(EaseType.QuadInOut));
        }
    }

    private void TakeDamage(int newHealth)
    {
        for (int i = health - 1; i > newHealth - 1; i--)
        {
            hearts[i].SetElementByName("heart_empty");
            for (int x = 0; x < hearts[i].width; x++)
            {
                for (int y = 0; y < hearts[i].height; y++)
                {
                    Particle pixel = Particle.ParticleTwo.getParticle(1.0f);
                    //pixel.SetPosition(hearts[i].GetPosition() + new Vector2(x-hearts[i].width/2f, -y + hearts[i].height/2f));
                    //pixel.activate(hearts[i].GetPosition() + new Vector2(x - hearts[i].width / 2f, -y + hearts[i].height / 2f - 1f), Vector2.zero, Vector2.zero, 0);
                    pixel.activate(hearts[i].GetPosition() + new Vector2(x - hearts[i].width / 2f, -y + hearts[i].height / 2f - 1f),
                        new Vector2(RXRandom.Float() * 40 - 20f, RXRandom.Float() * 30 + 15),
                        new Vector2(RXRandom.Float() * 4 - 2, RXRandom.Float() * -10 - 100), 0);
                    //Go.to(pixel, 1.0f, new TweenConfig().floatProp("x", RXRandom.Float() * 10 - 5f, true).floatProp("y", RXRandom.Float() * -10 - 10, true).setEaseType(EaseType.CircIn));
                    Color c = GetHeartColor(x, y);
                    if (c == Color.clear)
                        continue;
                    pixel.color = c;
                    this.AddChild(pixel);

                }
            }
        }
    }

    public Color GetHeartColor(int x, int y)
    {
        Color mainHeartColor = new Color(205 / 255f, 0, 0);
        Color darkHeartColor = new Color(150 / 255f, 0, 0);
        Color lightHeartColor = new Color(242 / 255f, 0, 0);
        switch (y)
        {
            case 0:
                return Color.clear;
            case 1:
                switch (x)
                {
                    case 0:
                    case 1:
                    case 4:
                    case 7:
                    case 8:
                        return Color.clear;
                    case 2:
                        return darkHeartColor;
                    case 3:
                    case 5:
                    case 6:
                        return mainHeartColor;
                }
                break;
            case 2:
                switch (x)
                {
                    case 0:
                    case 8:
                        return Color.clear;
                    case 1:
                        return darkHeartColor;
                    case 6:
                        return lightHeartColor;
                    default:
                        return mainHeartColor;

                }
            case 3:
            case 4:
                switch (x)
                {
                    case 0:
                    case 8:
                        return Color.clear;
                    case 1:
                        return darkHeartColor;
                    default:
                        return mainHeartColor;

                }
            case 5:
                switch (x)
                {
                    case 0:
                    case 1:
                    case 7:
                    case 8:
                        return Color.clear;
                    case 2:
                        return darkHeartColor;
                    default:
                        return mainHeartColor;

                }
            case 6:
                switch (x)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 6:
                    case 7:
                    case 8:
                        return Color.clear;
                    case 3:
                        return darkHeartColor;
                    default:
                        return mainHeartColor;

                }
            case 7:
                switch (x)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        return Color.clear;
                    case 4:
                        return darkHeartColor;
                    default:
                        return mainHeartColor;

                }
            case 8:
                return Color.clear;
        }
        return Color.clear;
    }

    private void GetHealth(int newHealth)
    {
        for (int i = health; i <= newHealth - 1; i++)
        {
            int index = i;
            Tween lastTween = null;
            FSprite[] pixels = new FSprite[(int)(hearts[i].width * hearts[i].height)];
            for (int x = 0; x < hearts[i].width; x++)
                for (int y = 0; y < hearts[i].height; y++)
                {
                    FSprite pixel = new FSprite(Futile.whiteElement);
                    pixel.width = 1;
                    pixel.height = 1;
                    pixel.SetPosition(hearts[i].GetPosition() + new Vector2(RXRandom.Float() * 50 - 25, RXRandom.Float() * 30 - 80));
                    //pixel.activate(hearts[i].GetPosition() + new Vector2(x - hearts[i].width / 2f, -y + hearts[i].height / 2f - 1f),
                    //    new Vector2(RXRandom.Float() * 20 - 10f, RXRandom.Float() * 15 + 6),
                    //    new Vector2(RXRandom.Float() * 4 - 2, RXRandom.Float() * -10 - 30), 0);
                    //Go.to(pixel, 1.0f, new TweenConfig().floatProp("x", RXRandom.Float() * 10 - 5f, true).floatProp("y", RXRandom.Float() * -10 - 10, true).setEaseType(EaseType.CircIn));
                    Color c = GetHeartColor(x, y);
                    if (c == Color.clear)
                        continue;
                    pixel.color = c;
                    pixel.alpha = 0;
                    Vector2 resultPos = hearts[i].GetPosition() + new Vector2(x - hearts[i].width / 2f, -y + hearts[i].height / 2f - 1f);
                    lastTween = Go.to(pixel, .5f, new TweenConfig().floatProp("x", resultPos.x).floatProp("y", resultPos.y).floatProp("alpha", 1).setDelay(x * .05f).setEaseType(EaseType.QuadOut).onComplete(() => {/* pixel.RemoveFromContainer(); */}));
                    this.AddChild(pixel);
                    pixels[(int)(x + y * hearts[i].width)] = pixel;

                }

            lastTween.setOnCompleteHandler((AbstractTween t) =>
            {
                hearts[index].SetElementByName("heart");
                foreach (FSprite s in pixels)
                    if (s != null)
                        s.RemoveFromContainer();
            });
        }
    }
}

