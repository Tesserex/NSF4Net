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
using System.Reflection;
using MyNes.Core.Boards.Nintendo;

namespace MyNes.Core.Boards
{
    public class BoardsManager
    {
        private static Board[] boards;

        //Methods
        public static void LoadAvailableBoards()
        {
            List<Board> availableBoards = new List<Board>();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type tp in types)
            {
                if (tp.IsSubclassOf(typeof(Board)))
                {
                    if (!tp.IsAbstract)
                    {
                        Board board = Activator.CreateInstance(tp) as Board;
                        availableBoards.Add(board);
                    }
                }
            }
            boards = availableBoards.ToArray();
        }

        public static Board GetBoard(byte soundChip)
        {
            foreach (Board board in boards)
            {
                if (board.SoundChip == soundChip)
                {
                    Type boardType = board.GetType();
                    return (Board)Activator.CreateInstance(boardType, new object[] { new byte[0x8000], new byte[0x8000], new byte[0x8000], false });
                }
            }
            return new NROM(new byte[0x8000], new byte[0x8000], new byte[0x8000], false);
        }

        //Properties
        public static Board[] AvailableBoards { get { return boards; } }
    }
}
