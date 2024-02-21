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
namespace MyNes.Core.APU.VRC6
{
    class VRC6sawtoothSoundChannel : Channel
    {
        public VRC6sawtoothSoundChannel(TimingInfo.System system)
            : base(system) { }

        private bool enabled = true;
        private int accum = 0;
        private int accumRate = 0;
        private int accumStep = 0;
        private byte output = 0;
        public override void HardReset()
        {
            base.HardReset(); 
            enabled = true;
            accum = 0;
            accumRate = 0;
            accumStep = 0;
            output = 0;
        }
        private void UpdateFrequency()
        {
            timing.single = system.Cpu * (frequency + 1);
        }
        protected override void PokeReg1(int address, byte data)
        {
            accumRate = data & 0x3F;
        }
        protected override void PokeReg2(int address, byte data)
        {
            frequency = (frequency & 0x0F00) | data;
            UpdateFrequency();
        }
        protected override void PokeReg3(int address, byte data)
        {
            frequency = (frequency & 0x00FF) | ((data & 0xF) << 8);
            UpdateFrequency();
            enabled = (data & 0x80) == 0x80;
        }
        public override byte GetSample()
        {
            if (enabled & frequency > 0x4)
                return output;
            return 0;
        }
        public override void Update()
        {
            accumStep++;
            switch (++accumStep)
            {
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                case 12:
                    accum += accumRate;
                    break;

                case 14:
                    accum = 0;
                    accumStep = 0;
                    break;
            }

            output = (byte)((accum >> 3) & 0x1F);
        }
        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(enabled);
            stream.Write(accum);
            stream.Write(accumRate);
            stream.Write(accumStep);
            stream.Write(output);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            enabled = stream.ReadBoolean() ;
            accum = stream.ReadInt32();
            accumRate = stream.ReadInt32();
            accumStep = stream.ReadInt32();
            output = stream.ReadByte();
        }
    }
}
