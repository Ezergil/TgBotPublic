using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class ReleaseStatusCommandHandler : CommandHandler
    {
        public ReleaseStatusCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }

        public override string[] PossibleCommands => new[] {"/releasestatus"};
        public override string Usage => string.Empty;
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var releases = await GetUnreleasedVersions();
            var builder = new StringBuilder();
            foreach (var release in releases)
            {
                var details = await GetVersionDetails(release.Id);
                builder.Append($"Release\r\nName {release.Name}\r\nStartDate {release.StartDate.ToShortDateString()}");
                builder.Append($"\r\nEndDate {release.ReleaseDate.ToShortDateString()}\r\nCompleted ");
                var completedPecentage = Math.
                    Round((1 - (double) details.IssuesUnresolvedCount / details.IssuesCount) * 100, 2);
                builder.Append($"{completedPecentage.ToString(CultureInfo.InvariantCulture)}%\r\n\r\n");
            }
            await Client.SendTextMessageAsync(message.Chat.Id, builder.ToString());
        }

        private static async Task<VersionDetails> GetVersionDetails(string versionId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
                    Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("kalinchuk.roman@gmail.com:YlypblRaot82eo5b6LMuA245")));
                var response = await client.GetAsync(
                    $"https://ezdreamteam.atlassian.net/rest/api/latest/version/{versionId}/unresolvedIssueCount");
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<VersionDetails>(content);
            }
        }

        private static async Task<List<JiraReleaseVersion>> GetUnreleasedVersions()
        {
            IEnumerable<JiraReleaseVersion> releases;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
                    Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("kalinchuk.roman@gmail.com:YlypblRaot82eo5b6LMuA245")));
                var response = await client.GetAsync(
                    "https://ezdreamteam.atlassian.net/rest/api/latest/project/BOT/versions/");
                var content = await response.Content.ReadAsStringAsync();
                releases = JsonConvert.DeserializeObject<IEnumerable<JiraReleaseVersion>>(content).
                    Where(v => !v.Released);
            }

            return releases.ToList();
        }
    }

    public class JiraReleaseVersion
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Released { get; set; }
    }

    public class VersionDetails
    {
        public int IssuesCount { get; set; }
        public int IssuesUnresolvedCount { get; set; }
    }
}