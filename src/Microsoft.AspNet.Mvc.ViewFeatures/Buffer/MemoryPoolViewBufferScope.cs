// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.Mvc.ViewFeatures.Buffer
{
    /// <summary>
    /// A <see cref="IViewBufferScope"/> that uses pooled memory.
    /// </summary>
    public class MemoryPoolViewBufferScope : IViewBufferScope, IDisposable
    {
        public static readonly int SegmentSize = 512;
        private readonly ArrayPool<ViewBufferValue> _viewBufferPool;
        private readonly ArrayPool<char> _charPool;
        private List<ViewBufferValue[]> _leased;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryPoolViewBufferScope"/>.
        /// </summary>
        /// <param name="viewBufferPool">
        /// The <see cref="ArrayPool{ViewBufferValue}"/> for creating <see cref="ViewBufferValue"/> instances.
        /// </param>
        /// <param name="charPool">
        /// The <see cref="ArrayPool{char}"/> for creating <see cref="ViewBufferTextWriter"/> instances.
        /// </param>
        public MemoryPoolViewBufferScope(ArrayPool<ViewBufferValue> viewBufferPool, ArrayPool<char> charPool)
        {
            _viewBufferPool = viewBufferPool;
            _charPool = charPool;
        }

        /// <inheritdoc />
        public ViewBufferValue[] GetSegment()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(MemoryPoolViewBufferScope).FullName);
            }

            if (_leased == null)
            {
                _leased = new List<ViewBufferValue[]>(1);
            }

            ViewBufferValue[] segment = null;

            try
            {
                segment = _viewBufferPool.Rent(SegmentSize);
                _leased.Add(segment);
            }
            catch when (segment != null)
            {
                _viewBufferPool.Return(segment);
                throw;
            }

            return segment;
        }

        public ViewBufferTextWriter CreateWriter(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            return new ViewBufferTextWriter(_charPool, writer);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_leased == null)
                {
                    return;
                }

                for (var i = 0; i < _leased.Count; i++)
                {
                    _viewBufferPool.Return(_leased[i]);
                }

                _leased.Clear();
            }
        }
    }
}
