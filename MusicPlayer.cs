using Godot;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Satyrs_Greenwood
{

    public class MusicPlayer
    {
        private Dictionary<string, string> musicPlayList = new Dictionary<string, string>();

        public MusicPlayer()
        {
            //musicPlayList = new Dictionary<string, string>();
        }

        public void AddSong(string songDescriptor)
        {

        }

        public StringCollection GetItemsForPlaylist(string playlistTag)
        {
            StringCollection resultList = new StringCollection();

            return resultList;
        }
    }
}
