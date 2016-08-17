using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Twitch_Viewer.Types;

namespace Twitch_Viewer
{
    static class Extensions
    {
        /// <summary>
        /// Sort the ObservableCollection as per the IComparable impementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count; i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        /// <summary>
        /// Sort the ObsersableCollection by a given property
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="collection"></param>
        /// <param name="keySelector"></param>
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector, bool desc = false)
        {
            List<TSource> sorted;

            if (!desc)
                sorted = collection.OrderBy(keySelector).ToList();
            else
                sorted = collection.OrderByDescending(keySelector).ToList();

            for (int i = 0; i < sorted.Count; i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        /// <summary>
        /// Adds an item to this Collection if no item with the same name exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="newItem"></param>
        public static void AddIfNew<T>(this ObservableCollection<GameItem> collection, GameItem newItem) where T : GameItem
        {
            if (collection.Where(x => x.FullName == newItem.FullName).Count() == 0)
                collection.Add(newItem);
        }

        /// <summary>
        /// Adds an item to this Collection if no item with the same name exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="newItem"></param>
        public static void AddIfNew<T>(this ObservableCollection<StreamItem> collection, StreamItem newItem) where T : StreamItem
        {
            if (collection.Where(x => x.Name == newItem.Name).Count() == 0)
                collection.Add(newItem);
        }
    }
}
