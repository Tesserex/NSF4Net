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
using MyNes.Core.Types;

namespace MyNes.Core
{
    public class ProcessorBase
    {
        protected TimingInfo timing;
        protected TimingInfo.System system;

        public ProcessorBase(TimingInfo.System system)
        {
            this.system = system;
        }

        public virtual void Initialize() { }
        public virtual void Shutdown() { }
        public virtual void SoftReset() { }
        public virtual void HardReset() { }
        public virtual void Update() { }
        public virtual void Update(int cycles)
        {
            while (timing.cycles < cycles)
            {
                timing.cycles += timing.single;

                Update();
            }

            timing.cycles -= cycles;
        }
        public virtual void SaveState(StateStream stream) { timing.SaveState(stream); }
        public virtual void LoadState(StateStream stream) { timing.LoadState(stream); }
    }
}