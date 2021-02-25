﻿using System;
using System.Buffers;
using System.Numerics;

namespace VoxelPizza.Client
{
    public class ChunkMesher
    {
        public StoredChunkMesh Mesh(ChunkVisual visual)
        {
            var ind = new ByteStore<uint>(ArrayPool<byte>.Shared);
            var spa = new ByteStore<ChunkSpaceVertex>(ArrayPool<byte>.Shared);
            var pai = new ByteStore<ChunkPaintVertex>(ArrayPool<byte>.Shared);

            var indGen = new CubeIndexGenerator();
            var indPro = new CubeIndexProvider<CubeIndexGenerator, uint>(indGen, CubeFaces.All);
            uint vertexOffset = 0;

            TextureAnimation[] anims = new TextureAnimation[]
            {
                TextureAnimation.Create(TextureAnimationType.Step, 3, 1f),
                TextureAnimation.Create(TextureAnimationType.MixStep, 3, 1f),
                TextureAnimation.Create(TextureAnimationType.Step, 2, 1f),
                TextureAnimation.Create(TextureAnimationType.MixStep, 2, 1f),
                TextureAnimation.Create(TextureAnimationType.Step, 3, 1.5f),
                TextureAnimation.Create(TextureAnimationType.MixStep, 3, 1.5f),
                TextureAnimation.Create(TextureAnimationType.Step, 2, 1.5f),
                TextureAnimation.Create(TextureAnimationType.MixStep, 2, 1.5f),
            };

            var rng = new Random();

            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        double fac = (visual.ChunkY * 16 + y) / 2048.0;
                        if (rng.NextDouble() > 0.1 * fac)
                            continue;

                        var spaGen = new CubeSpaceVertexGenerator(new Vector3(x, y, z));

                        var anim = anims[rng.Next(anims.Length)];
                        var paiGen = new CubePaintVertexGenerator(anim, 0);

                        var spaPro = new CubeVertexProvider<CubeSpaceVertexGenerator, ChunkSpaceVertex>(spaGen, CubeFaces.All);
                        var paiPro = new CubeVertexProvider<CubePaintVertexGenerator, ChunkPaintVertex>(paiGen, spaPro.Faces);

                        spaPro.AppendVertices(ref spa);
                        paiPro.AppendVertices(ref pai);
                        indPro.AppendIndices(ref ind, ref vertexOffset);
                    }
                }
            }

            return new StoredChunkMesh(ind, spa, pai);
        }
    }
}