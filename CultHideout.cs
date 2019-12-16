using Godot;
using System;
using System.Collections.Generic;

namespace Satyrs_Greenwood
{
    public class CultHideout : Spatial
    {
        private Sprite3D hudPlayer;
        private AnimatedSprite3D animspriteFirePit;
        private Sprite3D spriteTreesLayer1;
        private Control gameHelpPanel;
        private Timer sceneTimer;
        private EnemyAttackCard attackCardView;
        private SceneUtilities sceneUtil;
        private int gameTimerCounter = 0;
        private bool toggleVisibleAttackCard = true;
        private Dictionary<string, DateTime> keyBounceMap = new Dictionary<string, DateTime>();
        const string attackCardResource = "res://EnemyAttackCard.tscn";

        private void GenerateAttackCardView()
        {
            PackedScene attackCardScene = (PackedScene)ResourceLoader.Load(attackCardResource);
            if (attackCardScene != null)
            {
                attackCardView = (EnemyAttackCard)attackCardScene.Instance();
                GD.Print($"Loading attackCardView from PackedScene, attackCardView = {attackCardView.Name}");
                //playerViewport.AddChild(viewportScene.Instance());
                attackCardView.Visible = false;
                /*GetTree().Root.AddChild*/
                AddChild(attackCardView);
            }
        }

        private void DisplayAnimatedAttackView(Vector2 topAnchor)
        {
            if (attackCardView != null)
            {
                attackCardView.Visible = true;
                GD.Print("$displayAnimatedAttackView called, attackCardView visible = {attackCardView.Visible}");
            }
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print("Opening CultHideout scene");
            sceneUtil = new SceneUtilities();
            if (sceneUtil != null)
            {

            }
            sceneUtil.CleanPreviousScenes(this, "CultHideout");

            gameHelpPanel = this.GetNodeOrNull<Control>("HelpPanel");
            Diagnostics.PrintNullValueMessage(gameHelpPanel, "gameHelpPanel");

            GenerateAttackCardView();

            sceneTimer = this.GetNodeOrNull<Timer>("game1-Timer");
            Diagnostics.PrintNullValueMessage(sceneTimer, "sceneTimer");
            if (sceneTimer != null)
            {
                sceneTimer.Start();
                sceneTimer.Connect("timeout", this, nameof(_on_GameTimer_timeout));
            }
            InputAssistance.SetVerbose(true);
        }

        public void _on_GameTimer_timeout()
        {
            gameTimerCounter += 1;
            if (toggleVisibleAttackCard == false)
                attackCardView.Visible = false;
            if (gameTimerCounter == 40)
            {
                DisplayAnimatedAttackView(new Vector2(50, 50));
            }
            if ((gameTimerCounter % 10) == 0)
            {

            }
        }

        private void StartAnimatedIndicator(DateTime reftime)
        {
            //Shows an animation of a switch action being triggered
            //This will be less than 2 seconds of anim
        }

        private bool ShowInGameMenu()
        {
            bool res = false;
            SceneUtilities.ExitApplication(this);
            return res;
        }

        private bool DisplayPlayerSwitchAction()
        {
            bool res = true;
            DateTime refTime = DateTime.Now;
            toggleVisibleAttackCard = !toggleVisibleAttackCard;
            GD.Print($"DisplayPlayerSwitchAction() called, refTime = {refTime}");
            StartAnimatedIndicator(refTime);
            return res;
        }

        void DisplayHelpPanel()
        {
            if (gameHelpPanel != null)
            {
                gameHelpPanel.Visible = !gameHelpPanel.Visible;
                //help panel will have close button with signal ...
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            if (Input.IsActionPressed("ingame_menu"))
            {
                ShowInGameMenu();
                return;
            }

            if (Input.IsActionPressed("help_action"))
            {
                if (InputAssistance.KeyBounceCheck("HELP_ACTION", 0.65f, 1.2f))
                {
                    DisplayHelpPanel();
                }
            }

            if (Input.IsActionPressed("toggle_switch_action"))
            {
                if (InputAssistance.KeyBounceCheck("TOGGLE_SWITCH", 0.65f, 1.2f))
                {
                    DisplayPlayerSwitchAction();
                    return;
                }
            }
        }

        private bool KeyBounceCheck(string key, float secondsIgnore)
        {
            bool res = true;
            if ((key != null) && (secondsIgnore >= 0.05))
            {
                GD.Print($"KeyBounceCheck() called, key = {key}, ingnore seconds = {secondsIgnore}");
                if (keyBounceMap.ContainsKey(key))
                {
                    DateTime keyTimestamp = keyBounceMap[key];
                    var diff = DateTime.Now - keyTimestamp;
                    GD.Print($"KeyBounceCheck, milliseconds diff = {diff.Milliseconds}");
                    if (diff.Milliseconds <= (secondsIgnore * 1000.0f))
                    {
                        if (diff.Milliseconds >= (1.5f * secondsIgnore * 1000.0f))
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
    }
}
