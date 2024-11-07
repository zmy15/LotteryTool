using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace LotteryTool;

public partial class ConfigWindow : Window
{
    public ConfigWindow()
    {
        InitializeComponent();
        LoadConfig();
    }
    
    private Config config = new();
    private string configfilepath = ".//data//config.json";
    
    private void LoadFile(string filePath)
    {
        // 读取 JSON 文件内容
        string jsonContent = File.ReadAllText(filePath);

        // 使用 Newtonsoft.Json 反序列化
        config = JsonConvert.DeserializeObject<Config>(jsonContent);
    }
    private static void SaveToJson(Config result, string filePath)
    {
        // 序列化为 JSON 格式
        string json = JsonConvert.SerializeObject(result, Formatting.Indented);

        // 保存到文件
        File.WriteAllText(filePath, json);
    }
    private void LoadConfig()
    {
        LoadFile(configfilepath);
        SetCheckBoxes(config.RandomTeams);
        DisplayDictionaryInExistingTextBoxes(config.FixedTeams);
    }
    private void SetCheckBoxes(List<string> randomTeams)
    {
        foreach (var randomTeam in randomTeams)
        {
            switch (randomTeam)
            {
                case "A":
                    A.IsChecked = true;
                    break;
                case "B":
                    B.IsChecked = true;
                    break;
                case "C":
                    C.IsChecked = true;
                    break;
                case "D":
                    D.IsChecked = true;
                    break;
                case "E":
                    E.IsChecked = true;
                    break;
                case "F":
                    F.IsChecked = true;
                    break;
                case "G":
                    G.IsChecked = true;
                    break;
                case "H":
                    H.IsChecked = true;
                    break;
            }
        }
    }
    private void UncheckAllCheckBoxes(DependencyObject parent)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
        
            if (child is CheckBox checkBox)
            {
                checkBox.IsChecked = false;
            }
            else
            {
                // 递归调用，检查子元素中的 CheckBox
                UncheckAllCheckBoxes(child);
            }
        }
    }
    private void LoadCheckBoxes()
    {
        List<string> checkedCheckBoxes = new List<string>();
        foreach (var checkbox in new CheckBox[] { A, B, C, D, E, F, G, H })
        {
            // 检查 CheckBox 是否选中
            if (checkbox.IsChecked == true) // 注意这里的比较
            {
                // 如果选中，将名称添加到列表中
                checkedCheckBoxes.Add(checkbox.Name);
            }
        }
        config.RandomTeams = checkedCheckBoxes;
    }
    private void DisplayDictionaryInExistingTextBoxes(Dictionary<string, List<string>> dictionary)
    {
        TeamNumbers.Text = config.TeamNumbers.ToString();
        foreach (var entry in dictionary)
        {
            // 根据key找到对应的TextBox名称
            string textBoxName = $"{entry.Key}";

            // 使用FindName方法找到TextBox控件
            var textBox = FindName(textBoxName) as TextBox;

            // 如果找到对应的TextBox，设置其Text属性
            if (textBox != null)
            {
                textBox.Text = $"{string.Join(", ", entry.Value)}";
            }
        }
    }
    private void ConfirmConfig(object sender, RoutedEventArgs e)
    {
        LoadCheckBoxes();
        var newTeamNumbers = int.Parse(TeamNumbers.Text);
        config.TeamNumbers = newTeamNumbers;
        foreach (var entry in config.FixedTeams)
        {
            // 根据键生成 TextBox 名称
            string textBoxName = $"{entry.Key}";

            // 使用 FindName 查找对应的 TextBox
            var textBox = FindName(textBoxName) as TextBox;

            if (textBox != null)
            {
                // 获取 TextBox 的内容并解析
                string content = textBox.Text;
                var values = string.IsNullOrEmpty(content)
                    ? new List<string>() // 如果 content 为空，返回一个空列表
                    : content.Split(':').Last().Split(',')
                        .Select(item => item.Trim())
                        .ToList();
                var commonElements = config.RandomTeams.Intersect(values).ToList();
                var uncommonElements = entry.Value.Intersect(values).ToList();
                var toset = entry.Value.Except(uncommonElements).ToList();
                config.RandomTeams = config.RandomTeams.Except(commonElements).ToList();
                config.RandomTeams.AddRange(toset);
                config.FixedTeams[entry.Key] = values;
            }
        }
        UncheckAllCheckBoxes(this);
        SetCheckBoxes(config.RandomTeams);
        LoadCheckBoxes();
        SaveToJson(config, configfilepath);
        Close();
    }
}