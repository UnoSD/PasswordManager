using System;
using System.Collections.Generic;
using System.IO;

namespace Paccia
{
    public class EnumerableStream<T> : Stream
    {
        readonly Func<T, byte[]> _readMethod;
        readonly IEnumerator<T> _source;
        bool _completed;

        public EnumerableStream(IEnumerable<T> source, Func<T, byte[]> readMethod)
        {
            _readMethod = readMethod;
            _source = source.GetEnumerator();
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin) { throw new InvalidOperationException(); }

        public override void SetLength(long value) { throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_completed)
                throw new EndOfStreamException();

            if (count == 0)
                return 0;

            if (!_source.MoveNext())
            {
                _completed = true;

                return 0;
            }

            var bytes = _readMethod(_source.Current);

            for (var index = 0; index < bytes.Length; index++)
                buffer[offset + index] = bytes[index];

            return bytes.Length;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = false;
        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
    }
}