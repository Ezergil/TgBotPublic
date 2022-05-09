using System.Collections.Generic;
using TgBot.Base.DTO;

namespace TgBot.Base.Interfaces
{
    public interface IMentionUsersService
    {
        List<List<UserModel>> GetPartitionedIds(long chatId, string groupName, List<long> idsToExclude);
    }
}