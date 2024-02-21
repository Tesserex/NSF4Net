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
using MyNes.Core.Types;
namespace MyNes.Core.Boards.Other__Mappers_
{
    [BoardName("Unknown", 227)]
    class Mapper227 : Board
    {
        public Mapper227() : base() { }
        public Mapper227(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool chrRAMprotect = false;

        public override void Initialize()
        {
            base.Initialize();
            chr = new byte[1024 * 8];
        }
        public override void HardReset()
        {
            base.HardReset(); 
            chrRAMprotect = false;
            Switch16KPRG(0, 0xC000);
        }
        protected override void PokeChr(int address, byte data)
        {
            if (!chrRAMprotect)
                base.PokeChr(address, data);
        }
        protected override void PokePrg(int address, byte data)
        {
            Nes.PpuMemory.SwitchMirroring((address & 0x2) == 0x2 ? Mirroring.ModeHorz : Mirroring.ModeVert);
            chrRAMprotect = (address & 0x80) == 0x80;
            /*
$8000-FFFF:  A~[.... ..LP  OPPP PPMS]
    L = Last PRG Page Mode
    P = PRG Reg
    O = Mode
    M = Mirroring (0=Vert, 1=Horz)
    S = PRG Size
             
                  $8000   $A000   $C000   $E000  
                +---------------+---------------+
O=1, S=0:       |       P       |       P       |
                +-------------------------------+
O=1, S=1:       |             < P >             |
                +-------------------------------+
O=0, S=0, L=0:  |       P       |   P AND $38   |
                +---------------+---------------+
O=0, S=1, L=0:  |   P AND $3E   |   P AND $38   |
                +---------------+---------------+
O=0, S=0, L=1:  |       P       |   P  OR $07   |
                +---------------+---------------+
O=0, S=1, L=1:  |   P AND $3E   |   P  OR $07   |
                +---------------+---------------+
             */
            //get prg bank
            int prgBank = (address >> 2) & 0x1F;
            //switch modes
            if ((address & 0x80) == 0x80)//o=1
            {
                if ((address & 0x1) == 0x1) //s=1
                {
                    Switch32KPRG(prgBank >> 1);
                }
                else//s=0
                {
                    Switch16KPRG(prgBank, 0x8000);
                    Switch16KPRG(prgBank, 0xC000);
                }
            }
            else//o=0
            {
                if ((address & 0x1) == 0x1) //s=1
                {
                    if ((address & 0x0200) == 0x0200) //L=1
                    {
                        Switch16KPRG(prgBank & 0x3E, 0x8000);
                        Switch16KPRG(prgBank | 0x7, 0xC000);
                    }
                    else//L=0
                    {
                        Switch16KPRG(prgBank & 0x3E, 0x8000);
                        Switch16KPRG(prgBank & 0x38, 0xC000);
                    }
                }
                else//s=0
                {
                    if ((address & 0x0200) == 0x0200) //L=1
                    {
                        Switch16KPRG(prgBank, 0x8000);
                        Switch16KPRG(prgBank | 0x7, 0xC000);
                    }
                    else//L=0
                    {
                        Switch16KPRG(prgBank, 0x8000);
                        Switch16KPRG(prgBank & 0x38, 0xC000);
                    }
                }
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(chrRAMprotect);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            chrRAMprotect = stream.ReadBoolean();
        }
    }
}
