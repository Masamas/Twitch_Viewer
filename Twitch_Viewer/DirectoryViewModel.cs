using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Viewer
{
    public class DirectoryViewModel
    {
        Directory _directory = new Directory();

        public Directory Directory
        {
            get { return _directory; }
            set
            {
                if (_directory != value)
                    _directory = value;
            }
        }

        public ObservableCollection<GameItem> Games
        {
            get { return Directory.Games; }
            set
            {
                if (Directory.Games != value)
                    Directory.Games = value;
            }
        }

        public ObservableCollection<GameItem> GamesFiltered
        {
            get { return Directory.GamesFiltered; }
            set
            {
                if (Directory.GamesFiltered != value)
                    Directory.GamesFiltered = value;
            }
        }

        public ObservableCollection<StreamItem> GameStreams
        {
            get { return Directory.GameStreams; }
            set
            {
                if (Directory.GameStreams != value)
                    Directory.GameStreams = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryViewModel(Directory directory)
        {
            Directory = directory;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
