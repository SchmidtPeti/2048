namespace final_2048.data
{
    internal class GetSavedData
    {
        public GetSavedData(int[,] places, int[,] values)
        {
            Places = places;
            Values = values;
        }

        public int[,] Places { get; }
        public int[,] Values { get; }
    }
}