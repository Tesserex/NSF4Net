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
using MyNes.Core.APU;
namespace MyNes.Core.APU.Sunsoft5B
{
    class Sunsoft5BSoundChannel : Channel
    {
        public Sunsoft5BSoundChannel(TimingInfo.System system)
            : base(system) { }
        public bool Disable = false;
        private byte output = 0;
        private int dutyStep = 0;

        private void UpdateFrequency()
        {
            timing.single = (frequency + 1) * system.Cpu;
        }
        public new void PokeReg1(int address, byte data)
        {
            frequency = (frequency & 0x0F00) | data;
            UpdateFrequency();
        }
        public new void PokeReg2(int address, byte data)
        {
            frequency = (frequency & 0x00FF) | ((data & 0xF) << 8);
            UpdateFrequency();
        }
        public new void PokeReg3(int address, byte data)
        { EnvelopeVolume = (byte)(data & 0xF); }
        public override byte GetSample()
        {
            if (Disable)
                return 0;
            return output;
        }
        public override void Update()
        {
            dutyStep = (dutyStep + 1) & 0x1F;

            if (dutyStep <= 15)
                output = EnvelopeVolume;
            else
                output = 0;
        }
        public override void HardReset()
        {
            base.HardReset();
            Disable = false;
            output = 0;
            dutyStep = 0;
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(Disable); stream.Write(output); stream.Write(dutyStep);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            Disable = stream.ReadBoolean();
            output = stream.ReadByte();
            dutyStep = stream.ReadInt32();
        }
    }
}
