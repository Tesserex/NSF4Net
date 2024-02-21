namespace MyNes.Core
{
    public static class MathHelper
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
    }
}