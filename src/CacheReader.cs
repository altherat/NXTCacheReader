using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NXTCacheReader
{
    public abstract class Definition
    {

        public int Id;

        internal Definition(int id)
        {
            Id = id;
        }

        internal abstract void Deserialize(CacheReader cacheReader, byte[] bytes);

        public class QuickChat : Definition
        {

            public bool IsMenu;
            public int[] SubMenuIds;
            public int[] SubOptionIds;
            public string Text;

            public QuickChat(int id) : base(id)
            {
                IsMenu = id < 256;
            }

            internal override void Deserialize(CacheReader cacheReader, byte[] bytes)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    while (stream.Position < stream.Length)
                    {
                        int unknown1 = cacheReader.ReadByte(stream);
                        if (unknown1 == 4)
                        {
                            int unknown2 = cacheReader.ReadByte(stream);
                        }
                        Text = cacheReader.ReadString(stream);
                        byte unknown3 = cacheReader.ReadByte(stream);
                        if (unknown3 == 2)
                        {
                            byte count = cacheReader.ReadByte(stream);
                            if (IsMenu)
                            {
                                SubMenuIds = new int[count];
                                for (int i = 0; i < count; i++)
                                {
                                    SubMenuIds[i] = cacheReader.ReadUShort(stream);
                                    byte unknown5 = cacheReader.ReadByte(stream);
                                }
                                unknown3 = cacheReader.ReadByte(stream);
                            }
                            else
                            {
                                SubOptionIds = new int[count];
                                for (int i = 0; i < count; i++)
                                {
                                    SubOptionIds[i] = cacheReader.ReadUShort(stream) + 256;
                                }
                            }
                        }
                        if (unknown3 == 3)
                        {
                            byte count = cacheReader.ReadByte(stream);
                            SubOptionIds = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                SubOptionIds[i] = cacheReader.ReadShort(stream) + 256;
                                byte unknown4 = cacheReader.ReadUByte(stream);
                            }
                        }
                        break;
                    }
                }
            }

        }

        public class Component : Definition
        {

            public string[] Actions;
            public int AnimationId;
            public ushort BaseHeight;
            public short BasePositionX;
            public short BasePositionY;
            public ushort BaseWidth;
            public ushort ContentType;
            public bool IsHidden;
            public bool IsHoverDisabled;
            public int ParentId;
            public ushort ScrollWidth;
            public ushort ScrollHeight;
            public ushort SpritePitch;
            public ushort SpriteRoll;
            public ushort SpriteScale;
            public ushort SpriteYaw;
            public string Text;
            public int TextColor;
            public int Type;

            public Component(int id) : base(id)
            {
            }

            internal override void Deserialize(CacheReader cacheReader, byte[] bytes)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    while (stream.Position < stream.Length)
                    {
                        int unknown1 = cacheReader.ReadUByte(stream);
                        if (unknown1 == 255)
                        {
                            unknown1 = -1;
                        }
                        Type = cacheReader.ReadUByte(stream);
                        if ((Type & 0x80) != 0)
                        {
                            Type = (Type & 0x7f);
                            string unknown2 = cacheReader.ReadString(stream);
                        }
                        ContentType = cacheReader.ReadUShort(stream);
                        BasePositionX = cacheReader.ReadShort(stream);
                        BasePositionY = cacheReader.ReadShort(stream);
                        BaseWidth = cacheReader.ReadUShort(stream);
                        BaseHeight = cacheReader.ReadUShort(stream);
                        byte unknown3 = cacheReader.ReadByte(stream);
                        byte unknown4 = cacheReader.ReadByte(stream);
                        byte unknown5 = cacheReader.ReadByte(stream);
                        byte unknown6 = cacheReader.ReadByte(stream);
                        ParentId = cacheReader.ReadUShort(stream);
                        if (ParentId == 65535)
                        {
                            ParentId = 107738123;
                        }
                        else
                        {
                            ParentId = (Id & ~0xffff) + ParentId;
                        }
                        int unknown7 = cacheReader.ReadUByte(stream);
                        IsHidden = (unknown7 & 0x1) != 0;
                        if (unknown1 >= 0)
                        {
                            IsHoverDisabled = (unknown7 & 0x2) != 0;
                        }
                        switch (Type)
                        {
                            case 0:
                                ScrollWidth = cacheReader.ReadUShort(stream);
                                ScrollHeight = cacheReader.ReadUShort(stream);
                                if (unknown1 < 0)
                                {
                                    IsHoverDisabled = cacheReader.ReadUByte(stream) == 1;
                                }
                                break;
                            case 3:
                                TextColor = cacheReader.ReadInt(stream);
                                bool unknown8 = cacheReader.ReadUByte(stream) == 1;
                                byte unknown9 = cacheReader.ReadUByte(stream);
                                break;
                            case 4:
                                int unknown10 = cacheReader.ReadBigSmart(stream);
                                if (unknown1 >= 2)
                                {
                                    bool unknown11 = cacheReader.ReadUByte(stream) == 1;
                                }
                                Text = cacheReader.ReadString(stream);
                                byte unknown12 = cacheReader.ReadUByte(stream);
                                byte unknown13 = cacheReader.ReadUByte(stream);
                                byte unknown14 = cacheReader.ReadUByte(stream);
                                bool unknown15 = cacheReader.ReadUByte(stream) == 1;
                                TextColor = cacheReader.ReadInt(stream);
                                byte unknown16 = cacheReader.ReadUByte(stream);
                                if (unknown1 >= 0)
                                {
                                    byte unknown17 = cacheReader.ReadUByte(stream);
                                }
                                break;
                            case 5:
                                int unknown18 = cacheReader.ReadInt(stream);
                                ushort unknown19 = cacheReader.ReadUShort(stream);
                                int unknown20 = cacheReader.ReadUByte(stream);
                                //bool unknownUnused1 = (unknown20 & 0x1) != 0;
                                //bool unknownUnused2 = (unknown20 & 0x2) != 0;
                                byte unknown21 = cacheReader.ReadUByte(stream);
                                byte unknown22 = cacheReader.ReadUByte(stream);
                                int unknown23 = cacheReader.ReadInt(stream);
                                bool unknown24 = cacheReader.ReadUByte(stream) == 1;
                                bool unknown25 = cacheReader.ReadUByte(stream) == 1;
                                TextColor = cacheReader.ReadInt(stream);
                                if (unknown1 >= 3)
                                {
                                    bool unknown26 = cacheReader.ReadUByte(stream) == 1;
                                }
                                break;
                            case 6:
                                int unknown27 = cacheReader.ReadBigSmart(stream);
                                byte unknown28 = cacheReader.ReadUByte(stream);
                                if ((unknown28 & 0x1) == 1)
                                {
                                    short unknown29 = cacheReader.ReadShort(stream);
                                    short unknown30 = cacheReader.ReadShort(stream);
                                    SpritePitch = cacheReader.ReadUShort(stream);
                                    SpriteRoll = cacheReader.ReadUShort(stream);
                                    SpriteYaw = cacheReader.ReadUShort(stream);
                                    SpriteScale = cacheReader.ReadUShort(stream);
                                }
                                else if ((unknown28 & 0x2) == 2)
                                {
                                    short unknown31 = cacheReader.ReadShort(stream);
                                    short unknown32 = cacheReader.ReadShort(stream);
                                    short unknown33 = cacheReader.ReadShort(stream);
                                    SpritePitch = cacheReader.ReadUShort(stream);
                                    SpriteRoll = cacheReader.ReadUShort(stream);
                                    SpriteYaw = cacheReader.ReadUShort(stream);
                                    SpriteScale = cacheReader.ReadUShort(stream);
                                }
                                AnimationId = cacheReader.ReadBigSmart(stream);
                                if (unknown3 != 0)
                                {
                                    ushort unknown34 = cacheReader.ReadUShort(stream);
                                }
                                if (unknown4 != 0)
                                {
                                    ushort unknown35 = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 9:
                                byte unknown36 = cacheReader.ReadUByte(stream);
                                TextColor = cacheReader.ReadInt(stream);
                                bool unknown37 = cacheReader.ReadUByte(stream) == 1;
                                break;
                        }
                        uint unknown38 = cacheReader.ReadUInt24(stream);
                        int unknown39 = cacheReader.ReadUByte(stream);
                        if (unknown39 != 0)
                        {
                            byte[][] unknown40 = new byte[11][];
                            byte[][] unknown41 = new byte[11][];
                            int[] unknown42 = new int[11];
                            for (; unknown39 != 0; unknown39 = cacheReader.ReadUByte(stream))
                            {
                                int unknown44 = (unknown39 >> 4) - 1;
                                unknown39 = (unknown39 << 8 | cacheReader.ReadUByte(stream)) & 0xfff;
                                if (unknown39 == 4095)
                                {
                                    unknown39 = -1;
                                }
                                unknown42[unknown44] = unknown39;
                                unknown40[unknown44] = new byte[] { cacheReader.ReadByte(stream) };
                                unknown41[unknown44] = new byte[] { cacheReader.ReadByte(stream) };
                            }
                        }
                        string unknown45 = cacheReader.ReadString(stream);
                        byte unknown46 = cacheReader.ReadUByte(stream);
                        int unknown47 = unknown46 & 0xf;
                        int unknown48 = unknown46 >> 4;
                        if (unknown47 > 0)
                        {
                            Actions = new string[unknown47];
                            for (int i = 0; i < unknown47; i++)
                            {
                                Actions[i] = cacheReader.ReadString(stream);
                            }
                        }
                        if (unknown48 > 0)
                        {
                            int unknown49 = cacheReader.ReadUByte(stream);
                            int[] unknown50 = new int[1 + unknown49];
                            for (int i = 0; i < unknown50.Length; i++)
                            {
                                unknown50[i] = -1;
                            }
                            unknown50[unknown49] = cacheReader.ReadUShort(stream);
                            if (unknown48 > 1)
                            {
                                unknown50[cacheReader.ReadUByte(stream)] = cacheReader.ReadUShort(stream);
                            }
                        }
                        string unknown51 = cacheReader.ReadString(stream);
                        if (unknown51 == "")
                        {
                            unknown51 = null;
                        }
                        byte unknown52 = cacheReader.ReadUByte(stream);
                        byte unknown53 = cacheReader.ReadUByte(stream);
                        byte unknown54 = cacheReader.ReadUByte(stream);
                        string unknown55 = cacheReader.ReadString(stream);
                        if ((unknown38 >> 11 & 0x7f) != 0)
                        {
                            ushort unknown56 = cacheReader.ReadUShort(stream);
                            //if (unknown56 == 65535)
                            //{
                            //    unknown56 = -1;
                            //}
                            int unknown57 = cacheReader.ReadUShort(stream);
                            //if (unknown57 == 65535)
                            //{
                            //    unknown57 = 1887779447;
                            //}
                            int unknown58 = cacheReader.ReadUShort(stream);
                            //if (unknown58 == 65535)
                            //{
                            //    unknown58 = 1132968959;
                            //}
                        }
                        if (unknown1 >= 0)
                        {
                            //Dictionary<uint, object> unknown59 = new Dictionary<uint, object>();
                            int unknown59 = cacheReader.ReadUShort(stream);
                            //if (unknown59 == 65535)
                            //{
                            //    unknown59 = 592771235;
                            //}
                            for (int i = 0; i < cacheReader.ReadUByte(stream); i++)
                            {
                                cacheReader.ReadUInt24(stream);
                                cacheReader.ReadInt(stream);
                                //unknown59.Add(cacheReader.ReadUInt24(stream), cacheReader.ReadInt(stream));
                            }
                            for (int i = 0; i < cacheReader.ReadUByte(stream); i++)
                            {
                                cacheReader.ReadUInt24(stream);
                                cacheReader.ReadString(stream);
                                //unknown59.Add(cacheReader.ReadUInt24(stream), cacheReader.ReadString(stream));
                            }
                        }
                        object[] unknown60 = Unknown1(cacheReader, stream);
                        object[] unknown61 = Unknown1(cacheReader, stream);
                        object[] unknown62 = Unknown1(cacheReader, stream);
                        object[] unknown63 = Unknown1(cacheReader, stream);
                        object[] unknown64 = Unknown1(cacheReader, stream);
                        object[] unknown65 = Unknown1(cacheReader, stream);
                        object[] unknown66 = Unknown1(cacheReader, stream);
                        object[] unknown67 = Unknown1(cacheReader, stream);
                        object[] unknown68 = Unknown1(cacheReader, stream);
                        object[] unknown69 = Unknown1(cacheReader, stream);
                        if (unknown1 >= 0)
                        {
                            object[] unknown70 = Unknown1(cacheReader, stream);
                        }
                        object[] unknown71 = Unknown1(cacheReader, stream);
                        object[] unknown72 = Unknown1(cacheReader, stream);
                        object[] unknown73 = Unknown1(cacheReader, stream);
                        object[] unknown74 = Unknown1(cacheReader, stream);
                        object[] unknown75 = Unknown1(cacheReader, stream);
                        object[] unknown76 = Unknown1(cacheReader, stream);
                        object[] unknown77 = Unknown1(cacheReader, stream);
                        object[] unknown78 = Unknown1(cacheReader, stream);
                        object[] unknown79 = Unknown1(cacheReader, stream);
                        object[] unknown80 = Unknown1(cacheReader, stream);
                        int[] unknown81 = Unknown2(cacheReader, stream);
                        int[] unknown82 = Unknown2(cacheReader, stream);
                        int[] unknown83 = Unknown2(cacheReader, stream);
                        int[] unknown84 = Unknown2(cacheReader, stream);
                        int[] unknown85 = Unknown2(cacheReader, stream);
                        break;
                    }
                }
            }

        }

        private object[] Unknown1(CacheReader cacheReader, MemoryStream stream)
        {
            int unknown1 = cacheReader.ReadUByte(stream);
            if (unknown1 == 0)
            {
                return null;
            }
            object[] unknown2 = new object[unknown1];
            for (int i = 0; i < unknown1; i++)
            {
                int unknown3 = cacheReader.ReadUByte(stream);
                if (unknown3 == 0)
                {
                    unknown2[i] = cacheReader.ReadInt(stream);
                }
                else if (unknown3 == 1)
                {
                    unknown2[i] = cacheReader.ReadString(stream);
                }
            }
            return unknown2;
        }

        private int[] Unknown2(CacheReader cacheReader, MemoryStream stream)
        {
            byte unknown1 = cacheReader.ReadUByte(stream);
            if (unknown1 == 0)
            {
                return null;
            }
            int[] unknown2 = new int[unknown1];
            for (int i = 0; i < unknown1; i++)
            {
                unknown2[i] = cacheReader.ReadInt(stream);
            }
            return unknown2;
        }

        public class Object : Definition
        {

            public string[] Actions;
            public int Height;
            public bool IsWalkable;
            public string Name;
            public int Width;

            public Object(int id) : base(id)
            {
            }

            internal override void Deserialize(CacheReader cacheReader, byte[] bytes)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    string[] actions = new string[5];
                    while (stream.Position < stream.Length)
                    {
                        byte opcode = cacheReader.ReadUByte(stream);
                        switch (opcode)
                        {
                            case 1:
                                byte size = cacheReader.ReadUByte(stream);
                                byte[] modelTypes = new byte[size];
                                int[][] modelIds = new int[size][];
                                for (int i = 0; i < size; i++)
                                {
                                    modelTypes[i] = cacheReader.ReadByte(stream);
                                    byte size2 = cacheReader.ReadUByte(stream);
                                    modelIds[i] = new int[size2];
                                    for (int j = 0; j < size2; j++)
                                    {
                                        modelIds[i][j] = cacheReader.ReadBigSmart(stream);
                                    }
                                }
                                break;
                            case 2:
                                Name = cacheReader.ReadString(stream);
                                break;
                            case 14:
                                Width = cacheReader.ReadUByte(stream);
                                break;
                            case 15:
                                Height = cacheReader.ReadUByte(stream);
                                break;
                            case 17:
                                IsWalkable = false;
                                break;
                            case 18:
                                IsWalkable = false;
                                break;
                            case 19:
                                byte type19 = cacheReader.ReadUByte(stream);
                                break;
                            case 21:
                                break;
                            case 22:
                                break;
                            case 23:
                                break;
                            case 24:
                                int unknown24 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 27:
                                break;
                            case 28:
                                byte unknown28 = cacheReader.ReadUByte(stream);
                                break;
                            case 29:
                                byte unknown29 = cacheReader.ReadByte(stream);
                                break;
                            case 30:
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                actions[opcode - 30] = cacheReader.ReadString(stream);
                                break;
                            case 39:
                                byte unknown39 = cacheReader.ReadByte(stream);
                                break;
                            case 40:
                                size = cacheReader.ReadUByte(stream);
                                ushort[] originalColors = new ushort[size];
                                ushort[] modifiedColors = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    originalColors[i] = cacheReader.ReadUShort(stream);
                                    modifiedColors[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 41:
                                size = cacheReader.ReadUByte(stream);
                                short[] unknown41Array1 = new short[size];
                                short[] unknown41Array2 = new short[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown41Array1[i] = cacheReader.ReadShort(stream);
                                    unknown41Array2[i] = cacheReader.ReadShort(stream);
                                }
                                break;
                            case 42:
                                size = cacheReader.ReadUByte(stream);
                                byte[] unknown42Array = new byte[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown42Array[i] = cacheReader.ReadByte(stream);
                                }
                                break;
                            case 44:
                                ushort unknown44 = cacheReader.ReadUShort(stream);
                                break;
                            case 45:
                                ushort unknown45 = cacheReader.ReadUShort(stream);
                                break;
                            case 62:
                                break;
                            case 64:
                                break;
                            case 65:
                            case 66:
                            case 67:
                                short[] resize = { 128, 128, 128 };
                                resize[opcode - 65] = cacheReader.ReadShort(stream);
                                break;
                            case 69:
                                byte accessibleMask = cacheReader.ReadByte(stream);
                                break;
                            case 70:
                                short unknown70 = cacheReader.ReadShort(stream);
                                break;
                            case 71:
                                short unknown71 = cacheReader.ReadShort(stream);
                                break;
                            case 72:
                                short unknown72 = cacheReader.ReadShort(stream);
                                break;
                            case 73:
                                break;
                            case 74:
                                IsWalkable = true;
                                break;
                            case 75:
                                byte unknown75 = cacheReader.ReadByte(stream);
                                break;
                            case 77:
                                ushort scriptId = cacheReader.ReadUShort(stream);
                                ushort configId = cacheReader.ReadUShort(stream);
                                size = cacheReader.ReadUByte(stream);
                                int[] childrenIds = new int[size + 2];
                                for (int i = 0; i <= size; i++)
                                {
                                    childrenIds[i] = cacheReader.ReadBigSmart(stream);
                                }
                                break;
                            case 78:
                                ushort unknown781 = cacheReader.ReadUShort(stream);
                                byte unknown782 = cacheReader.ReadUByte(stream);
                                break;
                            case 79:
                                int unknown791 = cacheReader.ReadUShort(stream) * 10;
                                int unknown792 = cacheReader.ReadUShort(stream) * 10;
                                byte unknown793 = cacheReader.ReadUByte(stream);
                                size = cacheReader.ReadUByte(stream);
                                ushort[] unknown79Array = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown79Array[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 81:
                                byte unknown81 = cacheReader.ReadUByte(stream);
                                break;
                            case 82:
                                break;
                            case 88:
                                break;
                            case 89:
                                break;
                            case 91:
                                break;
                            case 92:
                                scriptId = cacheReader.ReadUShort(stream);
                                configId = cacheReader.ReadUShort(stream);
                                int defaultId = cacheReader.ReadBigSmart(stream);
                                size = cacheReader.ReadUByte(stream);
                                childrenIds = new int[size + 2];
                                for (int i = 0; i <= size; i++)
                                {
                                    childrenIds[i] = cacheReader.ReadBigSmart(stream);
                                }
                                childrenIds[size + 1] = defaultId;
                                break;
                            case 93:
                                ushort unknown93 = cacheReader.ReadUShort(stream);
                                break;
                            case 94:
                                break;
                            case 95:
                                ushort unknown95 = cacheReader.ReadUShort(stream);
                                break;
                            case 97:
                                break;
                            case 98:
                                break;
                            case 101:
                                byte unknown101 = cacheReader.ReadUByte(stream);
                                break;
                            case 102:
                                ushort unknown102 = cacheReader.ReadUShort(stream);
                                break;
                            case 103:
                                break;
                            case 104:
                                byte unknown104 = cacheReader.ReadUByte(stream);
                                break;
                            case 105:
                                break;
                            case 106:
                                size = cacheReader.ReadUByte(stream);
                                int accumulator = 0;
                                int[] unknown106Array1 = new int[size];
                                int[] unknown106Array2 = new int[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown106Array1[i] = cacheReader.ReadBigSmart(stream);
                                    accumulator += unknown106Array2[i] = cacheReader.ReadUByte(stream);
                                }
                                for (int i = 0; i < size; i++)
                                {
                                    unknown106Array2[i] = unknown106Array2[i] * 65535 / accumulator;
                                }
                                break;
                            case 107:
                                ushort unknown107 = cacheReader.ReadUShort(stream);
                                break;
                            case 150:
                            case 151:
                            case 152:
                            case 153:
                            case 154:
                                actions[opcode - 150] = cacheReader.ReadString(stream);
                                break;
                            case 160:
                                size = cacheReader.ReadUByte(stream);
                                ushort[] unknown160Array = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown160Array[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 162:
                                int unknown162 = cacheReader.ReadInt(stream);
                                break;
                            case 163:
                                byte unknown1631 = cacheReader.ReadByte(stream);
                                byte unknown1632 = cacheReader.ReadByte(stream);
                                byte unknown1633 = cacheReader.ReadByte(stream);
                                byte unknown1634 = cacheReader.ReadByte(stream);
                                break;
                            case 164:
                            case 165:
                            case 166:
                                short unknown164to166 = cacheReader.ReadShort(stream);
                                break;
                            case 167:
                                ushort unknown167 = cacheReader.ReadUShort(stream);
                                break;
                            case 168:
                                break;
                            case 169:
                                break;
                            case 170:
                                int unknown170 = cacheReader.ReadSignedSmart(stream);
                                break;
                            case 171:
                                int unknown171 = cacheReader.ReadSignedSmart(stream);
                                break;
                            case 173:
                                ushort unknown1731 = cacheReader.ReadUShort(stream);
                                ushort unknown1732 = cacheReader.ReadUShort(stream);
                                break;
                            case 177:
                                break;
                            case 178:
                                byte unknown178 = cacheReader.ReadUByte(stream);
                                break;
                            case 179:
                                break;
                            case 186:
                                byte unknown186 = cacheReader.ReadUByte(stream);
                                break;
                            case 189:
                                break;
                            case 190:
                            case 191:
                            case 192:
                            case 193:
                            case 194:
                            case 195:
                                ushort unknown190to195 = cacheReader.ReadUShort(stream);
                                break;
                            case 196:
                                byte unknown196 = cacheReader.ReadUByte(stream);
                                break;
                            case 197:
                                byte unknown197 = cacheReader.ReadUByte(stream);
                                break;
                            case 198:
                                break;
                            case 199:
                                break;
                            case 200:
                                break;
                            case 201:
                                int unknown2011 = cacheReader.ReadSmart(stream);
                                int unknown2012 = cacheReader.ReadSmart(stream);
                                int unknown2013 = cacheReader.ReadSmart(stream);
                                int unknown2014 = cacheReader.ReadSmart(stream);
                                int unknown2015 = cacheReader.ReadSmart(stream);
                                int unknown2016 = cacheReader.ReadSmart(stream);
                                break;
                            case 249:
                                Dictionary<int, object> unknown249 = cacheReader.Decode249(stream);
                                break;
                            case 250:
                                byte unknown250 = cacheReader.ReadUByte(stream);
                                break;
                            case 252:
                                ushort unknown2521 = cacheReader.ReadUShort(stream);
                                ushort unknown2522 = cacheReader.ReadUShort(stream);
                                ushort unknown2523 = cacheReader.ReadUShort(stream);
                                break;
                            case 253:
                                byte unknown253 = cacheReader.ReadUByte(stream);
                                break;
                            case 254:
                                byte unknown254 = cacheReader.ReadUByte(stream);
                                break;
                            case 255:
                                ushort unknown2551 = cacheReader.ReadUShort(stream);
                                ushort unknown2552 = cacheReader.ReadUShort(stream);
                                ushort unknown2553 = cacheReader.ReadUShort(stream);
                                break;
                        }
                    }
                    Actions = actions.Where(a => a != null).ToArray();
                }
            }

        }

        public class Npc : Definition
        {

            public string[] Actions;
            public int Armour = -1;
            public int AttackSpeed = -1;
            public int CombatLevel = -1;
            public int CombatStyleId = -1;
            public bool IsClickable;
            public bool IsVisible;
            public int MagicAccuracy = -1;
            public int MagicDamage = -1;
            public int MeleeAccuracy = -1;
            public int MeleeDamage = -1;
            public string Name;
            public int RangedAccuracy = -1;
            public int RangedDamage = -1;
            public int WeaknessId = -1;

            public Npc(int id) : base(id)
            {
            }

            internal override void Deserialize(CacheReader cacheReader, byte[] bytes)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    int[] modelIds = null;
                    string[] actions = new string[5];
                    while (stream.Position < stream.Length)
                    {
                        byte opcode = cacheReader.ReadUByte(stream);
                        switch (opcode)
                        {
                            case 1:
                                byte size = cacheReader.ReadUByte(stream);
                                modelIds = new int[size];
                                for (int i = 0; i < size; i++)
                                {
                                    modelIds[i] = cacheReader.ReadBigSmart(stream);
                                }
                                break;
                            case 2:
                                Name = cacheReader.ReadString(stream);
                                break;
                            case 12:
                                byte unknown12 = cacheReader.ReadUByte(stream);
                                break;
                            case 30:
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                actions[opcode - 30] = cacheReader.ReadString(stream);
                                break;
                            case 39:
                                byte unknown39 = cacheReader.ReadByte(stream);
                                break;
                            case 40:
                                size = cacheReader.ReadUByte(stream);
                                ushort[] originalColors = new ushort[size];
                                ushort[] modifiedColors = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    originalColors[i] = cacheReader.ReadUShort(stream);
                                    modifiedColors[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 41:
                                size = cacheReader.ReadUByte(stream);
                                short[] unknown41Array1 = new short[size];
                                short[] unknown41Array2 = new short[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown41Array1[i] = cacheReader.ReadShort(stream);
                                    unknown41Array2[i] = cacheReader.ReadShort(stream);
                                }
                                break;
                            case 42:
                                size = cacheReader.ReadUByte(stream);
                                short[] unknown42Array = new short[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown42Array[i] = cacheReader.ReadByte(stream);
                                }
                                break;
                            case 44:
                                ushort unkown44 = cacheReader.ReadUShort(stream);
                                break;
                            case 45:
                                ushort unknown45 = cacheReader.ReadUShort(stream);
                                break;
                            case 60:
                                size = cacheReader.ReadUByte(stream);
                                int[] unknown60Array = new int[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown60Array[i] = cacheReader.ReadBigSmart(stream);
                                }
                                break;
                            case 93:
                                IsVisible = false;
                                break;
                            case 95:
                                CombatLevel = cacheReader.ReadUShort(stream);
                                break;
                            case 97:
                                ushort[] resize = { 128, 128, 128 };
                                resize[0] = resize[2] = cacheReader.ReadUShort(stream);
                                break;
                            case 98:
                                resize = new ushort[] { 128, 128, 128 };
                                resize[1] = cacheReader.ReadUShort(stream);
                                break;
                            case 99:
                                break;
                            case 100:
                                byte unknown100 = cacheReader.ReadByte(stream);
                                break;
                            case 101:
                                byte unknown101 = cacheReader.ReadByte(stream);
                                break;
                            case 102:
                                byte unknown1021 = cacheReader.ReadUByte(stream);
                                byte unknown1022 = 0;
                                byte unknown1023 = unknown1021;
                                while (unknown1023 != 0)
                                {
                                    unknown1022++;
                                    unknown1023 >>= 1;
                                }
                                int[] unknown102Array1 = new int[unknown1022];
                                int[] unknown102Array2 = new int[unknown1022];
                                for (int k = 0; k < unknown1022; k++)
                                {
                                    if ((unknown1021 & (1 << k)) == 0)
                                    {
                                        unknown102Array1[k] = -1;
                                        unknown102Array2[k] = -1;
                                    }
                                    else
                                    {
                                        unknown102Array1[k] = cacheReader.ReadBigSmart(stream);
                                        unknown102Array2[k] = cacheReader.ReadSmart(stream) - 1;
                                    }
                                }
                                break;
                            case 103:
                                ushort unknown103 = cacheReader.ReadUShort(stream);
                                break;
                            case 106:
                                int scriptId = cacheReader.ReadUShort(stream);
                                int configId = cacheReader.ReadUShort(stream);
                                byte childCount = cacheReader.ReadUByte(stream);
                                int[] childrenIds = new int[childCount + 2];
                                for (int i = 0; i <= childCount; i++)
                                {
                                    childrenIds[i] = cacheReader.ReadUShort(stream);
                                }
                                childrenIds[childCount + 1] = -1;
                                break;
                            case 118:
                                scriptId = cacheReader.ReadUShort(stream);
                                configId = cacheReader.ReadUShort(stream);
                                ushort defaultChildId = cacheReader.ReadUShort(stream);
                                childCount = cacheReader.ReadByte(stream);
                                childrenIds = new int[childCount + 2];
                                for (int i = 0; i <= childCount; i++)
                                {
                                    childrenIds[i] = cacheReader.ReadUShort(stream);
                                }
                                childrenIds[childCount + 1] = defaultChildId;
                                break;
                            case 107:
                                IsClickable = false;
                                break;
                            case 109:
                                break;
                            case 111:
                                break;
                            case 113:
                                ushort unknown1131 = cacheReader.ReadUShort(stream);
                                ushort unknown1132 = cacheReader.ReadUShort(stream);
                                break;
                            case 114:
                                byte unknown1141 = cacheReader.ReadByte(stream);
                                byte unknown1142 = cacheReader.ReadByte(stream);
                                break;
                            case 119:
                                byte unknown119 = cacheReader.ReadUByte(stream);
                                break;
                            case 121:
                                byte[][] modelOffsets = new byte[modelIds.Length][];
                                size = cacheReader.ReadByte(stream);
                                for (int i = 0; i < size; i++)
                                {
                                    byte modelIndex = cacheReader.ReadByte(stream);
                                    modelOffsets[modelIndex] = new byte[] { cacheReader.ReadByte(stream), cacheReader.ReadByte(stream), cacheReader.ReadByte(stream) };
                                }
                                break;
                            case 123:
                                ushort unknown123 = cacheReader.ReadUShort(stream);
                                break;
                            case 125:
                                byte unknown125 = cacheReader.ReadByte(stream);
                                break;
                            case 127:
                                ushort unknown127 = cacheReader.ReadUShort(stream);
                                break;
                            case 128:
                                byte unknown128 = cacheReader.ReadUByte(stream);
                                break;
                            case 134:
                                ushort unknown1341 = cacheReader.ReadUShort(stream);
                                ushort unknown1342 = cacheReader.ReadUShort(stream);
                                ushort unknown1343 = cacheReader.ReadUShort(stream);
                                ushort unknown1344 = cacheReader.ReadUShort(stream);
                                byte unknown1345 = cacheReader.ReadUByte(stream);
                                break;
                            case 137:
                                ushort unknown137 = cacheReader.ReadUShort(stream);
                                break;
                            case 138:
                                int headIcon = cacheReader.ReadBigSmart(stream);
                                break;
                            case 139:
                                int unknown139 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 140:
                                byte unknown140 = cacheReader.ReadUByte(stream);
                                break;
                            case 141:
                                break;
                            case 142:
                                ushort unknown142 = cacheReader.ReadUShort(stream);
                                break;
                            case 143:
                                break;
                            case 150:
                            case 151:
                            case 152:
                            case 153:
                            case 154:
                                actions[opcode - 150] = cacheReader.ReadString(stream);
                                break;
                            case 155:
                                byte unknown1551 = cacheReader.ReadByte(stream);
                                byte unknown1552 = cacheReader.ReadByte(stream);
                                byte unknown1553 = cacheReader.ReadByte(stream);
                                byte unknown1554 = cacheReader.ReadByte(stream);
                                break;
                            case 158:
                                break;
                            case 159:
                                break;
                            case 160:
                                size = cacheReader.ReadUByte(stream);
                                ushort[] unknown160Array = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown160Array[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 162:
                                break;
                            case 163:
                                byte unknown163 = cacheReader.ReadUByte(stream);
                                break;
                            case 164:
                                ushort unknown1641 = cacheReader.ReadUShort(stream);
                                ushort unknown1642 = cacheReader.ReadUShort(stream);
                                break;
                            case 165:
                                byte unknown165 = cacheReader.ReadUByte(stream);
                                break;
                            case 168:
                                byte unknown166 = cacheReader.ReadUByte(stream);
                                break;
                            case 169:
                                break;
                            case 170:
                            case 171:
                            case 172:
                            case 173:
                            case 174:
                            case 175:
                                ushort unknown170to175 = cacheReader.ReadUShort(stream);
                                break;
                            case 178:
                                break;
                            case 179:
                                int unknown1791 = cacheReader.ReadSmart(stream);
                                int unknown1792 = cacheReader.ReadSmart(stream);
                                int unknown1793 = cacheReader.ReadSmart(stream);
                                int unknown1794 = cacheReader.ReadSmart(stream);
                                int unknown1795 = cacheReader.ReadSmart(stream);
                                int unknown1796 = cacheReader.ReadSmart(stream);
                                break;
                            case 180:
                                byte unknown180 = cacheReader.ReadUByte(stream);
                                break;
                            case 181:
                                ushort unknown1811 = cacheReader.ReadUShort(stream);
                                byte unknown1821 = cacheReader.ReadUByte(stream);
                                break;
                            case 182:
                                break;
                            case 249:
                                foreach (KeyValuePair<int, object> param in cacheReader.Decode249(stream))
                                {
                                    int code = param.Key;
                                    switch (code)
                                    {
                                        case 3:
                                            MagicAccuracy = (int)param.Value;
                                            break;
                                        case 4:
                                            RangedAccuracy = (int)param.Value;
                                            break;
                                        case 14:
                                            AttackSpeed = (int)param.Value;
                                            break;
                                        case 26:
                                            CombatStyleId = (int)param.Value;
                                            break;
                                        case 29:
                                            MeleeAccuracy = (int)param.Value;
                                            break;
                                        case 641:
                                            MeleeDamage = (int)param.Value;
                                            break;
                                        case 643:
                                            RangedDamage = (int)param.Value;
                                            break;
                                        case 965:
                                            MagicDamage = (int)param.Value;
                                            break;
                                        case 2848:
                                            WeaknessId = (int)param.Value;
                                            break;
                                        case 2849:
                                        case 2850:
                                        case 2851:
                                        case 2852:
                                            // Weakness modifiers
                                            break;
                                        case 2865:
                                            Armour = (int)param.Value;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    Actions = actions.Where(a => a != null).ToArray();
                }
            }
        }

        public class Item : Definition
        {

            public string[] Actions;
            public int ArmourBonus = -1;
            public int AttackSpeed = -1;
            public string[] BankActions;
            public int ConsumptionLifePointsQuantity = -1;
            public int CosmeticItemId = -1;
            public int CosmeticTemplateId = -1;
            public int[] CreationExperienceQuantities;
            public int[] CreationRequiredItemIds;
            public int[] CreationRequiredItemQuantities;
            public int[] CreationRequiredSkillIds;
            public int[] CreationRequiredToolItemIds;
            public string DestroyText;
            public int DegredationItemId = -1;
            public int DiangoReplacementCost = -1;
            public int DropSoundId = -1;
            public string[] EquipActions;
            public int EquipMagicRatio = -1;
            public int EquipMeleeRatio = -1;
            public int EquipRangedratio = -1;
            public int[] EquipRequiredSkillIds;
            public int[] EquipRequiredSkillLevels;
            public int EquipSlotId = -1;
            public int EquipSoundId = -1;
            public int GrandExchangeCategoryId = -1;
            public string[] GroundActions;
            public bool IsAlchable = true;
            public bool IsBankable = true;
            public bool IsCosmetic;
            public bool IsDegradable;
            public bool IsDungeoneeringItem;
            public bool IsLent;
            public bool IsMagicArmour;
            public bool IsMagicWeapon;
            public bool IsMeleeArmour;
            public bool IsMeleeWeapon;
            public bool IsMembers;
            public bool IsNoted;
            public bool IsRangedArmour;
            public bool IsRangedWeapon;
            public bool IsShield;
            public bool IsStackable;
            public bool IsTradable;
            public int LentItemId = -1;
            public int LentTemplateId = -1;
            public int LifePointBonus = -1;
            public int MagicAccuracy = -1;
            public int MagicDamage = -1;
            public int MaxCharges = -1;
            public int MeleeAccuracy = -1;
            public int MeleeDamage = -1;
            public int ModelId = -1;
            public int ModelZoom = -1;
            public string Name;
            public int NoteItemId = -1;
            public int NoteTemplateId = -1;
            public int PrayerBonus = -1;
            public int RangedAccuracy = -1;
            public int RangedDamage = -1;
            public int RepairCost = -1;
            public int Tier = -1;
            public int TreasureHunterCashOutValue = -1;
            public string TreasureHunterText;
            public int UseRequiredSkillId;
            public int UseRequiredSkillLevel;

            public Item(int id) : base(id)
            {
            }

            internal override void Deserialize(CacheReader cacheReader, byte[] bytes)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    string[] actions = { null, null, null, null, "Drop" };
                    string[] groundActions = { null, null, "Take", null, null };
                    while (stream.Position < stream.Length)
                    {
                        byte opcode = cacheReader.ReadUByte(stream);
                        switch (opcode)
                        {
                            case 1:
                                ModelId = cacheReader.ReadBigSmart(stream);
                                break;
                            case 2:
                                Name = cacheReader.ReadString(stream);
                                break;
                            case 4:
                                ModelZoom = cacheReader.ReadUShort(stream);
                                break;
                            case 5:
                                ushort modelRotation1 = cacheReader.ReadUShort(stream);
                                break;
                            case 6:
                                ushort modelRotation2 = cacheReader.ReadUShort(stream);
                                break;
                            case 7:
                                ushort modelOffset1 = cacheReader.ReadUShort(stream);
                                break;
                            case 8:
                                ushort modelOffset2 = cacheReader.ReadUShort(stream);
                                break;
                            case 11:
                                IsStackable = true;
                                break;
                            case 12:
                                int unknown12 = cacheReader.ReadInt(stream);
                                break;
                            case 13:
                                EquipSlotId = cacheReader.ReadUByte(stream);
                                break;
                            case 14:
                                byte unknown14 = cacheReader.ReadUByte(stream);
                                break;
                            case 16:
                                IsMembers = true;
                                break;
                            case 18:
                                ushort unknown18 = cacheReader.ReadUShort(stream);
                                break;
                            case 23:
                                int maleEquip1 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 24:
                                int femaleEquip1 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 25:
                                int maleEquip2 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 26:
                                int femaleEquip2 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 27:
                                byte unknown27 = cacheReader.ReadUByte(stream);
                                break;
                            case 30:
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                                groundActions[opcode - 30] = cacheReader.ReadString(stream);
                                break;
                            case 35:
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                                actions[opcode - 35] = cacheReader.ReadString(stream);
                                break;
                            case 40:
                                byte size = cacheReader.ReadUByte(stream);
                                ushort[] originalColors = new ushort[size];
                                ushort[] modifiedColors = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    originalColors[i] = cacheReader.ReadUShort(stream);
                                    modifiedColors[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 41:
                                size = cacheReader.ReadUByte(stream);
                                short[] unknown41Array1 = new short[size];
                                short[] unknown41Array2 = new short[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown41Array1[i] = cacheReader.ReadShort(stream);
                                    unknown41Array2[i] = cacheReader.ReadShort(stream);
                                }
                                break;
                            case 42:
                                size = cacheReader.ReadUByte(stream);
                                short[] unknown42Array = new short[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown42Array[i] = cacheReader.ReadByte(stream);
                                }
                                break;
                            case 43:
                                int unknown43 = cacheReader.ReadInt(stream);
                                break;
                            case 44:
                                ushort unknown44 = cacheReader.ReadUShort(stream);
                                break;
                            case 45:
                                ushort unkown45 = cacheReader.ReadUShort(stream);
                                break;
                            case 65:
                                IsTradable = true;
                                break;
                            case 78:
                                int unknown78 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 79:
                                int unknown79 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 80:
                                int unknown80 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 90:
                                int unknown90 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 91:
                                int unknown91 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 92:
                                int unknown92 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 93:
                                int unknown93 = cacheReader.ReadBigSmart(stream);
                                break;
                            case 94:
                                ushort unknown94 = cacheReader.ReadUShort(stream);
                                break;
                            case 95:
                                ushort modelRotation = cacheReader.ReadUShort(stream);
                                break;
                            case 96:
                                byte unknown96 = cacheReader.ReadByte(stream);
                                break;
                            case 97:
                                NoteItemId = cacheReader.ReadUShort(stream);
                                break;
                            case 98:
                                NoteTemplateId = cacheReader.ReadUShort(stream);
                                IsNoted = true;
                                break;
                            case 100:
                            case 101:
                            case 102:
                            case 103:
                            case 104:
                            case 105:
                            case 106:
                            case 107:
                            case 108:
                            case 109:
                                ushort stackId = cacheReader.ReadUShort(stream);
                                ushort stackAmount = cacheReader.ReadUShort(stream);
                                break;
                            case 110:
                            case 111:
                            case 112:
                                ushort unknown110to112 = cacheReader.ReadUShort(stream);
                                break;
                            case 113:
                                byte unknown113 = cacheReader.ReadByte(stream);
                                break;
                            case 114:
                                byte unknown114 = cacheReader.ReadByte(stream);
                                break;
                            case 115:
                                byte team = cacheReader.ReadUByte(stream);
                                break;
                            case 121:
                                LentItemId = cacheReader.ReadUShort(stream);
                                break;
                            case 122:
                                LentTemplateId = cacheReader.ReadUShort(stream);
                                IsLent = true;
                                break;
                            case 125:
                                byte unknown1251 = cacheReader.ReadByte(stream);
                                byte unknown1252 = cacheReader.ReadByte(stream);
                                byte unknown1253 = cacheReader.ReadByte(stream);
                                break;
                            case 126:
                                byte unknown1261 = cacheReader.ReadByte(stream);
                                byte unknown1262 = cacheReader.ReadByte(stream);
                                byte unknown1263 = cacheReader.ReadByte(stream);
                                break;
                            case 132:
                                size = cacheReader.ReadUByte(stream);
                                ushort[] unknown132Array = new ushort[size];
                                for (int i = 0; i < size; i++)
                                {
                                    unknown132Array[i] = cacheReader.ReadUShort(stream);
                                }
                                break;
                            case 134:
                                byte unknown134 = cacheReader.ReadUByte(stream);
                                break;
                            case 139:
                                CosmeticItemId = cacheReader.ReadUShort(stream);
                                break;
                            case 140:
                                CosmeticTemplateId = cacheReader.ReadUShort(stream);
                                IsCosmetic = true;
                                break;
                            case 142:
                            case 143:
                            case 144:
                            case 145:
                            case 146:
                                ushort unknown142to146 = cacheReader.ReadUShort(stream);
                                break;
                            case 150:
                            case 151:
                            case 152:
                            case 153:
                            case 154:
                                ushort unknown150to154 = cacheReader.ReadUShort(stream);
                                break;
                            case 156:
                                break;
                            case 157:
                                break;
                            case 161:
                                ushort unknown161 = cacheReader.ReadUShort(stream);
                                break;
                            case 162:
                                ushort unknown162 = cacheReader.ReadUShort(stream);
                                break;
                            case 163:
                                ushort unknown163 = cacheReader.ReadUShort(stream);
                                break;
                            case 164:
                                string unknown164 = cacheReader.ReadString(stream);
                                break;
                            case 165:
                                break;
                            case 249:
                                string[] bankActions = { null, null };
                                int[] creationExperienceGains = { -1, -1 };
                                int[] creationRequiredItemIds = { -1, -1, -1, -1, -1, -1, -1 };
                                int[] creationRequiredItemQuantities = { -1, -1, -1, -1, -1, -1, -1 };
                                int[] creationRequiredSkillIds = { -1, -1 };
                                int[] creationRequiredToolItemIds = { -1, -1 };
                                string[] equipActions = { null, null, null, null, null, null, null };
                                int[] equipRequiredSkillIds = { -1, -1 };
                                int[] equipRequiredSkillLevels = { -1, -1 };
                                foreach (KeyValuePair<int, object> param in cacheReader.Decode249(stream))
                                {
                                    int code = param.Key;
                                    switch (code)
                                    {
                                        case 3:
                                            MagicAccuracy = (int)param.Value;
                                            break;
                                        case 4:
                                            RangedAccuracy = (int)param.Value;
                                            break;
                                        case 14:
                                            AttackSpeed = (int)param.Value;
                                            break;
                                        case 23:
                                            Tier = (int)param.Value;
                                            break;
                                        case 31: //Death flag
                                            break;
                                        case 59:
                                            IsBankable = (int)param.Value == 0;
                                            break;
                                        case 118:
                                            EquipSoundId = (int)param.Value;
                                            break;
                                        case 528:
                                        case 529:
                                        case 530:
                                        case 531:
                                            equipActions[code - 528] = (string)param.Value;
                                            break;
                                        case 537:
                                            DropSoundId = (int)param.Value;
                                            break;
                                        case 641:
                                            MeleeDamage = (int)param.Value;
                                            break;
                                        case 643:
                                            RangedDamage = (int)param.Value;
                                            break;
                                        case 686: //Weapon animation struct ID
                                            break;
                                        case 689:
                                            IsAlchable = (int)param.Value == 0;
                                            break;
                                        case 749:
                                            equipRequiredSkillIds[0] = (int)param.Value;
                                            break;
                                        case 750:
                                            equipRequiredSkillLevels[0] = (int)param.Value;
                                            break;
                                        case 751:
                                            equipRequiredSkillIds[1] = (int)param.Value;
                                            break;
                                        case 752:
                                            equipRequiredSkillLevels[1] = (int)param.Value;
                                            break;
                                        case 770:
                                            UseRequiredSkillId = (int)param.Value;
                                            break;
                                        case 771:
                                            UseRequiredSkillLevel = (int)param.Value;
                                            break;
                                        case 963:
                                            ConsumptionLifePointsQuantity = (int)param.Value;
                                            break;
                                        case 965:
                                            MagicDamage = (int)param.Value;
                                            break;
                                        case 1047:
                                            IsDungeoneeringItem = (int)param.Value == 1;
                                            break;
                                        case 1211:
                                            equipActions[4] = (string)param.Value;
                                            break;
                                        case 1264:
                                        case 1265:
                                            bankActions[code - 1264] = (string)param.Value;
                                            break;
                                        case 1324:
                                            IsDegradable = (int)param.Value == 1;
                                            break;
                                        case 1326:
                                            LifePointBonus = (int)param.Value;
                                            break;
                                        case 1397: //Death flag
                                            break;
                                        case 2195:
                                            GrandExchangeCategoryId = (int)param.Value;
                                            break;
                                        case 2281: //Decant struct ID
                                            break;
                                        case 2650:
                                        case 2651:
                                            creationRequiredToolItemIds[code - 2650] = (int)param.Value;
                                            break;
                                        case 2655:
                                        case 2656:
                                        case 2657:
                                        case 2658:
                                        case 2659:
                                        case 2660:
                                        case 2661:
                                            creationRequiredItemIds[code - 2655] = (int)param.Value;
                                            break;
                                        case 2665:
                                        case 2666:
                                        case 2667:
                                        case 2668:
                                        case 2669:
                                        case 2670:
                                        case 2671:
                                            creationRequiredItemQuantities[code - 2665] = (int)param.Value;
                                            break;
                                        case 2696:
                                            creationRequiredSkillIds[0] = (int)param.Value;
                                            creationExperienceGains[0] = 0;
                                            break;
                                        case 2697:
                                            creationExperienceGains[0] = (int)param.Value;
                                            break;
                                        case 2698:
                                            creationRequiredSkillIds[1] = (int)param.Value;
                                            creationExperienceGains[1] = 0;
                                            break;
                                        case 2699:
                                            creationExperienceGains[1] = (int)param.Value;
                                            break;
                                        case 2821:
                                            IsMeleeArmour = (int)param.Value == 1;
                                            break;
                                        case 2822:
                                            IsRangedArmour = (int)param.Value == 1;
                                            break;
                                        case 2823:
                                            IsMagicArmour = (int)param.Value == 1;
                                            break;
                                        case 2825:
                                            IsMeleeWeapon = (int)param.Value == 1;
                                            break;
                                        case 2826:
                                            IsRangedWeapon = (int)param.Value == 1;
                                            break;
                                        case 2827:
                                            IsMagicWeapon = (int)param.Value == 1;
                                            break;
                                        case 2866:
                                            EquipRangedratio = (int)param.Value;
                                            break;
                                        case 2867:
                                            EquipMeleeRatio = (int)param.Value;
                                            break;
                                        case 2868:
                                            EquipMagicRatio = (int)param.Value;
                                            break;
                                        case 2870:
                                            ArmourBonus = (int)param.Value;
                                            break;
                                        case 2946:
                                            PrayerBonus = (int)param.Value;
                                            break;
                                        case 2951: //Required cooking level (old?)
                                            break;
                                        case 3267:
                                            MeleeAccuracy = (int)param.Value;
                                            break;
                                        case 3322:
                                            DiangoReplacementCost = (int)param.Value;
                                            break;
                                        case 3324: //Death flag
                                            break;
                                        case 3382:
                                            DegredationItemId = (int)param.Value;
                                            break;
                                        case 3383:
                                            RepairCost = (int)param.Value;
                                            break;
                                        case 3385:
                                            MaxCharges = (int)param.Value;
                                            break;
                                        case 4085:
                                            TreasureHunterText = (string)param.Value;
                                            break;
                                        case 4199:
                                            TreasureHunterCashOutValue = (int)param.Value;
                                            break;
                                        case 4749: //Treasure Hunter struct ID
                                            break;
                                        case 5417:
                                            DestroyText = (string)param.Value;
                                            break;
                                        case 6712:
                                            equipActions[5] = (string)param.Value;
                                            break;
                                        case 6713:
                                            equipActions[6] = (string)param.Value;
                                            break;

                                    }
                                }
                                BankActions = actions.Where(a => a != null).ToArray();
                                CreationExperienceQuantities = creationExperienceGains.Where(i => i != -1).ToArray();
                                CreationRequiredItemIds = creationRequiredItemIds.Where(i => i != -1).ToArray();
                                CreationRequiredItemQuantities = creationRequiredItemQuantities.Where(i => i != -1).ToArray();
                                CreationRequiredSkillIds = creationRequiredSkillIds.Where(i => i != -1).ToArray();
                                CreationRequiredToolItemIds = creationRequiredToolItemIds.Where(i => i != -1).ToArray();
                                EquipActions = equipActions.Where(a => a != null).ToArray();
                                EquipRequiredSkillIds = equipRequiredSkillIds.Where(i => i != -1).ToArray();
                                EquipRequiredSkillLevels = equipRequiredSkillLevels.Where(i => i != -1).ToArray();
                                break;
                        }
                    }
                    Actions = actions.Where(a => a != null).ToArray();
                    GroundActions = groundActions.Where(a => a != null).ToArray();
                    if (CosmeticTemplateId != -1)
                    {
                        CopyFromTemplate(cacheReader.LoadDefinition<Item>(CosmeticItemId));
                    }
                    if (LentTemplateId != -1)
                    {
                        CopyFromTemplate(cacheReader.LoadDefinition<Item>(LentItemId));
                    }
                    if (NoteTemplateId != -1)
                    {
                        CopyFromTemplate(cacheReader.LoadDefinition<Item>(NoteItemId));
                    }
                }
            }

            internal void CopyFromTemplate(Item template)
            {
                Actions = template.Actions;
                DestroyText = template.DestroyText;
                IsMembers = template.IsMembers;
                IsTradable = template.IsTradable;
                Name = template.Name;
            }

        }

    }

}
