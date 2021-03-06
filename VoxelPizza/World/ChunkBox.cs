﻿using VoxelPizza.Numerics;

namespace VoxelPizza.World
{
    public readonly struct ChunkBox
    {
        public ChunkPosition Chunk { get; }
        public BlockPosition OuterOrigin { get; }
        public BlockPosition InnerOrigin { get; }
        public Size3 Size { get; }

        public ChunkBox(ChunkPosition chunk, BlockPosition outerOrigin, BlockPosition innerOrigin, Size3 size)
        {
            Chunk = chunk;
            OuterOrigin = outerOrigin;
            InnerOrigin = innerOrigin;
            Size = size;
        }
    }
}
