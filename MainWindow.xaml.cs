using System.Windows;
using System.IO;
using Newtonsoft.Json;

namespace LotteryTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DrawLots DrawLotsGenerator = new();
    private Dictionary<string, List<string>> result;
    
    public MainWindow()
    {
        InitializeComponent();
        ConfirmResultButton.IsEnabled = false;
        LoadExistsResult(DrawLots.Resultfilepath);
    }
    private void OnDrawLotsGenerated(Dictionary<string, List<string>> groups)
    {
        var output = groups
            .Select(team => $"{team.Key}: {string.Join(", ", team.Value)}")
            .Aggregate((current, next) => $"{current}\n{next}");

        RandomDrawLotsText.Text = output; 
    }
    private void GenerateDrawLotsResult(object sender, RoutedEventArgs e)
    {
        DrawLotsGenerator.GenerateDrawLotsResult();
    }
    private void StopDrawLotsResult(object sender, RoutedEventArgs e)
    {
        DrawLotsGenerator.StopDrawLotsResult();
        ConfirmResultButton.IsEnabled = true;
    }

    private void ConfirmResult(object sender, RoutedEventArgs e)
    {
        if (DrawLotsGenerator.ConfirmResult())
        {
            IsConfirmResult.Text = "结果已保存";
            GenerateDrawLotsResultButton.IsEnabled = false;
            StopDrawLotsResultButton.IsEnabled = false;
        }
    }

    private void DrawLotsConfig(object sender, RoutedEventArgs e)
    {
        ConfigWindow configWindow = new ();
        configWindow.ShowDialog();
        IsConfirmResult.Text = "";
        RandomDrawLotsText.Text = "";
        GenerateDrawLotsResultButton.IsEnabled = true;
        StopDrawLotsResultButton.IsEnabled = true;
        DrawLotsGenerator.DrawLotsGenerated += OnDrawLotsGenerated;
    }
    public void LoadAndDisplayResult(string resultfilepath)
    {
        string json = File.ReadAllText(resultfilepath);
        result = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        DisplayResult();
    }
    private void DisplayResult()
    {
        string displayText = "";
        foreach (var team in result)
        {
            displayText += $"{team.Key}: {string.Join(", ", team.Value)}\n";
        }
        RandomDrawLotsText.Text = displayText;
        IsConfirmResult.Text = "结果已保存";
    }
    private void LoadExistsResult(string resultfilepath)
    {
        if (File.Exists(resultfilepath))
        {
            LoadAndDisplayResult(resultfilepath);
            GenerateDrawLotsResultButton.IsEnabled = false;
            StopDrawLotsResultButton.IsEnabled = false;
            ConfirmResultButton.IsEnabled = false;
        }
        else
        {
            DrawLotsGenerator.DrawLotsGenerated += OnDrawLotsGenerated;
        }
    }
}