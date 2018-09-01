using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using zlib;

namespace NXTTools
{

    public class CacheReader
    {

        private const string DEFAULT_CACHE_DIRECTORY = @"C:\ProgramData\Jagex\RuneScape";
        private readonly Dictionary<Type, Index> CACHE_INDEXES = new Dictionary<Type, Index>()
        {
            { typeof(Definition.Quest), new Index(2, 35, 8) },
            { typeof(Definition.WorldMap), new Index(2, 36, 8) },
            { typeof(Definition.Component), new Index(3, -1, 8) },
            { typeof(Definition.Object), new Index(16, -1, 8) },
            { typeof(Definition.Npc), new Index(18, -1, 7) },
            { typeof(Definition.Item), new Index(19, -1, 8) },
            { typeof(Definition.QuickChat), new Index(24, -1, 8) }
        };
        private const int FLAG_IDENTIFIERS = 0x1;
        private const int FLAG_WHIRLPOOL = 0x2;
        private const int FLAG_UNKNOWN_1 = 0x4;
        private const int FLAG_UNKNOWN_2 = 0x8;
        private const int WHIRLPOOL_SIZE = 64;
        private static readonly char[] SPECIAL_CHARACTERS = { '\u20AC', '\0', '\u201A', '\u0192', '\u201E', '\u2026', '\u2020', '\u2021', '\u02C6', '\u2030', '\u0160', '\u2039', '\u0152', '\0', '\u017D', '\0', '\0', '\u2018', '\u2019', '\u201C', '\u201D', '\u2022', '\u2013', '\u2014', '\u02DC', '\u2122', '\u0161', '\u203A', '\u0153', '\0', '\u017E', '\u0178' };

        private readonly string CacheDirectory;
        private Dictionary<Type, Dictionary<int, Record>> Records = new Dictionary<Type, Dictionary<int, Record>>();
        private Dictionary<Type, Dictionary<int, object>> LoadedDefinitions = new Dictionary<Type, Dictionary<int, object>>();

        public CacheReader(string cacheDirectory = DEFAULT_CACHE_DIRECTORY)
        {
            CacheDirectory = cacheDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        }

