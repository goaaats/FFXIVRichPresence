using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVRichPresenceRunner.Memory;

namespace FFXIVPlayerWardrobe.Memory
{
    public partial class MemoryManager
    {
        private readonly Mem _memory;

        public MemoryManager(Mem memory)
        {
            _memory = memory;
        }

        #region General

        public int GetTimeOffset()
        {
            return _memory.readInt(Definitions.Instance.TIMEOFFSETPTR);
        }

        public void SetTimeOffset(int offset)
        {
            _memory.writeMemory(Definitions.Instance.TIMEOFFSETPTR, "int", offset.ToString());
        }

        public int GetTerritoryType()
        {
            return _memory.readInt(Definitions.Instance.TERRITORYTYPEOFFSETPTR);
        }

        public int GetWeather()
        {
            return _memory.readByte(Definitions.Instance.WEATHEROFFSETPTR);
        }

        public void SetWeather(byte id)
        {
            _memory.writeBytes(Definitions.Instance.WEATHEROFFSETPTR, new[] {id});
        }

        #endregion

        #region ActorTable

        public class ActorTableEntry
        {
            public long Offset { get; set; }
            public string Name { get; set; }
            public UInt32 ActorID { get; set; }
            public UInt32 OwnerID { get; set; }
            public short ModelChara { get; set; }
            public UInt32 BnpcBase { get; set; }
            public byte Job { get; set; }
            public byte Level { get; set; }
        }

        public int GetActorTableLength()
        {
            return _memory.readByte(Definitions.Instance.ACTORTABLEOFFSET);
        }

        public ActorTableEntry[] GetActorTable()
        {
            List<ActorTableEntry> entries = new List<ActorTableEntry>();
            var offsets = GetActorTableOffsetList();

            foreach (var offset in offsets)
            {
                entries.Add(GetActorTableEntry((long)offset));
            }

            return entries.ToArray();
        }

        public UIntPtr[] GetActorTableOffsetList()
        {
            List<UIntPtr> offsets = new List<UIntPtr>();

            var tableOffset = _memory.getCode(Definitions.Instance.ACTORTABLEOFFSET, "") + 0x8;

            for (int i = 0; i < GetActorTableLength(); i++)
            {
                offsets.Add(_memory.getCode(((long)(tableOffset + i * 8)).ToString("X") + ",0", ""));
            }

            return offsets.ToArray();
        }

        public ActorTableEntry GetActorTableEntry(long offset)
        {
            var data = _memory.readBytes(offset.ToString("X"), 0x1800);
            //Debug.WriteLine(Util.ByteArrayToHex(data));

            return new ActorTableEntry
            {
                Offset = offset,
                ActorID = BitConverter.ToUInt32(data, 0x74),
                Name = Encoding.UTF8.GetString(data, 0x30, 32),
                BnpcBase = BitConverter.ToUInt32(data, 0x80),
                OwnerID = BitConverter.ToUInt32(data, 0x84),
                ModelChara = BitConverter.ToInt16(data, 0x16FC),
                Job = data[0x1738],
                Level = data[0x173A]
            };
        }

        public bool WriteActorTableEntry(ActorTableEntry entry)
        {
            var table = GetActorTable();
            var offsets = GetActorTableOffsetList();

            if (offsets.Length < table.Length)
            {
                Debug.WriteLine("Offset table shorter than parsed actor table???");
                return false;
            }

            for (int i = 0; i < table.Length; i++)
            {
                if (table[i].ActorID == entry.ActorID)
                {
                    Debug.WriteLine("Found at " + ((long)offsets[i]).ToString("X"));
                    _memory.writeBytes(((long) offsets[i] + 0x30).ToString("X"), new byte[32]);
                    _memory.writeMemory(((long) offsets[i] + 0x30).ToString("X"), "string", entry.Name);
                    _memory.writeMemory(((long) offsets[i] + 0x80).ToString("X"), "int", entry.BnpcBase.ToString());
                    _memory.writeMemory(((long) offsets[i] + 0x16FC).ToString("X"), "int", entry.ModelChara.ToString());
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
