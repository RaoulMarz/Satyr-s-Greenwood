using Godot;
using System;

namespace Satyrs_Greenwood
{
    public class SceneUtilities : Node
    {
        public PackedScene newScene;

        public void CleanPreviousScenes(Spatial referenceScene, String callSource = "")
        {
            int numChildren = referenceScene.GetTree().Root.GetChildCount();
            GD.Print($"CleanPreviousScenes() - scenes count = {numChildren}, called from {callSource} \n");
            Node previousScene = referenceScene.GetTree().Root.GetChild(0);
            /*
            var props = GetProperties(previousScene);
            if (props.Count > 0)
            {
                PrintObjectProperties("previousScene", introTimer);
            }
            */
            referenceScene.GetTree().Root.RemoveChild(previousScene);
            //previousScene.Free();
        }

        public static void DebugPrintScenesList(Spatial referenceScene)
        {
            int numChildren = referenceScene.GetTree().Root.GetChildCount();
            if (numChildren > 0)
            {
                GD.Print($"DebugPrintScenesList - scenes count = {numChildren}\n");
                int idx = 0;
                foreach (object sceneX in referenceScene.GetTree().Root.GetChildren())
                {
                    Diagnostics.PrintObjectProperties($"scene [{idx}] = ", sceneX);
                    idx += 1;
                }
            }
        }

        public void ChangeScene(Spatial referenceScene, string scenePath)
        {
            newScene = (PackedScene)ResourceLoader.Load(scenePath);
            if (newScene != null)
                referenceScene.GetTree().Root.AddChild(newScene.Instance());
        }

        public static void LinkSceneToViewport(string scenePath, Viewport parentViewport)
        {
            if (parentViewport != null)
            {
                PackedScene viewportScene = (PackedScene)ResourceLoader.Load(scenePath);
                if (viewportScene != null)
                {
                    //Link to the parent Viewport
                    ViewportFrameInterface.HookupExistingNode(parentViewport, (Node2D)viewportScene.Instance());
                }
            }
        }

        public static void ExitApplication(/*Spatial*/Node referenceScene)
        {
            referenceScene.GetTree().Quit();
        }

        public static void SetCameraPosition(Camera aCamera, Vector3 newPosition)
        {
            if ((aCamera != null) && (newPosition != null))
            {
                aCamera.Translation = newPosition;
            }
        }

        public static Godot.Vector3 CalculateCirclePosition(Godot.Vector3 circlePosition, float radiusCircle, float angle)
        {
            Godot.Vector3 res = new Vector3();
            if ((circlePosition != null) && (radiusCircle >= 0.1f))
            {

            }
            return res;
        }

        public static void MoveCameraAroundPosition(Camera aCamera, Godot.Vector3 circlePosition, float radiusCircle, int step)
        {
            Godot.Vector3 circleCoord = CalculateCirclePosition(circlePosition, radiusCircle, step / 1.0f);
            aCamera.Translation = circleCoord;
        }

        public static Rect2 GetApplicationWindowExtent(Spatial referenceScene)
        {
            Rect2 res;
            res = referenceScene.GetTree().Root.GetVisibleRect();
            return res;
        }

        public static Vector2 GetExtentOffsetsForCenter(Spatial referenceScene, Control graphicsControl)
        {
            Vector2 res = new Vector2();
            if (graphicsControl != null)
            {
                Rect2 appExtent = GetApplicationWindowExtent(referenceScene);
                if (appExtent.Size.x >= 100.0f)
                {
                    res = new Vector2(appExtent.Size.x, appExtent.Size.y);
                }
            }
            return res;
        }

        public static void PlaceControlTopLeft(Spatial referenceScene, Control graphicsControl, Vector2 placePosition)
        {
            if ( (graphicsControl != null) && (placePosition != null) )
            {
                Rect2 appExtent = GetApplicationWindowExtent(referenceScene);
                if (appExtent.Size.x >= 100.0f)
                {
                    Vector2 controlExtent = graphicsControl.RectSize;
                    graphicsControl.RectPosition = placePosition;
                }
            }
        }
    }
}
