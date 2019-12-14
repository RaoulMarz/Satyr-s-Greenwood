using Godot;
using System;
using System.Text;
//using System.Reflection;
using System.Collections.Generic;

namespace Satyrs_Greenwood
{
    public class IntroScene : Spatial
    {
        private double walkSpeed = 50.0f;
        private double runSpeed = 80.0f;
        private Camera gameCamera;
        private Sprite3D hudSpritePathFind;
        private Sprite3D spriteGameSplash;
        private Sprite3D spritePalms1;
        private Sprite3D spritePalms2;
        private Sprite3D spriteKingsHill;
        private Sprite3D spritePlayer;
        private AudioStreamPlayer musicPlayer;
        private AnimatedSprite3D animspriteRippleFlames;
        private SpriteAdvocate spriteGroupGrassMoving1;
        private AnimationPlayer animPlayerGrassEffects;
        private Godot.Vector3 cameraCirclePosition;
        private Timer introTimer;
        private EnumMovementEntity movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_CAMERA;
        private float radiusCameraCircle = 5.0f;
        private bool movementActionsActive = false;
        private bool completedFlagIntro = false;
        private bool showHUDPathFind = false;
        private int introTimerCounter = 0;
        private int introDoneTicks = 120;
        private int markTickGrassEffects = 35;
        private SceneUtilities sceneUtil;
        private Dictionary<string, DateTime> keyBounceMap = new Dictionary<string, DateTime>();
        const float cameraSlowPanSpeed = 0.075f;

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
            if ( (animPlayerGrassEffects != null) && (introTimerCounter == markTickGrassEffects) )
            {
                animPlayerGrassEffects.Play("Sway-Grass");
                GD.Print($"animPlayerGrassEffects play <Sway-Grass>, counter = {introTimerCounter}");
            }
            if ((introTimerCounter % 10) == 0)
            {
                GD.Print($"introTimer timeout, counter = {introTimerCounter}");
            }
            if ( (introTimerCounter > 70) && (introTimerCounter <= 310)) //Move the camera along an arc
            {
                //Godot.Vector3 cameraCirclePosition;
                //SceneUtilities.MoveCameraAroundPosition(gameCamera, cameraCirclePosition, radiusCameraCircle, introTimerCounter - 10);
            }
            if (spritePlayer != null)
            {
                if (introTimerCounter <= 70)
                    GameCameraTranslateAxis(0.125f, GeometricAxis.GEOMETRIC_AXIS_Z);
                if ((introTimerCounter >= 20) && (introTimerCounter <= 240))
                {
                    //MoveSpriteLaterally(spritePlayer, 0.025f, GeometricAxis.GEOMETRIC_AXIS_X);
                }
            }
            if (!completedFlagIntro)
            {
                /*
                if (introTimerCounter <= 50)
                {
                    spriteGameSplash.Opacity = 1.0f - ((introTimerCounter + 0.001f) / 100.0f);
                    var splashScreenPos = spriteGameSplash.Translation;
                    GD.Print($"spriteGameSplash, position = {splashScreenPos}");
                    if (introTimerCounter == 25)
                    {
                        animspriteRippleFlames.Visible = true;
                        animspriteRippleFlames.Animation = "default";
                        animspriteRippleFlames.Play();
                    }
                }
                */
            }
            if ((completedFlagIntro == false) && (introTimerCounter > introDoneTicks))
            {
                completedFlagIntro = true;
                //ResetSpritesVisibility(true);
                //spriteGameSplash.Visible = false;
                //movementActionsActive = true;
                //SetCameraPosition(new Vector3(0f, 0f, 12.0f));
                //showHUDPathFind = true;
            }
            if (((introTimerCounter % 5) == 0) && (showHUDPathFind))
            {

            }
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print("Testing the IntroScene");
            sceneUtil = new SceneUtilities();
            sceneUtil.CleanPreviousScenes(this);
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
            musicPlayer = this.GetNodeOrNull<AudioStreamPlayer>("music-TrackPlayer");
            if (musicPlayer != null)
            {
                musicPlayer.Play();
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

            animPlayerGrassEffects = this.GetNodeOrNull<AnimationPlayer>("animPlayer-WeaveGrass");
            spritePlayer = this.GetNodeOrNull<Sprite3D>("sprite-Player-Character");
            spriteGameSplash = this.GetNodeOrNull<Sprite3D>("sprite-GameTitle");
            if (spriteGameSplash != null)
            {
                //spriteGameSplash -- change scale ??
            }
            spritePalms1 = this.GetNodeOrNull<Sprite3D>("sprite-Palms1");
            spritePalms2 = this.GetNodeOrNull<Sprite3D>("sprite-Palms2");
            spriteKingsHill = this.GetNodeOrNull<Sprite3D>("sprite-KingsHill");


        }

        private bool InAxisBounds(float position, GeometricAxis axisChoice)
        {
            bool res = true;
            if ((position >= 100.0f) || (position <= -100.0f))
                res = false;
            return res;
        }

        private void MoveSpriteLaterally(Sprite3D gameSprite, float moveDistance, GeometricAxis axisChoice)
        {
            if ((gameSprite != null) && (gameSprite is Sprite3D))
            {
                var spritePosition = gameSprite.Translation;
                if (spritePosition != null)
                {
                    switch (axisChoice)
                    {
                        case GeometricAxis.GEOMETRIC_AXIS_X:
                            {
                                if (InAxisBounds(spritePosition.x, axisChoice))
                                    spritePosition.x += moveDistance;
                                break;
                            }
                        case GeometricAxis.GEOMETRIC_AXIS_Y:
                            {
                                spritePosition.y += moveDistance;
                                break;
                            }
                        case GeometricAxis.GEOMETRIC_AXIS_Z:
                            {
                                spritePosition.z += moveDistance;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
        }

        private bool KeyBounceCheck(string key, float secondsIgnore)
        {
            bool res = true;
            if ((key != null) && (secondsIgnore >= 0.1))
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

        private bool ShowInGameMenu()
        {
            bool res = false;
            SceneUtilities.ExitApplication(this);
            return res;
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            double x = 0.0;
            double y = 0.0;
            double speed = walkSpeed;

            if (Input.IsActionPressed("ingame_menu")) {
                ShowInGameMenu();
                return;
            }

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
            }

            if (movementActionsActive)
            {
                if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_CAMERA)
                {

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

}
