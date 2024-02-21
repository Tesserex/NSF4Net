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
namespace MyNes.Core.CPU
{
    // Emulates the undocumented direct memory access controller in the 2A03/2A07
    public class Dma
    {
        private bool step;
        private byte data;
        private int addr;
        private int size;
        private int timer = 0;
        private bool isOam=false;
        public void DmcFetch(int address)
        {
            if (size != 0)
                return; // transfer already occuring
            size = 4; 
            addr = address;
            isOam = false;
            Nes.Cpu.Lock();
        }
        public void OamTransfer(int address, byte data)
        {
            if (size != 0)
                return; // transfer already occuring
            step = false;
            addr = (data << 8);
            size = 256;
            isOam = true;
            /*http://nesdev.com/6502_cpu.txt
             -RDY is ignored during writes
             (This is why you must wait 3 cycles before doing any DMA --
             the maximum number of consecutive writes is 3, which occurs
             during interrupts except -RESET.)
             */
            timer = 2;

            Nes.Cpu.Lock();
        }
        public void Update()
        {
            if (size == 0)
                return;
            if (isOam)
            {
                if (timer > 0)
                {
                    Nes.Cpu.Dispatch();
                    timer--;
                    return;
                }
                if (step = !step)
                {
                    Nes.Cpu.Dispatch();
                    data = Nes.CpuMemory[addr];
                }
                else
                {
                    Nes.Cpu.Dispatch();
                    Nes.CpuMemory[0x2004] = data;

                    addr = (++addr) & 0xFFFF;
                    size = (--size) & 0xFFFF;

                    if (size == 0)
                        Nes.Cpu.Unlock();
                }
            }
            else//dmc
            {
                size--;
                if (size > 0)
                {
                    //do dummy peek
                    Nes.Cpu.Dispatch();
                    data = Nes.CpuMemory[addr];
                }
                else
                {
                    Nes.Apu.dmc.DoFetch();
                    Nes.Cpu.Unlock();
                }
            }
        }
        public virtual void SaveState(StateStream stream)
        {
            stream.Write(step);
            stream.Write(data);
            stream.Write(addr);
            stream.Write(size);
        }
        public virtual void LoadState(StateStream stream)
        {
            step = stream.ReadBooleans()[0];
            data = stream.ReadByte();
            addr = stream.ReadInt32();
            size = stream.ReadInt32();
        }
    }
}