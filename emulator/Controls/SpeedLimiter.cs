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
using System.Threading;
namespace MyNes.Core.Controls
{
    /// <summary>
    /// The speed limiter which should control speed depending on emulation system
    /// </summary>
    public class SpeedLimiter
    {
        public SpeedLimiter(ITimer timer, TimingInfo.System emuSystem)
        {
            this.timer = timer;
            this.emuSystem = emuSystem;

            this.ON = true;

            HardReset();
        }
        private ITimer timer;
        private TimingInfo.System emuSystem;
        public bool ON;

        public double CurrentFrameTime;
        public double DeadTime;
        public double LastFrameTime;
        public double FramePeriod = (1.0 / 60.0988);
        public double FPS = 0;

        /// <summary>
        /// Call this when a frame completed
        /// </summary>
        public void Update()
        {
            CurrentFrameTime = timer.GetCurrentTime() - LastFrameTime;
            DeadTime = FramePeriod - CurrentFrameTime;
            if (ON)
            {
                //This should relieve the pc's cpu for the dead time
                //but after monitoring performance this has no effect.
                //if (DeadTime > 0)
                //    Thread.Sleep((int)(DeadTime * 1000));

                while (CurrentFrameTime < FramePeriod)
                {
                    if ((timer.GetCurrentTime() - LastFrameTime) > FramePeriod)
                    {
                        break;
                    }
                }
            }
            LastFrameTime = timer.GetCurrentTime();
        }
        public void SleepOnPause()
        {
            Thread.Sleep(100);
        }
        public void HardReset()
        {
            if (emuSystem.Master == TimingInfo.NTSC.Master)
                FramePeriod = (1.0 / (FPS = 60.0988));
            else//PALB, DENDY
                FramePeriod = (1.0 / (FPS = 50.0070));
        }
    }
    public interface ITimer
    {
        /// <summary>
        /// Get current time
        /// </summary>
        /// <returns></returns>
        double GetCurrentTime();
    }
}
