using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.IO;

namespace Neo.SmartContract.CrossChain
{
    public static class ConstResource
    {
        /// <summary>
        /// 当前最新版本号
        /// </summary>
        public const int CURRENT_HEADER_VERSION = 1;
        /// <summary>
        /// 支持跨链的ChainID的版本号
        /// </summary>
        public const int SUPPORT_CHAIN_VERSION = 1;

        /// <summary>
        /// ONT链ID
        /// </summary>
        public const ulong ONT_CHAIN_ID = 2;

        /// <summary>
        /// NEO链ID
        /// </summary>
        public const ulong NEO_CHAIN_ID = 3;

        /// <summary>
        /// Relay链ID
        /// </summary>
        public const ulong RELAY_CHAIN_ID = 4;


        /// <summary>
        /// 默认交易hash
        /// </summary>
        public static UInt256 DEFAULT_TX_HASH = new UInt256(Enumerable.Repeat<byte>(0, 32).ToArray());


        /// <summary>
        /// 写入uint数据（不写长度，Hash256）
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static void WriteHash256(this BinaryWriter writer, UInt256 txId)
        {
            writer.Write(txId.ToArray());
        }


        /// <summary>
        /// 读取为UInt256（Hash256）
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static UInt256 ReadHash256(this BinaryReader reader)
        {
            return new UInt256(reader.ReadBytes(32));
        }

        /// <summary>
        /// 读取为UInt160（address）
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static UInt160 ReadHash160(this BinaryReader reader)
        {
            return new UInt160(reader.ReadBytes(20));
        }

        /// <summary>
        /// 连接byte[]
        /// </summary>
        /// <param name="first"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static byte[] ConcatArray(this IEnumerable<byte> first, params IEnumerable<byte>[] paras)
        {
            if (paras?.Length > 0)
            {
                foreach (var para in paras)
                {
                    first = first.Concat(para);
                }
            }
            return first.ToArray();
        }

        /// <summary>
        /// string 转化为byte[]
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] ToUTF8Bytes(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }


        /// <summary>
        /// byte[] 转化为string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToUTF8String(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 数字按照bigint转换为byte[]
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] ToBigIntegerBytes(this uint num)
        {
            return ((BigInteger)num).ToByteArray();
        }

        /// <summary>
        /// 数字按照bigint转换为byte[]
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] ToBigIntegerBytes(this ulong num)
        {
            return ((BigInteger)num).ToByteArray();
        }

        /// <summary>
        /// 多签验证
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKeys"></param>
        /// <param name="limit"></param>
        /// <param name="signs"></param>
        /// <returns></returns>
        public static bool VerifyMultiSignature(byte[] data, List<byte[]> publicKeys, int limit, List<byte[]> signs)
        {
            if (signs.Count < limit)
            {
                //签名过少
                return false;
            }

            var crypto = new Crypto();

            foreach (var sign in signs)
            {
                if (publicKeys.Any(pubkey => crypto.VerifySignature(data, sign, pubkey)))
                {
                    //当前签名验证通过，验下一个
                    continue;
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// 读取NextVarBytes转换为ulong
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ulong DecodeVarInt(this BinaryReader reader, ulong max = ulong.MaxValue)
        {
            var value = reader.ReadVarBytes();
            var big = new BigInteger(value);
            return (ulong)big;
        }


        /// <summary>
        /// ulong转化为NextVarBytes
        /// </summary>
        /// <param name="write"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void EncodeVarInt(this BinaryWriter write, ulong value)
        {
            var bigValue = (BigInteger)value;
            write.WriteVarBytes(bigValue.ToByteArray());

        }
    }
}
