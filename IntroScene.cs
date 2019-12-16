using Godot;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Satyrs_Greenwood
{
    public class IntroScene : Spatial
    {
        private double walkSpeed = 50.0f;
        private double runSpeed = 80.0f;
        private StoryArcBoard storyArcBoard = null;
        private Camera gameCamera;
        private Sprite3D hudSpritePathFind;
        private Sprite3D spriteGameSplash;
        private Sprite3D spritePalms1;
        private Sprite3D spritePalms2;
        private Sprite3D spriteKingsHill;
        private Sprite3D spritePlayer;
        private Sprite3D spriteExitScene;
        private AudioStreamPlayer musicPlayer;
        private AnimatedSprite3D animspriteRippleFlames;
        private AnimatedSprite3D animspriteMagicEffect1;
        private SpriteAdvocate spriteGroupGrassMoving1;
        private AnimationPlayer animPlayerGrassEffects;
        private Viewport playerViewport;
        private Panel panelOptions;
        private Godot.Vector3 cameraCirclePosition;
        private Godot.Timer introTimer;
        //private Viewport playerViewport;
        private EnumMovementEntity movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_CAMERA;
        private float radiusCameraCircle = 5.0f;
        private bool movementActionsActive = false;
        private bool completedFlagIntro = false;
        private bool showHUDPathFind = false;
        private bool flagSceneExitAllowed = true;
        private int introTimerCounter = 0;
        private int currentParagraph = 1;
        private int introDoneTicks = 120;
        private int markTickGrassEffects = 35;
        private String nextSceneResource = "res://CultHideout.tscn"; //"res://CultHideout.tscn" or "res://TestScene1.tscn"
        private SceneUtilities sceneUtil;
        private Dictionary<string, FlagDateTime> specialEffectsTrackMap = new Dictionary<string, FlagDateTime>();
        private ViewportFrameInterface viewportDialogueFrame;
        private bool triggerGameStage1 = false;
        const float cameraSlowPanSpeed = 0.075f;
        const float dialogueBoxHeight = 600.0f;
        const float dialogueBoxWidth = 420.0f;
        const int storyInitiateCutsceneTicks = 800;
        int[] markersStoryParagraph = { 200, 420, 640 };
        string[] introStoryTextArray = {"Our bard investigator sets out on foot, just leaving King's Hill. It is already very clear that the state of the kingdom is declining", 
            "(Kingsman) Destroyed caravans as far as the eye can see. Whoever did this is no common bandit. Let's see what we're up against.",
                "(Kingsman) A lot of thugs in this forest. This must be their hideout." };
        const string storyArcResource = "res://StoryArcBoard.tscn";

        public delegate void ElegantExitDelegate();
        public event ElegantExitDelegate ElegantExit;

        private Node2D Load2dScene(string scenePath)
        {
            PackedScene viewportScene = (PackedScene)ResourceLoader.Load(scenePath);
            if (viewportScene == null)
                return null;
            return (Node2D)viewportScene.Instance();
        }

        private void GeneratePlayerViewport()
        {
                PackedScene viewportScene = (PackedScene)ResourceLoader.Load(storyArcResource);
                if (viewportScene != null)
                {
                    storyArcBoard = (StoryArcBoard)viewportScene.Instance();
                    GD.Print($"Loading storyArcBoard from PackedScene, storyArcBoard = {storyArcBoard.Name}");
                    //playerViewport.AddChild(viewportScene.Instance());
                    storyArcBoard.Visible = false;
                    storyArcBoard.SetTitle("Leaving Castle Town");
                    /*GetTree().Root.AddChild*/
                    AddChild(storyArcBoard);
                }
        }

        private void ShowArcBoardParagraph(int paragraph)
        {
            if ( (storyArcBoard != null) && (paragraph <= 3) ) {
                storyArcBoard.Visible = true;
                string paraText = introStoryTextArray[paragraph - 1];
                storyArcBoard.SetParagraphText(paragraph, $"[code]{paraText}[/code]");
            }
        }

        private void _on_ElegantExit()
        {
            for (int ix = 0; ix < 5; ix++)
            {
                System.Threading.Thread.Sleep(100);
            }
            sceneUtil.ChangeScene(this, nextSceneResource);
        }

        private void ResetSpritesVisibility(bool visibleFlag)
        {
            spritePalms1.Visible = visibleFlag;
            spritePalms2.Visible = visibleFlag;
            animspriteRippleFlames.Visible = visibleFlag;
        }

        private void AddSpecialEffectStarted(string effectName/*, string action*/, DateTime timestamp, bool clearEffect = false)
        {
            if (clearEffect)
            {
                if (specialEffectsTrackMap.ContainsKey(effectName))
                    specialEffectsTrackMap.Remove(effectName);
                return;
            }
            if (specialEffectsTrackMap.ContainsKey(effectName) == false)
            {
                specialEffectsTrackMap.Add(effectName, new FlagDateTime(timestamp, true));
            }
        }

        private bool SpecialEffectHasStarted(string effectName)
        {
            return specialEffectsTrackMap.ContainsKey(effectName);
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
            if ( (currentParagraph <= 3) && (introTimerCounter == markersStoryParagraph[currentParagraph - 1]) )
            {
                ShowArcBoardParagraph(currentParagraph);
                currentParagraph += 1;
            }

            if (flagSceneExitAllowed)
            {
                if ((introTimerCounter >= storyInitiateCutsceneTicks) || (triggerGameStage1))
                {
                    introTimer.WaitTime = 5.0f;
                    GD.Print($"Switching scenes, Timer Counter = {introTimerCounter}");
                    introTimer.Stop();
                    introTimer.Free();
                    /* send a signal here */
                    if (sceneUtil != null)
                    {
                        //sceneUtil.ChangeScene(this, "res://CultHideout.tscn");
                        ElegantExit.Invoke();
                    }
                }
            }
            if ((introTimerCounter >= 40) && (introTimerCounter <= 300))
            {
                AnimateMagicEffect1(introTimerCounter - 40);
            }
            if ((animPlayerGrassEffects != null) && (introTimerCounter == markTickGrassEffects))
            {
                animPlayerGrassEffects.Play("Sway-Grass");
                GD.Print($"animPlayerGrassEffects play <Sway-Grass>, counter = {introTimerCounter}");
            }
            if ((introTimerCounter % 10) == 0)
            {
                GD.Print($"introTimer timeout, counter = {introTimerCounter}");
            }
            if ((introTimerCounter > 70) && (introTimerCounter <= 310)) //Move the camera along an arc
            {
                //Godot.Vector3 cameraCirclePosition;
                //SceneUtilities.MoveCameraAroundPosition(gameCamera, cameraCirclePosition, radiusCameraCircle, introTimerCounter - 10);
            }
            if (spritePlayer != null)
            {
                if (introTimerCounter <= 70)
                    GameCameraTranslateAxis(0.125f, GeometricAxis.GEOMETRIC_AXIS_Z);
                if ((introTimerCounter >= 120) && (introTimerCounter <= 175))
                {
                    MoveSpriteLaterally(spritePlayer, 0.0175f, GeometricAxis.GEOMETRIC_AXIS_X, false);
                }
                if ((introTimerCounter >= 70) && (introTimerCounter <= 460))
                {
                    MoveSpriteLaterally(spritePlayer, 0.0425f, GeometricAxis.GEOMETRIC_AXIS_Z, false);
                }
            }
            if (!completedFlagIntro)
            {

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
            GD.Print("Ready on IntroScene");
            sceneUtil = new SceneUtilities();
            sceneUtil.CleanPreviousScenes(this);
            introTimer = this.GetNodeOrNull<Godot.Timer>("intro-Timer");
            if (introTimer != null)
            {
                introTimer.Connect("timeout", this, nameof(_on_IntroTimer_timeout));
                //if (introTimer.isStopped()) {
                introTimer.Start();
                //}
                var props = Diagnostics.GetProperties(introTimer);
                if (props.Count > 0)
                {
                    GD.Print("introTimer, #properties = " + props.Count);
                    Diagnostics.PrintObjectProperties("intro-Timer", introTimer);
                }
            }

            if (playerViewport != null)
            {
                Diagnostics.PrintObjectProperties("playerViewport", playerViewport);
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
                var props = Diagnostics.GetProperties(gameCamera);
                if (props.Count > 0)
                {
                    GD.Print("gameCamera, #properties = " + props.Count);

                    Diagnostics.PrintObjectProperties("gameCamera", gameCamera);
                }
            }

            panelOptions = this.GetNodeOrNull<Panel>("panel-Options");
            animPlayerGrassEffects = this.GetNodeOrNull<AnimationPlayer>("animplayer-WeaveGrass");
            animspriteMagicEffect1 = this.GetNodeOrNull<AnimatedSprite3D>("animsprite-MagicEffect1");
            Diagnostics.PrintNullValueMessage(animPlayerGrassEffects, "animPlayerGrassEffects");
            Diagnostics.PrintNullValueMessage(animspriteMagicEffect1, "animspriteMagicEffect1");
            spritePlayer = this.GetNodeOrNull<Sprite3D>("sprite-Player-Character");
            spriteGameSplash = this.GetNodeOrNull<Sprite3D>("sprite-GameTitle");
            spriteExitScene = this.GetNodeOrNull<Sprite3D>("sprite-ExitScene");
            if (spriteGameSplash != null)
            {
                //spriteGameSplash -- change scale ??
            }
            spritePalms1 = this.GetNodeOrNull<Sprite3D>("sprite-Palms1");
            spritePalms2 = this.GetNodeOrNull<Sprite3D>("sprite-Palms2");
            spriteKingsHill = this.GetNodeOrNull<Sprite3D>("sprite-KingsHill");
            playerViewport = this.GetNodeOrNull<Viewport>("player-Viewport");
            Diagnostics.PrintNullValueMessage(playerViewport, "playerViewport");
            if (playerViewport != null)
            {
                Diagnostics.PrintObjectProperties("playerViewport", playerViewport);
                if (playerViewport.World2d != null)
                    Diagnostics.PrintObjectProperties("playerViewport->World2D", playerViewport.World2d);
            }
            //viewportDialogueFrame = new ViewportFrameInterface(new Vector2(dialogueBoxHeight, dialogueBoxWidth));
            ElegantExit += _on_ElegantExit;

            GeneratePlayerViewport();
            //SceneUtilities.LinkSceneToViewport("res://StoryArcBoard.tscn", playerViewport);
        }

        private bool CheckDimensionalBounds(SpriteBase3D gameSprite, String identifierText)
        {
            bool res = true;
            return res;
        }

        private void AnimateMagicEffect1(int tick)
        {
            if (tick >= 120)
                return;
            if ((animspriteMagicEffect1 != null) && (tick > 0))
            {
                if (SpecialEffectHasStarted("magic-effect1") == false) // ( (!animspriteMagicEffect1.Playing) && 
                {
                    GD.Print($"AnimateMagicEffect1, is playing = {animspriteMagicEffect1.Playing}");
                    animspriteMagicEffect1.Play("default");
                    animspriteMagicEffect1.Playing = true;
                    GD.Print($"AnimateMagicEffect1 play started, is playing = {animspriteMagicEffect1.Playing}");
                    AddSpecialEffectStarted("magic-effect1", DateTime.Now);
                }
                else
                {
                    MoveSpriteLaterally(animspriteMagicEffect1, 0.75f, GeometricAxis.GEOMETRIC_AXIS_Z, true);
                    //Translation
                    if ((tick >= 110) || (CheckDimensionalBounds(animspriteMagicEffect1, "magic-effect1")))
                    {
                        animspriteMagicEffect1.Stop();
                        animspriteMagicEffect1.Visible = true;
                    }
                }
            }
        }

        private bool InAxisBounds(float position, GeometricAxis axisChoice)
        {
            bool res = true;
            if ((position >= 100.0f) || (position <= -150.0f))
                res = false;
            return res;
        }

        /* MoveSpriteLaterally(Sprite3D gameSprite, float moveDistance, GeometricAxis axisChoice) */
        private void MoveSpriteLaterally(SpriteBase3D gameSprite, float moveDistance, GeometricAxis axisChoice, bool debugPrint = false)
        {
            if ((gameSprite != null)) // && (gameSprite is Sprite3D))
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
                                if (InAxisBounds(spritePosition.y, axisChoice))
                                    spritePosition.y += moveDistance;
                                break;
                            }
                        case GeometricAxis.GEOMETRIC_AXIS_Z:
                            {
                                if (InAxisBounds(spritePosition.z, axisChoice))
                                    spritePosition.z += moveDistance;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    gameSprite.Translation = spritePosition;
                    if (debugPrint)
                    {
                        GD.Print($"MoveSpriteLaterally(), Object = {gameSprite.Name}, new position = {spritePosition}");
                    }
                }
            }
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

            if (Input.IsActionPressed("ingame_menu"))
            {
                ShowInGameMenu();
                return;
            }

            if (Input.IsActionPressed("toggle_switch_action"))
            { //The TAB key ?? switch between camera change or player movement
                if (InputAssistance.KeyBounceCheck("TOGGLE_SWITCH", 0.45f, 1.2f))
                {
                    flagSceneExitAllowed = !flagSceneExitAllowed;
                    if (spriteExitScene != null)
                    {
                        spriteExitScene.Visible = !flagSceneExitAllowed;
                        if (spriteExitScene.Visible)
                            SceneUtilities.DebugPrintScenesList(this);
                    }
                    /*
                    if (movementEntity == EnumMovementEntity.MOVEMENT_ENTITY_CAMERA)
                    {
                        movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_PLAYERCHARACTER;
                    }
                    else
                        movementEntity = EnumMovementEntity.MOVEMENT_ENTITY_CAMERA;
                    */
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
