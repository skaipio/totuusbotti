//#define ENABLE_REDDITBOT

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TotuusBotti
{
    class Program
    {
        static ITelegramBotClient botClient;
        static ManualResetEvent quitEvent = new ManualResetEvent(false);
        static RedditBot redditBot;
        static Random random = new Random();

        static void Main()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                quitEvent.Set();
                eArgs.Cancel = true;
            };

            var apiToken = Environment.GetEnvironmentVariable("TOTUUSBOTTI_API_TOKEN");
            botClient = new TelegramBotClient(apiToken);

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            Console.WriteLine("Press Ctrl+C to exit");

            redditBot = new RedditBot();
#if (ENABLE_REDDITBOT)
            redditBot.Configure();
#endif
            // Keep program alive until STOP-signal
            quitEvent.WaitOne();

            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type != MessageType.Text)
                return;

            Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

            var action = (e.Message.Text.Split(' ').First()) switch
            {
                "/pcmeme" => SendPcMeme(e.Message),
                _ => Usage(e.Message)
            };

            await action;
        }

        static async Task SendPcMeme(Message message)
        {
            var countOfPostsToGet = 10;
            // Skip first because it's a moderator post
            var postIndex = random.Next(1, countOfPostsToGet - 1);
            var posts = await redditBot.GetPoliticalCompassMemes(countOfPostsToGet);
            var post = posts.Skip(postIndex).First();
            var title = post.Title;
            var postUrl = post.Shortlink; // post.Url.AbsoluteUri;
            var imageUrl = post.Url.AbsoluteUri;

            await botClient.SendTextMessageAsync(
              chatId: message.Chat,
              text: postUrl,
              disableWebPagePreview: true
            );

            await botClient.SendTextMessageAsync(
              chatId: message.Chat,
              text: title + "\n" + imageUrl
            );
        }

        static async Task Usage(Message message)
        {
            await botClient.SendTextMessageAsync(
              chatId: message.Chat,
              text: "/pcmeme for a random meme from r/politicalcompassmemes"
            );
        }
    }
}