﻿using System;
using System.Diagnostics;

namespace VoxelPizza.World
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public struct BlockPosition : IEquatable<BlockPosition>
    {
        public int X;
        public int Y;
        public int Z;

        public BlockPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public readonly ChunkPosition ToChunk()
        {
            return new ChunkPosition(
                Chunk.BlockToChunkX(X),
                Chunk.BlockToChunkY(Y),
                Chunk.BlockToChunkZ(Z));
        }

        public readonly bool Equals(BlockPosition other)
        {
            return X == other.X
                && Y == other.Y
                && Z == other.Z;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is BlockPosition other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public readonly override string ToString()
        {
            return $"X:{X} Y:{Y} Z:{Z}";
        }

        private readonly string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
