namespace final_2048.data
{
    internal class GameAreaSave
    {
        public GameAreaSave()
        {
        }

        public GameAreaSave(int buttonPlace, int buttonValue, int sor, int oszlop, int record, int side)
        {
            ButtonPlace = buttonPlace;
            ButtonValue = buttonValue;
            Sor = sor;
            Oszlop = oszlop;
            Record = record;
            Side = side;
        }

        public int ButtonPlace { get; }
        public int ButtonValue { get; }
        public int Sor { get; }
        public int Oszlop { get; }
        public int Record { get; }
        public int Side { get; }
    }
}