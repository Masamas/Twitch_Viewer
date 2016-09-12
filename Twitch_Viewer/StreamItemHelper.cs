using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwixelAPI;

namespace Twitch_Viewer
{
    public static class StreamItemHelper
    {
        private static Twixel twixel = new Twixel(Twixel.clientID, "http://localhost");

        public static async Task<TwixelAPI.Stream> getStream(string name)
        {
            Stream stream = null;

            try
            {
                stream = await twixel.RetrieveStream(name);
            }
            catch (Exception e)
            { Debugger.Log(1, "StreamOffline", e.Message + "\n"); }

            return stream;
        }

        public static async Task<List<Stream>> getStreams(string game = null, List<string> channels = null, int offset = 0, int limit = 25)
        {
            Total<List<Stream>> streams = null;

            try
            {
                streams = await twixel.RetrieveStreams(game, channels, offset, limit);
            }
            catch (Exception e)
            { Debugger.Log(1, "StreamsNotFound", e.Message + "\n"); }

            return streams.wrapped;
        }

        public static async Task<TwixelAPI.Channel> getChannel(string name, TwixelAPI.Stream stream = null)
        {
            if (stream != null)
                return stream.channel;

            Channel channel = null;

            try
            {
                channel = await twixel.RetrieveChannel(name);
            }
            catch (Exception e)
            { Debugger.Log(1, "ChannelNotAvailable", e.Message + "\n"); }

            return channel;
        }

        public static async Task<TwixelAPI.User> getUser(string name)
        {
            User user = null;

            try
            {
                user = await twixel.RetrieveUser(name);
            }
            catch (Exception e)
            { Debugger.Log(1, "UserNotFound", e.Message + "\n"); }

            return user;
        }

        public static async Task<List<string>> getFollowedChannels(string userName)
        {
            List<string> followedChannelNames = new List<string>();
            User user = await twixel.RetrieveUser(userName);

            for (int i = 0; i < 1000; i++)
            {
                var tmp = await user.RetrieveFollowing(i * 50, 50);
                if (tmp.wrapped.Count == 0)
                    break;

                var followList = tmp.wrapped;
                foreach (Follow<Channel> follow in followList)
                {
                    followedChannelNames.Add(follow.wrapped.name);
                }
            }

            return followedChannelNames;
        }

        public static string getPreview(Stream stream, Channel channel)
        {
            string path;

            path = stream != null ? stream.previewList["medium"].AbsoluteUri : channel?.videoBanner != null ? channel.videoBanner.AbsoluteUri : "imageResources/Error.png";

            return path;
        }
    }
}
