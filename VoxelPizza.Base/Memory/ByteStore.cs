﻿using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoxelPizza
{
    public ref struct ByteStore<T>
        where T : unmanaged
    {
        private ArrayPool<byte> _arrayPool;
        private Span<T> _buffer;
        private byte[]? _array;
        private int _count;

        public int Count => _count;
        public Span<T> Buffer => _buffer;
        public Span<T> Span => _buffer.Slice(0, _count);

        public ByteStore(ArrayPool<byte> arrayPool, Span<T> buffer, byte[]? initialArray)
        {
            _arrayPool = arrayPool ?? throw new ArgumentNullException(nameof(arrayPool));
            _buffer = buffer;
            _array = initialArray;
            _count = 0;
        }

        public ByteStore(ArrayPool<byte> arrayPool) : this(arrayPool, default, null)
        {
        }

        public void EnsureCapacity(int capacity)
        {
            Debug.Assert(_arrayPool != null);

            if (_buffer.Length < capacity)
            {
                if (_array != null)
                    _arrayPool.Return(_array);

                Span<T> oldBuffer = Span;
                _array = _arrayPool.Rent((capacity + 4096) * Unsafe.SizeOf<T>());
                _buffer = MemoryMarshal.Cast<byte, T>(_array);
                oldBuffer.CopyTo(_buffer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PrepareCapacity(int count)
        {
            EnsureCapacity(Count + count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in T value)
        {
            _buffer[_count++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> GetAppendRange(int count)
        {
            Span<T> slice = _buffer.Slice(_count, count);
            _count += count;
            return slice;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendRange(ReadOnlySpan<T> values)
        {
            values.CopyTo(_buffer.Slice(_count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendRange(T item0, T item1, T item2, T item3)
        {
            Span<T> slice = _buffer.Slice(_count, 4);
            _count += 4;
            slice[0] = item0;
            slice[1] = item1;
            slice[2] = item2;
            slice[3] = item3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendRange(T item0, T item1, T item2, T item3, T item4,  T item5)
        {
            Span<T> slice = _buffer.Slice(_count, 6);
            _count += 6;
            slice[0] = item0;
            slice[1] = item1;
            slice[2] = item2;
            slice[3] = item3;
            slice[4] = item4;
            slice[5] = item5;
        }

        public void Clear()
        {
            _count = 0;
        }

        public void Dispose()
        {
            _buffer = null;

            if (_array != null)
                _arrayPool?.Return(_array);
            _array = null;
        }
    }
}
