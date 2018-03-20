using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using final_2048.data;

namespace final_2048
{
    internal class GameArea : Game
    {
        private readonly int _aSide;

        // public int last_score;
        private readonly GameButton _gameButton;
        private readonly int[,] _gombPlace;
        private readonly InformationContainer _informationContainer;
        private readonly Context _parentContext;
        private readonly SaveGameArea _saveGameArea;
        private readonly ScoreDatabaseController _scoreDatabase;
        private ArrayIndexes[] _arrayIndexes;

        private TableLayout _gameAreaTableLayout;
        private FrameLayout[,] _gameButtons;
        private int[,] _lastPlaces;
        private int[,] _lastValues;
        private TableRow[] _tableRows;
        private bool _wasChange;

        public GameArea(Context context, int aSide,
            InformationContainer informationContainer) //elmentettem a context és az oldal hosszat, hogy ne kelljen minidg paraméterként megadni
        {
            _parentContext = context;
            _aSide = aSide;
            _gombPlace = new int[aSide, aSide];
            _gameButton = new GameButton(_parentContext, 0, aSide);
            _informationContainer = informationContainer;
            _saveGameArea = new SaveGameArea();
            _scoreDatabase = new ScoreDatabaseController();
        }

        public GameArea(Context context, int aSide)
        {
            _parentContext = context;
            _aSide = aSide;
            _saveGameArea = new SaveGameArea();
        }

        public TableLayout generate_game_field() //Ez adja vissza a kezdetleges pályát
        {
            _gameAreaTableLayout = new TableLayout(_parentContext)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            _gameAreaTableLayout.SetGravity(GravityFlags.Center);
            tableRows_set(_aSide, _gameAreaTableLayout);
            game_area_load();
            return _gameAreaTableLayout;
        }

        private void
            tableRows_set(int rowCount, ViewGroup gameField) //beállitm egy sor paraméterit a legalapabb dolgokat
        {
            _tableRows = new TableRow[rowCount];
            for (var i = 0; i < rowCount; i++)
            {
                _tableRows[i] = new TableRow(_parentContext)
                {
                    LayoutParameters = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent)
                };

                gameField.AddView(_tableRows[i]);
            }
        }

        private void
            game_area_load() //itt csak feltöltöm a sorokat a táblázatban üres gombokkal, hogy meg legyen az alakja a táblázatnak(így illeszkedni fog a telefon nagyságához)
        {
            _gameButtons = new FrameLayout[_aSide, _aSide];
            for (var i = 0; i < _gameAreaTableLayout.ChildCount; i++)
            {
                var tableRow = (TableRow) _gameAreaTableLayout.GetChildAt(i);
                for (var j = 0; j < _aSide; j++)
                {
                    var gameButton = new GameButton(_parentContext, 0, _aSide);
                    tableRow.AddView(gameButton.Get_game_Button(), j);
                    var btnBorder = (FrameLayout) tableRow.GetChildAt(j);
                    btnBorder.GetChildAt(0).SetBackgroundColor(Color.Gray);
                    _gameButtons[i, j] = (FrameLayout) tableRow.GetChildAt(j);
                }
            }
        }

        public void
            add_new_number(
                int number) //itt lehet majd újat hozzáadni a tablelayout-hoz úgyhogy a megfelelő sort és oszlop számot megadjuk
        {
            if (number < 10000)
                if (is_place_for_btn())
                {
                    var random = new Random();
                    int randomRow;
                    int randomColoumn;
                    do
                    {
                        randomRow = random.Next(0, _aSide);
                        randomColoumn = random.Next(0, _aSide);
                    } while (check_gameare_place(randomRow, randomColoumn));

                    GameButton.set_btn_number(_gameButtons[randomRow, randomColoumn],
                        number.ToString()); //game_Buttons[random_row, random_coloumn].Text = number.ToString();
                    _gameButtons[randomRow, randomColoumn].GetChildAt(0)
                        .SetBackgroundColor(_gameButton.get_color_from_number(number));
                    _gombPlace[randomRow, randomColoumn] = 1; //van ott már egy szám
                    _informationContainer.add_point(number);
                    //save_game_Area_();
                }
                else
                {
                    New_game();
                }
            else
                bigger_then_the_biggest();
        }

