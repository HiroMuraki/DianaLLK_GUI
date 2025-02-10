using DianaLLK_GUI.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DianaLLK_GUI
{
    public enum ImageType
    {
        Token,
        SKill,
        Background,
    }

    public partial class App : Application
    {
        public static ImageBrush GetImage(string imageName, ImageType imageType)
        {
            try
            {
                var nameRemap = new Dictionary<string, string>
                {
                    ["AVA_SKILL"] = "Skill-1",
                    ["BELLA_SKILL"] = "Skill-2",
                    ["CAROL_SKILL"] = "Skill-3",
                    ["DIANA_SKILL"] = "Skill-4",
                    ["EILEEN_SKILL"] = "Skill-5",
                };
                string remapedName = imageName;
                if (nameRemap.ContainsKey(imageName))
                {
                    remapedName = nameRemap[imageName];
                }

                if (Directory.Exists("Resources"))
                {
                    string replaceImage = Directory.EnumerateFiles("Resources", "*", SearchOption.AllDirectories)
                        .FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == remapedName);
                    if (replaceImage is not null)
                    {
                        BitmapImage imageSource = null;
                        switch (imageType)
                        {
                            case ImageType.Token:
                                imageSource = LoadAndResizeImage(replaceImage, 128, 128);
                                break;
                            case ImageType.SKill:
                                imageSource = LoadAndResizeImage(replaceImage, 256, 128);
                                break;
                            case ImageType.Background:
                                imageSource = LoadAndResizeImage(replaceImage, -1, -1);
                                break;
                        }
                        return new ImageBrush(imageSource);
                    }
                }
            }
            catch
            {
            }

            var imageDict = new ResourceDictionary()
            {
                Source = new Uri("Resources/Images.xaml", UriKind.Relative)
            };
            return imageDict[imageName] as ImageBrush;
        }

        public static SolidColorBrush GetColor(string colorName)
        {
            try
            {
                if (File.Exists("Resources/Colors.json"))
                {
                    var nameMap = new Dictionary<string, string>()
                    {
                        ["ASTheme"] = "Theme-0",
                        ["AvaTheme"] = "Theme-1",
                        ["BellaTheme"] = "Theme-2",
                        ["CarolTheme"] = "Theme-3",
                        ["DianaTheme"] = "Theme-4",
                        ["EileenTheme"] = "Theme-5",
                    };
                    Dictionary<string, string> colors = JsonSerializer.Deserialize<Dictionary<string, string>>(
                       File.ReadAllText("Resources/Colors.json"));
                    if (colors is not null
                        && colors.TryGetValue(nameMap[colorName], out string color)
                        && Regex.IsMatch(color, @"#[0-9a-fA-F]{6}"))
                    {
                        return new SolidColorBrush(Color.FromRgb(
                            Convert.ToByte(color[1..3], 16),
                            Convert.ToByte(color[3..5], 16),
                            Convert.ToByte(color[5..7], 16)));
                    }
                }
            }
            catch (Exception)
            {
            }

            var colorDIct = new ResourceDictionary()
            {
                Source = new Uri("Resources/PresetColors.xaml", UriKind.Relative)
            };
            return colorDIct[colorName] as SolidColorBrush;
        }

        #region NonPublic
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow(new LianLianKan.ASLLKGame());
            ResolveLaunchArguments(e.Args);
            window.Show();
        }
        private void ResolveLaunchArguments(string[] args)
        {
            var gameSound = GameSoundPlayer.GetInstance();
            GameSetter.Instance.RowSize = 7;
            GameSetter.Instance.ColumnSize = 14;
            GameSetter.Instance.TokenAmount = 15;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string currentArg = args[i].ToUpper();
                    if (currentArg == "-ROW")
                    {
                        GameSetter.Instance.RowSize = Convert.ToInt32(args[i + 1]);
                        i += 1;
                    }
                    else if (currentArg == "-COLUMN" || currentArg == "-COL")
                    {
                        GameSetter.Instance.ColumnSize = Convert.ToInt32(args[i + 1]);
                        i += 1;
                    }
                    else if (currentArg == "-TYPES")
                    {
                        GameSetter.Instance.TokenAmount = Convert.ToInt32(args[i + 1]);
                        i += 1;
                    }
                    else if (currentArg == "-SOUND" || currentArg == "SOUNDPACK")
                    {
                        gameSound.GameSoundDirectory = args[i + 1];
                    }
                }
                gameSound.LoadSounds();
            }
            catch
            {
                GameSetter.Instance.RowSize = 7;
                GameSetter.Instance.ColumnSize = 14;
                GameSetter.Instance.TokenAmount = 15;
                MessageBox.Show("启动参数解析错误，使用默认值");
            }
        }
        private static BitmapImage LoadAndResizeImage(string filePath, int width, int height)
        {
            // 创建ImageBrush实例
            // 配置BitmapImage加载设置
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 加载后释放文件锁
            bitmapImage.UriSource = new Uri(filePath, UriKind.Relative);
            if (width != -1 && height != -1)
            {
                bitmapImage.DecodePixelWidth = 128;   // 关键缩放设置
                bitmapImage.DecodePixelHeight = 128;  // 强制保持纵横比时需要只设置其中一个
            }
            bitmapImage.EndInit();

            return bitmapImage;
        }
        #endregion
    }
}
