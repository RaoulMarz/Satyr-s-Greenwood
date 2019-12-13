using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public class TestScene1 : Spatial
{
    private double speed = 50.0f;
    private Camera gameCamera;
    private Sprite3D hudSpritePathFind;
    private Sprite3D spriteGameSplash;
    private Sprite3D spritePalms1;
    private Sprite3D spritePalms2;
    private Sprite3D spriteIndicatorCamera;
    private Sprite3D spriteIndicatorPlayer;
    private AnimatedSprite3D animspriteRippleFlames;
    private Timer introTimer;
    private EnumMovementEntity movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_CAMERA;
    private bool movementActionsActive = false;
    private bool completedFlagIntro = false;
    private bool showHUDPathFind = false;
    private int introTimerCounter = 0;
    private int introDoneTicks = 120;
    private Dictionary<string, DateTime> keyBounceMap = new Dictionary<string, DateTime>();
    const float CAMERA_FOV_DECREASE = -0.2f;
    const float CAMERA_FOV_INCREASE = 0.2f;
    const float CAMERA_FAR_DECREASE = -0.25f;
    const float CAMERA_FAR_INCREASE = 0.25f;

    private static Dictionary<string, string> GetProperties(object obj)
    {
        var props = new Dictionary<string, string>();
        if (obj == null)
            return props;

        var type = obj.GetType();
        foreach (var prop in type.GetProperties())
        {
            var val = prop.GetValue(obj, new object[] { });
            var valStr = val == null ? "" : val.ToString();
            props.Add(prop.Name, valStr);
        }

        return props;
    }

    private void SetCameraPosition(Vector3 newPosition)
    {
        if ((gameCamera != null) && (newPosition != null))
        {
            gameCamera.Translation = newPosition;
        }
    }

    private static void PrintObjectProperties(string tag, object obj)
    {
        var props = GetProperties(obj);
        if (props.Count > 0)
        {
            GD.Print($"{tag} #properties =  {props.Count}");

            //string propValueStr = "";
            StringBuilder sb = new StringBuilder();
            foreach (var prop in props)
            {
                sb.Clear();
                //propValueStr = "";
                sb.Append(prop.Key);
                sb.Append(": ");
                sb.Append(prop.Value);
                GD.Print($"[ {tag} ] property = {sb.ToString()}");
            }
        }
    }

    private void ResetSpritesVisibility(bool visibleFlag)
    {
        spritePalms1.Visible = visibleFlag;
        spritePalms2.Visible = visibleFlag;
        animspriteRippleFlames.Visible = visibleFlag;
    }

    private void GameCameraTranslateAxis(float xlateValue, GeometricAxis axisChoice/* = GeometricAxis.GEOMETRIC_AXIS_Z*/)
    {
        if (gameCamera != null)
        {
            var cameraPos = gameCamera.Translation;
            switch (axisChoice)
            {
                case GeometricAxis.GEOMETRIC_AXIS_X:
                    {
                        cameraPos.x += xlateValue;
                        break;
                    }
                case GeometricAxis.GEOMETRIC_AXIS_Y:
                    {
                        cameraPos.y += xlateValue;
                        break;
                    }
                case GeometricAxis.GEOMETRIC_AXIS_Z:
                    {
                        cameraPos.z += xlateValue;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            GD.Print($"gameCameraTranslateZ , position = {cameraPos}");
            gameCamera.Translation = cameraPos;
        }
    }

    public void _on_IntroTimer_timeout()
    {
        introTimerCounter += 1;
        if ((introTimerCounter % 10) == 0)
        {
            GD.Print($"introTimer timeout, counter = {introTimerCounter}");
        }
        if (!completedFlagIntro)
        {
            GameCameraTranslateAxis(0.125f, GeometricAxis.GEOMETRIC_AXIS_Z);
            if (introTimerCounter <= 50)
            {
                spriteGameSplash.Opacity = 1.0f - ((introTimerCounter + 0.001f) / 100.0f);
                var splashScreenPos = spriteGameSplash.Translation;
                GD.Print($"spriteGameSplash, position = {splashScreenPos}");
                if (introTimerCounter == 25) {
                    animspriteRippleFlames.Visible = true;
                    animspriteRippleFlames.Animation = "default";
                    animspriteRippleFlames.Play();  
                }
            }
        }
        if ((completedFlagIntro == false) && (introTimerCounter > introDoneTicks))
        {
            completedFlagIntro = true;
            ResetSpritesVisibility(true);
            spriteGameSplash.Visible = false;
            movementActionsActive = true;
            SetCameraPosition(new Vector3(0f, 0f, 12.0f));
            showHUDPathFind = true;
            OrientActiveIndicator();
        }
        if ( ((introTimerCounter % 5) == 0) && (showHUDPathFind))
        {

        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Testing the IntroScene");
        introTimer = this.GetNodeOrNull<Timer>("intro-Timer");
        if (introTimer != null)
        {
            introTimer.Connect("timeout", this, nameof(_on_IntroTimer_timeout));
            //if (introTimer.isStopped()) {
            introTimer.Start();
            //}
            var props = GetProperties(introTimer);
            if (props.Count > 0)
            {
                GD.Print("introTimer, #properties = " + props.Count);
                PrintObjectProperties("intro-Timer", introTimer);
            }
        }
        //Sprite mySprite = GetNodeOrNull<Sprite>("MySprite");
        gameCamera = this.GetNodeOrNull<Camera>("game-Camera");
        if (gameCamera != null)
        {
            //gameCamera.Fov = 180;
            var props = GetProperties(gameCamera);
            if (props.Count > 0)
            {
                GD.Print("gameCamera, #properties = " + props.Count);

                //string propValueStr = "";
                StringBuilder sb = new StringBuilder();
                foreach (var prop in props)
                {
                    sb.Clear();
                    //propValueStr = "";
                    sb.Append(prop.Key);
                    //writer.Write(prop.Key);
                    sb.Append(": ");
                    sb.Append(prop.Value);
                    GD.Print($"gameCamera, property = {sb.ToString()}");
                }
            }
        }

        spriteGameSplash = this.GetNodeOrNull<Sprite3D>("sprite-GameTitle");
        if (spriteGameSplash != null)
        {
            //spriteGameSplash -- change scale ??
        }
        spritePalms1 = this.GetNodeOrNull<Sprite3D>("sprite-Palms1");
        spritePalms2 = this.GetNodeOrNull<Sprite3D>("sprite-Palms2");
        spritePalms1.Visible = false;
        spritePalms2.Visible = false;
        spriteIndicatorCamera = this.GetNodeOrNull<Sprite3D>("sprite-Camera-Indicator");
        spriteIndicatorPlayer = this.GetNodeOrNull<Sprite3D>("sprite-Player-Indicator");
        spriteIndicatorCamera.Visible = false;
        spriteIndicatorPlayer.Visible = false;
        //gameCamera.GetAnimation();
        hudSpritePathFind = this.GetNodeOrNull<Sprite3D>("sprite-HUD-PathFind");
        if (hudSpritePathFind != null)
        {
            hudSpritePathFind.Visible = false;
            var props = GetProperties(gameCamera);
            if (props.Count > 0)
            {
                GD.Print("hudSpritePathFind, #properties = " + props.Count);
            }
        }
        //hudSpritePathFind?.SetPosition(new Vector2(5.0f ,5.0f));

        
        animspriteRippleFlames = this.GetNodeOrNull<AnimatedSprite3D>("animsprite-RippleFlames");
        if (animspriteRippleFlames != null)
        {
            animspriteRippleFlames.Visible = false;
            PrintObjectProperties("intro-Timer", introTimer);
        }
        
    }

    private void OrientHUDPathFind()
    {
    // Determine the best placement on the sides of the screen to place this HUD panel ...
        if (hudSpritePathFind != null)
        {

        }
    }

    private void OrientActiveIndicator()
    {
        if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_CAMERA)
        {
            spriteIndicatorCamera.Visible = true;
            spriteIndicatorPlayer.Visible = false;
        }
        if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_PLAYERCHARACTER)
        {
            spriteIndicatorCamera.Visible = false;
            spriteIndicatorPlayer.Visible = true;
        }
    }

    private void MoveSpriteLaterally(Sprite3D gameSprite, float moveDistance)
    {
        if ( (gameSprite != null) && (gameSprite is Sprite3D) )
        {
            var spritePosition = gameSprite.Translation;
            if (spritePosition != null)
            {

            }
        }
    }

    private bool KeyBounceCheck(string key, float secondsIgnore)
    {
        bool res = true;
        if ( (key != null) && (secondsIgnore >= 0.1))
        {
            if (keyBounceMap.ContainsKey(key))
            {
                DateTime keyTimestamp = keyBounceMap[key];
                var diff = DateTime.Now - keyTimestamp;
                GD.Print($"KeyBounceCheck, milliseconds diff = {diff.Milliseconds}");
                if (diff.Milliseconds >= (secondsIgnore * 1000.0f))
                {
                    if (diff.Milliseconds >= (2 * secondsIgnore * 1000.0f))
                        keyBounceMap.Remove(key);
                    res = false;
                }
            }
            else
            {
                keyBounceMap.Add(key, DateTime.Now);
            }
        }
        return res;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        double x = 0.0;
        double y = 0.0;

        if (Input.IsActionPressed("toggle_control_object"))
        { //The TAB key ?? switch between camera change or player movement
            if ((KeyBounceCheck("TOGGLE_CONTROL", 0.25f)) == false)
            {
                if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_CAMERA)
                {
                    movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_PLAYERCHARACTER;
                }
                else
                    movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_CAMERA;
            }
            gameCamera.Fov = 90.0f;
            gameCamera.Far = 15.0f;
            OrientActiveIndicator();
        }

        if (movementActionsActive)
        {
            if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_CAMERA)
            {
                //GD.Print("movementEntity = MOVEMENT_ENTITY_CAMERA");
                if (Input.IsActionPressed("player_left"))
                {
                    gameCamera.Fov += CAMERA_FOV_DECREASE;
                    if (gameCamera.Fov <= 20.0f)
                        gameCamera.Fov = 20.0f;
                    GD.Print($"move_left pressed, fov = {gameCamera.Fov}");
                }

                //MOVE RIGHT
                if (Input.IsActionPressed("player_right"))
                {
                    gameCamera.Fov += CAMERA_FOV_INCREASE;
                    if (gameCamera.Fov >= 150.0f)
                        gameCamera.Fov = 150.0f;
                    GD.Print($"move_right pressed, fov = {gameCamera.Fov}");
                }

                if (Input.IsActionPressed("player_up"))
                {
                    gameCamera.Far += CAMERA_FAR_INCREASE;
                    if (gameCamera.Far >= 160.0f)
                        gameCamera.Far = 160.0f;
                    GD.Print($"player_up pressed, far = {gameCamera.Far}");
                }

                if (Input.IsActionPressed("player_down"))
                {
                    gameCamera.Far += CAMERA_FAR_DECREASE;
                    if (gameCamera.Far <= 2.0f)
                        gameCamera.Far = 2.0f;
                    GD.Print($"player_down pressed, far = {gameCamera.Far}");
                }
            }

            if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_PLAYERCHARACTER)
            {
                //MOVE LEFT
                if (Input.IsActionPressed("player_left"))
                {
                    x -= speed * delta;
                    GD.Print("move_left pressed");
                }

                //MOVE RIGHT
                if (Input.IsActionPressed("player_left"))
                {
                    x += speed * delta;
                    GD.Print("move_right pressed");
                }

                //MOVE UP
                if (Input.IsActionPressed("player_up"))
                    y -= speed * delta;


                //MOVE DOWN
                if (Input.IsActionPressed("player_down"))
                    y += speed * delta;
            }
        }
    }
}