        public void save_game_Area_()
        {
            var frameValues = new int[_aSide, _aSide];
            for (var i = 0; i < _gombPlace.GetLength(0); i++)
            for (var j = 0; j < _gombPlace.GetLength(1); j++)
                if (_gombPlace[i, j] != 0)
                    frameValues[i, j] = int.Parse(GameButton.get_btn_value(_gameButtons[i, j]));
            _saveGameArea.save_game_area(_gombPlace, frameValues, _aSide);
        }

        public void save_last_scene()
        {
            _lastValues = new int[_aSide, _aSide];
            _lastPlaces = new int[_gombPlace.GetLength(0), _gombPlace.GetLength(1)];
            for (var i = 0; i < _gombPlace.GetLength(0); i++)
            for (var j = 0; j < _gombPlace.GetLength(1); j++)
                if (_gombPlace[i, j] != 0)
                {
                    _lastValues[i, j] = int.Parse(GameButton.get_btn_value(_gameButtons[i, j]));
                    _lastPlaces[i, j] = _gombPlace[i, j];
                }

            // last_score = information_Container.current_score;
            // last_places = gomb_place;
        }

        public void get_last_scene()
        {
            //   reset_game(false);
            var sum = 0;
            for (var i = 0; i < _lastPlaces.GetLength(0); i++)
            for (var j = 0; j < _lastPlaces.GetLength(1); j++)
                if (_lastPlaces[i, j] == 1)
                {
                    _gameButtons[i, j].GetChildAt(0)
                        .SetBackgroundColor(_gameButton.get_color_from_number(_lastValues[i, j]));
                    sum += _lastValues[i, j];
                    _gombPlace[i, j] = 1; //van ott már egy szám
                    GameButton.set_btn_number(_gameButtons[i, j], _lastValues[i, j].ToString());
                }
                else
                {
                    _gombPlace[i, j] = 0;
                    GameButton.normal_btn(_gameButtons[i, j]);
                }

            _informationContainer.set_score(sum);
        }

        public void load_saved_game_Area(int[,] places, int[,] values)
        {
            var sum = 0;
            for (var i = 0; i < places.GetLength(0); i++)
            for (var j = 0; j < places.GetLength(1); j++)
                if (places[i, j] == 1)
                {
                    _gameButtons[i, j].GetChildAt(0)
                        .SetBackgroundColor(_gameButton.get_color_from_number(values[i, j]));
                    sum += values[i, j];
                    _gombPlace[i, j] = 1; //van ott már egy szám
                    GameButton.set_btn_number(_gameButtons[i, j], values[i, j].ToString());
                }

            _informationContainer.add_point(sum);
        }

        public void big_move(string destination, bool frameCheckerNeed)
        {
            if (destination != "left")
                if (destination != "right")
                    if (destination != "up")
                    {
                        if (destination == "down") up_down_move("down", frameCheckerNeed);
                    }
                    else
                    {
                        up_down_move("up", frameCheckerNeed);
                    }
                else
                    left_right_move("right", frameCheckerNeed);
            else
                left_right_move("left", frameCheckerNeed);
        }

