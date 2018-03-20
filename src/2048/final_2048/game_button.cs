using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
namespace final_2048
{
    class game_button/*: View.IOnTouchListener*/
    {
        public Context parent_context;
        public int number;
        information_container information_Container;
        int a_side;
        

        public game_button(Context context,int number,information_container information_Container,int a_side)//paraméter átadás csökkentése érdekében elmentem publikus változoban őket
        {
            parent_context = context;
            this.number = number;
            this.information_Container = information_Container;
            this.a_side = a_side;
        }
        public FrameLayout Get_game_Button()//egy gomb legenerálása
        {
            FrameLayout game_button_button = new FrameLayout(parent_context);
            TextView its_value = new TextView(parent_context);
            its_value.LayoutParameters = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent, FrameLayout.LayoutParams.MatchParent);
            its_value.SetTextColor(Color.Black);
            its_value.TextSize = 40;
            its_value.Gravity = GravityFlags.Center;
            if (number != 0)
            {
                its_value.Text = number.ToString();
            }
            else//ez nem lesz benne a rendes játékban csak ezzel jelzem, hogy hogyan jelenik meg a szöveg a button-ben
            {
               // game_button_button.Text = "s";
            }
            var metrics = parent_context.Resources.DisplayMetrics;
            var widthInDp = metrics.WidthPixels-20;
            game_button_button.LayoutParameters = new TableRow.LayoutParams(widthInDp/a_side, widthInDp / a_side);
            game_button_button.AddView(its_value);
            return game_button_button;
        }
        public void normal_btn(FrameLayout button)
        {
            //game_button game_Button = new game_button(parent_context, 0,information_Container);
            button.SetBackgroundColor(Color.Gray);
            set_btn_number(button,"");
        }
        public void numbered_btn(FrameLayout button,int number)
        {
            button.SetBackgroundColor(Color.White);
            set_btn_number(button, number.ToString()); //button = number.ToString();
        }
        public void set_btn_number(FrameLayout button,string number)
        {
            TextView textView = (TextView)button.GetChildAt(0);
            textView.Text = number;
        }
        public string get_btn_value(FrameLayout button)
        {
            TextView textView = (TextView)button.GetChildAt(0);
            return textView.Text;
        }
    }
}