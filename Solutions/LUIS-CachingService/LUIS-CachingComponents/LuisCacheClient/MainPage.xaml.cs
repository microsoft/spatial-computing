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
        static TextProcessor _processor;

        public MainPage()
        {
            this.InitializeComponent();
            // MAKE SURE TO POPULATE THE VALUES BELOW WITH THE VALUES FROM YOUR OWN LUIS AND APP SERVICE
            // DEPLOYMENT IN AZURE.
            // See the instruction on the README page on GitHub for the LUIS Cachine Service.
            var luisSubscriptionKey = "INSERT YOUR LUIS SECRET KEY HERE";
            var luisAppId = "INSERT YOUR LUIS APPLICATION ID HERE";
            var mobileAppUri = "https://INSERT-YOU-APPSERVICE-NAME-HERE.azurewebsites.net";
            var luisregion = "westus";  // Must match the exact region spelling where you deployed your LUIS service

            _processor = new TextProcessor(luisAppId, luisSubscriptionKey, mobileAppUri, luisregion);            
        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            ResultLabel.Text = String.Empty;
            FromCacheLabel.Text = String.Empty;

            var result = await _processor.Predict(txtInput.Text);

            if (result == null)
            {
                ResultLabel.Text = "No results - Error or Offline - "+DateTime.Now.ToString();
                return;
            }

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
