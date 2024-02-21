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
    [BoardName("Irem G-101", 32)]
    class IremG101 : Board
    {
        public IremG101() : base() { }
        public IremG101(byte[] chr, byte[] prg, byte[] trainer, bool isVram) : base(chr, prg, trainer, isVram) { }

        protected bool prgmode = false;
        protected byte[] prgRegs = new byte[2];
        private bool enableMirroringSwitch = true;

        public override void Initialize()
        {
            base.Initialize();
            //Major League special case, wants hardwired 1-screen mirroring
            if (Nes.RomInfo.SHA1.ToUpper() == "7E4180432726A433C46BA2206D9E13B32761C11E")
            {
                enableMirroringSwitch = false;
                Nes.PpuMemory.SwitchMirroring(Types.Mirroring.Mode1ScA);
            }
            else
                enableMirroringSwitch = true;
        }
        public override void HardReset()
        {
            // Switch 32KB prg bank at 0x8000
            // Switch 08KB chr bank at 0x0000
            base.HardReset();

            Switch16KPRG((prg.Length - 0x4000) >> 14, 0xC000);
        }
        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xF007)
            {
                case 0x8000:
                case 0x8001:
                case 0x8002:
                case 0x8003:
                case 0x8004:
                case 0x8005:
                case 0x8006:
                case 0x8007: prgRegs[0] = data; SetupPRG(); break;

                case 0x9000:
                case 0x9001:
                case 0x9002:
                case 0x9003:
                case 0x9004:
                case 0x9005:
                case 0x9006:
                case 0x9007:
                    prgmode = (data & 0x2) == 0x2;
                    if (enableMirroringSwitch) Nes.PpuMemory.SwitchMirroring(((data & 0x1) == 0) ? Types.Mirroring.ModeVert : Types.Mirroring.ModeHorz);
                    break;

                case 0xA000:
                case 0xA001:
                case 0xA002:
                case 0xA003:
                case 0xA004:
                case 0xA005:
                case 0xA006:
                case 0xA007: prgRegs[1] = data; SetupPRG(); break;

                case 0xB000: Switch01kCHR(data, 0x0000); break;
                case 0xB001: Switch01kCHR(data, 0x0400); break;
                case 0xB002: Switch01kCHR(data, 0x0800); break;
                case 0xB003: Switch01kCHR(data, 0x0C00); break;
                case 0xB004: Switch01kCHR(data, 0x1000); break;
                case 0xB005: Switch01kCHR(data, 0x1400); break;
                case 0xB006: Switch01kCHR(data, 0x1800); break;
                case 0xB007: Switch01kCHR(data, 0x1C00); break;
            }
        }
        protected void SetupPRG()
        {
            if (!prgmode)
            {
                base.Switch08KPRG(prgRegs[0], 0x8000);
                base.Switch08KPRG(prgRegs[1], 0xA000);
                base.Switch08KPRG((prg.Length - 0x4000) >> 13, 0xC000);
                base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            }
            else
            {
                base.Switch08KPRG((prg.Length - 0x4000) >> 13, 0x8000);
                base.Switch08KPRG(prgRegs[1], 0xA000);
                base.Switch08KPRG(prgRegs[0], 0xC000);
                base.Switch08KPRG((prg.Length - 0x2000) >> 13, 0xE000);
            }
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(prgmode, enableMirroringSwitch);
            stream.Write(prgRegs);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            bool[] flags = stream.ReadBooleans();
            prgmode = flags[0];
            enableMirroringSwitch = flags[1];
            stream.Read(prgRegs);
        }
    }
}
