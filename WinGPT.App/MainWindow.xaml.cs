
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using WinGPT.Services.Interface;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using WinGPT.Entities.Configurations;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace WinGPT.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int messageCounter;
    private readonly IOpenApiService openApiService;
    private enum userType { User, AI };
    IList<string> modelList;
    private Dictionary<string, string> configs;

    public MainWindow(IOpenApiService openApiService)
    {
        configs = openApiService.GetConfigurations();
        modelList = openApiService.GetModel().ToArray();
        InitializeComponent();

        Model.ItemsSource = modelList;
        Model.SelectedItem = configs.GetValueOrDefault("Model");
        MaxTokens.Text = configs.GetValueOrDefault("MaxToken");
        OpenAIAPIKey.Password = configs.GetValueOrDefault("OpenAIAPIKey");
        OpenAIOrganizationId.Text = configs.GetValueOrDefault("OpenAIOrgId");

        if (configs.Keys.Count < 4 || true)
        {
            dialog.IsOpen = true;
        }

        messageCounter = 0;
        this.openApiService = openApiService;
    }

    private async void SendMessage_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(messageBox.Text))
        {
            this.AddMessage(userType.User, messageBox.Text);
            messageBox.IsEnabled = false;
            thinkingBar.Visibility = Visibility.Visible;

            //var response = await this.openApiService.GetTextCompletion(messageBox.Text);
            
            var response = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            this.AddMessage(userType.AI, response);

            thinkingBar.Visibility = Visibility.Hidden;
            messageBox.IsEnabled = true;

            messageBox.Text = null;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "https://twitter.com/N0NYYYYY",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void AddMessage(userType user, string message)
    {
        var row = new Card();
        messageHistory.RowDefinitions.Add(new RowDefinition());

        bool isUser = user == userType.User;

        row.Name = $"message_{messageCounter}";
        row.Margin = new Thickness(isUser ? 150 : 0, 10, isUser ? 0 : 150, 10);
        row.Padding = new Thickness(10);
        row.HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        row.MaxHeight = isUser ? 100 : 600;
        var textBlock = new TextBlock()
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
        };
        row.Content = textBlock;
        messageHistory.Children.Add(row);

        Grid.SetRow(row, messageHistory.RowDefinitions.Count - 1);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!EnableSave())
        {
            return;
        }

        var cfg = new AppConfigurations()
        {
            Model = Model.Text,
            MaxToken = MaxTokens.Text,
            OpenAIAPIKey = OpenAIAPIKey.Password,
            OpenAIOrgId = OpenAIOrganizationId.Text
        };

        File.WriteAllText("Configurations.json", JsonConvert.SerializeObject(cfg));
        dialog.IsOpen = false;
    }

    private void messageBox_OnEnter(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            this.SendMessage_Click(sender, new RoutedEventArgs());
        }
    }

    private bool EnableSave()
    {
        if (!string.IsNullOrEmpty(Model.Text) && !string.IsNullOrEmpty(MaxTokens.Text) && !string.IsNullOrEmpty(OpenAIOrganizationId.Text) && !string.IsNullOrEmpty(OpenAIAPIKey.Password))
        {
            return true;
        }
        return false;
    }
}
