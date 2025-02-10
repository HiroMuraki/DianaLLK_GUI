using LianLianKan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace DianaLLK_GUI.ViewModel
{
    public class GameSoundPlayer
    {
        private static GameSoundPlayer _singletonObject;
        private static object _singletonLocker = new object();
        private readonly Random _rnd;
        private readonly List<Uri> _clickFXSounds; // 点击音效
        private readonly List<Uri> _matchedFXSounds; // 连接成功音效
        private readonly List<Uri> _gameCompletedSounds; // 结算音效
        private readonly List<Uri> _gameMusic; // 背景音乐
        private readonly List<Uri> _avaSkillSounds; // 向晚技能音效
        private readonly List<Uri> _bellaSkillSounds; // 贝拉技能音效
        private readonly List<Uri> _carolSkillSounds; // 珈乐技能音效
        private readonly List<Uri> _dianaSkillSounds; // 嘉然技能音效
        private readonly List<Uri> _eileenSkillSounds; // 乃琳技能音效
        private readonly MediaPlayer _clickSoundPlayer; // 点击技能音播放器
        private readonly MediaPlayer _matchedSoundPlayer; // 点击技能音播放器
        private readonly MediaPlayer _musicSoundPlayer; // 背景音乐播放器
        private readonly MediaPlayer _gameCompletedSoundPlayer; // 结算音播放器
        private readonly MediaPlayer _skillActivedSoundPlayer; // 技能启动音播放器

        public string GameSoundDirectory { get; set; }

        private GameSoundPlayer()
        {
            _rnd = new Random();
            GameSoundDirectory = "ASSounds";

            _clickSoundPlayer = new MediaPlayer();
            _matchedSoundPlayer = new MediaPlayer();
            _musicSoundPlayer = new MediaPlayer();
            _musicSoundPlayer.MediaEnded += MusicSoundPlayer_MediaEnded; // 背景音乐重复播放
            _gameCompletedSoundPlayer = new MediaPlayer();
            _skillActivedSoundPlayer = new MediaPlayer();

            _clickFXSounds = new List<Uri>();
            _matchedFXSounds = new List<Uri>();
            _gameCompletedSounds = new List<Uri>();
            _avaSkillSounds = new List<Uri>();
            _bellaSkillSounds = new List<Uri>();
            _carolSkillSounds = new List<Uri>();
            _dianaSkillSounds = new List<Uri>();
            _eileenSkillSounds = new List<Uri>();
            _gameMusic = new List<Uri>();
        }

        public static GameSoundPlayer GetInstance()
        {
            if (_singletonObject == null)
            {
                lock (_singletonLocker)
                {
                    _singletonObject ??= new GameSoundPlayer();
                }
            }
            return _singletonObject;
        }

        public void LoadSounds()
        {
            if (!Directory.Exists(GameSoundDirectory))
            {
                return;
            }
            foreach (var file in Directory.GetFiles(GameSoundDirectory))
            {
                string fileName = Path.GetFileName(file).ToUpper();
                Uri soundUri = new Uri($@"{GameSoundDirectory}\{fileName}", UriKind.Relative);
                // 跳过非MP3
                if (!(Path.GetExtension(fileName) == ".MP3"))
                {
                    continue;
                }
                //
                if (fileName.StartsWith("CLICK"))
                {
                    _clickFXSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("COMPLETED"))
                {
                    _gameCompletedSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("MUSIC"))
                {
                    _gameMusic.Add(soundUri);
                }
                else if (fileName.StartsWith("MATCHED"))
                {
                    _matchedFXSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("#AVA"))
                {
                    _avaSkillSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("#BELLA"))
                {
                    _bellaSkillSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("#CAROL"))
                {
                    _carolSkillSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("#DIANA"))
                {
                    _dianaSkillSounds.Add(soundUri);
                }
                else if (fileName.StartsWith("#EILEEN"))
                {
                    _eileenSkillSounds.Add(soundUri);
                }
            }
        }

        public void PlayClickFXSound()
        {
            RandomPlay(_clickSoundPlayer, _clickFXSounds);
        }

        public void PlayMatchedFXSound()
        {
            RandomPlay(_matchedSoundPlayer, _matchedFXSounds);
        }

        public void PlayMusic()
        {
            RandomPlay(_musicSoundPlayer, _gameMusic);
        }

        public void PlayGameCompletedSound()
        {
            RandomPlay(_gameCompletedSoundPlayer, _gameCompletedSounds);
        }

        public void PlaySkillActivedSound(LLKSkill skill)
        {
            switch (skill)
            {
                case LLKSkill.None:
                    break;
                case LLKSkill.AvaPower:
                    RandomPlay(_skillActivedSoundPlayer, _avaSkillSounds);
                    break;
                case LLKSkill.BellaPower:
                    RandomPlay(_skillActivedSoundPlayer, _bellaSkillSounds);
                    break;
                case LLKSkill.CarolPower:
                    RandomPlay(_skillActivedSoundPlayer, _carolSkillSounds);
                    break;
                case LLKSkill.DianaPower:
                    RandomPlay(_skillActivedSoundPlayer, _dianaSkillSounds);
                    break;
                case LLKSkill.EileenPower:
                    RandomPlay(_skillActivedSoundPlayer, _eileenSkillSounds);
                    break;
                default:
                    break;
            }
            _skillActivedSoundPlayer.Play();
        }

        private void RandomPlay(MediaPlayer player, List<Uri> resources)
        {
            if (resources.Count <= 0)
            {
                return;
            }
            player.Open(resources[_rnd.Next(0, resources.Count)]);
            player.Play();
        }

        private void MusicSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            PlayMusic();
        }
    }
}
