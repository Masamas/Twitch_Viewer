using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twitch_Viewer
{
    public class Directory
    {
        private ObservableCollection<GameItem> _games = new ObservableCollection<GameItem>();
        private ObservableCollection<GameItem> _gamesFiltered = new ObservableCollection<GameItem>();
        private ObservableCollection<StreamItem> _gameStreams = new ObservableCollection<StreamItem>();

        private readonly object _lockObjectGames = new object();
        private readonly object _lockObjectGameStreams = new object();

        public ObservableCollection<GameItem> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                BindingOperations.EnableCollectionSynchronization(_games, _lockObjectGames);
            }
        }
        
        public ObservableCollection<GameItem> GamesFiltered
        {
            get { return _gamesFiltered; }
            set
            {
                _gamesFiltered = value;
                BindingOperations.EnableCollectionSynchronization(_gamesFiltered, _lockObjectGames);
            }
        }

        public ObservableCollection<StreamItem> GameStreams
        {
            get { return _gameStreams; }
            set
            {
                _gameStreams = value;
                BindingOperations.EnableCollectionSynchronization(_gameStreams, _lockObjectGameStreams);
            }
        }
    }
}
