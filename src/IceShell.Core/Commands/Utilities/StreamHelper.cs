using System.Buffers;

namespace IceShell.Core.Commands.Utilities;

internal static class StreamHelper
{
    private const int TextBufferSize = 512;

    internal static void CopyText(TextReader from, TextWriter to)
    {
        char[] bufArr = ArrayPool<char>.Shared.Rent(TextBufferSize);
        Span<char> buf = bufArr.AsSpan();

        try
        {
            int readLen = from.Read(buf);
            if (readLen == 0)
            {
                return;
            }

            to.Write(buf[..readLen]);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(bufArr);
        }
    }
}
