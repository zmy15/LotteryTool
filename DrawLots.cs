using System.Windows.Threading;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace LotteryTool;

public class Config
{
    public List<string> RandomTeams { get; set; }
    public Dictionary<string, List<string>> FixedTeams { get; set; }
    public int TeamNumbers { get; set; }
}
public class DrawLots
{
    private readonly Random _random = new(); 
    private readonly DispatcherTimer _timer = new();
    private Config config = new();
    
    public event Action<Dictionary<string, List<string>>> DrawLotsGenerated;
    
    private string Configfilepath = ".//data//config.json";
    public static string Resultfilepath = ".//data//result.json";
    
    public DrawLots()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(50); // 设置滚动速度
        _timer.Tick += Timer_Tick;
    }
    private void LoadFile(string filePath)
    {
        // 读取 JSON 文件内容
        string jsonContent = File.ReadAllText(filePath);

        // 使用 Newtonsoft.Json 反序列化
        config = JsonConvert.DeserializeObject<Config>(jsonContent);
    }
    private static void SaveToJson(Dictionary<string, List<string>> fixedTeams, string filePath)
    {
        // 序列化为 JSON 格式
        string json = JsonConvert.SerializeObject(fixedTeams, Formatting.Indented);

        // 保存到文件
        File.WriteAllText(filePath, json);
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        LoadFile(Configfilepath);
        var teamNumbers = config.TeamNumbers;
        var counter = 0;
        List<string> randomTeams = config.RandomTeams;
        Dictionary<string, List<string>> fixedTeams = config.FixedTeams;

        foreach (var key in fixedTeams.Keys.ToList())
        {
            var currentTeam = fixedTeams[key];
            int needed = 2 - currentTeam.Count;

            // 从随机团队中抽取所需数量的成员
            var randomResults = randomTeams.OrderBy(x => _random.Next()).Take(needed).ToList();

            // 将抽取的成员添加到固定团队
            fixedTeams[key].AddRange(randomResults);

            // 从随机团队中移除已抽取的成员
            foreach (var item in randomResults)
            {
                randomTeams.Remove(item);
            }
            counter++;
            if (counter == teamNumbers)
            {
                var keysToRemove = fixedTeams.Keys.Skip(teamNumbers).ToList();
                foreach (var keyToRemove in keysToRemove)
                {
                    fixedTeams.Remove(keyToRemove);
                }
                break;
            }
        }
        DrawLotsGenerated?.Invoke(fixedTeams);
    }

    public void GenerateDrawLotsResult()
    {
        _timer.Start();
    }
    
    public void StopDrawLotsResult()
    {
        _timer.Stop();
    }

    public bool ConfirmResult()
    {
        try
        {
            SaveToJson(config.FixedTeams, Resultfilepath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
}