﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor {
    public partial class MainWindow : Window {
        private STCM2L stcm2l;

        public MainWindow() {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true) {
                stcm2l = new STCM2L(openFileDialog.FileName);

                if (stcm2l.Load()) {
                    TextsList.ItemsSource = stcm2l.Texts;
                }
                else {
                   Console.WriteLine("Invalid File");
                }

            }
        }

        private void ListBoxItemClick(object sender, MouseButtonEventArgs e) {
            LinesList.DataContext = (sender as ListBoxItem).DataContext;

            Binding binding = new Binding();
            binding.Path = new PropertyPath("Lines");
            binding.Source = (sender as ListBoxItem).DataContext;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            LinesList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private void ResetAllText(object sender, RoutedEventArgs e) {
            if(LinesList.DataContext != null) {
                (LinesList.DataContext as TextEntity).ResetText();
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true) {
                if (!stcm2l.Save(saveFileDialog.FileName)) {
                    Console.WriteLine("Failed to save.");
                }
            }
        }

        private void ResetLine (object sender, RoutedEventArgs e) {
            ((sender as Button).DataContext as Line).Reset();
        }
    }
}
