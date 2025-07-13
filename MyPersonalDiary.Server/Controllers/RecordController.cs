using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Services.record;
using System;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly IRecordsService _recordsService;
        public RecordController(IRecordsService recordsService)
        {
            _recordsService = recordsService;
        }

        [HttpPost()]
        public async Task<ActionResult> CreateRecord(RecordCreateDto dto)
        {
            return await _recordsService.CreateRecord(dto);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<RecordGetDto>>> SearchRecords(string? text, DateTime? from, DateTime? to, int page, int pageSize)
        {
            return await _recordsService.SearchRecords(text, from, to, page, pageSize);
        }

        [HttpGet()]
        public async Task<ActionResult> GetUsersRecords()
        {
            return await _recordsService.GetUsersRecords();
        }

        [HttpGet("{publicId}")]
        public async Task<ActionResult> GetUsersRecord(string publicId)
        {
            return await _recordsService.GetUsersRecord(publicId);
        }

        [HttpDelete("{publicId}")]
        public async Task<ActionResult> DeleteRecord(string publicId)
        {
            return await _recordsService.DeleteRecord(publicId);
        }

        [HttpPut("{publicId}")]
        public async Task<ActionResult> UpdateRecord(string publicId, RecordUpdateDto dto)
        {
            return await _recordsService.UpdateRecord(publicId, dto);
        }
    }
}
