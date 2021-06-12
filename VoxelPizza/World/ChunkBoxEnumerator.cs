﻿using System;
using System.Collections;
using System.Collections.Generic;
using VoxelPizza.Numerics;

namespace VoxelPizza.World
{
    public struct ChunkBox
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

    public struct ChunkBoxEnumerator : IEnumerator<ChunkBox>
    {
        private int processedY;
        private int blockY;
        private int chunkY;
        private int height;

        private int processedZ;
        private int blockZ;
        private int chunkZ;
        private int depth;

        private int processedX;
        private int blockX;
        private int chunkX;
        private int width;

        public BlockPosition Origin { get; }
        public Size3 Size { get; }
        public BlockPosition Max { get; }

        public ChunkPosition CurrentChunk => new(chunkX, chunkY, chunkZ);

        public ChunkBox Current
        {
            get
            {
                ChunkPosition chunk = CurrentChunk;
                Size3 size = new((uint)width, (uint)height, (uint)depth);

                BlockPosition innerOrigin = new(
                    (int)((uint)blockX % Chunk.Width),
                    (int)((uint)blockY % Chunk.Depth),
                    (int)((uint)blockZ % Chunk.Height));

                BlockPosition outerOrigin = new(
                    (blockX - Origin.X),
                    (blockY - Origin.Y),
                    (blockZ - Origin.Z));

                return new ChunkBox(chunk, outerOrigin, innerOrigin, size);
            }
        }

        object IEnumerator.Current => Current;

        public ChunkBoxEnumerator(BlockPosition origin, Size3 size) : this()
        {
            Origin = origin;
            Size = size;
            Max = new WorldBox(origin, size).Max;

            UpdateY();
            UpdateZ();
        }

        public ChunkBoxEnumerator GetEnumerator()
        {
            return this;
        }

        private void UpdateY()
        {
            blockY = Origin.Y + processedY;
            chunkY = Chunk.BlockToChunkY(blockY);
            int min1Y = chunkY * Chunk.Height;
            int max1Y = min1Y + Chunk.Height;
            int bottomSide = Math.Max(min1Y, Origin.Y);
            int topSide = Math.Min(max1Y, Max.Y);
            height = topSide - bottomSide;
        }

        private void UpdateZ()
        {
            blockZ = Origin.Z + processedZ;
            chunkZ = Chunk.BlockToChunkZ(blockZ);
            int min1Z = chunkZ * Chunk.Depth;
            int max1Z = min1Z + Chunk.Depth;
            int backSide = Math.Max(min1Z, Origin.Z);
            int frontSide = Math.Min(max1Z, Max.Z);
            depth = frontSide - backSide;
        }

        public bool MoveNext()
        {
            while (processedY < Size.H)
            {
                while (processedZ < Size.D)
                {
                    while (processedX < Size.W)
                    {
                        blockX = Origin.X + processedX;
                        chunkX = Chunk.BlockToChunkX(blockX);
                        int min1X = chunkX * Chunk.Width;
                        int max1X = min1X + Chunk.Width;
                        int leftSide = Math.Max(min1X, Origin.X);
                        int rightSide = Math.Min(max1X, Max.X);
                        width = rightSide - leftSide;

                        processedX += width;

                        return true;
                    }

                    processedX = 0;
                    // X will be updated in the next call

                    processedZ += depth;
                    UpdateZ();
                }

                processedZ = 0;
                UpdateZ();

                processedY += height;
                UpdateY();
            }

            return false;
        }

        public void Reset()
        {
            processedY = 0;
            chunkY = 0;
            height = 0;

            processedZ = 0;
            chunkZ = 0;
            depth = 0;

            processedX = 0;
            chunkX = 0;
            width = 0;
        }

        public void Dispose()
        {
        }
    }
}