        public Dictionary<int, T> LoadDefinitions<T>() where T : Definition
        {
            Type type = typeof(T);
            Index index = CACHE_INDEXES[typeof(T)];
            int idNumBits = index.IdNumBits;
            int recordKey = index.RecordKey;
            Dictionary<int, T> definitions = new Dictionary<int, T>();
            if (!LoadedDefinitions.TryGetValue(type, out Dictionary<int, object> loadedDefinitions))
            {
                loadedDefinitions = new Dictionary<int, object>();
                LoadedDefinitions[type] = loadedDefinitions;
            }
            if (!Records.TryGetValue(type, out Dictionary<int, Record> records))
            {
                records = new Dictionary<int, Record>();
                Records[type] = records;
            }
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + CacheDirectory + "js5-" + index.FileId + ".jcache;Version=3;"))
            {
                connection.Open();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT DATA FROM cache_index";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    using (MemoryStream stream = new MemoryStream(DecompressBlob((byte[])reader[0])))
                    {
                        stream.Position = 5;
                        byte flags = ReadByte(stream);
                        int[] recordKeys = DecodeIds(stream, ReadSmart(stream, 2, 4, false));
                        int recordKeyCount = recordKeys.Length;
                        long position = stream.Position;
                        if ((flags & FLAG_IDENTIFIERS) == FLAG_IDENTIFIERS)
                        {
                            position += recordKeyCount * 4;
                        }
                        if ((flags & FLAG_UNKNOWN_2) == FLAG_UNKNOWN_2)
                        {
                            position += recordKeyCount * 4;
                        }
                        if ((flags & FLAG_WHIRLPOOL) == FLAG_WHIRLPOOL)
                        {
                            position += recordKeyCount * WHIRLPOOL_SIZE;
                        }
                        if ((flags & FLAG_UNKNOWN_1) == FLAG_UNKNOWN_1)
                        {
                            position += recordKeyCount * 8;
                        }
                        stream.Position = position + recordKeyCount * 8;
                        foreach (int key in recordKeys)
                        {
                            records[key] = new Record(new List<int>(ReadSmart(stream, 2, 4, false)));
                        }
                        foreach (int key in recordKeys)
                        {
                            List<int> ids = records[key].Ids;
                            foreach (int id in DecodeIds(stream, ids.Capacity))
                            {
                                ids.Add(id);
                            }
                        }
                    }
                    
                }
                reader.Close();
                command.CommandText = "SELECT KEY, DATA FROM cache" + (recordKey == -1 ? "" : " WHERE KEY = " + recordKey);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int currentRecordKey = reader.GetInt32(0);
                    if (records.TryGetValue(currentRecordKey, out Record record))
                    {
                        byte[] blobBytes = DecompressBlob((byte[])reader[1]);
                        record.Bytes = blobBytes;
                        using (MemoryStream blobStream = new MemoryStream(blobBytes))
                        {
                            blobStream.Position = 1;
                            int nextPosition = -1;
                            foreach (int id in record.Ids)
                            {
                                int actualId = recordKey == -1 ? (currentRecordKey << idNumBits) + id : id;
                                int startPosition = nextPosition == -1 ? ReadInt(blobStream) : nextPosition;
                                nextPosition = ReadInt(blobStream);
                                long prevPosition = blobStream.Position;
                                blobStream.Position = startPosition;
                                byte[] bytes = new byte[nextPosition - startPosition];
                                blobStream.Read(bytes, 0, bytes.Length);
                                T definition = (T)Activator.CreateInstance(typeof(T), new object[] { actualId });
                                definition.Deserialize(this, bytes);
                                loadedDefinitions[actualId] = definition;
                                definitions[actualId] = definition;
                                blobStream.Position = prevPosition;
                            }
                        }
                    }
                }
            }
            return definitions;
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

        public T LoadDefinition<T>(int id) where T : Definition
        {
            Type type = typeof(T);
            if (LoadedDefinitions.TryGetValue(type, out Dictionary<int, object> definitions))
            {
                if (!definitions.TryGetValue(id, out object definition))
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

        private T TryReadDefinition<T>(int id) where T : Definition
        {
            Type type = typeof(T);
            int idNumBits = CACHE_INDEXES[type].IdNumBits;
            int recordKey = id >> idNumBits;
            if (Records.TryGetValue(type, out Dictionary<int, Record> records))
            {
                if (!records.TryGetValue(recordKey, out Record record))
                {
                    record = ReadRecord(CACHE_INDEXES[type].FileId, recordKey);
                    records[recordKey] = record;
                }
                return ReadDefinition<T>(type, id, record, idNumBits);
            }
            else
            {
                Record record = ReadRecord(CACHE_INDEXES[type].FileId, recordKey);
                Records[type] = new Dictionary<int, Record>()
                {
                    { recordKey, record }
                };
                return ReadDefinition<T>(type, id, record, idNumBits);
            }
        }

        private T ReadDefinition<T>(Type type, int id, Record record, int idNumBits) where T : Definition
        {
            using (MemoryStream stream = new MemoryStream(record.Bytes))
            {
                stream.Position = 1 + record.Ids.IndexOf(id & (1 << idNumBits) - 1) * 4;
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

        private Record ReadRecord(int fileId, int recordKey)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + CacheDirectory + "js5-" + fileId + ".jcache;Version=3;"))
            {
                connection.Open();
                Dictionary<int, List<int>> recordIds = new Dictionary<int, List<int>>();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT DATA FROM cache_index";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    using (MemoryStream stream = new MemoryStream(DecompressBlob((byte[])reader[0])))
                    {
                        stream.Position = 5;
                        byte flags = ReadByte(stream);
                        int[] recordKeys = DecodeIds(stream, ReadSmart(stream, 2, 4, false));
                        int recordKeyCount = recordKeys.Length;
                        long position = stream.Position;
                        if ((flags & FLAG_IDENTIFIERS) == FLAG_IDENTIFIERS)
                        {
                            position += recordKeyCount * 4;
                        }
                        if ((flags & FLAG_UNKNOWN_2) == FLAG_UNKNOWN_2)
                        {
                            position += recordKeyCount * 4;
                        }
                        if ((flags & FLAG_WHIRLPOOL) == FLAG_WHIRLPOOL)
                        {
                            position += recordKeyCount * WHIRLPOOL_SIZE;
                        }
                        if ((flags & FLAG_UNKNOWN_1) == FLAG_UNKNOWN_1)
                        {
                            position += recordKeyCount * 8;
                        }
                        stream.Position = position + recordKeyCount * 8;
                        foreach (int key in recordKeys)
                        {
                            recordIds[key] = new List<int>(ReadSmart(stream, 2, 4, false));
                        }
                        foreach (int key in recordKeys)
                        {
                            List<int> ids = recordIds[key];
                            foreach (int id in DecodeIds(stream, ids.Capacity))
                            {
                                ids.Add(id);
                            }
                        }
                    }
                }
                reader.Close();
                command.CommandText = "SELECT DATA FROM cache WHERE KEY = " + recordKey;
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Record(recordIds[recordKey], DecompressBlob((byte[])reader[0]));
                }
            }
            return null;
        }

        private static byte[] DecompressBlob(byte[] inData)
        {
            using (MemoryStream outStream = new MemoryStream())
            using (MemoryStream inStream = new MemoryStream(inData))
            using (ZOutputStream outZStream = new ZOutputStream(outStream))
            {
                inStream.Seek(8, 0);
                byte[] buffer = new byte[16384];
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

        internal string ReadJagexString(MemoryStream stream)
        {
            ReadByte(stream);
            return ReadString(stream);
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

        private class Index
        {

            public int FileId;
            public int RecordKey;
            public int IdNumBits;

            public Index(int fileId, int recordKey, int idNumBits)
            {
                FileId = fileId;
                RecordKey = recordKey;
                IdNumBits = idNumBits;
            }

        }

        private class Record
        {
            public List<int> Ids;
            public byte[] Bytes;

            public Record(List<int> ids)
            {
                Ids = ids;
            }

            public Record(List<int> ids, byte[] bytes)
            {
                Ids = ids;
                Bytes = bytes;
            }

        }

    }

}
