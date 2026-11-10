namespace LiveLearn.Ordering.API.Dto.Requests;


// ExpectedTotal = the cart Total the student confirmed (GET /cart).
public sealed record RequestPlaceOrderDto(decimal ExpectedTotal);
