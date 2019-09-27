using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Neo.SmartContract.CrossChain
{
    public class VbftBlockInfo
    {
        [JsonProperty("leader")]
        public uint Proposer { get; set; }

        [JsonProperty("vrf_value")]
        public byte[] VrfValue { get; set; }

        [JsonProperty("vrf_proof")]
        public byte[] VrfProof { get; set; }
        [JsonProperty("last_config_block_num")]
        public uint LastConfigBlockNum { get; set; }

        [JsonProperty("new_chain_config")]
        public ChainConfig NewChainConfig { get; set; }
    }

    public class ChainConfig
    {
        [JsonProperty("version")]
        public uint Version { get; set; }  // software version
        [JsonProperty("view")]
        public uint View { get; set; }   // config-updated version
        [JsonProperty("n")]
        public uint N { get; set; }            // network size
        [JsonProperty("c")]
        public uint C { get; set; }     // consensus quorum
        //public  BlockMsgDelay time.Duration `json:"block_msg_delay"`

        //HashMsgDelay time.Duration `json:"hash_msg_delay"`

        //PeerHandshakeTimeout time.Duration `json:"peer_handshake_timeout"`

        [JsonProperty("peers")]
        public PeerConfig[] Peers { get; set; }
        [JsonProperty("pos_table")]
        public uint[] PosTable { get; set; }
        [JsonProperty("MaxBlockChangeView")]
        public uint MaxBlockChangeView { get; set; }
    }

    public class PeerConfig
    {
        public uint Index { get; set; }
        public string ID { get; set; }
    }


}
