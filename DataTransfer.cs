using System;
using System.Linq;

namespace SuperFramework
{
    public abstract class DataTransfer : SafeObject
    {
        public abstract bool IsConnected { get; }
        protected virtual byte[] BeginFrame { get; }
        protected virtual byte[] EndFrame { get; }

        protected abstract byte[] ReadByStream(int maxCount, int timeout);
        protected abstract int WriteToStream(byte[] data, int timeout);

        protected virtual bool CheckData(byte[] data) => true;
        public bool ReadByte(out byte data, int timeout = -1)
        {
            var buffer = ReadByStream(1, timeout);
            if (buffer?.Length == 1)
            {
                data = buffer[0];
                return true;
            }
            data = 0;
            return false;
        }
        public int Read(byte[] buffer, int offset, int count, int timeout = -1)
        {
            var data = ReadByStream(count, timeout);
            if (data != null)
            {
                Array.Copy(data, 0, buffer, offset, data.Length);
                return data.Length;
            }
            return -1;
        }
        public bool ReadBytes(int count, out byte[] data, int timeout = -1)
        {
            var buffer = new byte[0];
            do
            {
                var temp = ReadByStream(count - (buffer?.Length ?? 0), timeout);
                if (temp?.Length > 0)
                {
                    buffer = buffer.Join(temp);
                }
                else
                {
                    data = null;
                    return false;
                }
            } while ((buffer?.Length ?? 0) < count);
            data = buffer;
            return data?.Length == count;
        }
        public bool WriteByte(byte value) => WriteToStream(new byte[] { value }, -1) == 1;
        public int Write(byte[] buffer, int offset, int count)
        {
            var data = new byte[count];
            Array.Copy(buffer, offset, data, 0, count);
            return WriteToStream(data, -1);
        }
        public bool FindBeginFrame()
        {
            if (BeginFrame?.Length == 0)
                throw new Exception("按帧头接数据必须重写 \"BeginFrame\" 属性以指定帧头");
            byte[] buffer = null;
            int index = -1;
            do
            {
                buffer = buffer.Join(ReadByStream(BeginFrame.Length - (buffer?.Length ?? 0), -1));
                index = buffer.IndexOfBlock(BeginFrame, index);
                if ((buffer?.Length >= BeginFrame?.Length) && index < 0)
                    buffer = buffer.RemoveBlock(0, 1);
            } while (index < 0);
            return true;
        }
        public byte[] ReadToEndFrame()
        {
            if (BeginFrame?.Length == 0 || EndFrame?.Length == 0)
                throw new Exception("按帧头接数据必须重写 \"BeginFrame\"&\"EndFrame\"&\"CheckData\" 属性以指定帧头&帧尾和数据确认方法");
            byte[] buffer = null;
            int index = -1;
            buffer = ReadByStream(EndFrame.Length - (buffer?.Length ?? 0), -1);
        FIND_FRAME:
            index = buffer.IndexOfBlock(EndFrame, index);
            if (index < 0 || !CheckData(buffer))
            {
                buffer = buffer.Join(ReadByStream(1, -1));
                goto FIND_FRAME;
            }
            return buffer.GetBlock(0, buffer.Length - EndFrame.Length);
        }
    }
}