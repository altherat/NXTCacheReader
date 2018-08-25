using NXTCacheReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {

            // Instantiate the cache reader
            CacheReader cacheReader = new CacheReader(@"C:\ProgramData\Jagex\\RuneScape");

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

            Console.ReadLine();
        }
        
    }
}
