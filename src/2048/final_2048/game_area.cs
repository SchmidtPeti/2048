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
    class game_area : game
    {
        public TableLayout game_area_tableLayout;
        public TableRow[] tableRows;
        public Context parent_context;
        public int a_side;
        public int[,] gomb_place;
        public FrameLayout[,] game_Buttons;
        public game_button game_Button;
        information_container information_Container;
        public game_area(Context context, int a_side,information_container information_Container)//elmentettem a context és az oldal hosszat, hogy ne kelljen minidg paraméterként megadni
        {
            parent_context = context;
            this.a_side = a_side;
            gomb_place = new int[a_side, a_side];
            game_Button = new game_button(parent_context,0,information_Container,a_side);
            this.information_Container = information_Container;
        }
        public TableLayout generate_game_field()//Ez adja vissza a kezdetleges pályát
        {
            game_area_tableLayout = new TableLayout(parent_context);
            game_area_tableLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            game_area_tableLayout.SetGravity(GravityFlags.Center);
            tableRows_set(a_side, game_area_tableLayout);
            game_area_load();
            return game_area_tableLayout;
        }
        private void tableRows_set(int row_count, TableLayout game_field)//beállitm egy sor paraméterit a legalapabb dolgokat
        {
            tableRows = new TableRow[row_count];
            for (int i = 0; i < row_count; i++)
            {
                tableRows[i] = new TableRow(parent_context);
               
                tableRows[i].LayoutParameters = new TableLayout.LayoutParams(TableLayout.LayoutParams.MatchParent, TableLayout.LayoutParams.WrapContent);
                game_field.AddView(tableRows[i]);
            }
        }
        public void game_area_load()//itt csak feltöltöm a sorokat a táblázatban üres gombokkal, hogy meg legyen az alakja a táblázatnak(így illeszkedni fog a telefon nagyságához)
        {
            game_Buttons = new FrameLayout[a_side, a_side];
            for (int i = 0; i < game_area_tableLayout.ChildCount; i++)
            {
                TableRow tableRow = (TableRow)game_area_tableLayout.GetChildAt(i);
                for (int j = 0; j < a_side; j++)
                {
                    game_button game_Button = new game_button(parent_context, 0,information_Container,a_side);
                    tableRow.AddView(game_Button.Get_game_Button(), j);
                    tableRow.GetChildAt(j).SetBackgroundColor(Color.Gray);
                    game_Buttons[i, j] = (FrameLayout)tableRow.GetChildAt(j);
                }
            }
        }
        public void add_new_number(int number)//itt lehet majd újat hozzáadni a tablelayout-hoz úgyhogy a megfelelő sort és oszlop számot megadjuk
        {
            int random_row;
            int random_coloumn;
            if (is_place_for_btn())
            {
                Random random = new Random();
                do
                {
                    random_row = random.Next(0, a_side);
                    random_coloumn = random.Next(0, a_side);
                } while (check_gameare_place(random_row, random_coloumn));
                TableRow tableRow = (TableRow)game_area_tableLayout.GetChildAt(random_row);
                //game_button game_Button = new game_button(parent_context, random_coloumn,textView);
                game_Button.set_btn_number(game_Buttons[random_row, random_coloumn], number.ToString());  //game_Buttons[random_row, random_coloumn].Text = number.ToString();
                game_Buttons[random_row, random_coloumn].SetBackgroundColor(Color.White);
                gomb_place[random_row, random_coloumn] = 1;//van ott már egy szám
                information_Container.add_point();
            }
            else
            {
                New_game();
            }
        }
        public void big_move(string destination,bool frame_checker_need)
        {
            switch (destination)
            {
                case "left":
                    left_right_move("left",frame_checker_need);
                    break;
                case "right":
                    left_right_move("right",frame_checker_need);
                    break;
                case "up":
                    up_down_move("up",frame_checker_need);
                    break;
                case "down":
                    up_down_move("down",frame_checker_need);
                    break;

            }
        }
        array_indexes[] array_Indexes;
        bool was_change = false;
        public void left_right_move(string right_or_left, bool frame_checker)
        {
            was_change = false;
            for (int i = 0; i < gomb_place.GetLength(0); i++)
            {
                for (int proba = 0; proba < gomb_place.GetLength(0) - 1; proba++)
                {
                    if (right_or_left == "left")
                    {
                        for (int j = 1; j < gomb_place.GetLength(1); j++)
                        {
                            if (gomb_place[i, j - 1] == 0 && gomb_place[i, j] == 1)
                            {
                                change_buttons(i, j - 1, i, j);
                                was_change = true;
                            }
                        }
                    }
                    else if (right_or_left == "right")
                    {
                        for (int j = gomb_place.GetLength(1) - 1; j >= 1; j--)
                        {
                            if (gomb_place[i, j] == 0 && gomb_place[i, j-1] == 1)
                            {
                                change_buttons(i, j, i, j-1);
                                was_change = true;
                            }
                        }
                    }
                }
            }
            if (frame_checker) {
                array_Indexes = new array_indexes[a_side * a_side];
                int array_of_index = 0;
                for (int i = 0; i < gomb_place.GetLength(0); i++)
                {
                    if (right_or_left == "left")
                    {
                        for (int j = 1; j < gomb_place.GetLength(1); j++)
                        {
                            if ((gomb_place[i, j - 1] == 1 && gomb_place[i, j] == 1) && (int.Parse(game_Button.get_btn_value(game_Buttons[i, j - 1])) == int.Parse(game_Button.get_btn_value(game_Buttons[i, j]))))
                            {
                                array_indexes array_ = new array_indexes();
                                array_.to_x = i;
                                array_.to_y = j - 1;
                                array_.from_x = i;
                                array_.from_y = j;
                                array_.direction = "left";
                                array_Indexes[array_of_index] = array_;
                                array_of_index++;
                                j++;
                            }
                        }
                    }
                    else if (right_or_left == "right")
                    {
                        for (int j = gomb_place.GetLength(1) - 1; j >= 1; j--)
                        {
                            if (gomb_place[i, j] == 1 && gomb_place[i, j - 1] == 1&& (int.Parse(game_Button.get_btn_value(game_Buttons[i, j - 1])) == int.Parse(game_Button.get_btn_value(game_Buttons[i, j]))))
                            {
                                array_indexes array_ = new array_indexes();
                                array_.to_x = i;
                                array_.to_y = j;
                                array_.from_x = i;
                                array_.from_y = j - 1;
                                array_.direction = "right";
                                array_Indexes[array_of_index] = array_;
                                array_of_index++;
                                j--;
                            }
                        }

                    }
                           
                    
                }
                if (was_change || array_of_index > 0)
                {
                    add_new_number(2);
                }
                frame_fushion(array_Indexes, array_of_index);
            }
        }
        private void frame_fushion(array_indexes[] array_Indexes,int length)//row checker nem müködik
        {
            for (int i = 0; i < length; i++)
            {
                if (array_Indexes[i].direction == "left")
                {
                    number_fusion(array_Indexes[i].to_x, array_Indexes[i].to_y, array_Indexes[i].from_x, array_Indexes[i].from_y);
                }
                else if (array_Indexes[i].direction == "right")
                {
                    number_fusion(array_Indexes[i].to_x, array_Indexes[i].to_y, array_Indexes[i].from_x, array_Indexes[i].from_y);
                }
                else if (array_Indexes[i].direction == "up")
                {
                    number_fusion(array_Indexes[i].to_x, array_Indexes[i].to_y, array_Indexes[i].from_x, array_Indexes[i].from_y);
                }
                else if (array_Indexes[i].direction == "down")
                {
                    number_fusion(array_Indexes[i].to_x, array_Indexes[i].to_y, array_Indexes[i].from_x, array_Indexes[i].from_y);
                }
            }
            if (length > 0)
            {
                big_move(array_Indexes[0].direction,false);
            }
        }
        public void up_down_move(string up_or_down,bool fushion_checker)
        {
            was_change = false;
            int sor;
            for (int i = 0; i < gomb_place.GetLength(0); i++)
            {
                for (int proba = 0; proba < gomb_place.GetLength(0) - 1; proba++)
                {
                    if (up_or_down == "up")
                    {
                        sor = 1;
                        while (sor < gomb_place.GetLength(1))
                        {
                            if (gomb_place[sor, i] == 1 && gomb_place[sor - 1, i] == 0)
                            {
                                change_buttons(sor - 1, i, sor, i);
                                was_change = true;
                            }
                            sor++;
                        }
                    }
                    else if (up_or_down == "down")
                    {
                        sor = gomb_place.GetLength(0)-1;
                        while (sor > 0)
                        {

                            if (gomb_place[sor, i] == 0 && gomb_place[sor-1, i] == 1)
                            {
                                change_buttons(sor, i, sor - 1, i);
                                was_change = true;

                            }
                            sor--;
                        }
                    }
                        
                }
            }
            if (fushion_checker)
            {
                array_Indexes = new array_indexes[a_side * a_side];
                int array_of_index = 0;
                for (int i = 0; i < gomb_place.GetLength(0); i++)
                {
                    if (up_or_down == "up")
                    {
                        sor = 1;
                        while (sor < gomb_place.GetLength(1))
                        {
                            if ((gomb_place[sor, i] == 1 && gomb_place[sor - 1, i] == 1) && int.Parse(game_Button.get_btn_value(game_Buttons[sor, i])) == int.Parse(game_Button.get_btn_value(game_Buttons[sor - 1, i])))
                            {
                                array_indexes array_ = new array_indexes();
                                array_.to_x = sor - 1;
                                array_.to_y = i;
                                array_.from_x = sor;
                                array_.from_y = i;
                                array_.direction = "up";
                                array_Indexes[array_of_index] = array_;
                                array_of_index++;
                                sor++;
                            }
                            sor++;
                        }
                    }
                    else if (up_or_down == "down")
                    {
                        sor = gomb_place.GetLength(0) - 1;
                        while (sor > 0)
                        {
                            if ((gomb_place[sor, i] == 1 && gomb_place[sor - 1, i] == 1) && int.Parse(game_Button.get_btn_value(game_Buttons[sor, i])) == int.Parse(game_Button.get_btn_value(game_Buttons[sor - 1, i])))
                            {
                                array_indexes array_ = new array_indexes();
                                array_.to_x = sor;
                                array_.to_y = i;
                                array_.from_x = sor - 1;
                                array_.from_y = i;
                                array_.direction = "down";
                                array_Indexes[array_of_index] = array_;
                                array_of_index++;
                                sor--;
                            }
                            sor--;
                        }
                    }
                }
                        
                if (was_change || array_of_index > 0)
                {
                    add_new_number(2);
                }
                frame_fushion(array_Indexes, array_of_index);
            }
        }
        public void change_buttons(int to_number_x,int to_number_y,int to_normal_x,int to_normal_y)
        {
            gomb_place[to_number_x, to_number_y] = 1;
            gomb_place[to_normal_x, to_normal_y] = 0;
            string number = game_Button.get_btn_value(game_Buttons[to_normal_x, to_normal_y]);
            game_Button.numbered_btn(game_Buttons[to_number_x, to_number_y], int.Parse(number));
            game_Button.normal_btn(game_Buttons[to_normal_x, to_normal_y]);
        }
        public void number_fusion(int new_place_X,int new_place_Y,int empty_place_X,int empty_place_Y) 
        {
            gomb_place[new_place_X, new_place_Y] = 1;
            gomb_place[empty_place_X, empty_place_Y] = 0;
            string numbert_one_string = game_Button.get_btn_value(game_Buttons[new_place_X, new_place_Y]);
            string numbert_two_String = game_Button.get_btn_value(game_Buttons[empty_place_X, empty_place_Y]);
            int number_one = int.Parse(game_Button.get_btn_value(game_Buttons[new_place_X, new_place_Y]));
            int number_two = int.Parse(game_Button.get_btn_value(game_Buttons[empty_place_X, empty_place_Y]));//itt huncotkodik a kód
            int number = int.Parse(game_Button.get_btn_value(game_Buttons[new_place_X, new_place_Y])) + int.Parse(game_Button.get_btn_value(game_Buttons[empty_place_X, empty_place_Y]));
            game_Button.numbered_btn(game_Buttons[new_place_X, new_place_Y], number);
            game_Button.normal_btn(game_Buttons[empty_place_X, empty_place_Y]);
        }
        private bool check_gameare_place(int x,int y)
        {
            if (gomb_place[x, y] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool is_place_for_btn()
        {
            for (int i = 0; i < gomb_place.GetLength(0); i++)//place checker
            {
                for (int j = 0; j < gomb_place.GetLength(1); j++)
                {
                    if (gomb_place[i, j] != 1)
                    {
                        return true;
                    }
                }
            }
            for (int index = 0; index < gomb_place.GetLength(0); index++)//up or down fushion checker
            {
                int sor = 1;
                while (sor < gomb_place.GetLength(1))
                {
                    if ((gomb_place[sor, index] == 1 && gomb_place[sor - 1, index] == 1) && int.Parse(game_Button.get_btn_value(game_Buttons[sor, index])) == int.Parse(game_Button.get_btn_value(game_Buttons[sor - 1, index])))
                    {
                        return true;
                    }
                    sor++;
                }
            }
            for (int l = 0; l < gomb_place.GetLength(0); l++)
            {
                for (int j = 1; j < gomb_place.GetLength(1); j++)
                {
                    if ((gomb_place[l, j - 1] == 1 && gomb_place[l, j] == 1) && (int.Parse(game_Button.get_btn_value(game_Buttons[l, j - 1])) == int.Parse(game_Button.get_btn_value(game_Buttons[l, j]))))
                    {
                        return true;
                    }
                }
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
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(parent_context);
            alertDialog.SetTitle("2048");
            alertDialog.SetMessage("Try again? ");
            alertDialog.SetPositiveButton("Yes", (senderAlert, args) => { reset_game(); });
            Dialog alert = alertDialog.Create();
            alert.Show();
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
        public void reset_game()
        {
            for (int i = 0; i < gomb_place.GetLength(0); i++)
            {
                for (int j = 0; j < gomb_place.GetLength(1); j++)
                {
                    gomb_place[i, j] = 0;
                }
            }
            for (int i = 0; i < game_Buttons.GetLength(0); i++)
            {
                for (int j = 0; j < game_Buttons.GetLength(1); j++)
                {
                    game_Button.normal_btn(game_Buttons[i, j]);
                }
            }
            information_Container.reset_Score();
            add_new_number(2);
            add_new_number(2);
        }
    }
}