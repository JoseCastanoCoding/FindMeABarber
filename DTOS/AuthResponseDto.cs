namespace FindMeABarber.DTOS
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;      // JWT token
        public UserResponseDto User { get; set; } = null!;
    }
}
