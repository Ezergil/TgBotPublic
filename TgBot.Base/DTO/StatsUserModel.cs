namespace TgBot.Base.DTO
{
    public class StatsUserModel
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public int MessageCount { get; set; }
        public string Prefix { get; set; }
    }
}