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
using MyNes.Core.Boards;
using MyNes.Core.CPU;

namespace MyNes.Core
{
    public class Nes
    {
        public static Cpu Cpu;
        public static Apu Apu;
        public static CpuMemory CpuMemory;
        public static Board Board;
        public static bool SoundEnabled = true;
        //emulation controls
        public static bool ON;
        public static bool Pause;
        private static bool softResetRequest = false;
        private static bool hardResetRequest = false;
        public static bool Initialized = false;
        //events
        /// <summary>
        /// Rised when the emulation shutdown
        /// </summary>
        public static event System.EventHandler EmuShutdown;
        /// <summary>
        /// Rised when the renderer need to shutdown
        /// </summary>
        public static event System.EventHandler RendererShutdown;
        /// <summary>
        /// Rised when the user request a fullscreen state change
        /// </summary>
        public static event System.EventHandler FullscreenSwitch;
        //others
        public static bool SaveSramOnShutdown = true;
        //state
        private static bool saveStateRequest;
        private static bool loadStateRequest;
        private static string requestStatePath;
        public static int StateSlot = 0;

        public static TimingInfo.System emuSystem = TimingInfo.NTSC;

        public static void InitializeComponents()
        {
            //memory first
            CpuMemory = new CpuMemory();

            CpuMemory.Initialize();

            Cpu = new Cpu(emuSystem);
            Apu = new Apu(emuSystem);

            Apu.Initialize();
            Board.Initialize(); 
            Cpu.Initialize();
            Initialized = true;
        }

        /// <summary>
        /// Get the emu into the active state
        /// </summary>
        public static void TurnOn()
        {
            Pause = false;
            ON = true;
        }
        public static void TogglePause(bool pause)
        {
            Pause = pause;
        }
        /// <summary>
        /// Run the emu, keep executing the cpu while is ON
        /// </summary>
        public static void Run()
        {
            while (ON)
            {
                if (!Pause)
                {
                    Cpu.Update();
                }
                else
                {
                    if (softResetRequest)
                    {
                        softResetRequest = false;
                        _softReset();
                        Pause = false;
                    }
                    else if (hardResetRequest)
                    {
                        hardResetRequest = false;
                        _hardReset();
                        Pause = false;
                    }
                }
            }
        }

        /// <summary>
        /// Stop the emulation and dispose components
        /// </summary>
        public static void Shutdown()
        {
            if (ON)
            {
                ON = false;
                Apu.Shutdown();
                Cpu.Shutdown();
                CpuMemory.Shutdown();

                OnEmuShutdown();
                Initialized = false;
            }
        }
        public static void OnEmuShutdown()
        {
            if (EmuShutdown != null)
                EmuShutdown(null, null);
        }
        public static void SoftReset()
        {
            if (ON)
            {
                Pause = true;
                softResetRequest = true;
            }
        }
        private static void _softReset()
        {
            CpuMemory.SoftReset();
            Board.SoftReset();
            Cpu.SoftReset();
            Apu.SoftReset();
        }
        public static void HardReset()
        {
            if (ON)
            {
                Pause = true;
                hardResetRequest = true;
            } 
        }
        private static void _hardReset()
        {
            CpuMemory.HardReset();
            Board.HardReset();
            Cpu.HardReset();
            Apu.HardReset();
        }
    }
}