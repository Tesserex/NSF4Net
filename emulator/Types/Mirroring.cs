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

namespace MyNes.Core.Types
{
    public enum Mirroring : byte
    {
        /// <summary>
        /// [ A ][ B ]
        /// [ A ][ B ]
        /// </summary>
        ModeVert = 0x11,
        /// <summary>
        /// [ A ][ A ]
        /// [ B ][ B ]
        /// </summary>
        ModeHorz = 0x05,
        /// <summary>
        /// [ A ][ A ]
        /// [ A ][ A ]
        /// </summary>
        Mode1ScA = 0x00,
        /// <summary>
        /// [ B ][ B ]
        /// [ B ][ B ]
        /// </summary>
        Mode1ScB = 0x55,
        /// <summary>
        /// [ A ][ B ]
        /// [ C ][ D ]
        /// </summary>
        ModeFull = 0x1B,
    }
}