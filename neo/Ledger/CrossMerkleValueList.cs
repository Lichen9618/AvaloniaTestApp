using Neo.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.Ledger
{
    public class CrossMerkleValueList : StateBase, ICloneable<CrossMerkleValueList>
    {
        public List<byte[]> merkle_value;
        //public byte[][] merkle_value;
        //public void Add(byte[] value)
        //{
        //    this.merkle_value.Add(value);
        //}
        public CrossMerkleValueList()
        {
            this.merkle_value = new List<byte[]>();
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                byte[] value = reader.ReadVarBytes();
                merkle_value.Add(value);
            }
        }
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(merkle_value.Count);
            foreach (byte[] s in merkle_value)
            {
                writer.WriteVarBytes(s);
            }
        }

        public CrossMerkleValueList Clone()
        {
            return new CrossMerkleValueList
            {
                merkle_value = merkle_value,
            };
        }

        public void FromReplica(CrossMerkleValueList replica)
        {
            merkle_value = replica.merkle_value;
        }
    }
}
