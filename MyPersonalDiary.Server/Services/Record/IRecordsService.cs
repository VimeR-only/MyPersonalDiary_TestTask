using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.DTOs;

namespace MyPersonalDiary.Server.Services.record
{
    public interface IRecordsService
    {
        Task<ActionResult> CreateRecord(RecordCreateDto dto);
        Task<ActionResult> GetUsersRecords();
        Task<ActionResult> GetUsersRecord(string publicId);
        Task<ActionResult> SearchRecords(string? text, DateTime? from, DateTime? to, int page, int pageSize);
        Task<ActionResult> DeleteRecord(string publicId);
        Task<ActionResult> UpdateRecord(string publicId, RecordUpdateDto dto);
    }
}
