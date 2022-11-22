using System.Text;

namespace System.IO
{
    public static class StreamEx
    {
        public static void Write(this Stream stream, byte[] buffer) => stream.Write(buffer, 0, buffer.Length);

        public static void Write(this Stream stream, string text) => stream.Write(Encoding.UTF8.GetBytes(text));

        public static void WriteLine(this Stream stream) => stream.WriteLine(string.Empty);
        public static void WriteLine(this Stream stream, string text) => stream.Write($"{text}\r\n");
    }
}