        private void left_right_move(string rightOrLeft, bool frameChecker)
        {
            _wasChange = false;
            for (var i = 0; i < _gombPlace.GetLength(0); i++)
            for (var proba = 0; proba < _gombPlace.GetLength(0) - 1; proba++)
                if (rightOrLeft == "left")
                    for (var k = 1; k < _gombPlace.GetLength(1); k++)
                        if (_gombPlace[i, k - 1] == 0 && _gombPlace[i, k] == 1)
                        {
                            change_buttons(i, k - 1, i, k);
                            _wasChange = true;
                        }
                        else if (rightOrLeft == "right")
                        {
                            for (var j = _gombPlace.GetLength(1) - 1; j >= 1; j--)
                                if (_gombPlace[i, j] == 0 && _gombPlace[i, j - 1] == 1)
                                {
                                    change_buttons(i, j, i, j - 1);
                                    _wasChange = true;
                                }
                        }

            if (!frameChecker) return;
            {
                _arrayIndexes = new ArrayIndexes[_aSide * _aSide];
                var arrayOfIndex = 0;
                for (var i = 0; i < _gombPlace.GetLength(0); i++)
                    if (rightOrLeft == "left")
                        for (var k = 1; k < _gombPlace.GetLength(1); k++)
                            if (_gombPlace[i, k - 1] == 1 && _gombPlace[i, k] == 1 &&
                                int.Parse(GameButton.get_btn_value(_gameButtons[i, k - 1])) ==
                                int.Parse(GameButton.get_btn_value(_gameButtons[i, k])))
                            {
                                var array = new ArrayIndexes
                                {
                                    ToX = i,
                                    ToY = k - 1,
                                    FromX = i,
                                    FromY = k,
                                    Direction = "left"
                                };
                                _arrayIndexes[arrayOfIndex] = array;
                                arrayOfIndex++;
                                k++;
                            }
                            else if (rightOrLeft == "right")
                            {
                                for (var j = _gombPlace.GetLength(1) - 1; j >= 1; j--)
                                    if (_gombPlace[i, j] == 1 && _gombPlace[i, j - 1] == 1 &&
                                        int.Parse(GameButton.get_btn_value(_gameButtons[i, j - 1])) ==
                                        int.Parse(GameButton.get_btn_value(_gameButtons[i, j])))
                                    {
                                        var array = new ArrayIndexes
                                        {
                                            ToX = i,
                                            ToY = j,
                                            FromY = j - 1,
                                            FromX = i,
                                            Direction = "right"
                                        };
                                        _arrayIndexes[arrayOfIndex] = array;
                                        arrayOfIndex++;
                                        j--;
                                    }
                            }

                if (_wasChange || arrayOfIndex > 0 && is_place_for_btn()) add_new_number(GameButton.get_new_btn());
                frame_fushion(_arrayIndexes, arrayOfIndex);
            }
        }

        private void frame_fushion(IReadOnlyList<ArrayIndexes> arrayIndexes, int length) //row checker nem müködik
        {
            for (var i = 0; i < length; i++)
                switch (arrayIndexes[i].Direction)
                {
                    case "left":
                        number_fusion(arrayIndexes[i].ToX, arrayIndexes[i].ToY, arrayIndexes[i].FromX,
                            arrayIndexes[i].FromY);
                        break;
                    case "right":
                        number_fusion(arrayIndexes[i].ToX, arrayIndexes[i].ToY, arrayIndexes[i].FromX,
                            arrayIndexes[i].FromY);
                        break;
                    case "up":
                        number_fusion(arrayIndexes[i].ToX, arrayIndexes[i].ToY, arrayIndexes[i].FromX,
                            arrayIndexes[i].FromY);
                        break;
                    case "down":
                        number_fusion(arrayIndexes[i].ToX, arrayIndexes[i].ToY, arrayIndexes[i].FromX,
                            arrayIndexes[i].FromY);
                        break;
                }
            if (length > 0) big_move(arrayIndexes[0].Direction, false);
        }

