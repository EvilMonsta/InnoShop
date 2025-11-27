namespace InnoShop.Users.Application.Users.Commands;
public record ConfirmPasswordResetCommand(Guid UserId, string NewPassword);