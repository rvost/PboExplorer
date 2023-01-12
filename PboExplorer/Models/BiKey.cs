﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PboExplorer.Models;

// Based on https://github.com/wrdg/Bisign2Bikey
class BiKey
{
    public string Authority => Encoding.UTF8.GetString(Buffer);

    public byte[] Buffer { get; init; }
    public uint Temp { get; init; }
    public uint[] Garbage { get; init; }
    public uint Length { get; init; }
    public uint Exponent { get; init; }
    public byte[] Key { get; init; }
    public uint Version { get; init; }

    public override string ToString()
    {
        return BitConverter.ToString(Key).Replace("-", ":");
    }

    public void WriteToStream(Stream output)
    {
        using var writer = new BinaryWriter(output);

        writer.Write(Buffer);
        writer.Write((byte)0);

        writer.Write(Temp);

        writer.Write(Garbage[0]);
        writer.Write(Garbage[1]);
        writer.Write(Garbage[2]);

        writer.Write(Length);
        writer.Write(Exponent);

        writer.Write(Key);
    }

    public static BiKey ReadFromSignature(Stream input)
    {
        using (var reader = new BinaryReader(input))
        {
            var buffer = new List<byte>();

            while (reader.PeekChar() != 0)
            {
                var num = reader.ReadByte();
                buffer.Add(num);
            }

            reader.ReadByte();
            var authority = Encoding.UTF8.GetString(buffer.ToArray());

            var temp = reader.ReadUInt32();

            uint[] garbage = {
                reader.ReadUInt32(),
                reader.ReadUInt32() ,
                reader.ReadUInt32()
            };

            var length = reader.ReadUInt32();
            var exponent = reader.ReadUInt32();

            if (temp != length / 8 + 20)
            {
                throw new InvalidOperationException("Improper signature");
            }

            var key = reader.ReadBytes((int)length / 8);
            var block = reader.ReadUInt32();
            reader.ReadBytes((int)block);

            var version = reader.ReadUInt32();

            return new BiKey()
            {
                Buffer = buffer.ToArray(),
                Temp = temp,
                Garbage = garbage,
                Length = length,
                Exponent = exponent,
                Key = key,
                Version = version
            };
        }
    }
}