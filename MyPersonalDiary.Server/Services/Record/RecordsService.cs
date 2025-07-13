using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Models;
using MyPersonalDiary.Server.Repositories;
using Org.BouncyCastle.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

namespace MyPersonalDiary.Server.Services.record
{
    public class RecordsService : IRecordsService
    {
        private readonly AppDbContext _db;
        private readonly IUserRepository _userRepository;
        private readonly IRecordRepository _recordRepository;
        private readonly IHttpContextAccessor _httpContext;
        private HttpContext? HttpContext => _httpContext.HttpContext;

        private readonly string recordsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "records");
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public RecordsService(AppDbContext db, IConfiguration config, IUserRepository userRepository, IHttpContextAccessor httpContext, IRecordRepository recordRepository) 
        {
            _db = db;
            _userRepository = userRepository;
            _httpContext = httpContext;
            _recordRepository = recordRepository;

            Directory.CreateDirectory(recordsPath);

            _key = Encoding.UTF8.GetBytes(config["Encryption:Key"]!);
            _iv = Encoding.UTF8.GetBytes(config["Encryption:IV"]!);
        }

        private string Encrypt(string text)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(text);
            sw.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        private string Decrypt(string text)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(text));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        public async Task<ActionResult> CreateRecord(RecordCreateDto dto)
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            if (string.IsNullOrEmpty(dto.content) )
            {
                return new BadRequestObjectResult(new { Message = "Content cannot be empty." });
            }

            if (dto.Image != null && !dto.Image.ContentType.StartsWith("image/"))
                return new BadRequestObjectResult(new { Message = "Only images are allowed." });

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));
            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            Record record = new Record
            {
                Content = Encrypt(dto.content.Substring(0, Math.Min(500, dto.content.Length))),
                UserId = user.Id,
            };

            if (dto.Image != null)
            {
                string fileName = record.PublicId + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(recordsPath, fileName);

                if ((int)(dto.Image.Length / 1024 / 1024) >= 10)
                {
                    using var stream = dto.Image.OpenReadStream();
                    using var image = await Image.LoadAsync(stream);

                    await image.SaveAsJpegAsync(filePath, new JpegEncoder
                    {
                        Quality = 50
                    });
                }
                else
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.Image.CopyToAsync(stream);
                }

                record.ImagePath = $"/records/{fileName}";
            }


            _db.Records.Add(record);

            await _db.SaveChangesAsync();

            return new OkObjectResult(new { Message = "Record create" });
        }
        public async Task<ActionResult> GetUsersRecords()
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));
            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            var records = await _recordRepository.GetRecordsUserById(user.Id);

            var dto = records.Select(record => new RecordGetDto
            {
                PublicId = record.PublicId,
                Content = Decrypt(record.Content),
                ImagePath = record.ImagePath,
                CreatedAt = record.CreatedAt,
            }).ToList();

            return new OkObjectResult(dto);
        }
        public async Task<ActionResult> GetUsersRecord(string publicId)
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));
            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            var record = await _recordRepository.GetRecordByPublicId(publicId);

            if (record == null)
                return new NotFoundResult();


            return new OkObjectResult(new RecordGetDto
            {
                PublicId = record.PublicId,
                Content = Decrypt(record.Content),
                CreatedAt = record.CreatedAt,
                ImagePath = record.ImagePath
            });
        }
        public async Task<ActionResult> SearchRecords(string? text, DateTime? from, DateTime? to, int page, int pageSize)
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));
            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            var records = await _recordRepository.GetRecordsUserById(user.Id);

            if (to.HasValue)
            {
                to = to.Value.Date.AddDays(1).AddTicks(-1);
            }

            var filtered = records
                .Where(r =>
                {
                    var decrypted = Decrypt(r.Content);
                    bool isText = string.IsNullOrWhiteSpace(text) || decrypted.Contains(text, StringComparison.OrdinalIgnoreCase);
                    bool isFrom = !from.HasValue || r.CreatedAt >= from.Value;
                    bool isTo = !to.HasValue || r.CreatedAt <= to.Value;

                    return isText && isFrom && isTo;
                }).Select(r => new RecordGetDto
                {
                    PublicId = r.PublicId,
                    Content = Decrypt(r.Content),
                    ImagePath = r.ImagePath,
                    CreatedAt = r.CreatedAt
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

           return new OkObjectResult(filtered);
        }
        public async Task<ActionResult> DeleteRecord(string publicId)
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));
            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            var record = await _recordRepository.GetRecordByPublicId(publicId);

            if (record == null)
                return new NotFoundObjectResult(new { Message = "Record not found." });

            if (record.CreatedAt < DateTime.UtcNow.AddDays(-2))
            {
                return new BadRequestObjectResult(new { Message = "Unable to delete entry. Creation date is more than 2 days old.." });
            }

            var status = await _recordRepository.DeleteRecordByPublicId(publicId);

            if (!status)
                return new BadRequestObjectResult(new { Message = "Record not found." });

            if (!string.IsNullOrEmpty(record.ImagePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", record.ImagePath.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            await _db.SaveChangesAsync();

            return new OkObjectResult(true);
        }
        public async Task<ActionResult> UpdateRecord(string publicId, RecordUpdateDto dto)
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            if (string.IsNullOrEmpty(dto.content))
            {
                return new BadRequestObjectResult(new { Message = "Content cannot be empty." });
            }

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));
            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            var record = await _recordRepository.GetRecordByPublicId(publicId);

            if (record == null)
                return new NotFoundResult();

            record.Content = Encrypt(dto.content.Substring(0, Math.Min(500, dto.content.Length)));

            if (dto.Image != null)
            {
                string fileName = record.PublicId + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(recordsPath, fileName);

                if ((int)(dto.Image.Length / 1024 / 1024) >= 10)
                {
                    using var stream = dto.Image.OpenReadStream();
                    using var image = await Image.LoadAsync(stream);

                    await image.SaveAsJpegAsync(filePath, new JpegEncoder
                    {
                        Quality = 50
                    });
                }
                else
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.Image.CopyToAsync(stream);
                }

                record.ImagePath = $"/records/{fileName}";
            }

            _db.Update(record);

            await _db.SaveChangesAsync();

            return new OkResult();
        }
    }
}
