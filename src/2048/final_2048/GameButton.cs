using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace final_2048
{
    internal class GameButton /*: View.IOnTouchListener*/
    {
        private readonly int _aSide;

        private readonly Color[] _colors =
        {
            Color.Turquoise, Color.Brown, Color.Magenta, Color.Yellow, Color.Cyan, Color.Green, Color.Orange,
            Color.Lime, Color.Beige, Color.Pink, Color.LightBlue
        };

        private readonly int _number;

        private readonly int[] _numbers = {2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048};
        private readonly Context _parentContext;

        public GameButton(Context context, int number,
            int aSide) //paraméter átadás csökkentése érdekében elmentem publikus változoban őket
        {
            _parentContext = context;
            _number = number;
            _aSide = aSide;
        }

        public GameButton()
        {
        }

        public FrameLayout Get_game_Button() //egy gomb legenerálása
        {
            FrameLayout gamebuttonBorder;
            using (var gameButtonButton = new FrameLayout(_parentContext))
            {
                gamebuttonBorder = new FrameLayout(_parentContext);
                var itsValue = new TextView(_parentContext)
                {
                    LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.MatchParent)
                };
                itsValue.SetTextColor(Color.Black);
                itsValue.TextSize = get_text_Size();
                itsValue.Gravity = GravityFlags.Center;
                if (_number != 0) itsValue.Text = _number.ToString();

                var metrics = _parentContext.Resources.DisplayMetrics;
                var widthInDp = metrics.WidthPixels - _aSide * 10;
                gameButtonButton.LayoutParameters =
                    new TableRow.LayoutParams(widthInDp / _aSide - 10, widthInDp / _aSide - 10);
                gamebuttonBorder.LayoutParameters =
                    new TableRow.LayoutParams(widthInDp / _aSide + 10, widthInDp / _aSide + 10);
                gamebuttonBorder.SetBackgroundColor(Color.Black);
                gameButtonButton.AddView(itsValue);
                gamebuttonBorder.AddView(gameButtonButton);
            }

            return gamebuttonBorder;
        }

        public static void normal_btn(FrameLayout button)
        {
            //game_button game_Button = new game_button(parent_context, 0,information_Container);
            button.GetChildAt(0).SetBackgroundColor(Color.Gray);
            set_btn_number(button, "");
        }

        public void numbered_btn(FrameLayout button, int number)
        {
            button.GetChildAt(0).SetBackgroundColor(get_color_from_number(number));
            set_btn_number(button, number.ToString()); //button = number.ToString();
        }

        public static void set_btn_number(ViewGroup button, string number)
        {
            var gameBtn = (FrameLayout) button.GetChildAt(0);
            var textView = (TextView) gameBtn.GetChildAt(0);
            textView.Text = number;
        }

        public static string get_btn_value(FrameLayout button)
        {
            var gameBtn = (FrameLayout) button.GetChildAt(0);
            var textView = (TextView) gameBtn.GetChildAt(0);
            return textView.Text;
        }

        public Color get_color_from_number(int number)
        {
            for (var i = 0; i < _numbers.Length; i++)
                if (_numbers[i] == number)
                    return _colors[i];
            return Color.LightGoldenrodYellow;
        }

        private int get_text_Size()
        {
            switch (_aSide)
            {
                case 3:
                    return 40;
                case 4:
                    return 36;
                case 5:
                    return 30;
                case 6:
                    return 24;
                case 7:
                    return 20;
                case 8:
                    return 18;
            }

            return 0;
        }

        public static int get_new_btn()
        {
            int[] randomButtons = {2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4};
            var random = new Random();
            var rndIndex = random.Next(0, randomButtons.Length - 1);
            return randomButtons[rndIndex];
        }
    }
}