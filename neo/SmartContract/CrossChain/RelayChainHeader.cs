using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.IO;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;

namespace Neo.SmartContract.CrossChain
{
    public class RelayChainHeader : ISerializable
    {
        public uint Version { get; set; }

        public ulong ChainID { get; set; }

        public UInt256 PrevBlockHash { get; set; }
        public UInt256 TransactionsRoot { get; set; }

        public UInt256 CrossStatesRoot { get; set; }
        public UInt256 BlockRoot { get; set; }
        public uint Timestamp { get; set; }
        public uint Height { get; set; }

        public ulong ConsensusData { get; set; }
        public byte[] ConsensusPayload { get; set; }

        public UInt160 NextBookkeeper { get; set; } //common.Address

        public List<byte[]> Bookkeepers { get; set; } = new List<byte[]>();
        public List<byte[]> SigData { get; set; } = new List<byte[]>();

        //没用??
        //public UInt256 hash { get; set; }
        public int Size { get; }




        public void Serialize(BinaryWriter writer)
        {
            SerializeUnsigned(writer);
            writer.WriteVarInt((long)Bookkeepers.Count);

            foreach (var bookkeeper in Bookkeepers)
            {
                writer.WriteVarBytes(bookkeeper);
            }

            writer.WriteVarInt((long)SigData.Count);
            foreach (var sig in SigData)
            {
                writer.WriteVarBytes(sig);
            }
        }


        public void SerializeUnsigned(BinaryWriter writer)
        {
            if (Version > ConstResource.CURRENT_HEADER_VERSION)
            {
                //panic(fmt.Errorf("invalid header version:%d", bd.Version))

            }
            writer.Write(Version);
            writer.Write(ChainID);
            writer.Write(PrevBlockHash);
            writer.Write(TransactionsRoot);
            writer.Write(CrossStatesRoot);//length+data???
            writer.Write(BlockRoot);
            writer.Write(Timestamp);
            writer.Write(Height);
            writer.Write(ConsensusData);
            writer.WriteVarBytes(ConsensusPayload);
            writer.Write(NextBookkeeper);
        }
        public void Deserialize(BinaryReader reader)
        {
            DeserializeUnsigned(reader);

            var pubkeyCount = reader.ReadVarInt();
            var bookkeepers = new List<byte[]>();
            for (ulong i = 0; i < pubkeyCount; i++)
            {
                bookkeepers.Add(reader.ReadVarBytes());
            }
            Bookkeepers = bookkeepers;

            var sigCount = reader.ReadVarInt();
            var sigs = new List<byte[]>();
            for (ulong i = 0; i < sigCount; i++)
            {
                sigs.Add(reader.ReadVarBytes());
            }

            SigData = sigs;
        }

        public void DeserializeUnsigned(BinaryReader reader)
        {
            Version = reader.ReadUInt32();
            if (Version > ConstResource.CURRENT_HEADER_VERSION)
            {
                throw new Exception($"Invalid Version <{Version}>");
            }
            ChainID = reader.ReadUInt64();
            PrevBlockHash = reader.ReadHash256();
            TransactionsRoot = reader.ReadHash256();
            CrossStatesRoot = reader.ReadHash256();
            BlockRoot = reader.ReadHash256();
            Timestamp = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            ConsensusData = reader.ReadUInt64();
            ConsensusPayload = reader.ReadVarBytes();
            NextBookkeeper = reader.ReadHash160();
        }


        public UInt256 Hash()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                SerializeUnsigned(writer);
                writer.Flush();
                var data = ms.ToArray();
                return new UInt256(data.Sha256().Sha256());
            }
        }


        public string ToJson()
        {
            var json = new JObject();
            json["Version"] = Version;
            json["ChainID"] = ChainID;
            json["PrevBlockHash"] = PrevBlockHash.ToArray().ToHexString();
            json["TransactionsRoot"] = TransactionsRoot.ToArray().ToHexString();
            json["CrossStatesRoot"] = CrossStatesRoot.ToArray().ToHexString();
            json["BlockRoot"] = BlockRoot.ToArray().ToHexString();
            json["Timestamp"] = Timestamp;
            json["Height"] = Height;
            json["ConsensusData"] = ConsensusData.ToString();
            json["NextBookkeeper"] = NextBookkeeper.ToArray().ToHexString();
            var bookkeepers = new JArray();
            foreach (var book in Bookkeepers)
            {
                bookkeepers.Add(book.ToHexString());
            }
            json["Bookkeepers"] = bookkeepers;
            var signs = new JArray();
            foreach (var sig in SigData)
            {
                signs.Add(sig.ToHexString());
            }
            json["SigData"] = signs;

            return json.ToString();

        }
    }
}
