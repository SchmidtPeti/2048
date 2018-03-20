using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace final_2048
{
    class information_container
    {
        public int score = 0;
        public Context context;
        public TextView GetTextView;
        public information_container(Context context,int score,TextView scroe)
        {
            this.score = score;
            this.context = context;
            GetTextView = scroe;
        }
        public void add_point()
        {
            if (GetTextView.Text != "")
            {
                GetTextView.Text = "Score:" + (int.Parse(GetTextView.Text.Split(':')[1]) + 2);
            }
            else
            {
                GetTextView.Text = "Score:2";
            }
        }
        public void reset_Score()
        {
            GetTextView.Text = "";
        }
    }
}