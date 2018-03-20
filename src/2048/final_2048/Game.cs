using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using final_2048.data;

namespace final_2048
{
    [Activity(Label = "2048", Icon = "@drawable/mango", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Game : Activity, View.IOnTouchListener
    {
        private const int Sensitive = 80;
        private GameArea _gameArea;
        private InformationContainer _informationContainer;
        private Button _lastSceneBtn;
        private SaveGameArea _saveGame;
        private ScoreDatabaseController _scoreDatabase;

        private int _side;
        private TextView _textView;
        private bool _touched;
        private float _viewX;
        private float _viewy;

        //public bool right = false;
        public bool
            OnTouch(View v,
                MotionEvent e) //mindig csak az adott elemnél hívodik és megmondja, hogy éppen hol van az ujjunk rajta
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _viewX = e.GetX();
                    _viewy = e.GetY();
                    _gameArea.save_last_scene();
                    _lastSceneBtn.Enabled = true;
                    break;
                case MotionEventActions.Move:
                    if (!_touched)
                        if (_viewX + Sensitive < e.GetX())
                        {
                            _touched = true;
                            _gameArea.big_move("right", true);
                            //   game_Area.add_new_number(2);
                        }
                        else if (_viewX - Sensitive > e.GetX())
                        {
                            _touched = true;
                            _gameArea.big_move("left", true);
                            //   game_Area.add_new_number(2);
                        }
                        else if (_viewy + Sensitive < e.GetY())
                        {
                            _touched = true;
                            _gameArea.big_move("down", true);
                            //   game_Area.add_new_number(2);
                        }
                        else if (_viewy - Sensitive > e.GetY())
                        {
                            _touched = true;
                            _gameArea.big_move("up", true);
                            //  game_Area.add_new_number(2);
                        }

                    break;
                case MotionEventActions.Up:
                    if (_touched) _touched = false;
                    if (!_gameArea.is_place_for_btn() && !_gameArea.fushion_is_avaiable()) _gameArea.New_game();
                    break;
                case MotionEventActions.ButtonPress:
                    break;
                case MotionEventActions.ButtonRelease:
                    break;
                case MotionEventActions.Cancel:
                    break;
                case MotionEventActions.HoverEnter:
                    break;
                case MotionEventActions.HoverExit:
                    break;
                case MotionEventActions.HoverMove:
                    break;
                case MotionEventActions.Mask:
                    break;
                case MotionEventActions.Outside:
                    break;
                case MotionEventActions.Pointer1Down:
                    break;
                case MotionEventActions.Pointer1Up:
                    break;
                case MotionEventActions.Pointer2Down:
                    break;
                case MotionEventActions.Pointer2Up:
                    break;
                case MotionEventActions.Pointer3Down:
                    break;
                case MotionEventActions.Pointer3Up:
                    break;
                case MotionEventActions.PointerIdMask:
                    break;
                case MotionEventActions.PointerIdShift:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game);
            _side = int.Parse(Intent.GetStringExtra("a_side"));
            _scoreDatabase = new ScoreDatabaseController();
            _textView = FindViewById<TextView>(Resource.Id.textView1);
            var gameLinearLayout =
                FindViewById<LinearLayout>(Resource.Id
                    .game_screen); //Ez a legalapbb úgynevett root layout ehhez fogunk majd mindent hozzáadni vertical-is
            _informationContainer =
                new InformationContainer(_textView, int.Parse(Intent.GetStringExtra("a_side")));
            _gameArea = new GameArea(this, int.Parse(Intent.GetStringExtra("a_side")),
                _informationContainer); //itt átadtam ennek az activity-t és hogy mekkora legyen a pálya!!!!!!!!!!!!!!!!!!! itt lehet beállitani, hogy hányszor hányas legyen a pálya
            var tableLayout = _gameArea.generate_game_field();
            gameLinearLayout.AddView(tableLayout); //legenáráltam az üres pályát és hozzáadtam a root layout-hoz
            var button = FindViewById<Button>(Resource.Id.new_game);
            var bckBtn = FindViewById<Button>(Resource.Id.button1);
            _lastSceneBtn = FindViewById<Button>(Resource.Id.button2);
            bckBtn.Click += (s, e) =>
            {
                var score = new Score
                {
                    _score = _informationContainer.HighScore,
                    _side = _side
                };
                _scoreDatabase.set_high_Score(score);
                _gameArea.save_game_Area_();
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };
            button.Click += (s, e) => { _gameArea.New_game(); };
            _lastSceneBtn.Click += (s, e) => { _gameArea.get_last_scene(); };
            _lastSceneBtn.Enabled = false;
            //  button.SetOnTouchListener(this);//touch sensor érzékelésének hozzáadása a textview-hoz
            set_onclick_for_buttons(tableLayout);
            _saveGame = new SaveGameArea();
            if (_saveGame.GetGame_Area_(int.Parse(Intent.GetStringExtra("a_side"))) != null)
            {
                var getSavedData = _saveGame.GetGame_Area_(int.Parse(Intent.GetStringExtra("a_side")));
                _gameArea.load_saved_game_Area(getSavedData.Places, getSavedData.Values);
            }
            else
            {
                _gameArea.add_new_number(GameButton.get_new_btn()); //az első generálás
                _gameArea.add_new_number(GameButton.get_new_btn());
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            _gameArea.save_game_Area_();
            if (_informationContainer.HighScore > 0)
            {
                var score = new Score
                {
                    _score = _informationContainer.HighScore,
                    _side = _side
                };
                _scoreDatabase.set_high_Score(score);
            }

            int[] sides = {3, 4, 5, 6, 7, 8};
            foreach (var t in sides)
                _saveGame.clean_save_game_area_db(t);

            base.OnSaveInstanceState(outState);
        }

        public override void OnBackPressed()
        {
            _gameArea.save_game_Area_();
            if (_informationContainer.HighScore > 0)
            {
                var score = new Score
                {
                    _score = _informationContainer.HighScore,
                    _side = _side
                };
                _scoreDatabase.set_high_Score(score);
            }

            base.OnBackPressed();
        }

        private void
            set_onclick_for_buttons(
                ViewGroup tableLayout) //itt álítom rá a sensor-t a tablelayoutgombokra, mivel csak innen lehet
        {
            for (var i = 0; i < tableLayout.ChildCount; i++)
            {
                var tableRow = (TableRow) tableLayout.GetChildAt(i);
                for (var j = 0; j < tableRow.ChildCount; j++)
                {
                    var button = (FrameLayout) tableRow.GetChildAt(j);
                    button.SetOnTouchListener(this);
                }
            }
        }
    }
}