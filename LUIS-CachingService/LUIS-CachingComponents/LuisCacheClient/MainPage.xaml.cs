using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuisCacheLib;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LuisCacheClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TextProcessor _processor;

        public MainPage()
        {
            this.InitializeComponent();

            _processor = TextProcessor.Instance;
        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            ResultLabel.Text = String.Empty;
            FromCacheLabel.Text = String.Empty;

            _processor = TextProcessor.Instance;

            var result = await _processor.Predict(txtInput.Text);

            if (result == null) { return; }

            var resultLabel = result.Intent;

            foreach(var entity in result.Entities)
            {
                resultLabel += ", " + entity;
            }

            ResultLabel.Text = resultLabel;
            
            FromCacheLabel.Text = result.IsFromCache.ToString();
        }
    }
}
