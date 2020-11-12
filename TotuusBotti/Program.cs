using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TotuusBotti
{
    class Program
    {
        static ITelegramBotClient botClient;
        static ManualResetEvent quitEvent = new ManualResetEvent(false);

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

            // Keep program alive until STOP-signal
            quitEvent.WaitOne();

            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                await botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: "You said:\n" + e.Message.Text
                );
            }
        }
    }
}