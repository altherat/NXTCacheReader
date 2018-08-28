using System;
using System.Collections.Generic;
using System.Linq;

namespace NXTCacheReader
{
    class Program
    {

        private static CacheReader cacheReader;

        static void Main(string[] args)
        {

            // Instantiate the cache reader
            cacheReader = new CacheReader(@"C:\ProgramData\Jagex\\RuneScape");

            // Load the game object with ID 69860
            Definition.Object objectDefinition = cacheReader.LoadDefinition<Definition.Object>(69860);
            // Print the object's name
            Console.WriteLine(objectDefinition.Name);

            // Load Npcs with IDs 1382 and 12404
            Dictionary<int, Definition.Npc> npcDefinitions = cacheReader.LoadDefinitions<Definition.Npc>(1382, 12404);
            // Print the Npcs' names
            foreach (Definition.Npc definition in npcDefinitions.Values)
            {
                Console.WriteLine(definition.Name);
            }

            // Load all item definitions
            Dictionary<int, Definition.Item> itemDefinitions = cacheReader.LoadDefinitions<Definition.Item>();
            // Find the item with name "Blue partyhat"
            Definition.Item bluePartyhat = itemDefinitions.Values.First(def => def.Name == "Blue partyhat");
            // Print the item's actions
            Console.WriteLine(String.Join(", ", bluePartyhat.Actions));

            //Quick chat demo
            Console.WriteLine("\nQuick chat demo:");
            QuickChatDemo();

            Console.ReadLine();
        }

        private static void QuickChatDemo()
        {
            Dictionary<int, Definition.QuickChat> definitions = cacheReader.LoadDefinitions<Definition.QuickChat>();
            int id = 85;
            while (true)
            {
                Definition.QuickChat definition = definitions[id];
                int[] subMenuIds = definition.SubMenuIds;
                int[] subOptionIds = definition.SubOptionIds;
                if (definition.IsMenu)
                {
                    int subMenuCount;
                    int subOptionCount;
                    if (subMenuIds == null)
                    {
                        subMenuCount = 0;
                    }
                    else
                    {
                        subMenuCount = subMenuIds.Length;
                        for (int i = 0; i < subMenuCount; i++)
                        {
                            Console.WriteLine(i + ": " + definitions[subMenuIds[i]].Text + " ->");
                        }
                    }
                    if (subOptionIds == null)
                    {
                        subOptionCount = 0;
                    }
                    else
                    {
                        subOptionCount = subOptionIds.Length;
                        for (int i = 0; i < subOptionCount; i++)
                        {
                            Console.WriteLine((subMenuCount + i) + ": " + definitions[subOptionIds[i]].Text);
                        }
                    }
                    Console.Write("\nSelect option: ");
                    id = int.Parse(Console.ReadLine());
                    if (id >= subMenuCount)
                    {
                        id = subOptionIds[id - subMenuCount];
                    }
                    else
                    {
                        id = subMenuIds[id];
                    }
                }
                else
                {
                    Console.WriteLine("\nSelected option: " + definition.Text);
                    if (subOptionIds == null)
                    {
                        Console.WriteLine("Quick-reply options: None");
                    }
                    else {
                        int quickReplyCount = subOptionIds.Length;
                        Console.WriteLine("Quick-reply options: " + quickReplyCount);
                        for (int i = 0; i < quickReplyCount; i++)
                        {
                            Console.WriteLine("\t" + i + ": " + definitions[subOptionIds[i]].Text);
                        }
                    }
                    break;
                }
            }
        }
        
    }
}
