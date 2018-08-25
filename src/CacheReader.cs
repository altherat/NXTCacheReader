using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using zlib;

namespace NXTCacheReader
{

    public class CacheReader
    {

        private readonly Dictionary<Type, int> CACHE_INDEXES = new Dictionary<Type, int>()
        {
            { typeof(Definition.Object), 16 },
            { typeof(Definition.Npc), 18 },
            { typeof(Definition.Item), 19 }
        };
        private const int FLAG_IDENTIFIERS = 0x1;
        private const int FLAG_WHIRLPOOL = 0x2;
        private const int FLAG_UNKNOWN_1 = 0x4;
        private const int FLAG_UNKNOWN_2 = 0x8;
        private const int WHIRLPOOL_SIZE = 64;
        private static readonly char[] SPECIAL_CHARACTERS = { '\u20AC', '\0', '\u201A', '\u0192', '\u201E', '\u2026', '\u2020', '\u2021', '\u02C6', '\u2030', '\u0160', '\u2039', '\u0152', '\0', '\u017D', '\0', '\0', '\u2018', '\u2019', '\u201C', '\u201D', '\u2022', '\u2013', '\u2014', '\u02DC', '\u2122', '\u0161', '\u203A', '\u0153', '\0', '\u017E', '\u0178' };

        private string CacheDirectory;
        private Dictionary<Type, Dictionary<int, byte[]>> BlobBytes = new Dictionary<Type, Dictionary<int, byte[]>>();
        private Dictionary<Type, Dictionary<int, object>> LoadedDefinitions = new Dictionary<Type, Dictionary<int, object>>();

        public CacheReader(string cacheDirectory)
        {
            CacheDirectory = cacheDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        }

        public T LoadDefinition<T>(int id) where T : Definition
        {
            Type type = typeof(T);
            Dictionary<int, object> definitions;
            if (LoadedDefinitions.TryGetValue(type, out definitions))
            {
                object definition;
                if (!definitions.TryGetValue(id, out definition))
                {
                    definition = TryReadDefinition<T>(id);
                    definitions.Add(id, definition);
                }
                return (T)definition;
            }
            else
            {
                T definition = TryReadDefinition<T>(id);
                LoadedDefinitions[type] = new Dictionary<int, object>()
                {
                    {id, definition }
                };
                return definition;
            }
        }

        public Dictionary<int, T> LoadDefinitions<T>(params int[] ids) where T : Definition
        {
            Dictionary<int, T> definitions = new Dictionary<int, T>();
            foreach (int id in ids)
            {
                definitions.Add(id, LoadDefinition<T>(id));
            }
            return definitions;
        }