        private void up_down_move(string upOrDown, bool fushionChecker)
        {
            _wasChange = false;
            int sor;
            for (var i = 0; i < _gombPlace.GetLength(0); i++)
            for (var proba = 0; proba < _gombPlace.GetLength(0) - 1; proba++)
                switch (upOrDown)
                {
                    case "up":
                        sor = 1;
                        while (sor < _gombPlace.GetLength(1))
                        {
                            if (_gombPlace[sor, i] == 1 && _gombPlace[sor - 1, i] == 0)
                            {
                                change_buttons(sor - 1, i, sor, i);
                                _wasChange = true;
                            }

                            sor++;
                        }

                        break;
                    case "down":
                        sor = _gombPlace.GetLength(0) - 1;
                        while (sor > 0)
                        {
                            if (_gombPlace[sor, i] == 0 && _gombPlace[sor - 1, i] == 1)
                            {
                                change_buttons(sor, i, sor - 1, i);
                                _wasChange = true;
                            }

                            sor--;
                        }

                        break;
                }

            if (!fushionChecker) return;
            {
                _arrayIndexes = new ArrayIndexes[_aSide * _aSide];
                var arrayOfIndex = 0;
                for (var i = 0; i < _gombPlace.GetLength(0); i++)
                    switch (upOrDown)
                    {
                        case "up":
                            sor = 1;
                            while (sor < _gombPlace.GetLength(1))
                            {
                                if (_gombPlace[sor, i] == 1 && _gombPlace[sor - 1, i] == 1 &&
                                    int.Parse(GameButton.get_btn_value(_gameButtons[sor, i])) ==
                                    int.Parse(GameButton.get_btn_value(_gameButtons[sor - 1, i])))
                                {
                                    var array = new ArrayIndexes
                                    {
                                        ToX = sor - 1,
                                        ToY = i,
                                        FromX = sor,
                                        FromY = i,
                                        Direction = "up"
                                    };
                                    _arrayIndexes[arrayOfIndex] = array;
                                    arrayOfIndex++;
                                    sor++;
                                }

                                sor++;
                            }

                            break;
                        case "down":
                            sor = _gombPlace.GetLength(0) - 1;
                            while (sor > 0)
                            {
                                if (_gombPlace[sor, i] == 1 && _gombPlace[sor - 1, i] == 1 &&
                                    int.Parse(GameButton.get_btn_value(_gameButtons[sor, i])) ==
                                    int.Parse(GameButton.get_btn_value(_gameButtons[sor - 1, i])))
                                {
                                    var array = new ArrayIndexes
                                    {
                                        ToX = sor,
                                        ToY = i,
                                        FromX = sor - 1,
                                        FromY = i,
                                        Direction = "down"
                                    };
                                    _arrayIndexes[arrayOfIndex] = array;
                                    arrayOfIndex++;
                                    sor--;
                                }

                                sor--;
                            }

                            break;
                    }

                if (_wasChange || arrayOfIndex > 0 && is_place_for_btn()) add_new_number(GameButton.get_new_btn());
                frame_fushion(_arrayIndexes, arrayOfIndex);
            }
        }

        private void change_buttons(int toNumberX, int toNumberY, int toNormalX, int toNormalY)
        {
            _gombPlace[toNumberX, toNumberY] = 1;
            _gombPlace[toNormalX, toNormalY] = 0;
            var number = GameButton.get_btn_value(_gameButtons[toNormalX, toNormalY]);
            _gameButton.numbered_btn(_gameButtons[toNumberX, toNumberY], int.Parse(number));
            GameButton.normal_btn(_gameButtons[toNormalX, toNormalY]);
        }

        private void number_fusion(int newPlaceX, int newPlaceY, int emptyPlaceX, int emptyPlaceY)
        {
            _gombPlace[newPlaceX, newPlaceY] = 1;
            _gombPlace[emptyPlaceX, emptyPlaceY] = 0;
            var number = int.Parse(GameButton.get_btn_value(_gameButtons[newPlaceX, newPlaceY])) +
                         int.Parse(GameButton.get_btn_value(_gameButtons[emptyPlaceX, emptyPlaceY]));
            if (number == 2048) win_mess();
            _gameButton.numbered_btn(_gameButtons[newPlaceX, newPlaceY], number);
            GameButton.normal_btn(_gameButtons[emptyPlaceX, emptyPlaceY]);
        }

        private bool check_gameare_place(int x, int y)
        {
            return _gombPlace[x, y] == 1;
        }

        public bool is_place_for_btn()
        {
            for (var i = 0; i < _gombPlace.GetLength(0); i++) //place checker
            for (var j = 0; j < _gombPlace.GetLength(1); j++)
                if (_gombPlace[i, j] != 1)
                    return true;
            return false;
        }

        public bool fushion_is_avaiable()
        {
            for (var i = 0; i < _gombPlace.GetLength(0); i++)
            {
                for (var j = 1; j < _gombPlace.GetLength(1); j++)
                    if (GameButton.get_btn_value(_gameButtons[i, j - 1]) ==
                        GameButton.get_btn_value(_gameButtons[i, j]))
                        return true;
                for (var l = 1; l < _gombPlace.GetLength(1); l++)
                    if (GameButton.get_btn_value(_gameButtons[l - 1, i]) ==
                        GameButton.get_btn_value(_gameButtons[l, i]))
                        return true;
            }

            return false;
        }

