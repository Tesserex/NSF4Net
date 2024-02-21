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
namespace MyNes.Core.IO.Output
{
    public interface IVideoDevice
    {
        /// <summary>
        /// Render a frame into the screen, the ppu calls this when the buffer is ready to be presented
        /// </summary>
        /// <param name="ScreenBuffer">The screen buffer, size = 256 * 240, each value is int32-ARBG color</param>
        void RenderFrame(int[][] ScreenBuffer);
        /// <summary>
        /// Begin the rendering operation, Ppu calls this at the beginning of the frame
        /// </summary>
        void Begin();
        /// <summary>
        /// Shutdown the device then dispose, emu calls this when it shutdown
        /// </summary>
        void Shutdown();
        /// <summary>
        /// Take a snapshot
        /// </summary>
        /// <param name="path">The desired snap path</param>
        /// <param name="format">The image format</param>
        void TakeSnapshot(string path, string format);
        /// <summary>
        /// Get a value indecate if this video device is initialized and ready to use
        /// </summary>
        bool Initialized { get; }
    }
}
