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
    public interface IAudioDevice
    {
        /// <summary>
        /// Emu calls this when it needs to update a frame
        /// </summary>
        void UpdateFrame();
        /// <summary>
        /// Emu calls this when it finishes a buffer and needs to submit
        /// </summary>
        /// <param name="samplesBuffer"></param>
        void SubmitBuffer(byte[] samplesBuffer);
        /// <summary>
        /// Shutdown the device then dispose, emu calls this when it shutdown
        /// </summary>
        void Shutdown();
        /// <summary>
        /// Play/Resume
        /// </summary>
        void Play();
        /// <summary>
        /// Pause
        /// </summary>
        void Stop();
        /// <summary>
        /// Get if the device is recording sound
        /// </summary>
        bool IsRecording { get; }
        /// <summary>
        /// Record the sound
        /// </summary>
        /// <param name="filePath">The complete wav file path</param>
        void Record(string filePath);
        /// <summary>
        /// Stop recording
        /// </summary>
        void RecordStop();
        /// <summary>
        /// Get the record time in seconds
        /// </summary>
        int RecordTime { get; }
    }
}