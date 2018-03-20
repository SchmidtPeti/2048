using SQLite;

namespace final_2048.data
{
    public class Score
    {
        [PrimaryKey] [AutoIncrement] public static int Id => 0;

        public int _score { get; set; }
        public int _side { get; set; }
    }
}