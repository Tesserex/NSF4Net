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
/*Written by Ala Ibrahim Hadid*/
namespace MyNes.Core.Boards.Discreet
{
    [BoardName("BxROM/NINA-001", 34)]
    class BxROM : Board
    {
        /*Don't know how to chose which board version should use, BxROM or NINA-001*/
        public BxROM() : base() { }
        public BxROM(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool IsNINA = false;

        public override void Initialize()
        {
            base.Initialize();
            Nes.CpuMemory.Hook(0x7FFD, PokePrg);
            Nes.CpuMemory.Hook(0x7FFE, PokePrg);
            Nes.CpuMemory.Hook(0x7FFF, PokePrg);
            //let's see if this game is NINA 101 or BxROM
            //The only way to do this is to use sha1
            /* Note from 034.txt, mapper docs
             * "How these two seperate and completely imcompatible mappers got assigned the same mapper number is a mystery.
             * BxROM and NINA-001 are both assigned mapper 034, however they both work totally differently.  There is no
             * reliable way to tell the difference between the two apart from a CRC or Hash check."
             */
            if (Nes.RomInfo.SHA1.ToUpper() == "68315AFB344108CB0D43E119BA0353D5A44BD489")
            {
                //Impossible Mission 2   (NINA-001)
                IsNINA = true;
            }
            else
            {
                //BxROM
                IsNINA = false;
            }
        }
        protected override void PokePrg(int address, byte data)
        {
            if (IsNINA)//NINA-001
            {
                switch (address)
                {
                    case 0x7FFD: Switch32KPRG(data); break;
                    case 0x7FFE: Switch04kCHR(data, 0x0000); break;
                    case 0x7FFF: Switch04kCHR(data, 0x1000); break;
                }
            }
            else//BxROM
            {
                Switch32KPRG(data);
            }
        }
    }
}
