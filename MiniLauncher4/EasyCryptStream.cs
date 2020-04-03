using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace MiniLauncher4
{
    public class EasyCryptStream : Stream
    {
        public static string Salt { get; set; }

        private const int tableSize = 64;

        //private readonly string pass = "unknown";

        private readonly byte[] salt = new UTF8Encoding().GetBytes(Salt);

        private readonly byte[] repTable;

        private Stream innerStream;

        public EasyCryptStream(Stream stream)
        {
            this.innerStream = stream;
            var sha = new SHA512Managed();
            //var r = new Rfc2898DeriveBytes(pass, salt);
            repTable = sha.ComputeHash(salt);
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => innerStream.Length;

        public override long Position { get => innerStream.Position; set => innerStream.Position = value; }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long lcur = innerStream.Position + offset;

            int icur = (int)(lcur % tableSize);

            var buf = new byte[buffer.Length];
            int ret = innerStream.Read(buf, offset, count);

            for (int i = 0; ret > i; i++, icur++)
            {
                buffer[i] = (byte)(buf[i] ^ this.repTable[icur % tableSize]);
            }

            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            long lcur = innerStream.Position + offset;

            int icur = (int)(lcur % tableSize);

            var buf = new byte[count];

            for (int i = 0; count > i; i++, icur++)
            {
                buf[i] = (byte)(buffer[i] ^ this.repTable[icur % tableSize]);
            }

            innerStream.Write(buf, offset, count);
        }
    }
}
