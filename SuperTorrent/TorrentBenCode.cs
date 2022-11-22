
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperFramework.SuperTorrent
{
    interface IBenCode
    {
        byte[] ToByteArray();
    }

    abstract class BenByteString : IBenCode
    {
        abstract protected byte[] GetByteArray();
        public byte[] ToByteArray()
        {
            List<byte> byteList = new();
            byte[] byteContent = GetByteArray();
            string sizeHeader = string.Format("{0}:", byteContent.Length);
            byteList.AddRange(Encoding.UTF8.GetBytes(sizeHeader));
            byteList.AddRange(byteContent);
            return byteList.ToArray();
        }
    }

    class BenStringFormString : BenByteString, IComparable<BenStringFormString>
    {
        public BenStringFormString(string value)
        {
            m_value = value;
        }
 
        override protected byte[] GetByteArray()
        {
            return Encoding.UTF8.GetBytes(m_value);
        }

        public int CompareTo(BenStringFormString other)
        {
            return string.CompareOrdinal(m_value, other.m_value);
        }

        private string m_value;
    }

    class BenBinaryFormString : BenByteString
    {
        public BenBinaryFormString(byte[] binaryArray)
        {
            m_binaryArray = binaryArray;
        }

        override protected byte[] GetByteArray()
        {
            return m_binaryArray;
        }

        private byte[] m_binaryArray;
    }

    class BenInt : IBenCode
    {
        public BenInt(long value)
        {
            m_value = value;
        }

        public byte[] ToByteArray()
        {
            string resultInStringForm = string.Format("i{0}e", m_value);
            return Encoding.UTF8.GetBytes(resultInStringForm);
        }

        private long m_value;
    }

    class BenList : IBenCode
    {
        public BenList()
        {
            m_items = new List<IBenCode>();
        }

        public void Add(IBenCode item)
        {
            m_items.Add(item);
        }

        public byte[] ToByteArray()
        {
            List<byte> byteList = new();
            byteList.AddRange(Encoding.UTF8.GetBytes("l"));
            foreach (IBenCode item in m_items)
            {
                byteList.AddRange(item.ToByteArray());
            }
            byteList.AddRange(Encoding.UTF8.GetBytes("e"));
            return byteList.ToArray();
        }

        private List<IBenCode> m_items;
    }

    class BenDictionary : IBenCode
    {
        public BenDictionary()
        {
            m_directionary = new SortedDictionary<BenStringFormString, IBenCode>();
        }

        public void Add(string key, IBenCode value)
        {
            BenStringFormString benKey = new(key);
            Add(benKey, value);
        }

        public void Add(BenStringFormString key, IBenCode value)
        {
            m_directionary.Add(key, value);
        }

        public byte[] ToByteArray()
        {
            List<byte> byteList = new();
            byteList.AddRange(Encoding.UTF8.GetBytes("d"));
            foreach (KeyValuePair<BenStringFormString, IBenCode> entry in m_directionary)
            {
                byteList.AddRange(entry.Key.ToByteArray());
                byteList.AddRange(entry.Value.ToByteArray());
            }
            byteList.AddRange(Encoding.UTF8.GetBytes("e"));
            return byteList.ToArray();
        }

        private SortedDictionary<BenStringFormString, IBenCode> m_directionary;
    }
}
