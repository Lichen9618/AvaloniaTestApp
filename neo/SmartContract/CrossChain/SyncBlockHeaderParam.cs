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
    /// 接收
    /// </summary>
    public class SyncBlockHeaderParam:ISerializable
    {
        /// <summary>
        /// relayer地址？
        /// </summary>
        public UInt160 Address { get; set; }

        /// <summary>
        /// 区块头Raw数据
        /// </summary>
        public List<byte[]> Headers { get; set; }


        public int Size { get; }
        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(BinaryReader reader)
        {
            var addressBytes = reader.ReadVarBytes();
            if (addressBytes.Length != 20)
            {
                throw new ArgumentException("Invalid address!");
            }
            Address = new UInt160(addressBytes);
            var count = reader.DecodeVarInt();
            var headers = new List<byte[]>();
            for (ulong i = 0; i < count; i++)
            {
                headers.Add(reader.ReadVarBytes());
            }

            Headers = headers;
        }
    }
}
