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
    class VRC6pulseSoundChannel : Channel
    {
        public VRC6pulseSoundChannel(TimingInfo.System system)
            : base(system) { }

        private int dutyForm;
        private int dutyStep;
        private bool enabled = true;
        private bool mode = false;
        private byte output;

        public override void HardReset()
        {
            base.HardReset();
            dutyForm = 0;
            dutyStep = 0xF;
            enabled = true;
            mode = false;
            output = 0;
        }
        private void UpdateFrequency()
        {
            timing.single = GetCycles((frequency + 1));
        }
        protected override void PokeReg1(int address, byte data)
        {
            mode = (data & 0x80) == 0x80;
            dutyForm = ((data & 0x70) >> 4) + 1;
            EnvelopeVolume = (byte)(data & 0xF);
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
            //if (!enabled)
            //    dutyStep = 0;
        }
        public override void Update()
        {
            if (enabled)
            {
                if (mode)
                    output = EnvelopeVolume;
                else
                {
                    dutyStep = (dutyStep + 1) & 0xF;

                    if (dutyStep >= dutyForm)
                        output = EnvelopeVolume;
                    else
                        output = 0;
                }
            }
            else
                output = 0;
        }
        public override byte GetSample()
        {
            if (frequency > 0x4)
                return output;
            return 0;
        }

        public override void SaveState(Types.StateStream stream)
        {
            base.SaveState(stream);
            stream.Write(dutyForm);
            stream.Write(dutyStep);
            stream.Write(enabled);
            stream.Write(mode);
            stream.Write(output);
        }
        public override void LoadState(Types.StateStream stream)
        {
            base.LoadState(stream);
            dutyForm = stream.ReadInt32();
            dutyStep = stream.ReadInt32();
            enabled = stream.ReadBoolean();
            mode = stream.ReadBoolean();
            output = stream.ReadByte();
        }
    }
}
