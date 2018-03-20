using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using final_2048.data;

namespace final_2048
{
    [Activity(Label = "2048", MainLauncher = true, Icon = "@drawable/mango",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private int _selectedItem = 3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            var newGameBtn = FindViewById<Button>(Resource.Id.New_Game_btn);
            var mainLayer = FindViewById<LinearLayout>(Resource.Id.main_layer);
            var spinner = FindViewById<Spinner>(Resource.Id.choose_side);
            var highscore = FindViewById<TextView>(Resource.Id.high_score);
            string[] data = {"3", "4", "5", "6", "7", "8"};
            var adapter = new ArrayAdapter(this, Resource.Layout.forlistlayout, data);
            spinner.Adapter = adapter;
            GameArea gameArea;
            var scoreDatabaseController = new ScoreDatabaseController();
            var was = false;
            //  TableLayout tableLayout = new TableLayout(this);
            var saveGame = new SaveGameArea();
            spinner.ItemSelected += (s, e) =>
            {
                _selectedItem = int.Parse(spinner.GetItemAtPosition(e.Position).ToString());
                refresh_high_score(scoreDatabaseController, highscore);
                gameArea = new GameArea(this, _selectedItem);
                if (was) mainLayer.RemoveViewAt(1);
                mainLayer.AddView(gameArea.generate_game_field(), mainLayer.ChildCount - 2);
                was = true;
                if (saveGame.is_side_Exist(_selectedItem))
                    load_saved_scene((TableLayout) mainLayer.GetChildAt(1), _selectedItem);
            };
            refresh_high_score(scoreDatabaseController, highscore);
            newGameBtn.Click += (s, e) =>
            {
                var intent = new Intent(this, typeof(Game));
                intent.PutExtra("a_side", _selectedItem.ToString());
                StartActivity(
                    intent); //Elkezdi a játékot és átvisz a game activity-re(ebben az activity-ban semmi fontos nem történik a játék szemponjtából)
            };
        }

        private void refresh_high_score(ScoreDatabaseController scoreDatabaseController, TextView textView)
        {
            textView.Text = "High score:" + scoreDatabaseController.get_high_score(_selectedItem);
        }

        private static void load_saved_scene(ViewGroup tableLayout, int aSide)
        {
            var saveGameArea = new SaveGameArea();
            var getSavedData = saveGameArea.GetGame_Area_(aSide);
            var gameButton = new GameButton();
            for (var i = 0; i < aSide; i++)
            for (var j = 0; j < aSide; j++)
            {
                var tableRow = (TableRow) tableLayout.GetChildAt(i);
                var border = (FrameLayout) tableRow.GetChildAt(j);
                var frameLayout = (FrameLayout) border.GetChildAt(0);
                if (getSavedData != null)
                {
                    if (getSavedData.Values[i, j] <= 0) continue;
                    frameLayout.SetBackgroundColor(gameButton.get_color_from_number(getSavedData.Values[i, j]));
                    GameButton.set_btn_number(border, getSavedData.Values[i, j].ToString());
                }
                else
                {
                    return;
                }
            }
        }
    }
}