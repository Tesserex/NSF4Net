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
    public class Channel : ProcessorBase
    {
        protected int frequency;

        protected static byte[] DurationTable = 
        {
            0x0A, 0xFE, 0x14, 0x02, 0x28, 0x04, 0x50, 0x06, 0xA0, 0x08, 0x3C, 0x0A, 0x0E, 0x0C, 0x1A, 0x0E,
            0x0C, 0x10, 0x18, 0x12, 0x30, 0x14, 0x60, 0x16, 0xC0, 0x18, 0x48, 0x1A, 0x10, 0x1C, 0x20, 0x1E,
        };

        protected bool DurationHalt = false;
        protected bool DurationHaltRequset = false;
        protected byte DurationCounter;
        protected bool DurationReloadEnabled;
        protected byte DurationReload = 0;
        protected bool DurationReloadRequst = false;

        protected bool EnvelopeLooping;
        protected bool EnvelopeEnabled;
        protected byte EnvelopeVolume;
        protected byte EnvelopeCount;
        protected byte EnvelopeDelay;
        protected bool EnvelopeRefresh;
        protected byte EnvelopeTimer;
        protected byte EnvelopeSound
        {
            get
            {
                if (EnvelopeEnabled)
                {
                    return EnvelopeVolume;
                }
                else
                {
                    return EnvelopeCount;
                }
            }
            set
            {
                EnvelopeDelay = value;
                if (EnvelopeEnabled)
                    EnvelopeVolume = EnvelopeDelay;
                else
                    EnvelopeVolume = EnvelopeCount;
            }
        }

        public virtual bool Status
        {
            get { return DurationCounter > 0; }
            set
            {
                DurationReloadEnabled = value;
                if (!DurationReloadEnabled)
                    DurationCounter = 0;
            }
        }

        public Channel(TimingInfo.System system)
            : base(system)
        {
            timing.period = system.Master;
            timing.single = GetCycles(frequency + 1);
        }

        protected int GetCycles(int cycles)
        {
            return cycles * system.Spu;
        }

        protected virtual void PokeReg1(int address, byte data)
        {
            DurationHaltRequset = (data & 0x20) != 0;
            EnvelopeLooping = (data & 0x20) != 0;
            EnvelopeEnabled = (data & 0x10) != 0;
            EnvelopeSound = (byte)(data & 0x0F);
        }
        protected virtual void PokeReg2(int address, byte data) { }
        protected virtual void PokeReg3(int address, byte data) { }
        protected virtual void PokeReg4(int address, byte data)
        {
            DurationReload = DurationTable[data >> 3];
            DurationReloadRequst = true;
            EnvelopeRefresh = true;
        }

        public virtual void Hook(int address)
        {
            Nes.CpuMemory.Hook(address++, PokeReg1);
            Nes.CpuMemory.Hook(address++, PokeReg2);
            Nes.CpuMemory.Hook(address++, PokeReg3);
            Nes.CpuMemory.Hook(address++, PokeReg4);
        }
        public virtual void Hook(int address1, int address2, int address3, int address4)
        {
            Nes.CpuMemory.Hook(address1, PokeReg1);
            Nes.CpuMemory.Hook(address2, PokeReg2);
            Nes.CpuMemory.Hook(address3, PokeReg3);
            Nes.CpuMemory.Hook(address4, PokeReg4);
        }
        public virtual void ClockSingle(bool isClockingLength)
        {
            DurationHalt = DurationHaltRequset;
            if (isClockingLength && DurationCounter > 0)
                DurationReloadRequst = false;
            if (DurationReloadRequst)
            {
                if (DurationReloadEnabled)
                    DurationCounter = DurationReload;
                DurationReloadRequst = false;
            }
        }
        public virtual void ClockDuration()
        {
            if (!DurationHalt)
            {
                if (DurationCounter > 0)
                {
                    DurationCounter--;
                }
            }
        }
        public virtual void ClockEnvelope()
        {
            if (EnvelopeRefresh)
            {
                EnvelopeRefresh = false;
                EnvelopeTimer = EnvelopeDelay;
                EnvelopeCount = 0x0F;
            }
            else
            {
                if (EnvelopeTimer != 0)
                {
                    EnvelopeTimer--;
                }
                else
                {
                    EnvelopeTimer = EnvelopeDelay;

                    if (EnvelopeLooping || EnvelopeCount != 0)
                        EnvelopeCount = (byte)((EnvelopeCount - 1) & 0x0F);
                }
            }
        }
        public virtual byte GetSample() { return 0; }
        public override void Initialize()
        {
            HardReset();
            base.Initialize();
        }

        public override void SoftReset()
        {
            DurationReloadEnabled = false;
            DurationCounter = 0;
        }
        public override void HardReset()
        {
            DurationHalt = false;
            DurationHaltRequset = false;
            DurationCounter = 0;
            DurationReloadEnabled = false;
            DurationReload = 0;
            DurationReloadRequst = false;

            EnvelopeLooping = false;
            EnvelopeEnabled = false;
            EnvelopeVolume = 0;
            EnvelopeCount = 0;
            EnvelopeDelay = 0;
            EnvelopeRefresh = false;
            EnvelopeTimer = 0;
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(DurationHalt, DurationHaltRequset, DurationReloadEnabled, DurationReloadRequst
                , EnvelopeLooping, EnvelopeEnabled, EnvelopeRefresh);
            stream.Write(frequency);
            stream.Write(DurationCounter);
            stream.Write(DurationReload);
            stream.Write(EnvelopeVolume);
            stream.Write(EnvelopeCount);
            stream.Write(EnvelopeDelay);
            stream.Write(EnvelopeTimer);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            bool[] flags = stream.ReadBooleans();
            DurationHalt = flags[0];
            DurationHaltRequset = flags[1];
            DurationReloadEnabled = flags[2];
            DurationReloadRequst = flags[3];
            EnvelopeLooping = flags[4];
            EnvelopeEnabled = flags[5];
            EnvelopeRefresh = flags[6];
            frequency = stream.ReadInt32();
            DurationCounter = stream.ReadByte();
            DurationReload = stream.ReadByte();
            EnvelopeVolume = stream.ReadByte();
            EnvelopeCount = stream.ReadByte();
            EnvelopeDelay = stream.ReadByte();
            EnvelopeTimer = stream.ReadByte();
        }
    }
}
