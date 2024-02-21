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
using MyNes.Core.Debug;
namespace MyNes.Core.Boards.Nintendo
{
    [BoardName("MMC3", 4)]
    class MMC3 : Board
    {
        public MMC3()
            : base()
        { }
        public MMC3(byte[] chr, byte[] prg, byte[] trainer, bool isVram)
            : base(chr, prg, trainer, isVram)
        { }
        protected bool chrmode = false;
        protected bool prgmode = false;
        protected int addrSelect = 0;
        protected byte[] chrRegs = new byte[6];
        protected byte[] prgRegs = new byte[4];
        protected bool wramON = true;
        protected bool wramReadOnly = false;

        protected byte irqReload = 0xFF;
        protected byte irqCounter = 0;
        protected bool IrqEnable = false;
        public bool mmc3_alt_behavior = false;//true=rev A, false= rev B
        protected bool clear = false;
        protected int oldA12;
        protected int newA12;
        protected int timer;

        public override void Initialize()
        {
            base.Initialize();

            Nes.Ppu.AddressLineUpdating = this.PPU_AddressLineUpdating;
            Nes.Ppu.CycleTimer = this.TickPPU;
        }
        public override void HardReset()
        {
            base.HardReset();
            chrmode = false;
            prgmode = false;
            addrSelect = 0;
            chrRegs = new byte[6];
            prgRegs = new byte[4];
            for (int i = 0; i < 6; i++)
                chrRegs[i] = 0;

            prgRegs[0] = 0;
            prgRegs[1] = 1;
            prgRegs[2] = (byte)((prg.Length - 0x4000) >> 13);
            prgRegs[3] = (byte)((prg.Length - 0x2000) >> 13);
            SetupPRG();
            sram = new byte[0x2000];
            wramON = true;
            wramReadOnly = false;

            irqReload = 0xFF;
            irqCounter = 0;
            IrqEnable = false;
            clear = false;
            oldA12 = 0;
            newA12 = 0;
            timer = 0;

            if (Nes.RomInfo.DatabaseGameInfo.Game_Name != null)
            {
                switch (Nes.RomInfo.DatabaseGameInfo.chip_type[0].ToLower())
                {
                    case "mmc3a": mmc3_alt_behavior = true; Console.WriteLine("Chip= MMC3 A, MMC3 IQR mode switched to RevA"); break;
                    case "mmc3b": mmc3_alt_behavior = false; Console.WriteLine("Chip= MMC3 B, MMC3 IQR mode switched to RevB"); break;
                    case "mmc3c": mmc3_alt_behavior = false; Console.WriteLine("Chip= MMC3 C, MMC3 IQR mode switched to RevB"); break;
                }
            }
        }

