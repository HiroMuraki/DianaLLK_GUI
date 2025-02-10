using DianaLLK_GUI.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DianaLLK_GUI
{
    public partial class App : Application
    {
        public static ImageBrush GetImage(string imageName)
        {
            string replaceImage = Directory.EnumerateFiles("Resources")
                .FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == imageName);
            if (replaceImage is not null)
            {
                var imageSource = new BitmapImage(new Uri(replaceImage, UriKind.Relative));
                return new ImageBrush(imageSource);
            }

            var imageDict = new ResourceDictionary()
            {
                Source = new Uri("Resources/Images.xaml", UriKind.Relative)
            };
            return imageDict[imageName] as ImageBrush;
        }

        public static readonly ResourceDictionary ColorDict = new()
        {
            Source = new Uri("Resources/PresetColors.xaml", UriKind.Relative)
        };

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow(new LianLianKan.ASLLKGame());
            ResolveLaunchArguments(e.Args);
            window.Show();
        }
        private void ResolveLaunchArguments(string[] args)
        {
            var setter = GameSetter.GetInstance();
            var gameSound = GameSoundPlayer.GetInstance();
            setter.RowSize = 7;
            setter.ColumnSize = 14;
            setter.TokenAmount = 15;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string currentArg = args[i].ToUpper();
                    if (currentArg == "-ROW")
                    {
                        setter.RowSize = Convert.ToInt32(args[i + 1]);
                        i += 1;
                    }
                    else if (currentArg == "-COLUMN" || currentArg == "-COL")
                    {
                        setter.ColumnSize = Convert.ToInt32(args[i + 1]);
                        i += 1;
                    }
                    else if (currentArg == "-TYPES")
                    {
                        setter.TokenAmount = Convert.ToInt32(args[i + 1]);
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
                setter.RowSize = 7;
                setter.ColumnSize = 14;
                setter.TokenAmount = 15;
                MessageBox.Show("启动参数解析错误，使用默认值");
            }
        }
    }
}
