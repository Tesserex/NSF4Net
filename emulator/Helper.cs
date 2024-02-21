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
namespace MyNes.Core
{
    public static class Helper
    {
        public static int GreatestCommonFactor(int a, int b)
        {
            int remainder;

            while (b != 0)
            {
                remainder = (a % b);
                a = b;
                b = remainder;
            }

            return a;
        }

        public static void Reduce(ref int a, ref int b)
        {
            var gcf = GreatestCommonFactor(a, b);

            a /= gcf;
            b /= gcf;
        }

        public static T[][] CreateArray<T>(int length1, int length2)
        {
            T[][] result = new T[length1][];

            for (int i = 0; i < length1; i++)
            {
                result[i] = new T[length2];
            }

            return result;
        }
    }
}