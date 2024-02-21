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
using System.IO;
using System.Security.Cryptography;
using MyNes.Core.Database;

namespace MyNes.Core.ROM
{
    public class RomInfo
    {
        public RomInfo(string romPath)
        {
            this.romPath = romPath;
            FileStream stream = new FileStream(romPath, FileMode.Open, FileAccess.Read);
            stream.Position = 16;
            byte[] buffer = new byte[stream.Length - 16];

            stream.Read(buffer, 0, (int)(stream.Length - 16));
            stream.Close();

            //SHA1
            sha1 = "";
            SHA1Managed managedSHA1 = new SHA1Managed();
            byte[] shaBuffer = managedSHA1.ComputeHash(buffer);

            foreach (byte b in shaBuffer)
                sha1 += b.ToString("x2").ToLower();
            //DATABASE
            dataBaseInfo = NesDatabase.Find(sha1);
            //set cart info
            if (dataBaseInfo.Cartridges != null)
            {
                foreach (NesDatabaseCartridgeInfo cartinf in dataBaseInfo.Cartridges)
                    if (cartinf.SHA1.ToLower() == sha1.ToLower())
                    { dataBaseCartInfo = cartinf; break; }
            }
        }
        public RomInfo(string romPath, NesDatabaseObject database)
        {
            LoadFile(romPath, database);
        }
        private string format = "INES";
        private string romPath;
        private string sha1;
        private byte prgs;
        private byte chrs;
        private string board;
        private bool hasSaveRam;
        private bool vsunisystem = false;
        private bool pc10 = false;
        private MyNes.Core.Types.Mirroring mirroring;
        private NesDatabaseGameInfo dataBaseInfo;
        private NesDatabaseCartridgeInfo dataBaseCartInfo;

        public void LoadFile(string romPath, NesDatabaseObject database) 
        {
            this.romPath = romPath;
            FileStream stream = new FileStream(romPath, FileMode.Open, FileAccess.Read);
            stream.Position = 16;
            byte[] buffer = new byte[stream.Length - 16];

            stream.Read(buffer, 0, (int)(stream.Length - 16));
            stream.Close();

            //SHA1
            sha1 = "";
            SHA1Managed managedSHA1 = new SHA1Managed();
            byte[] shaBuffer = managedSHA1.ComputeHash(buffer);

            foreach (byte b in shaBuffer)
                sha1 += b.ToString("x2").ToLower();
            //DATABASE
            dataBaseInfo = database.Find(sha1);
            //set cart info
            if (dataBaseInfo.Cartridges != null)
            {
                foreach (NesDatabaseCartridgeInfo cartinf in dataBaseInfo.Cartridges)
                    if (cartinf.SHA1.ToLower() == sha1.ToLower())
                    { dataBaseCartInfo = cartinf; break; }
            }
        }
        /// <summary>
        /// Get the rom path
        /// </summary>
        public string Path
        { get { return romPath; } }
        /// <summary>
        /// Get the rom's SHA1
        /// </summary>
        public string SHA1
        { get { return sha1; } }
        /// <summary>
        /// Get NesDatabaseGameInfo element
        /// </summary>
        public NesDatabaseGameInfo DatabaseGameInfo
        { get { return dataBaseInfo; } }
        /// <summary>
        /// Get NesDatabaseCartridgeInfo element
        /// </summary>
        public NesDatabaseCartridgeInfo DatabaseCartInfo
        { get { return dataBaseCartInfo; } }
        /// <summary>
        /// Get or set the prgs count, must be set manually and can't be loaded using this class
        /// </summary>
        public byte PRGcount
        { get { return prgs; } set { prgs = value; } }
        /// <summary>
        /// Get or set the chr count, must be set manually and can't be loaded using this class
        /// </summary>
        public byte CHRcount
        { get { return chrs; } set { chrs = value; } }
        /// <summary>
        /// Get or set the Mirroring, must be set manually and can't be loaded using this class
        /// </summary>
        public  MyNes.Core.Types.Mirroring Mirroring
        { get { return this.mirroring; } set { this.mirroring = value; } }
        /// <summary>
        /// Get or set the Mapper/Board, must be set manually and can't be loaded using this class
        /// </summary>
        public string MapperBoard
        { get { return board; } set { board = value; } }
        /// <summary>
        /// Get or set if this rom has sram
        /// </summary>
        public bool HasSaveRam
        { get { return hasSaveRam; } set { hasSaveRam = value; } }
        /// <summary>
        /// Get or set if this rom is for VSUisystem
        /// </summary>
        public bool VSUnisystem
        { get { return vsunisystem; } set { vsunisystem = value; } }
        /// <summary>
        /// Get or set if this rom is for PC10
        /// </summary>
        public bool PC10
        { get { return pc10; } set { pc10 = value; } }
        /// <summary>
        /// Get or set the current rom file format
        /// </summary>
        public string Format
        { get { return format; } set { format = value; } }
    }
}
