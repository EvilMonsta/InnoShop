namespace InnoShop.Users.Contracts.Requests;


public record ConfirmPasswordResetRequest(Guid UserId, string NewPassword);