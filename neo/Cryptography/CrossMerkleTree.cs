using Neo.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Cryptography
{
    public class CrossMerkleTree
    {
        private MerkleTreeNode root;

        public int Depth { get; private set; }

        const byte LEFT = 0;
        const byte RIGHT = 1;

        internal CrossMerkleTree(UInt256[] hashes)
        {
            if (hashes.Length == 0) throw new ArgumentException();
            this.root = Build(hashes.Select(p => new MerkleTreeNode { Hash = p }).ToArray());
            int depth = 1;
            for (MerkleTreeNode i = root; i.LeftChild != null; i = i.LeftChild)
                depth++;
            this.Depth = depth;
        }

        private static MerkleTreeNode Build(MerkleTreeNode[] leaves)
        {
            if (leaves.Length == 0) throw new ArgumentException();
            if (leaves.Length == 1) return leaves[0];
            MerkleTreeNode[] parents = new MerkleTreeNode[(leaves.Length + 1) / 2];
            for (int i = 0; i < parents.Length; i++)
            {
                parents[i] = new MerkleTreeNode();
                parents[i].LeftChild = leaves[i * 2];
                leaves[i * 2].Parent = parents[i];
                if (i * 2 + 1 == leaves.Length)
                {
                    parents[i].RightChild = parents[i].LeftChild;
                }
                else
                {
                    parents[i].RightChild = leaves[i * 2 + 1];
                    leaves[i * 2 + 1].Parent = parents[i];
                }
                byte [] a = { 1 };
                parents[i].Hash = new UInt256((a.Concat(parents[i].LeftChild.Hash.ToArray()).Concat(parents[i].RightChild.Hash.ToArray()).ToArray()).Sha256());
            }
            return Build(parents); //TailCall
        }

        public static UInt256 ComputeRoot(UInt256[] hashes)
        {
            if (hashes.Length == 0) throw new ArgumentException();
            if (hashes.Length == 1) return hashes[0];
            CrossMerkleTree tree = new CrossMerkleTree(hashes);
            return tree.root.Hash;
        }

        private static void DepthFirstSearch(MerkleTreeNode node, IList<UInt256> hashes)
        {
            if (node.LeftChild == null)
            {
                // if left is null, then right must be null
                hashes.Add(node.Hash);
            }
            else
            {
                DepthFirstSearch(node.LeftChild, hashes);
                DepthFirstSearch(node.RightChild, hashes);
            }
        }

        // depth-first order
        public UInt256[] ToHashArray()
        {
            List<UInt256> hashes = new List<UInt256>();
            DepthFirstSearch(root, hashes);
            return hashes.ToArray();
        }

        public void Trim(BitArray flags)
        {
            flags = new BitArray(flags);
            flags.Length = 1 << (Depth - 1);
            Trim(root, 0, Depth, flags);
        }

        private static void Trim(MerkleTreeNode node, int index, int depth, BitArray flags)
        {
            if (depth == 1) return;
            if (node.LeftChild == null) return; // if left is null, then right must be null
            if (depth == 2)
            {
                if (!flags.Get(index * 2) && !flags.Get(index * 2 + 1))
                {
                    node.LeftChild = null;
                    node.RightChild = null;
                }
            }
            else
            {
                Trim(node.LeftChild, index * 2, depth - 1, flags);
                Trim(node.RightChild, index * 2 + 1, depth - 1, flags);
                if (node.LeftChild.LeftChild == null && node.RightChild.RightChild == null)
                {
                    node.LeftChild = null;
                    node.RightChild = null;
                }
            }
        }

        public static byte[] MerkleProve(byte[] path, UInt256 root)
        {
            using (MemoryStream ms = new MemoryStream(path, false))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                int l = (int)reader.ReadUInt64();
                var value = reader.ReadBytes(l);
                var hash = HashLeaf(value);
                int size = (int)(ms.Length - ms.Position) / 32;
                for (int i = 0 ; i < size ; i++ )
                {
                    var f = reader.ReadByte();
                    byte[] v = reader.ReadBytes(32);
                    if (f == 0)
                        hash = HashChildren(v, hash);
                    else
                        hash = HashChildren(hash, v);
                }
                if(!ByteArrayEquals(hash,root.ToArray()))
                {
                    return null;
                }                   
                return value;
            }      
        }

        public static byte[] GetMerkleProof(byte[] data,UInt256[] hashes)
        {
            int index = GetIndex(HashLeaf(data), hashes);
            if (index < 0)
                return null;
            int d = GetDepth(hashes.Length);
            var merkleTree = MerkleHashes(hashes,d);

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                ulong s = (ulong)data.Length;
                writer.Write(s);
                writer.Write(data);
                for(int i = d ; i > 0 ; i--)
                {
                    var subTree = merkleTree[i];
                    var subLen = subTree.Length;
                    var nIndex = index / 2 ;
                    if(index == subLen - 1 && subLen % 2 != 0)
                    {
                        index = nIndex;
                        continue;
                    }
                    if (index % 2 != 0)
                    {
                        writer.Write(LEFT);
                        writer.Write(subTree[index - 1].ToArray());
                    }
                    else
                    {
                        writer.Write(RIGHT);
                        writer.Write(subTree[index + 1].ToArray());
                    }
                    index = nIndex;
                }
                return ms.ToArray();
            }
        }
        
        public static UInt256[][] MerkleHashes(UInt256[] preLeaves, int depth)
        {
            UInt256[][] levels = new UInt256[depth+1][];
            levels[depth] = preLeaves;
            for(int i = depth; i > 0 ; i--)
            {
                var level = levels[i];
                int levelLen = level.Length;
                int remainder = levelLen % 2;
                var nextLevel = new UInt256[levelLen / 2 + remainder];
                int k = 0;
                for(int j = 0; j < levelLen -1; j += 2)
                {
                    var left = level[j];
                    var right = level[j + 1];
                    nextLevel[k] = new UInt256(HashChildren(left.ToArray(),right.ToArray()));
                    k++;
                }
                if(remainder!=0)
                {
                    nextLevel[k] = level[levelLen - 1];
                }
                levels[i - 1] = nextLevel;
            }
            return levels;
        }

        public static int GetIndex(byte[] v, UInt256[] hashes)
        {
            for(int i=0;i<hashes.Length;i++ )
            {
                if (hashes[i].Equals(new UInt256(v)))
                    return i;
            }
            return -1;
        }

        public static byte[] HashChildren(byte[] v, byte[] hash)
        {
            byte[] prefix = { 1 };
            return prefix.Concat(v).Concat(hash).Sha256();
        }

        public static byte[] HashLeaf(byte[] value)
        {
            byte[] prefix = { 0 };
            return prefix.Concat(value).Sha256();
        }

        public static int GetDepth(int n)
        {
            return (int)(Math.Ceiling(Math.Log((float)n, 2)));
        }

        public static bool ByteArrayEquals(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }
    }
}
