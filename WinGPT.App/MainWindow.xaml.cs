
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using WinGPT.Services.Interface;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using WinGPT.Entities.Configurations;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using System.Collections.Specialized;

namespace WinGPT.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int messageCounter;
    private readonly IOpenApiService openApiService;
    private enum userType { User, AI };

    public MainWindow(IOpenApiService openApiService)
    {
        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var settings = configFile.AppSettings.Settings;

        var c = openApiService.GetConfigurations();
        settings.Clear();
        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        foreach (var kvp in c)
        {
            settings.Add(kvp.Key.ToString(), kvp.Value.ToString());
        }
        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        var appSettings = ConfigurationManager.AppSettings;
        InitializeComponent();

        Model.Text = appSettings["Model"];
        MaxTokens.Text = appSettings["MaxToken"];
        OpenAIAPIKey.Text = appSettings["OpenAIAPIKey"];
        OpenAIOrganizationId.Text = appSettings["OpenAIOrgId"];

        if (settings.AllKeys.Length < 4)
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

            var response = await this.openApiService.GetTextCompletion(messageBox.Text);
            
            //var response = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            this.AddMessage(userType.AI, response);

            thinkingBar.Visibility = Visibility.Hidden;
            messageBox.IsEnabled = true;

            messageBox.Text = null;
        }
    }

    private void Button_Twitter(object sender, RoutedEventArgs e)
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

        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var settings = configFile.AppSettings.Settings;

        var cfg = new AppConfigurations()
        {
            Model = Model.Text,
            MaxToken = MaxTokens.Text,
            OpenAIAPIKey = OpenAIAPIKey.Text,
            OpenAIOrgId = OpenAIOrganizationId.Text
        };

        settings.Clear();
        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        var json = JsonConvert.SerializeObject(cfg);
        var c = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        foreach (var kvp in c)
        {
            settings.Add(kvp.Key.ToString(), kvp.Value.ToString());
        }
        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        File.WriteAllText("Configurations.json", json);
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
        if (!string.IsNullOrEmpty(Model.Text) && !string.IsNullOrEmpty(MaxTokens.Text) && !string.IsNullOrEmpty(OpenAIOrganizationId.Text) && !string.IsNullOrEmpty(OpenAIAPIKey.Text))
        {
            return true;
        }
        return false;
    }

    private void Button_OpenConfigDialog(object sender, RoutedEventArgs e)
    {
        dialog.IsOpen = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (!EnableSave())
        {
            return;
        }
        dialog.IsOpen = false;
    }
}
