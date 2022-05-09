using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class CoronaCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] { "/corona" };
        public override string Usage => "Usage: \r\nCommand /corona provides last stats about pandemic situation in the world and Russia. (It lags behind for about 24 hours, sorry)";


        public CoronaCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://pomber.github.io/covid19/");
                var responseMessage = await client.GetAsync("timeseries.json");
                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                GetRussiaData(responseContent, out var ruToday, out var ruDiff);
                GetWorldData(responseContent, out var worldToday, out var worldDiff);
                var messageText = $"Last data present for {ruToday.Date.Date:dd.MM.yyyy}\r\n\r\n" +
                    "World:\r\n" +
                    $"Confirmed cases: {worldToday.Confirmed} (+{worldDiff.Confirmed})\r\n" +
                    $"Recovered {worldToday.Recovered} (+{worldDiff.Recovered})\r\n" +
                    $"Casualties {worldToday.Deaths} (+{worldDiff.Deaths})\r\n\r\n" +
                    "Russia:\r\n" +
                    $"Confirmed cases: {ruToday.Confirmed} (+{ruDiff.Confirmed})\r\n" +
                    $"Recovered {ruToday.Recovered} (+{ruDiff.Recovered})\r\n" +
                    $"Casualties {ruToday.Deaths} (+{ruDiff.Deaths})\r\n\r\n" +
                    "Havvvvve ▉ nicE DAy.";
                await Client.SendTextMessageAsync(message.Chat.Id, messageText);
                await Client.SendAnimationAsync(message.Chat.Id, new InputOnlineFile("https://media.giphy.com/media/IbmS6XKR5fTVchlxcN/giphy.gif"));
            }
        }

        private void GetWorldData(string responseContent, out DailyCoronaData worldToday, out DailyCoronaData worldDiff)
        {
            var world = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var allRecords = new List<DailyCoronaData>();
            foreach(JProperty country in world)
            {
                var countryRecords = JsonConvert.
                    DeserializeObject<DailyCoronaData[]>(JsonConvert.SerializeObject(country.Value));
                allRecords.AddRange(countryRecords);
            }
            var maxDate = allRecords.Max(r => r.Date);
            var allPrevDayRecords = allRecords.Where(r => r.Date == maxDate.AddDays(-1));
            var allMaxDayRecords = allRecords.Where(r => r.Date == maxDate);
            var aggregatePrevDay = allPrevDayRecords.Aggregate((x, y) => x + y);
            var aggregateMaxDay = allMaxDayRecords.Aggregate((x, y) => x + y);
            worldToday = aggregateMaxDay;
            worldDiff = aggregateMaxDay - aggregatePrevDay;
        }

        private static void GetRussiaData(string responseContent, out DailyCoronaData today, out DailyCoronaData diff)
        {
            var ru = JsonConvert.DeserializeObject<dynamic>(responseContent)["Russia"];
            DailyCoronaData[] deserializedRu = JsonConvert.
                DeserializeObject<DailyCoronaData[]>(JsonConvert.SerializeObject(ru));
            var lastTwoRecords = deserializedRu.OrderByDescending(r => r.Date).Take(2);
            var dailyCoronaData = lastTwoRecords as DailyCoronaData[] ?? lastTwoRecords.ToArray();
            today = dailyCoronaData.First();
            diff = today - dailyCoronaData.Last();
        }
    }

    public class DailyCoronaData
    {
        public DateTime Date { get; set; }
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }

        public static DailyCoronaData operator -(DailyCoronaData a, DailyCoronaData b)
        {
            return new DailyCoronaData
            {
                Confirmed = Math.Abs(a.Confirmed - b.Confirmed),
                Recovered = Math.Abs(a.Recovered - b.Recovered),
                Deaths = Math.Abs(a.Deaths - b.Deaths),
            };
        }

        public static DailyCoronaData operator +(DailyCoronaData a, DailyCoronaData b)
        {
            return new DailyCoronaData
            {
                Confirmed = a.Confirmed + b.Confirmed,
                Recovered = a.Recovered + b.Recovered,
                Deaths = a.Deaths + b.Deaths,
            };
        }
    }
}
