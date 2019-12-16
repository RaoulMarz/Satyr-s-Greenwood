using Godot;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Satyrs_Greenwood
{
    public class StoryArcBoard : Control
    {
        private Panel panelStoryBoard;
        private RichTextLabel textTitle;
        private RichTextLabel textParagraph1;
        private RichTextLabel textParagraph2;
        private RichTextLabel textParagraph3;
        private StringCollection paragraphContent = new StringCollection();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print("Ready on StoryArcBoard");
            panelStoryBoard = this.GetNodeOrNull<Panel>("panelStoryBoard");
            if (panelStoryBoard != null)
            {
                Diagnostics.PrintObjectProperties("StoryArcBoard.panelStoryBoard", panelStoryBoard);
                Diagnostics.PrintChildrenList("StoryArcBoard.panelStoryBoard", panelStoryBoard);
                textTitle = panelStoryBoard.GetNodeOrNull<RichTextLabel>("X");
                Diagnostics.PrintNullValueMessage(textTitle, "StoryArcBoard.textTitle");
                textParagraph1 = panelStoryBoard.GetNodeOrNull<RichTextLabel>("textStory_paragraph1");
                Diagnostics.PrintNullValueMessage(textParagraph1, "StoryArcBoard.textParagraph1");
                textParagraph2 = panelStoryBoard.GetNodeOrNull<RichTextLabel>("textStory_paragraph2");
                textParagraph3 = panelStoryBoard.GetNodeOrNull<RichTextLabel>("textStory_paragraph3");
            }
        }

        public void DisplayParagraphText(int paragraphNumber)
        {
            if ((paragraphNumber >= 1) && (paragraphContent != null))
            {
                string paraText = paragraphContent[paragraphNumber - 1];
                if ( (paraText != null) && (textParagraph1 != null) )
                {
                    if (paragraphNumber == 1)
                        textParagraph1.BbcodeText = paragraphContent[paragraphNumber - 1];
                    if (paragraphNumber == 2)
                        textParagraph2.BbcodeText = paragraphContent[paragraphNumber - 1];
                    if (paragraphNumber == 3)
                        textParagraph3.BbcodeText = paragraphContent[paragraphNumber - 1];
                }
            }
        }

        public void SetTitle(string title)
        {
            Diagnostics.PrintNullValueMessage(textTitle, "StoryArcBoard.textTitle");
            if ( (textTitle != null) && (title != null) )
            {
                GD.Print($"StoryArcBoard SetTitle() called, textTitle = {textTitle.Name}");
                //labelTitle.Text = titleText;
                textTitle.BbcodeEnabled = true;
                textTitle.BbcodeText = $"[code]{title}[/code]";
                textTitle.Visible = true;
            }
        }

        public void SetParagraphText(int paragraphNumber, string paragraphText, bool showImmediate = true)
        {
            if (paragraphNumber >= 1)
            {
                paragraphContent.Insert(paragraphNumber - 1, paragraphText);
                if (showImmediate)
                {
                    DisplayParagraphText(paragraphNumber);
                }
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
