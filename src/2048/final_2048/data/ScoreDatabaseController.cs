using System;
using System.Collections.Generic;
using System.IO;
using SQLite;

namespace final_2048.data
{
    public class ScoreDatabaseController
    {
        //SQLiteConnection database;
        private readonly SQLiteConnection _db;

        public ScoreDatabaseController()
        {
            //database = DependencyService.Get<sqlite>().GetSQLiteConnection();
            const string sqliteFileName = "score.db3";
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(docPath, sqliteFileName);
            _db = new SQLiteConnection(path);
            _db.CreateTable<Score>();
        }

        public int get_high_score(int aSide)
        {
            var score = 0;
            var table = _db.Table<Score>();
            var valami = new List<int>();
            foreach (var item in table)
                if (item._side == aSide)
                {
                    valami.Add(Score.Id);
                    score = item._score;
                }

            clean_score(valami);
            return score;
        }

        public void set_high_Score(Score score)
        {
            _db.Insert(score);
        }

        private void clean_score(IReadOnlyList<int> ids)
        {
            if (ids.Count <= 1) return;
            for (var i = 0; i < ids.Count - 1; i++) //az utolsót ne törölje ki
                _db.Delete<Score>(ids[i]);
        }
    }
}