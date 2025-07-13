using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;


namespace MyPersonalDiary.Server.Services.user
{
    public interface IUserService
    {
        User? UserCreate(string userName, string email, string password);
        Task<ActionResult> DeleteUserAsync();
        Task<ActionResult> RestoreUserAsync();
        Task<ActionResult> DeletionStatus();
        Task<ActionResult> CheckExistence();
    }
}
