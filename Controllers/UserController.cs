using Microsoft.AspNetCore.Mvc;
using StudyGroup.Api.Services;

namespace StudyGroup.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
    private readonly UserService _userService;
    public UserController(UserService userService) { _userService = userService; }

    [HttpPost("{id}/score")]
    public IActionResult AddScore(int id) {
        _userService.AddUserScore(id, 10); // 서비스에 일을 시킴
        return Ok("점수 반영 완료");
    }
}