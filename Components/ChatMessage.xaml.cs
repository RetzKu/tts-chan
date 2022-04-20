﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using TTS_Chan.Database;
using TTS_Chan.Twitch;

namespace TTS_Chan.Components
{
    /// <summary>
    /// Interaction logic for ChatMessage.xaml
    /// </summary>
    public partial class ChatMessage
    {
        public string MessageText { get; }
        public string Username { get; }
        public string UserDisplayName { get; }
        public Brush UserColor { get; }
        public TwitchMessage TwitchMessage { get; }
        public ChatMessage(TwitchMessage message)
        {
            DataContext = this;
            MessageText = message.SpeakableText;
            Username = message.Username;
            UserDisplayName = message.DisplayName;
            UserColor = (SolidColorBrush)new BrushConverter().ConvertFrom(message.Color);
            TwitchMessage = message;
            InitializeComponent();
            if (Username == UserDisplayName || UserDisplayName == null)
            {
                UsernameTextBlock.Visibility = Visibility.Hidden;
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var userVoice = DatabaseManager.Context.UserVoices.FirstOrDefault(userVoice => userVoice.UserId == TwitchMessage.Userid);
            if (userVoice == null)
            {
                var foundByUsername = DatabaseManager.Context.UserVoices.FirstOrDefault(voice1 => voice1.UserId == null && voice1.Username == TwitchMessage.Username);
                if (foundByUsername != null)
                {
                    userVoice = foundByUsername;
                    foundByUsername.UserId = TwitchMessage.Userid;
                    DatabaseManager.Context.SaveChanges();
                }
            }

            if (userVoice == null)
            {
                userVoice = new UserVoice()
                {
                    IsMuted = false,
                    Pitch = 0,
                    Rate = 0,
                    UserId = TwitchMessage.Userid,
                    Username = Username
                };
                DatabaseManager.Context.UserVoices.Add(userVoice);
            }

            var voiceWindow = new UserVoiceWindow(userVoice);
            voiceWindow.ShowDialog();
            DatabaseManager.Context.SaveChangesAsync();
        }
    }
}
