# NXTCacheReader
A simple C# program to help read the RuneScape NXT client cache files

## Usage

1. Download, install and run the RuneScape NXT client from the official website
2. Let the client download the cache. By default, the location of the cache will be in C:\ProgramData\Jagex\Runescape
3. Create the CacheReader object with the cache's path:
```
CacheReader cacheReader = new CacheReader(@"C:\ProgramData\Jagex\RuneScape");
```

#### Examples
##### Read a single item definition from the cache and print the name
```
Definition.Item bluePartyhat = cacheReader.LoadDefinition<Definition.Item>(1042);
Console.WriteLine(bluePartyhat.Name);
```
Output:
```
Blue partyhat
```

##### Load several item definitions
```
Dictionary<int, Definition.Item> itemDefinitions = cacheReader.LoadDefinitions<Definition.Item>(1038, 1040, 1042, 1044, 1046, 1048);
```

##### Load all item definitions
```
Dictionary<int, Definition.Item> itemDefinitions = cacheReader.LoadDefinitions<Definition.Item>();
```

CacheReader implements its own caching system to make subsequent definition retrieval faster.

## Support
Currently only some of the cache is recognized and supported.

##### Items
- Actions
- Armour bonus
- Attack speed
- Bank actions
- Consumption life points quantity
- Cosmetic item ID
- Cosmetic template ID
- Creation experience quantities
- Creation required item IDs
- Creation required item quantities
- Creation required skill IDs
- Creation required tool item IDs
- Destroy text
- Degredation item ID
- Diango replacement cost
- Drop sound ID
- Equip actions
- Equip magic ratio
- Equip melee ratio
- Equip ranged ratio
- Equip requirement skill ID
- Equip requirement skill level
- Equip slot ID
- Equip sound ID
- Grand Exchange category ID
- Ground actions
- Is alchable
- Is bankable
- Is cosmetic
- Is degradable
- Is dungeoneering item
- Is lent
- Is magic armour
- Is magic weapon
- Is melee armour
- Is melee weapon
- Is members
- Is noted
- Is ranged armour
- Is raned weapon
- Is stackable
- Is tradable
- Lent item ID
- Lent template ID
- Life point bonus
- Magic accuracy
- Magic damage
- Max charges
- Melee accuracy
- Melee damage
- Model ID
- Model zoom
- Name
- Note item ID
- Note template ID
- Prayer bonus
- Ranged accuracy
- Ranged damage
- Repair cost
- Tier
- Treasure Hunter cash out value
- Treasure Hunter text
- Use required skill ID
- Use required skill level

##### Npcs
- Actions
- Armour
- Attack speed
- Combat level
- Combat style ID
- Is clickable
- Is visible
- Magic accuracy
- Magic damage
- Melee accuracy
- Melee damage
- Name
- Ranged accuracy
- Ranged damage
- Weakness ID

##### Objects
- Actions
- Height
- Is walkable
- Name
- Width

##### Widgets
- Actions
- Animation ID
- Base height
- Base position X
- Base position Y
- Base width
- Content type
- Is hidden
- Is hover disabled
- Parent ID
- Scroll width
- Scroll height
- Sprite pitch
- Sprite roll
- Sprite scale
- Sprite yaw
- Text
- Textcolor
- Type
