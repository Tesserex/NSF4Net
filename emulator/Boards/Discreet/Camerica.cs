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
    [BoardName("Camerica", 71)]
    class Camerica : Board
    {
        public Camerica()
            : base()
        { }
        public Camerica(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        public override void Initialize()
        {
            Nes.CpuMemory.Hook(0x4018, 0x7FFF, PokePrg);
            base.Initialize();
        }
        public override void HardReset()
        {
            base.HardReset();
            base.Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            if ((address & 0xE000) == 0x6000)
                base.Switch16KPRG((data & 0x0F), 0x8000);
            else
                switch (address & 0xF000)
                {
                    case 0xF000:
                    case 0xE000:
                    case 0xD000:
                    case 0xC000: base.Switch16KPRG((data & 0x0F), 0x8000); break;
                    case 0x9000:
                        if ((data & 0x10) != 0)
                            Nes.PpuMemory.SwitchMirroring(MyNes.Core.Types.Mirroring.Mode1ScB);
                        else
                            Nes.PpuMemory.SwitchMirroring(MyNes.Core.Types.Mirroring.Mode1ScA);
                        break;
                }

        }
    }
}
