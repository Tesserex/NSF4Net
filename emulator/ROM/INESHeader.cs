/* This file is part of My Nes
 * A Nintendo Entertainment System Emulator.
 *
 * Copyright © Ala Ibrahim Hadid 2009 - 2013
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using MyNes.Core.Types;

namespace MyNes.Core.ROM
{
    /// <summary>
    /// Encapsulates an INES format header
    /// </summary>
    public class INESHeader
    {
        private const int bufferChunk = 0x80000;// 512 KB
        public bool IsValid;
        public byte ChrPages;
        public byte PrgPages;
        public byte Mapper;
        public bool HasTrainer;
        public bool HasSaveRam;
        public bool IsVram;
        public bool IsVSUnisystem;
        public bool IsPlaychoice10;
        public bool IsVersion2;
        public Mirroring Mirroring;
        public INESRVSystem TVSystem;

        /// <summary>
        /// Create a new instance of INES format header
        /// </summary>
        public INESHeader()
        {

        }
        /// <summary>
        /// Create a new instance of INES format header
        /// </summary>
        /// <param name="romPath">The absolute ROM path</param>
        public INESHeader(string romPath)
        {
            FileStream stream = new FileStream(romPath, FileMode.Open, FileAccess.Read);
            byte[] header = new byte[16];

            stream.Read(header, 0, 16);
            stream.Close();

            if (header[0] != 'N' ||
                header[1] != 'E' ||
                header[2] != 'S' ||
                header[3] != 0x1A)
            {
                IsValid = false;
                return;
            }

            PrgPages = header[4];
            ChrPages = header[5];

            IsVram = ChrPages == 0;

            switch (header[6] & 0x9)
            {
                case 0x0: this.Mirroring = Types.Mirroring.ModeHorz; break;
                case 0x1: this.Mirroring = Types.Mirroring.ModeVert; break;
                case 0x8:
                case 0x9: this.Mirroring = Types.Mirroring.ModeFull; break;
            }

            HasSaveRam = (header[6] & 0x2) != 0x0;
            HasTrainer = (header[6] & 0x4) != 0x0;

            if ((header[7] & 0x0F) == 0)
                Mapper = (byte)((header[7] & 0xF0) | (header[6] >> 4));
            else
                Mapper = (byte)((header[6] >> 4));

            IsVSUnisystem = (header[7] & 0x01) != 0;
            IsPlaychoice10 = (header[7] & 0x02) != 0;

            //Pal system detect, Though in the official specification, very few emulators honor this 
            //bit as virtually no ROM images in circulation make use of it. 
            TVSystem = (header[9] & 1) == 1 ? INESRVSystem.PAL : INESRVSystem.NTSC;
            IsValid = true;
        }
        /// <summary>
        /// Save this header to a file.
        /// </summary>
        /// <param name="fileName">The complete file path. This file must be INES file with complete header.</param>
        public void SaveFile(string fileName)
        {
            // 1 the original file stream
            Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // create the temp file
            int j = 0;
            string tempFolder = Path.GetDirectoryName(fileName);
            if (tempFolder.Length == 0)
                tempFolder = Path.GetPathRoot(fileName);
            string tempName = tempFolder + Path.GetFileNameWithoutExtension(fileName) + "_temp" + j;
            while (File.Exists(tempName))
            {
                j++;
                tempName = tempFolder + Path.GetFileNameWithoutExtension(fileName) + "_temp" + j;
            }
            Stream tempStream = new FileStream(tempName, FileMode.Create, FileAccess.Write);
            try
            {
                // write the header buffer to the temp file
                byte[] data = GetBuffer();

                tempStream.Write(GetBuffer(), 0, data.Length);

                // skip original file header
                fileStream.Position = 16;

                // copy the original file data to the temp file without id3
                byte[] buff = new byte[bufferChunk];
                // We can't just read file as one byte buffer and write it directly into temp, what if the file is too large ?
                // so do it chunck by chunck to limit the buffer memory to bufferChunk value....
                // larger bufferChunk value make saving goes faster but need more memory.
                // however, bufferChunk = 1 MB for now.
                while (fileStream.Position <= fileStream.Length)
                {
                    long siz = fileStream.Length - fileStream.Position;
                    if (siz >= bufferChunk)
                    {
                        buff = new byte[bufferChunk];
                        fileStream.Read(buff, 0, bufferChunk);// ->|
                        tempStream.Write(buff, 0, bufferChunk);//<-|
                    }
                    else
                    {
                        buff = new byte[siz];
                        fileStream.Read(buff, 0, (int)(siz));// ->|
                        tempStream.Write(buff, 0, (int)(siz));//<-|
                        break;
                    }
                }

                // close streams
                fileStream.Close();
                fileStream.Dispose();
                tempStream.Close();
                tempStream.Dispose();
                // delete original then make temp as original 
                File.Delete(fileName);
                File.Copy(tempName, fileName);
                File.Delete(tempName);
            }
            catch
            {
                // close streams
                fileStream.Close();
                fileStream.Dispose();
                tempStream.Close();
                tempStream.Dispose();
                // delete temp if found
                if (File.Exists(tempName))
                    File.Delete(tempName);
            }
        }
        /// <summary>
        /// Save header as
        /// </summary>
        /// <param name="originalFile">The original file which includes rom banks. Header of this file is not used at all.</param>
        /// <param name="targetPath">The target file where the header with banks will be saved.</param>
        public void SaveAs(string originalFile, string targetPath)
        {
            // 1 the original file stream
            Stream fileStream = new FileStream(originalFile, FileMode.Open, FileAccess.Read);
            // 2 target file stream
            Stream tempStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);

            try
            {
                // write the header buffer to the target file
                byte[] data = GetBuffer();

                tempStream.Write(GetBuffer(), 0, data.Length);

                // skip original file header
                fileStream.Position = 16;

                // copy the original file data to the temp file without id3
                byte[] buff = new byte[bufferChunk];
                // We can't just read file as one byte buffer and write it directly into temp, what if the file is too large ?
                // so do it chunck by chunck to limit the buffer memory to bufferChunk value....
                // larger bufferChunk value make saving goes faster but need more memory.
                // however, bufferChunk = 1 MB for now.
                while (fileStream.Position <= fileStream.Length)
                {
                    long siz = fileStream.Length - fileStream.Position;
                    if (siz >= bufferChunk)
                    {
                        buff = new byte[bufferChunk];
                        fileStream.Read(buff, 0, bufferChunk);// ->|
                        tempStream.Write(buff, 0, bufferChunk);//<-|
                    }
                    else
                    {
                        buff = new byte[siz];
                        fileStream.Read(buff, 0, (int)(siz));// ->|
                        tempStream.Write(buff, 0, (int)(siz));//<-|
                        break;
                    }
                }

                // close streams
                fileStream.Close();
                fileStream.Dispose();
                tempStream.Close();
                tempStream.Dispose();
            }
            catch
            {
                // close streams
                fileStream.Close();
                fileStream.Dispose();
                tempStream.Close();
                tempStream.Dispose();
            }
        }
        private byte[] GetBuffer()
        {
            List<byte> buffer = new List<byte>();
            // 1 add NES
            ASCIIEncoding encoding = new ASCIIEncoding();
            buffer.AddRange(encoding.GetBytes("NES"));
            buffer.Add(0x1A);
            // 2 banks
            buffer.Add(PrgPages);
            buffer.Add(ChrPages);
            // 3 the flags
            byte flags = 0;
            // mirroring
            switch (this.Mirroring)
            {
                case Types.Mirroring.ModeHorz: break;//nothing
                case Types.Mirroring.ModeVert: flags = 1; break;
                case Types.Mirroring.ModeFull: flags = 0x8; break;
            }
            // has save ram, has trainer and mapper
            flags |= (byte)(HasSaveRam ? 0x2 : 0);
            flags |= (byte)(HasTrainer ? 0x4 : 0);
            flags = (byte)((flags & 0x0F) | ((Mapper & 0xF) << 4));
            buffer.Add(flags);
            // 4 the second flags byte
            flags = 0;
            flags |= (byte)(IsVSUnisystem ? 0x1 : 0);
            flags |= (byte)(IsPlaychoice10 ? 0x2 : 0);
            flags = (byte)((flags & 0x0F) | (Mapper & 0xF0));
            buffer.Add(flags);
            // 5 header byte # 8 is not used
            buffer.Add(0);
            // 6 TV system
            buffer.Add((byte)(TVSystem == INESRVSystem.PAL ? 1 : 0));
            // reach the 16 length
            for (int i = 0; i < 6; i++)
                buffer.Add(0);

            return buffer.ToArray();
        }
    }
    public enum INESRVSystem
    {
        NTSC, PAL, DualCompatible
    }
}