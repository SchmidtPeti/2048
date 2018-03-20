using System;
using System.Collections.Generic;
using System.IO;
using SQLite;

namespace final_2048.data
{
    internal class SaveGameArea
    {
        private readonly SQLiteConnection _db;
        private TableQuery<GameAreaSave> _table;

        public SaveGameArea()
        {
            const string databaseName = "save_gamearea.db3";
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(filePath, databaseName);
            _db = new SQLiteConnection(path);
            _db.CreateTable<GameAreaSave>();
            _table = _db.Table<GameAreaSave>();
        }

        public void save_game_area(int[,] places, int[,] values, int aSide)
        {
            var record = get_latest_record(aSide) + 1;
            for (var i = 0; i < places.GetLength(0); i++)
            for (var j = 0; j < places.GetLength(1); j++)
                _db.Insert(new GameAreaSave(places[i, j], values[i, j], i, j, record, aSide));
            _table = _db.Table<GameAreaSave>();
        }

        public GetSavedData GetGame_Area_(int aSide)
        {
            GetSavedData gameArea = null;
            var places = new int[aSide, aSide];
            var values = new int[aSide, aSide];
            var was = false;
            foreach (var item in _table)
                if (item.Record == get_latest_record(aSide) && item.Side == aSide)
                {
                    //list.Add(item);
                    places[item.Sor, item.Oszlop] = item.ButtonPlace;
                    values[item.Sor, item.Oszlop] = item.ButtonValue;
                    was = true;
                }

            if (was) gameArea = new GetSavedData(places, values);

            return gameArea;
        }

        private void clean_db(IEnumerable<GameAreaSave> list)
        {
            foreach (var t in list)
                delete_this_record(t);
        }

        public void clean_save_game_area_db(int side)
        {
            var list = new List<GameAreaSave>();
            foreach (var item in _table)
                if (item.Record < get_latest_record(side) && item.Side == side)
                    list.Add(item);
            clean_db(list);
        }

        private void delete_this_record(GameAreaSave id)
        {
            _db.Delete(id);
        }

        private int get_latest_record(int side)
        {
            var record = 0;
            foreach (var item in _table)
                if (item.Side == side)
                    record = item.Record;
            return record;
        }

        public void delete_this_side(int side)
        {
            foreach (var item in _table)
                if (item.Side == side)
                    delete_this_record(item);
        }

        public bool is_side_Exist(int side)
        {
            foreach (var item in _table)
                if (item.Side == side)
                    return true;
            return false;
        }
    }
}