namespace USF.Utils
{
    public static class MathUtils
    {
        public static int Mod(int a, int b) {
            int c = a % b;
            if ((c < 0 && b > 0) || (c > 0 && b < 0)) {
                c += b;
            }
            return c;
        }
    }
}