using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IO;

namespace Neo.SmartContract.CrossChain
{
    public class MerkleValue:ISerializable
    {
        /// <summary>
        /// 跨链请求id
        /// </summary>
        public ulong RequestID { get; set; }

        /// <summary>
        /// 跨链请求参数
        /// </summary>
        public CreateCrossChainTxParam Para { get; set; }

        public int Size { get; }
        public void Serialize(BinaryWriter writer)
        {
            writer.WriteVarInt((long)RequestID);
            Para.Serialize(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            RequestID = reader.ReadVarInt();
            Para=new CreateCrossChainTxParam();
            Para.Deserialize(reader);
        }
    }
}
