﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelManagement.View.Admin.RoomTypeManagement
{
    /// <summary>
    /// Interaction logic for AddRoomType.xaml
    /// </summary>
    public partial class AddRoomType : Window
    {
        public AddRoomType()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text


        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void AddRoomTypeWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Escape) return;

            e.Handled = true;
            this.Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

      
    }
}
