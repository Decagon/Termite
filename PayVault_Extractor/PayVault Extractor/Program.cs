using Newtonsoft.Json;
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace PayVault_Extractor
{
    class Program
    {
        /// <summary>
        /// The global connection
        /// </summary>
        public static Connection globalConn = null;
        /// <summary>
        /// The global client
        /// </summary>
        public static Client globalClient = null;
        /// <summary>
        /// The shop data item length
        /// </summary>
        const uint ITEM_LENGTH = 25;

        public const string saveLocation = @"C:\\Users\username\Desktop\PayVault.txt";

        public const string email = "guest";
        public const string password = "guest";

        public static StreamWriter file = new StreamWriter(saveLocation);
        public static int longestStringLength { get; set; }

        static void Main(string[] args)
        {
            PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", email, password, null, delegate (Client localClient)
            {
                globalClient = localClient;
                localClient.Multiplayer.CreateJoinRoom(localClient.ConnectUserId, "Lobby" + localClient.BigDB.Load("config", "config")["version"], true, null, null, delegate (Connection conn)
                {
                    globalConn = conn;
                    conn.OnMessage += Conn_OnMessage;
                });
            });

            Thread.Sleep(Timeout.Infinite);
        }

        private static void Conn_OnMessage(object sender, Message e)
        {
            if (e.Type == "connectioncomplete")
            {
                globalClient.PayVault.Refresh();
                globalConn.Send("getShop");
            }
            else if (e.Type == "getShop")
            {
                getItems(e)
                    .Where(y =>
                    {
                        if (y.shopItem.Length > longestStringLength) longestStringLength = y.shopItem.Length;
                        return true;
                    }) // get longest item by piping every value into the function and always return true
                    .OrderBy(o => o.group) // alphabetize group names
                    .ThenBy(q => PadNumericPortion(q.shopItem)) // natural sort names ending with numbers
                    .GroupBy(x => x.group) // groups are unique; consolidate into one
                    .ToList() // execute query
                    .ForEach(items =>
                {
                    Console.WriteLine(items.Key + ":");

                    foreach (var item in items)
                    {
                        Console.WriteLine(
                            string.Format("     - {0} # {1}",
                            item.shopItem.PadRight(longestStringLength),
                            item.description)); // used to be longestStringLength
                    }

                });
            }
        }

        /// <summary>
        /// Pads the numeric portion of the shop item with zeros, so that sorting the
        /// values with LINQ will work correctly.
        /// </summary>
        /// <param name="q">The PayVault item.</param>
        /// <returns>A numerically padded shop item.</returns>
        private static string PadNumericPortion(string item)
        {
            //http://stackoverflow.com/questions/9788807/
            var numericPortion = Regex.Match(item, @"([0-9]+)$", RegexOptions.IgnorePatternWhitespace);

            return string.Format("{0}{1}",
                numericPortion.Value.PadLeft(longestStringLength, '0'),
                numericPortion.Success ?
                    numericPortion.Result(string.Empty) :
                    numericPortion.Value);

        }

        private static IEnumerable<PayVault_Item> getItems(Message e)
        {
            for (uint i = 4; i < (e.Count - 2); i = i + ITEM_LENGTH) // subtract 2 because it will go too far when splitting the shop items
            {
                yield return new PayVault_Item
                {
                    shopItem = Convert.ToString(e[i + 1]),
                    group = Convert.ToString(e[i + 2]),
                    description = Convert.ToString(e[i + 10]) // use 9 to get desc
                };
            }

        }
    }

}


