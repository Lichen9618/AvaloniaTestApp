using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IO;

namespace Neo.SmartContract.CrossChain
{
    /// <summary>
    /// 关键区块头高度
    /// </summary>
    public class KeyHeight:ISerializable
    {
        public List<uint> HeightList { get; set; }=new List<uint>();
        public int Size { get; }
        public void Serialize(BinaryWriter writer)
        {
            writer.WriteVarInt((long)HeightList.Count);
            foreach (var height in HeightList.OrderByDescending(h=>h))
            {
                writer.WriteVarInt((long)height);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadVarInt();
            for (ulong i = 0; i < count; i++)
            {
                HeightList.Add((uint)reader.ReadVarInt());
            }
        }
    }
}
