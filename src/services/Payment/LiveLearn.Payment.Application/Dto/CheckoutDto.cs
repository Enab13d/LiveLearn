namespace LiveLearn.Payment.Application.Dto;


// Url is null until the Checkout Session is attached; the client keeps polling (D16).
public sealed record CheckoutDto(string? Url);
