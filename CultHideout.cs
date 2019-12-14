using Godot;
using System;

namespace Satyrs_Greenwood
{
    public class CultHideout : Spatial
    {
        private Sprite3D hudPlayer;
        private AnimatedSprite3D animspriteFirePit;
        private Sprite3D spriteTreesLayer1;
        private Timer sceneTimer;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            //SceneUtilities.CleanPreviousScenes();
            sceneTimer = this.GetNodeOrNull<Timer>("intro-Timer");
            if (sceneTimer != null)
            {
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