        //public bool left_right_full()
        //{
        //    for (int i = 0; i < gomb_place.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < gomb_place.GetLength(1); j++)
        //        {
        //            if (gomb_place[i, j] == 0)
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
        //public bool up_down_full()
        //{
        //    int sor;
        //    for (int i = 0; i < gomb_place.GetLength(0); i++)
        //    {
        //        sor = 0;
        //        while (sor < gomb_place.GetLength(1))
        //        {
        //            if (gomb_place[sor, i] == 0)
        //            {
        //                return false;
        //            }
        //            sor++;
        //        }
        //    }
        //    return true;
        //}
        public void New_game()
        {
            var saveGameArea = new SaveGameArea();
            saveGameArea.delete_this_side(_aSide);
            var alertDialog = new AlertDialog.Builder(_parentContext);
            alertDialog.SetTitle("2048");
            alertDialog.SetMessage("Try again? ");
            alertDialog.SetPositiveButton("Yes", (senderAlert, args) => { reset_game(true); });
            Dialog alert = alertDialog.Create();
            alert.Show();
        }

        private void win_mess()
        {
            if (_informationContainer.HighScore > 0)
            {
                var score = new Score
                {
                    _score = _informationContainer.HighScore,
                    _side = _aSide
                };
                _scoreDatabase.set_high_Score(score);
            }

            var alertDialog = new AlertDialog.Builder(_parentContext);
            alertDialog.SetTitle("2048");
            alertDialog.SetMessage("You win!Do you want to continue?");
            alertDialog.SetPositiveButton("Yes", (senderAlert, args) => { });
            alertDialog.SetNegativeButton("No", (senderAlert, args) => { reset_game(true); });
            Dialog alert = alertDialog.Create();
            alert.Show();
        }

        private void bigger_then_the_biggest()
        {
            Toast.MakeText(_parentContext, "You reached the final limit", ToastLength.Long).Show();
            var score = new Score
            {
                _score = _informationContainer.HighScore,
                _side = _aSide
            };
            _scoreDatabase.set_high_Score(score);
            reset_game(true);
        }

        //public void row_checker(int x,string left_or_right)
        //{
        //    for (int index = 0; index < gomb_place.GetLength(0)-1; index++)
        //    {
        //        if (left_or_right == "right")
        //        {
        //            for (int i = 1; i < gomb_place.GetLength(1); i++)
        //            {
        //                if (gomb_place[x, i - 1] == 1 && gomb_place[x, i] == 0)
        //                {
        //                    change_buttons(x, i, x, i - 1);
        //                }
        //            }
        //        }
        //        if (left_or_right == "left")
        //        {
        //            for (int i = 1; i < gomb_place.GetLength(1); i++)
        //            {
        //                if (gomb_place[x, i - 1] == 0 && gomb_place[x, i] == 1)
        //                {
        //                    change_buttons(x, i - 1, x, i);
        //                }
        //            }
        //        }
        //    }
        //}
        //public void coloumn_checker(int y,string top_or_down)
        //{
        //    for (int index = 0; index < gomb_place.GetLength(1)-1; index++)
        //    {
        //        if (top_or_down == "up")
        //        {
        //            int i = 1;
        //            while (i < gomb_place.GetLength(0))
        //            {
        //                if (gomb_place[i - 1, y] == 0 && gomb_place[i, y] == 1)
        //                {
        //                    change_buttons(i - 1, y, i, y);
        //                }
        //                i++;
        //            }
        //        }
        //        if (top_or_down == "down")
        //        {
        //            int i = 1;
        //            while (i < gomb_place.GetLength(0))
        //            {
        //                if (gomb_place[i - 1, y] == 1 && gomb_place[i, y] == 0)
        //                {
        //                    change_buttons(i, y, i - 1, y);
        //                }
        //                i++;
        //            }
        //        }
        //    }
        //}
        private void reset_game(bool needBeginGameButton)
        {
            _saveGameArea.delete_this_side(_aSide);
            for (var i = 0; i < _gombPlace.GetLength(0); i++)
            for (var j = 0; j < _gombPlace.GetLength(1); j++)
                _gombPlace[i, j] = 0;
            for (var i = 0; i < _gameButtons.GetLength(0); i++)
            for (var j = 0; j < _gameButtons.GetLength(1); j++)
                GameButton.normal_btn(_gameButtons[i, j]);
            _informationContainer.reset_Score();
            if (!needBeginGameButton) return;
            add_new_number(2);
            add_new_number(2);
        }
    }
}