        public Dictionary<int, T> LoadDefinitions<T>() where T : Definition
        {
            Type type = typeof(T);
            Dictionary<int, T> definitions = new Dictionary<int, T>();
            Dictionary<int, object> loadedDefinitions;
            if (!LoadedDefinitions.TryGetValue(type, out loadedDefinitions))
            {
                loadedDefinitions = new Dictionary<int, object>();
                LoadedDefinitions[type] = loadedDefinitions;
            }
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + CacheDirectory + "js5-" + CACHE_INDEXES[typeof(T)] + ".jcache;Version=3;"))
            {
                connection.Open();
                Dictionary<int, List<int>> records = new Dictionary<int, List<int>>();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT DATA FROM cache_index";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    MemoryStream stream = new MemoryStream(DecompressBlob((byte[])reader[0]));
                    byte format = ReadByte(stream);
                    int version = ReadInt(stream);
                    byte flags = ReadByte(stream);
                    int[] entryKeys = DecodeIds(stream, ReadSmart(stream, 2, 4, false));
                    if ((flags & FLAG_IDENTIFIERS) == FLAG_IDENTIFIERS)
                    {
                        foreach (int key in entryKeys)
                        {
                            int identifier = ReadInt(stream);
                        }
                    }
                    foreach (int key in entryKeys)
                    {
                        int entryCrc = ReadInt(stream);
                    }
                    if ((flags & FLAG_UNKNOWN_2) == FLAG_UNKNOWN_2)
                    {
                        foreach (int key in entryKeys)
                        {
                            int unknown2Value = ReadInt(stream);
                        }
                    }
                    if ((flags & FLAG_WHIRLPOOL) == FLAG_WHIRLPOOL)
                    {
                        foreach (int key in entryKeys)
                        {
                            byte[] whirlpoolBytes = new byte[WHIRLPOOL_SIZE];
                            stream.Read(whirlpoolBytes, 0, WHIRLPOOL_SIZE);
                        }
                    }
                    if ((flags & FLAG_UNKNOWN_1) == FLAG_UNKNOWN_1)
                    {
                        foreach (int key in entryKeys)
                        {
                            int unknown1Value1 = ReadInt(stream);
                            int unknown1Value2 = ReadInt(stream);
                        }
                    }
                    foreach (int key in entryKeys)
                    {
                        int entryVersion = ReadInt(stream);
                    }
                    foreach (int key in entryKeys)
                    {
                        int childCount = ReadSmart(stream, 2, 4, false);
                        records.Add(key, new List<int>(childCount));
                    }
                    foreach (List<int> ids in records.Values)
                    {
                        foreach (int id in DecodeIds(stream, ids.Capacity))
                        {
                            ids.Add(id);
                        }
                    }
                    if ((flags & FLAG_IDENTIFIERS) == FLAG_IDENTIFIERS)
                    {
                        foreach (int key in entryKeys)
                        {
                            int childIdentifier = ReadInt(stream);
                        }
                    }
                    stream.Close();
                }
                reader.Close();
                command.CommandText = "SELECT KEY, DATA FROM cache";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int recordKey = reader.GetInt32(0);
                    byte[] blobBytes = DecompressBlob((byte[])reader[1]);
                    if (BlobBytes.ContainsKey(type))
                    {
                        BlobBytes[type][recordKey] = blobBytes;
                    }
                    else
                    {
                        BlobBytes[type] = new Dictionary<int, byte[]>()
                        {
                            {recordKey, blobBytes }
                        };
                    }
                    using (MemoryStream blobStream = new MemoryStream(blobBytes))
                    {
                        blobStream.Position = 1;
                        int nextPosition = -1;
                        foreach (int id in records[recordKey])
                        {
                            int actualId = (recordKey << 8) + id;
                            int startPosition = nextPosition == -1 ? ReadInt(blobStream) : nextPosition;
                            nextPosition = ReadInt(blobStream);
                            long prevPosition = blobStream.Position;
                            blobStream.Position = startPosition;
                            byte[] data = new byte[nextPosition - startPosition];
                            blobStream.Read(data, 0, data.Length);
                            T definition = (T)Activator.CreateInstance(typeof(T), new object[] { actualId });
                            definition.Deserialize(this, data);
                            loadedDefinitions[actualId] = definition;
                            definitions.Add(actualId, definition);
                            blobStream.Position = prevPosition;
                        }
                    }
                }
            }
            return definitions;
        }

        private T ReadDefinition<T>(Type type, int id, byte[] blobBytes) where T : Definition
        {
            using (MemoryStream stream = new MemoryStream(blobBytes))
            {
                stream.Position = 1 + (id & 0xFF) * 4;
                int startPosition = ReadInt(stream);
                int endPosition = ReadInt(stream);
                stream.Position = startPosition;
                byte[] bytes = new byte[endPosition - startPosition];
                stream.Read(bytes, 0, bytes.Length);
                T definition = (T)Activator.CreateInstance(typeof(T), new object[] { id });
                definition.Deserialize(this, bytes);
                return definition;
            }
        }

        private byte[] ReadCacheData(int cacheIndex, int key)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + CacheDirectory + "js5-" + cacheIndex + ".jcache;Version=3;"))
            {
                connection.Open();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT DATA FROM cache WHERE KEY = " + key;
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return DecompressBlob((byte[])reader[0]);
                }
            }
            return null;
        }

        private T TryReadDefinition<T>(int id) where T : Definition
        {
            Type type = typeof(T);
            int recordKey = id >> 8;
            Dictionary<int, byte[]> cacheFileBytes;
            if (BlobBytes.TryGetValue(type, out cacheFileBytes))
            {
                byte[] blobBytes;
                if (!cacheFileBytes.TryGetValue(recordKey, out blobBytes))
                {
                    blobBytes = ReadCacheData(CACHE_INDEXES[type], recordKey);
                    BlobBytes[type][recordKey] = blobBytes;
                }
                return ReadDefinition<T>(type, id, blobBytes);
            }
            else
            {
                byte[] blobBytes = ReadCacheData(CACHE_INDEXES[type], recordKey);
                BlobBytes[type] = new Dictionary<int, byte[]>()
                {
                    { recordKey, blobBytes }
                };
                return ReadDefinition<T>(type, id, blobBytes);
            }
        }

        private static byte[] DecompressBlob(byte[] inData)
        {
            using (MemoryStream outStream = new MemoryStream())
            using (MemoryStream inStream = new MemoryStream(inData))
            using (ZOutputStream outZStream = new ZOutputStream(outStream))
            {
                inStream.Seek(8, 0);
                byte[] buffer = new byte[16 * 1024];
                int len;
                while ((len = inStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outZStream.Write(buffer, 0, len);
                }
                outZStream.Flush();
                outZStream.finish();
                return outStream.ToArray();
            }
        }

        private int[] DecodeIds(MemoryStream stream, int size)
        {
            int[] ids = new int[size];
            int accumulator = 0;
            for (int i = 0; i < ids.Length; i++)
            {
                int delta = ReadSmart(stream, 2, 4, false);
                accumulator += delta;
                ids[i] = accumulator;
            }
            return ids;
        }

        internal Dictionary<int, object> Decode249(MemoryStream stream)
        {
            Dictionary<int, object> parameters = new Dictionary<int, object>();
            byte size = ReadUByte(stream);
            for (int i = 0; i < size; i++)
            {
                bool unknown = ReadUByte(stream) == 1;
                int key = (int)ReadUInt24(stream);
                parameters[key] = unknown ? (object)ReadString(stream) : ReadInt(stream);
            }
            return parameters;
        }

        internal int ReadSmart(MemoryStream stream)
        {
            return ReadSmart(stream, 1, 2, false);
        }

        internal int ReadSignedSmart(MemoryStream stream)
        {
            return ReadSmart(stream, 1, 2, true);
        }

        internal int ReadSmart(MemoryStream stream, int smallSize, int bigSize, bool signed)
        {
            byte num = ReadByte(stream);
            int size = ((num & 128) == 128 ? bigSize : smallSize);
            return JoinSmart(stream, num, size, signed);
        }

        internal int ReadBigSmart(MemoryStream stream)
        {
            byte first = ReadByte(stream);
            int joined = 0;
            if ((first & 128) != 128)
            {
                joined = JoinSmart(stream, first, 2, false);
                if (joined == 0x7FFF)
                {
                    joined = -1;
                }
            }
            else
            {
                joined = JoinSmart(stream, first, 4, false);
            }
            return joined;
        }

        internal int JoinSmart(MemoryStream stream, int first, int size, bool signed)
        {
            int num = first & ~128;
            for (int i = 1; i < size; ++i)
            {
                num = (num << 8) | ReadByte(stream);
            }
            if (signed)
            {
                num -= (1 << size * 8 - 2);
            }
            return num;
        }

        internal byte ReadByte(MemoryStream stream)
        {
            return (byte)stream.ReadByte();
        }

        internal byte ReadUByte(MemoryStream stream)
        {
            return (byte)(ReadByte(stream) & 0xFF);
        }

        internal short ReadShort(MemoryStream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        internal ushort ReadUShort(MemoryStream stream)
        {
            return (ushort)ReadShort(stream);
        }

        internal int ReadInt24(MemoryStream stream)
        {
            byte[] buffer = new byte[3];
            stream.Read(buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            return buffer[0] + (buffer[1] << 8) + (buffer[2] << 16);
        }

        internal uint ReadUInt24(MemoryStream stream)
        {
            return (uint)ReadInt24(stream);
        }

        internal int ReadInt(MemoryStream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        internal uint ReadUInt(MemoryStream stream)
        {
            return (uint)ReadInt(stream);
        }

        internal string ReadString(MemoryStream stream)
        {
            string toReturn = "";
            char c;
            while ((c = (char)stream.ReadByte()) != 0)
            {
                if (c >= 128 && c < 160)
                {
                    char specialChar = SPECIAL_CHARACTERS[c - 128];
                    if (specialChar == 0)
                    {
                        specialChar = '?';
                    }
                    c = specialChar;
                }
                toReturn += c;
            }
            return toReturn == "null" ? null : toReturn;
        }

    }

}
