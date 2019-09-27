using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IO;

namespace Neo.SmartContract.CrossChain
{
    public class CreateCrossChainTxParam:ISerializable
    {
        /// <summary>
        /// 当前交易hash
        /// </summary>
        public UInt256 TxID { get; set; }
        /// <summary>
        /// Neo链ID
        /// </summary>
        public ulong FromChainID { get; set; }
        /// <summary>
        /// neo上发起跨链交易的合约(NEP5)
        /// </summary>
        public byte[] FromContractAddress { get; set; }
        /// <summary>
        /// 矿工费
        /// </summary>
        public ulong Fee { get; set; }
        /// <summary>
        /// 目标chainid
        /// </summary>
        public ulong ToChainID { get; set; }

        /// <summary>
        /// 调用其它链的方法
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 传递给其它链的参数
        /// </summary>
        public byte[] Args { get; set; }

        public int Size { get; }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteHash256(TxID);
            writer.Write(FromChainID);
            writer.WriteVarString(FromContractAddress.ToHexString());//方便兼容各种链
            writer.Write(ToChainID);
            writer.Write(Fee);
            writer.WriteVarString(FunctionName);
            writer.WriteVarBytes(Args);

        }

        public void Deserialize(BinaryReader reader)
        {
            TxID = reader.ReadHash256();
            FromChainID = reader.ReadUInt64();
            FromContractAddress = reader.ReadVarString().HexToBytes();//方便兼容各种链
            ToChainID = reader.ReadUInt64();
            Fee = reader.ReadUInt64();
            FunctionName = reader.ReadVarString();
            Args = reader.ReadVarBytes();
        }

    }
}
