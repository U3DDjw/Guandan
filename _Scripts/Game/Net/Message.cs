using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using STR_LEN = System.Int32;
using ZhiWa;
using UnityEngine;
using MsgContainer;

public class Message
{

    private int _position = 0;
    private int _size = 0;
    private string _type;
    private byte[] _buffer;



     const  int INIT_LENGTH = 1024;
        public Message()
        {
            _buffer = new byte[INIT_LENGTH];
            _position = 0;
            _size = 0;
        }


    public Message(byte[] bytes)
    {
        _buffer = new byte[INIT_LENGTH];
        addToBuffer(bytes,0, bytes.Length);
    }

    public void init(byte[] bytes, int size, string msgKey)
        {
            try
            {
                Array.Copy(bytes, 0, _buffer, 0, size);
                this._size = size;
                _type = msgKey;
                _position = 0;
            }
            catch (System.Exception what)
            {
                Debug.LogError(" **************************** " + what.ToString() + "name:" + _type);
            }
        }


        public string getType()
        {
            return _type;
        }

        public void write(byte val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            _buffer[_position] = bytes[0];
            ++_position;
            ++_size;
        }

        public void write(Int16 val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            for (int i = 0; i < sizeof(short); ++i)
            {
                _buffer[_position] = bytes[i];
                ++_position;
                ++_size;
            }
        }

        public void write(Int32 val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            for (int i = 0; i < sizeof(int); ++i)
            {
                _buffer[_position] = bytes[i];
                ++_position;
                ++_size;
            }
        }

        public void write(Int64 val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            for (int i = 0; i < sizeof(long); ++i)
            {
                _buffer[_position] = bytes[i];
                ++_position;
                ++_size;
            }
        }

        public void write(byte[] bytes)
        {
            write((STR_LEN)bytes.Length);
            addToBuffer(bytes, 0, bytes.Length);
        }

        public void write(string val)
        {
            write((STR_LEN)val.Length);
            addToBuffer(Encoding.Default.GetBytes(val), 0, val.Length); //	Encoding.UTF8.GetBytes(s); Encoding.UTF8.GetString(t_data, 0, i);
        }

        public void writeUnicode(string val)
        {
            write((STR_LEN)val.Length);
            addToBuffer(Encoding.Unicode.GetBytes(val), 0, val.Length);	//	Encoding.UTF8.GetBytes(s); Encoding.UTF8.GetString(t_data, 0, i);
        }

        public byte ReadByte()
        {
            byte val = _buffer[_position];
            ++_position;
            return val;
        }
    /// <summary>
    /// 读取VarintInt 方式的字节
    /// </summary>
    /// <returns></returns>
    public byte ReadRawVarintByte()
    {
        byte val = _buffer[_position];
        if ((val & 128) > 0) //大于127
        {
            var t = ~((int)val);
            val = (byte)t;
        }
        ++_position;
        return val;
    }
    public Int16 ReadInt16()
        {
            const int SIZE = sizeof(Int16);
            byte[] bytes = new byte[SIZE];
            Array.Copy(_buffer, _position, bytes, 0, SIZE);
            Int16 val = BitConverter.ToInt16(bytes, 0);
            _position += SIZE;
            return val;
        }


        public Int32 ReadInt32()
        {
            const int SIZE = sizeof(Int32);
            byte[] bytes = new byte[SIZE];
            Array.Copy(_buffer, _position, bytes, 0, SIZE);
            Int32 val = BitConverter.ToInt32(bytes, 0);
            _position += SIZE;
            return val;
        }

        public UInt32 ReadUInt32()
        {
            const int SIZE = sizeof(UInt32);
            byte[] bytes = new byte[SIZE];
            Array.Copy(_buffer, _position, bytes, 0, SIZE);
            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            _position += SIZE;
            return val;
        }

        public Int64 ReadInt64()
        {
            const int SIZE = sizeof(Int64);
            byte[] bytes = new byte[SIZE];
            Array.Copy(_buffer, _position, bytes, 0, SIZE);
            Int64 val = BitConverter.ToInt64(bytes, 0);
            _position += SIZE;
            return val;
        }

        public UInt64 ReadUInt64()
        {
            const int SIZE = sizeof(UInt64);
            byte[] bytes = new byte[SIZE];
            Array.Copy(_buffer, _position, bytes, 0, SIZE);
            UInt64 val = BitConverter.ToUInt64(bytes, 0);
            _position += SIZE;
            return val;
        }

        public byte[] ReadBytes()
        {
            byte[] bytes = new byte[this._size];
            Array.Copy(_buffer, 0, bytes, 0, this._size);
            return bytes;
        }

        public string ReadString()
        {
            Int32 size = ReadInt32();
            byte[] bytes = new byte[size];
            string str = Encoding.Default.GetString(_buffer, _position, size);
            _position += size;
            return str;
        }

        public void readUnicode(ref string val)
        {
            Int32 size = ReadInt32();
            val = Encoding.UTF8.GetString(_buffer, _position, size);
            _position += size;
        }

        public int getSize()
        {
            return _size;
        }

        public byte[] getBuffer()
        {
            return _buffer;
        }


        public  void addToBuffer(byte[] bytes, int index, int size)
        {
            Array.Copy(bytes, index, _buffer, _position, size);
            _position += size;
            _size += size;
        }

    public void AddBuffers(byte[] bytes, int size)
    {
        Array.Copy(bytes, 0, _buffer, _position, size);
        _position += size;
        _size += size;
    }

    public void WriteRaw(int value)
    {
        while (true)
        {
            if ((value & ~0x7F) == 0)
            {
                Console.WriteLine("value:" + value);
                this.write((byte)value);
                return;
            }
            else
            {
                var t = (value & 0x7F) | 0x80;
                this.write((byte)t);
                value >>= 7;
            }
        }
    }

    public void ReadRawVarint()
    {
        _position = 0;

        byte tmp = this.ReadByte();
    }
}

