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
namespace MyNes.Core.APU
{
    public class ChannelDmc : Channel
    {
        public ChannelDmc(TimingInfo.System system)
            : base(system)
        {
        }
        private static readonly int[][] FrequencyTable = 
        { 
            new int[]//NTSC
            { 
              0xD6, 0xBE, 0xAA, 0xA0, 0x8F, 0x7F, 0x71, 0x6B,
              0x5F, 0x50, 0x47, 0x40, 0x35, 0x2A, 0x24, 0x1B
            },
            new int[]//PAL
            { 
              0xC7, 0xB1, 0x9E, 0x95, 0x8A, 0x76, 0x69, 0x63,
              0x58, 0x4A, 0x42, 0x3B, 0x31, 0x27, 0x21, 0x19,
            },  
            new int[]//DENDY (same as ntsc for now)
            { 
              0xD6, 0xBE, 0xAA, 0xA0, 0x8F, 0x7F, 0x71, 0x6B,
              0x5F, 0x50, 0x47, 0x40, 0x35, 0x2A, 0x24, 0x1B
            },
        };
        private const int dmaCycles = 4;
        public bool DeltaIrqOccur;
        private bool IrqEnabled;
        private bool dmaLooping;
        private bool dmaEnabled;
        private bool bufferFull = false;
        private byte output;
        private int dmaAddrRefresh;
        private int dmaSizeRefresh;
        private int dmaSize;
        private int dmaBits = 0;
        private byte dmaByte = 0;
        private int dmaAddr = 0;
        private byte dmaBuffer = 0;

        protected override void PokeReg1(int address, byte data)
        {
            IrqEnabled = (data & 0x80) != 0;
            dmaLooping = (data & 0x40) != 0;

            if (!IrqEnabled)
            {
                DeltaIrqOccur = false;
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Dmc, false);
            }

            timing.single = GetCycles(FrequencyTable[system.Serial][data & 0x0F]);
        }
        protected override void PokeReg2(int address, byte data)
        {
            output = (byte)(data & 0x7F);
        }
        protected override void PokeReg3(int address, byte data)
        {
            dmaAddrRefresh = (data << 6) | 0xC000;
        }
        protected override void PokeReg4(int address, byte data)
        {
            dmaSizeRefresh = (data << 4) | 0x0001;
        }

        public void DoFetch()
        {
            bufferFull = true;
            dmaBuffer = Nes.CpuMemory[dmaAddr];

            if (++dmaAddr == 0x10000)
                dmaAddr = 0x8000;
            dmaSize--;

            if (dmaSize == 0)
            {
                if (dmaLooping)
                {
                    dmaAddr = dmaAddrRefresh;
                    dmaSize = dmaSizeRefresh;
                }
                else if (IrqEnabled)
                {
                    Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Dmc, true);
                    DeltaIrqOccur = true;
                }
            }
        }
        public override byte GetSample()
        {
            return output;
        }
        public override bool Status
        {
            get
            {
                return dmaSize > 0;
            }
            set
            {
                if (value)
                {
                    if (dmaSize == 0)
                    {
                        dmaSize = dmaSizeRefresh;
                        dmaAddr = dmaAddrRefresh;

                        if (!bufferFull)
                        {
                            Nes.Cpu.DMCdmaCycles = dmaCycles; 
                            //Nes.Cpu.dma.DmcFetch(dmaAddr);
                        }
                    }
                }
                else
                {
                    dmaSize = 0;
                }
                DeltaIrqOccur = false;
                Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Dmc, false);
            }
        }

        public override void Update()
        {
            if (dmaEnabled)
            {
                if ((dmaByte & 0x01) != 0)
                {
                    if (output <= 0x7D)
                        output += 2;
                }
                else
                {
                    if (output >= 0x02)
                        output -= 2;
                }
                dmaByte >>= 1;
            }
            dmaBits++;
            if (dmaBits == 8)
            {
                dmaBits = 0;
                if (bufferFull)
                {
                    bufferFull = false;
                    dmaEnabled = true;
                    dmaByte = dmaBuffer;
                    if (dmaSize > 0)
                    {
                        Nes.Cpu.DMCdmaCycles = dmaCycles;
                        //Nes.Cpu.dma.DmcFetch(dmaAddr);
                    }
                }
                else
                {
                    dmaEnabled = false;
                }
            }
        }
        public override void Initialize()
        {
            HardReset();
            base.Initialize();
        }
        public override void SoftReset()
        {
            DeltaIrqOccur = false;
            Nes.Cpu.Interrupt(CPU.Cpu.IsrType.Dmc, false);
            base.SoftReset();
        }
        public override void HardReset()
        {
            DeltaIrqOccur = false;
            IrqEnabled = false;
            dmaLooping = false;
            dmaEnabled = false;
            bufferFull = false;
            output = 0;
            dmaAddrRefresh = 0;
            dmaSizeRefresh = 0;
            dmaSize = 0;
            dmaBits = 0;
            dmaByte = 0;
            dmaAddr = 0;
            dmaBuffer = 0;
            base.HardReset();
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(DeltaIrqOccur, IrqEnabled, dmaLooping, dmaEnabled, bufferFull);
            stream.Write(output);
            stream.Write(dmaAddrRefresh);
            stream.Write(dmaSizeRefresh);
            stream.Write(dmaSize);
            stream.Write(dmaBits);
            stream.Write(dmaByte);
            stream.Write(dmaAddr);
            stream.Write(dmaBuffer);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            bool[] flags = stream.ReadBooleans();
            DeltaIrqOccur = flags[0];
            IrqEnabled = flags[1];
            dmaLooping = flags[2];
            dmaEnabled = flags[3];
            bufferFull = flags[4];

            output = stream.ReadByte();
            dmaAddrRefresh = stream.ReadInt32();
            dmaSizeRefresh = stream.ReadInt32();
            dmaSize = stream.ReadInt32();
            dmaBits = stream.ReadInt32();
            dmaByte = stream.ReadByte();
            dmaAddr = stream.ReadInt32();
            dmaBuffer = stream.ReadByte();
        }
    }
}