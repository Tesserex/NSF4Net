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
    [BoardName("Caltron 6-in-1", 41)]
    class Caltron6in1 : Board
    {
        public Caltron6in1() : base() { }
        public Caltron6in1(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        private bool enableReg = false;
        private int vromReg = 0;

        public override void Initialize()
        {
            // Maps prg writes to 0x8000 - 0xFFFF. Maps sram reads and writes to 0x6000 - 0x8000.
            // Then do a hard reset.
            base.Initialize();
            Nes.CpuMemory.Hook(0x6000, 0x67FF, PokePrg);
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            enableReg = true;
        }
        protected override void PokePrg(int address, byte data)
        {
            if (address < 0x8000)
            {
                Switch32KPRG(address & 0x7);
                enableReg = (address & 0x4) == 0x4;

                vromReg = (vromReg & 0x03) | ((address >> 1) & 0x0C);
                Switch08kCHR(vromReg);
                Nes.PpuMemory.SwitchMirroring((address & 0x20) == 0x20 ? Types.Mirroring.ModeHorz : Types.Mirroring.ModeVert);
            }
            else
            {
                if (enableReg)
                {
                    vromReg = (vromReg & 0x0C) | (data & 0x3);
                    Switch08kCHR(vromReg);
                }
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(enableReg); 
            stream.Write(vromReg);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            enableReg = stream.ReadBoolean();
            vromReg = stream.ReadInt32();
        }
    }
}
