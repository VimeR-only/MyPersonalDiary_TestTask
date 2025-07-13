using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Repositories
{
    public interface IRecordRepository
    {
        Task<List<Record>> GetRecordsUserById(int userId);
        Task<Record?> GetRecordById(int id);
        Task<Record?> GetRecordByPublicId(string publicId);
        Task<bool> DeleteRecordById(int id);
        Task<bool> DeleteRecordByPublicId(string publicId);
    }
}
