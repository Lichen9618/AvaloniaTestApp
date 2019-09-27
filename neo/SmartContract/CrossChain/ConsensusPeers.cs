using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.IO;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Neo.SmartContract.CrossChain
{
    public class ConsensusPeers : ISerializable
    {
        public ulong ChainID { get; set; }

        public uint Height { get; set; }

        public Dictionary<string, Peer> PeerMap { get; set; }=new Dictionary<string, Peer>();


        public int Size { get; }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteVarInt((long)ChainID);
            writer.WriteVarInt((long)Height);
            writer.WriteVarInt((long)PeerMap.Count);

            var peerList = PeerMap.Values.OrderByDescending(v => v.PeerPubkey).ToList();
            foreach (var peer in peerList)
            {
                peer.Serialize(writer);
            }
        }


        public void Deserialize(BinaryReader reader)
        {
            ChainID = reader.ReadVarInt();
            Height = (uint)reader.ReadVarInt();
            var count = reader.ReadVarInt();
            for (ulong i = 0; i < count; i++)
            {
                var peer = new Peer();
                peer.Deserialize(reader);
                PeerMap[peer.PeerPubkey] = peer;
            }
        }
    }

    public class Peer : ISerializable
    {
        public uint Index { get; set; }
        public string PeerPubkey { get; set; }
        public int Size { get; }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write((ulong)Index);
            writer.WriteVarString(PeerPubkey);
        }

        public void Deserialize(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}