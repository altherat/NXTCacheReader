# NXTCacheReader
A C# library to help read the RuneScape NXT client cache files

## Usage

1. Download, install and run the RuneScape NXT client from the official website
2. Let the client download the cache. By default, the location of the cache will be in C:\ProgramData\Jagex\Runescape
3. Create the CacheReader object with the cache's path:
```
CacheReader cacheReader = new CacheReader(@"C:\ProgramData\Jagex\RuneScape");
```

#### Read a single item definition from the cache and print the name
```
Definition.Item bluePartyhat = cacheReader.LoadDefinition<Definition.Item>(1042);
Console.WriteLine(bluePartyhat.Name);
```
Output:
```
Blue partyhat
```

#### Load several item definitions
```
Dictionary<int, Definition.Item> itemDefinitions = cacheReader.LoadDefinitions<Definition.Item>(1038, 1040, 1042, 1044, 1046, 1048);
```

#### Load all item definitions
```
Dictionary<int, Definition.Item> itemDefinitions = cacheReader.LoadDefinitions<Definition.Item>();
```

CacheReader implements its own cache system to make subsequent definition retrieval faster.

## Support
Currently only some of the cache is recognized and supported.

##### Objects
- Actions
- Name

##### Npcs
- Actions
- Combat level
- Is clickable
- Is visible
- Name

##### Items
- Actions
- Cosmetic ID
- Cosmetic template ID
- Ground actions
- Is cosmetic
- Is lent
- Is members
- Is noted
- Is stackable
- Is tradeable
- Lent ID
- Lent template ID
- Name
- Note ID
- Note template ID
