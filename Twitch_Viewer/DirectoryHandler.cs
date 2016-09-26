using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TwixelAPI;

namespace Twitch_Viewer
{
    public static class DirectoryHandler
    {
        private static Twixel twixel = new Twixel("mfsqshg9m7s9bw3fxyb6niddken7c39", "http://localhost");

        public static async Task<List<Game>> getGames(int offset = 0, int limit = 25)
        {
            List<Game> games = new List<Game>();

            Total<List<Game>> tmp = null;

            tmp = await twixel.RetrieveTopGames(offset, limit);

            games = tmp?.wrapped;

            return games;
        }

        public static async Task<List<Stream>> getChannels(int offset = 0, int limit = 25)
        {
            List<Stream> channels = new List<Stream>();

            Total<List<Stream>> tmp = null;

            try
            {
                tmp = await twixel.RetrieveStreams(offset: offset, limit: limit);
            }
            catch (Exception e)
            { Debugger.Log(1, "getChannelsFailed", e.Message + "\n"); }

            channels = tmp?.wrapped;

            return channels;
        }

        public static string getBoxImage(Game game)
        {
            string path;

            path = game.box["medium"].AbsoluteUri;

            return path;
        }
    }
}