        protected override void PokePrg(int address, byte data)
        {
            switch (address & 0xE001)
            {
                case 0x8000: Poke8000(address, data); break;
                case 0x8001: Poke8001(address, data); break;
                case 0xA000: PokeA000(address, data); break;
                case 0xA001: PokeA001(address, data); break;
                case 0xC000: PokeC000(address, data); break;
                case 0xC001: PokeC001(address, data); break;
                case 0xE000: PokeE000(address, data); break;
                case 0xE001: PokeE001(address, data); break;
            }
        }
        protected virtual void Poke8000(int address, byte data)
        {
            chrmode = (data & 0x80) == 0x80;
            prgmode = (data & 0x40) == 0x40;
            addrSelect = data & 0x7;
            SetupPRG();
            SetupCHR();
        }
        protected virtual void Poke8001(int address, byte data)
        {
            switch (addrSelect)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5: chrRegs[addrSelect] = data; SetupCHR(); break;
                case 6: prgRegs[0] = (byte)(data & 0x3F); SetupPRG(); break;
                case 7: prgRegs[1] = (byte)(data & 0x3F); SetupPRG(); break;
            }
        }
        protected virtual void PokeA000(int address, byte data)
        {
            if (Nes.RomInfo.Mirroring != Mirroring.ModeFull)
                Nes.PpuMemory.SwitchMirroring(((data & 1) == 0) ? Mirroring.ModeVert : Mirroring.ModeHorz);
        }
        protected virtual void PokeA001(int address, byte data)
        {
            wramON = (data & 0x80) == 0x80; wramReadOnly = (data & 0x40) == 0x40;
        }
        protected virtual void PokeC000(int address, byte data)
        {
            irqReload = data;
        }
        protected virtual void PokeC001(int address, byte data)
        {
            if (mmc3_alt_behavior) clear = true; irqCounter = 0;
        }
        protected virtual void PokeE000(int address, byte data)
        {
            IrqEnable = false; Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, false);
        }
        protected virtual void PokeE001(int address, byte data)
        {
            IrqEnable = true;
        }

        protected virtual void SetupPRG()
        {
            if (!prgmode)
            {
                base.Switch08KPRG(prgRegs[0], 0x8000);
                base.Switch08KPRG(prgRegs[1], 0xA000);
                base.Switch08KPRG(prgRegs[2], 0xC000);
                base.Switch08KPRG(prgRegs[3], 0xE000);
            }
            else
            {
                base.Switch08KPRG(prgRegs[2], 0x8000);
                base.Switch08KPRG(prgRegs[1], 0xA000);
                base.Switch08KPRG(prgRegs[0], 0xC000);
                base.Switch08KPRG(prgRegs[3], 0xE000);
            }
        }
        protected virtual void SetupCHR()
        {
            if (!chrmode)
            {
                base.Switch02kCHR(chrRegs[0] >> 1, 0x0000);
                base.Switch02kCHR(chrRegs[1] >> 1, 0x0800);
                base.Switch01kCHR(chrRegs[2], 0x1000);
                base.Switch01kCHR(chrRegs[3], 0x1400);
                base.Switch01kCHR(chrRegs[4], 0x1800);
                base.Switch01kCHR(chrRegs[5], 0x1C00);
            }
            else
            {
                base.Switch02kCHR(chrRegs[0] >> 1, 0x1000);
                base.Switch02kCHR(chrRegs[1] >> 1, 0x1800);
                base.Switch01kCHR(chrRegs[2], 0x0000);
                base.Switch01kCHR(chrRegs[3], 0x0400);
                base.Switch01kCHR(chrRegs[4], 0x0800);
                base.Switch01kCHR(chrRegs[5], 0x0C00);
            }
        }

        protected override void PokeSram(int address, byte data)
        {
            if (wramON && !wramReadOnly)
                base.PokeSram(address, data);
        }
        protected override byte PeekSram(int address)
        {
            if (wramON)
                return base.PeekSram(address);
            return 0;
        }

        private void TickPPU()
        {
            timer++;
        }
        private void PPU_AddressLineUpdating(int addr)
        {
            oldA12 = newA12;
            newA12 = addr & 0x1000;

            if (oldA12 < newA12)
            {
                if (timer > 8)
                {
                    int old = irqCounter;

                    if (irqCounter == 0 || clear)
                        irqCounter = irqReload;
                    else
                        irqCounter = (byte)(irqCounter - 1);

                    if ((!mmc3_alt_behavior || old != 0 || clear) && irqCounter == 0 && IrqEnable)
                        Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Brd, true);

                    clear = false;
                }

                timer = 0;
            }
        }

        public override void SaveState(StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(addrSelect);
            stream.Write(chrRegs);
            stream.Write(prgRegs);
            stream.Write(sram);
            stream.Write(irqReload);
            stream.Write(irqCounter);
            stream.Write(oldA12);
            stream.Write(newA12);
            stream.Write(timer);
            stream.Write(chrmode, prgmode, wramON, wramReadOnly, IrqEnable, clear);
        }
        public override void LoadState(StateStream stream)
        {
            base.LoadState(stream);
            addrSelect = stream.ReadInt32();
            stream.Read(chrRegs);
            stream.Read(prgRegs);
            stream.Read(sram);
            irqReload = stream.ReadByte();
            irqCounter = stream.ReadByte();
            oldA12 = stream.ReadInt32();
            newA12 = stream.ReadInt32();
            timer = stream.ReadInt32();
            bool[] flags = stream.ReadBooleans();
            chrmode = flags[0];
            prgmode = flags[1];
            wramON = flags[2];
            wramReadOnly = flags[3];
            IrqEnable = flags[4];
            clear = flags[5];
        }
    }
    class MMC3ConsoleCommands : ConsoleCommand
    {
        public override string Method
        {
            get { return "mmc3"; }
        }
        public override string Description
        {
            get { return "Call MMC3 board commands, use parameters for options"; }
        }
        public override ConsoleCommandParameter[] Parameters
        {
            get
            {
                return new ConsoleCommandParameter[]
                {
                new ConsoleCommandParameter("reva","Set MMC3 IQR mode to RevA"),
                new ConsoleCommandParameter("revb","Set MMC3 IQR mode to RevB"),
                };
            }
        }
        public override void Execute(string parameters)
        {
            if (!Nes.ON)
            {
                Console.WriteLine("The emulation is off, you can't access the board.", DebugCode.Error);
                return;
            }
            if (!Nes.Board.Name.Contains("MMC3"))
            {
                Console.WriteLine("The current loaded board is not MMC3 !", DebugCode.Error);
                return;
            }
            if (parameters.Length > 0)
            {
                string[] codes = parameters.Split(new char[] { ' ' });
                for (int i = 0; i < codes.Length; i++)
                {
                    if (codes[i] == "reva")
                    {
                        ((MMC3)Nes.Board).mmc3_alt_behavior = true;
                        Console.WriteLine("MMC3 IQR mode switched to RevA");
                    }
                    if (codes[i] == "revb")
                    {
                        ((MMC3)Nes.Board).mmc3_alt_behavior = false;
                        Console.WriteLine("MMC3 IQR mode switched to RevB");
                    }
                }
            }
        }
    }
}
