using Godot;
using System;
using System.Text;
using System.Collections.Generic;

namespace Satyrs_Greenwood
{
    public class CultHideout : Spatial
    {
        private Sprite3D hudPlayer;
        private AnimatedSprite3D animspriteFirePit;
        private Sprite3D spriteTreesLayer1;
        private Control gameHelpPanel;
        private Control inGameMenuPanel;
        private Timer sceneTimer;
        private EnemyAttackCard attackCardView = null;
        private StoryArcBoard debugBoard = null;
        private SceneUtilities sceneUtil;
        private int gameTimerCounter = 0;
        private bool toggleVisibleAttackCard = true;
        //private Dictionary<string, DateTime> keyBounceMap = new Dictionary<string, DateTime>();
        private Dictionary<string, bool> toggleEventsMap = new Dictionary<string, bool>();
        /* toggle Events -> "toggle_event_help", "toggle_event_gamemenu", "toggle_event_cardupdate" */
        const string attackCardResource = "res://EnemyAttackCard.tscn";
        const string storyArcResource = "res://StoryArcBoard.tscn";

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

        private void GenerateDebugView()
        {
            PackedScene debugFrameScene = (PackedScene)ResourceLoader.Load(storyArcResource);
            if (debugFrameScene != null)
            {
                debugBoard = (StoryArcBoard)debugFrameScene.Instance();
                GD.Print($"Loading debugBoard from PackedScene, debugBoard = {debugBoard.Name}");
                //playerViewport.AddChild(viewportScene.Instance());
                debugBoard.Visible = true;
                debugBoard.Connect("ready", this, nameof(_on_debugBoard_Ready));
                AddChild(debugBoard);
            }
        }

        private void _on_debugBoard_Ready()
        {
            GD.Print($"_on_debugBoard_Ready(), debugBoard = {debugBoard.Name}");
            debugBoard.SetTitle("Debug");
            SceneUtilities.PlaceControlTopLeft(this, debugBoard, new Vector2(820.0f, 20f));
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
            inGameMenuPanel = this.GetNodeOrNull<Control>("menu-GameContext");
            Diagnostics.PrintNullValueMessage(gameHelpPanel, "menu-GameContext");

            GenerateAttackCardView();
            GenerateDebugView();

            sceneTimer = this.GetNodeOrNull<Timer>("game1-Timer");
            Diagnostics.PrintNullValueMessage(sceneTimer, "sceneTimer");
            if (sceneTimer != null)
            {
                /* If Windows then reduce timer resolution */
                //sceneTimer.WaitTime = 0.1f;
                sceneTimer.Start();
                sceneTimer.Connect("timeout", this, nameof(_on_GameTimer_timeout));
            }
            InputAssistance.SetVerbose(true);

            SceneUtilities.PlaceControlTopLeft(this, gameHelpPanel, new Vector2(421.0f, 5f));
            SceneUtilities.PlaceControlTopLeft(this, inGameMenuPanel, new Vector2(325.0f, 155f));
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

        private void StartAnimatedIndicator(Int32 refTimeTicks)
        {
            //Shows an animation of a switch action being triggered
            //This will be less than 2 seconds of anim
        }

        private string PrintDebugToggleActions()
        {
            StringBuilder sb = new StringBuilder();
            if ((toggleEventsMap == null) || (toggleEventsMap.Count <= 0))
            {
                sb.Append($"Toggle Actions Map is empty\n");
            }
            else
            {
                sb.Append($"Toggle Actions Map, length = {toggleEventsMap.Count}\n");
                foreach (string key in toggleEventsMap.Keys)
                {
                    bool value = toggleEventsMap[key];
                    sb.Append($"Item for key = {key}, value = {value}\n");
                }
            }
            return sb.ToString();
        }

        private void DisplayDebugToggleInfo()
        {
            string res = PrintDebugToggleActions();
            if (string.IsNullOrEmpty(res) == false)
            {
                debugBoard.SetParagraphText(1, res);
            }
        }

        private bool IsToggleOnForActions(string[] actionsList)
        {
            bool res = false;
            if ( (actionsList != null) && (toggleEventsMap != null) )
            {
                foreach (string action in actionsList)
                {
                    if (toggleEventsMap.ContainsKey(action))
                    {
                        res = toggleEventsMap[action];
                        if (res)
                            return res;
                    }
                }
            }
            return res;
        }

        private void ToggleActionFor(string action, bool toggleState)
        {
            if (toggleEventsMap != null)
            {
                //toggleEventsMap.Remove(action);
                toggleEventsMap[action] = toggleState;
            }
        }

        private bool DisplayPlayerSwitchAction()
        {
            bool res = true;
            //DateTime refTime = DateTime.Now;
            string[] otherEvents = { "toggle_event_gamemenu", "toggle_event_help" };
            DisplayDebugToggleInfo();
            if (IsToggleOnForActions(null) == false)
            {
                Int32 currTicks = System.Environment.TickCount;
                toggleVisibleAttackCard = !toggleVisibleAttackCard;
                GD.Print($"DisplayPlayerSwitchAction() called, currTicks = {currTicks}");
                ToggleActionFor("toggle_event_cardupdate", toggleVisibleAttackCard);
                /*
                if (toggleVisibleAttackCard)
                    StartAnimatedIndicator(currTicks);
                    */                   
            }
            return res;
        }

        void DisplayHelpPanel()
        {
            string[] otherEvents = { "toggle_event_gamemenu", "toggle_event_cardupdate"};

            if ( (gameHelpPanel != null) && (IsToggleOnForActions(otherEvents) == false) )
            {
                gameHelpPanel.Visible = !gameHelpPanel.Visible;
                ToggleActionFor("toggle_event_help", gameHelpPanel.Visible);
                //help panel will have close button with signal ...
            }
            DisplayDebugToggleInfo();
        }

        private bool ShowInGameMenu()
        {
            bool res = false;

            if (inGameMenuPanel != null)
            {
                string[] otherEvents = { "toggle_event_help", "toggle_event_cardupdate" };
                if ( (inGameMenuPanel != null) && (IsToggleOnForActions(otherEvents) == false) )
                {
                    inGameMenuPanel.Visible = !inGameMenuPanel.Visible;
                    ToggleActionFor("toggle_event_gamemenu", inGameMenuPanel.Visible);
                }
            }
            else
                SceneUtilities.ExitApplication(this);
            DisplayDebugToggleInfo();
            return res;
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            if (Input.IsActionPressed("ingame_menu"))
            {
                if (InputAssistance.KeyBounceCheckAlternative("INGAME_MENU_ACTION", 0.55f, 0.8f))
                {
                    ShowInGameMenu();
                }
                return;
            }

            if (Input.IsActionPressed("help_action"))
            {
                if (InputAssistance.KeyBounceCheckAlternative("HELP_ACTION", 0.55f, 0.8f))
                {
                    DisplayHelpPanel();
                }
            }

            if (Input.IsActionPressed("toggle_switch_action"))
            {
                if (InputAssistance.KeyBounceCheckAlternative("TOGGLE_SWITCH", 0.55f, 0.8f))
                {
                    DisplayPlayerSwitchAction();
                    return;
                }
            }
        }

    }
}
