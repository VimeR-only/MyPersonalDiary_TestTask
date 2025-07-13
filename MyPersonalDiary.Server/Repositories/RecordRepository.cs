using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly AppDbContext _db;

        public RecordRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Record>> GetRecordsUserById(int userId)
        {
            return await _db.Records.Where(r => r.UserId == userId).ToListAsync();
        }
        public async Task<Record?> GetRecordById(int id)
        {
            return await _db.Records.FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<Record?> GetRecordByPublicId(string publicId)
        {
            return await _db.Records.FirstOrDefaultAsync(r =>r.PublicId == publicId);
        }
        public async Task<bool> DeleteRecordById(int id)
        {
            var record = await GetRecordById(id);

            if (record == null) 
                return false;

            _db.Records.Remove(record);

            return true;
        }

        public async Task<bool> DeleteRecordByPublicId(string publicId)
        {
            var record = await GetRecordByPublicId(publicId);

            if (record == null)
                return false;

            _db.Records.Remove(record);

            return true;
        }
    }
}
