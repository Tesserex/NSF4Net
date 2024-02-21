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
using System.Reflection;
using MyNes.Core.Types;
namespace MyNes.Core.Boards
{
    public abstract class Board
    {
        protected byte[] chr;
        protected byte[] prg;
        protected byte[] trainer;
        protected int[] chrPage;
        protected int[] prgPage;
        protected byte[] sram = new byte[0x4000];
        protected bool isVram;
        protected int prgMask;
        protected int chrMask;
        private string name;
        private int mapperNumber;
        private bool isGameGenieActive;

        public Board()
        {
            //load information only
            LoadAttributes();
        }
        public Board(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
        {
            this.chr = chr;
            this.prg = prg;
            this.trainer = trainer;
            this.isVram = isVram;

            this.chrPage = new int[8];
            this.prgPage = new int[4];

            this.prgMask = prg.Length - 1;
            this.chrMask = chr.Length - 1;

            LoadAttributes();
        }

        private void LoadAttributes()
        {
            foreach (Attribute attr in Attribute.GetCustomAttributes(this.GetType()))
            {
                if (attr.GetType() == typeof(BoardName))
                {
                    this.name = ((BoardName)attr).Name;
                    this.mapperNumber = ((BoardName)attr).InesMapperNumber;
                    break;
                }
                else if (attr.GetType() == typeof(BoardSoundChip))
                {
                    this.SoundChip = ((BoardSoundChip)attr).SoundChip;
                    break;
                }
            }
        }

        protected virtual byte PeekChr(int address)
        {
            return chr[DecodeChrAddress(address) & chrMask];
        }
        protected virtual byte PeekPrg(int address)
        {
            return prg[DecodePrgAddress(address) & prgMask];
        }
        protected virtual void PokeChr(int address, byte data)
        {
            chr[DecodeChrAddress(address) & chrMask] = data;
        }
        protected virtual void PokePrg(int address, byte data) 
        {
            prg[DecodePrgAddress(address) & prgMask] = data;
        }

        protected virtual int DecodePrgAddress(int address)
        {
            if (address < 0xA000)
                return (address & 0x1FFF) | prgPage[0];
            else if (address < 0xC000)
                return (address & 0x1FFF) | prgPage[1];
            else if (address < 0xE000)
                return (address & 0x1FFF) | prgPage[2];
            else
                return (address & 0x1FFF) | prgPage[3];
        }
        protected virtual int DecodeChrAddress(int address)
        {
            if (address < 0x0400)
                return (address & 0x03FF) | chrPage[0];
            if (address < 0x0800)
                return (address & 0x03FF) | chrPage[1];
            if (address < 0x0C00)
                return (address & 0x03FF) | chrPage[2];
            if (address < 0x1000)
                return (address & 0x03FF) | chrPage[3];
            if (address < 0x1400)
                return (address & 0x03FF) | chrPage[4];
            if (address < 0x1800)
                return (address & 0x03FF) | chrPage[5];
            if (address < 0x1C00)
                return (address & 0x03FF) | chrPage[6];
            else
                return (address & 0x03FF) | chrPage[7];
        }

        public virtual void Initialize()
        {
            Nes.CpuMemory.Hook(0x8000, 0xFFFF, PeekPrg, PokePrg);
            Nes.CpuMemory.Hook(0x4020, 0x7FFF, PeekSram, PokeSram);
            //Nes.PpuMemory.Hook(0x0000, 0x1FFF, PeekChr, PokeChr);
            HardReset();
        }
        public virtual void HardReset()
        {
            sram = new byte[0x4000];
            Switch08kCHR(0);
            Switch32KPRG(0);
        }
        public virtual void SoftReset() { }

        #region SRAM
        /// <summary>
        /// Get the save-ram array that need to be saved
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetSaveRam()
        { return sram; }
        /// <summary>
        /// Set the sram from buffer
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void SetSram(byte[] buffer)
        {
            buffer.CopyTo(sram, 0);
        }
        protected virtual void PokeSram(int address, byte data)
        {
            sram[address - 0x4020] = data;
        }
        protected virtual byte PeekSram(int address)
        {
            return sram[address - 0x4020];
        }
        #endregion

        #region Switch Controls
        /// <summary>
        /// Switch 8k prg bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x8000, 0xA000, 0xC000 or 0xE000</param>
        protected void Switch08KPRG(int index, int where)
        {
            int bank = index << 13;
            switch (where)
            {
                case 0x8000: prgPage[0] = bank; break;
                case 0xA000: prgPage[1] = bank; break;
                case 0xC000: prgPage[2] = bank; break;
                case 0xE000: prgPage[3] = bank; break;
            }
        }
        /// <summary>
        /// Switch 16k prg bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x8000 or 0xC000</param>
        protected void Switch16KPRG(int index, int where)
        {
            int bank = index << 14;
            switch (where)
            {
                case 0x8000: prgPage[0] = bank; bank += 0x2000; prgPage[1] = bank; break;
                case 0xC000: prgPage[2] = bank; bank += 0x2000; prgPage[3] = bank; break;
            }
        }
        /// <summary>
        /// Switch 32k prg bank to 0x8000
        /// </summary>
        /// <param name="index">The index within cart</param>
        protected void Switch32KPRG(int index)
        {
            int bank = index << 15;
            for (int i = 0; i < 4; i++)
            {
                prgPage[i] = bank;
                bank += 0x2000;
            }
        }

        /// <summary>
        /// Switch 1k chr bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x0000, 0x0400, 0x0800, 0x0C00, 0x1000, 0x1400, 0x1800 or 0x1800</param>
        protected void Switch01kCHR(int index, int where)
        {
            chrPage[(where >> 10) & 0x07] = index << 10;
        }
        /// <summary>
        /// Switch 2k chr bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x0000, 0x800, 0x1000 or 0x1800</param>
        protected void Switch02kCHR(int index, int where)
        {
            int area = (where >> 10) & 0x07;
            int bank = index << 11;
            for (int i = 0; i < 2; i++)
            {
                chrPage[area] = bank;
                area++;
                bank += 0x400;
            }
        }
        /// <summary>
        /// Switch 4k chr bank to area
        /// </summary>
        /// <param name="index">The index within cart</param>
        /// <param name="where">The area where to switch. 0x0000, or 0x1000</param>
        protected void Switch04kCHR(int index, int where)
        {
            int area = (where >> 10) & 0x07;
            int bank = index << 12;
            for (int i = 0; i < 4; i++)
            {
                chrPage[area] = bank;
                area++;
                bank += 0x400;
            }
        }
        /// <summary>
        /// Switch 8k chr bank to 0x0000
        /// </summary>
        /// <param name="index">The index within cart</param>
        public void Switch08kCHR(int index)
        {
            int bank = index << 13;
            for (int i = 0; i < 8; i++)
            {
                chrPage[i] = bank;
                bank += 0x400;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the board name
        /// </summary>
        public virtual string Name
        { get { return name; } }
        /// <summary>
        /// Get the board name
        /// </summary>
        public virtual int INESMapperNumber
        { get { return mapperNumber; } }

        public virtual byte SoundChip { get; private set; }
        #endregion
    }
    public class BoardName : Attribute
    {
        public BoardName(string name, int inesMapperNumber)
        {
            this.name = name;
            this.inesMapperNumber = inesMapperNumber;
        }
        private string name;
        private int inesMapperNumber;

        public string Name { get { return name; } }
        public int InesMapperNumber { get { return inesMapperNumber; } }
    }

    public class BoardSoundChip : Attribute
    {
        public BoardSoundChip(byte soundChip)
        {
            this.SoundChip = soundChip;
        }

        public byte SoundChip { get; private init; }
    }
}