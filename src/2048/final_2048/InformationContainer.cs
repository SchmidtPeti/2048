using Android.Widget;
using final_2048.data;

namespace final_2048
{
    internal class InformationContainer
    {
        private readonly TextView _getTextView;
        public int HighScore;

        public InformationContainer(TextView scroe, int aSide)
        {
            _getTextView = scroe;
            var scoreDatabaseController = new ScoreDatabaseController();
            HighScore = scoreDatabaseController.get_high_score(aSide);
        }

        public void add_point(int numberPlus)
        {
            if (_getTextView.Text != "")
            {
                var number = int.Parse(_getTextView.Text.Split(':')[1]) + numberPlus;
                _getTextView.Text = "Score:" + number;
                if (HighScore < number) HighScore = number;
            }
            else
            {
                _getTextView.Text = "Score:" + numberPlus;
            }
        }

        public void set_score(int number)
        {
            _getTextView.Text = "Score:" + number;
        }

        public void reset_Score()
        {
            _getTextView.Text = "";
        }
    }
}