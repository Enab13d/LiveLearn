using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Domain.Errors;

namespace LiveLearn.Ordering.Application.Commands;


internal sealed class AddToCartCommandHandler(
    ICurrentUser currentUser,
    ICourseProjectionLookup courseLookup,
    IEnrollmentLookup enrollmentLookup,
    IActiveOrderLineLookup activeOrderLineLookup,
    ICartStore cartStore,
    TimeProvider timeProvider) : ICommandHandler<AddToCartCommand>
{
    public async Task<Result> Handle(AddToCartCommand request, CancellationToken ct)
    {
        var studentId = currentUser.UserId;
        Guid[] courseIds = [request.CourseId];

        var courses = await courseLookup.GetByIdsAsync(courseIds, ct);
        var enrolled = await enrollmentLookup.GetEnrolledCourseIdsAsync(studentId, courseIds, ct);
        var activeOrders = await activeOrderLineLookup.GetOrderIdsByCourseAsync(studentId, courseIds, ct);

        var facts = new PurchaseFacts(
            courses.GetValueOrDefault(request.CourseId),
            enrolled.Contains(request.CourseId),
            activeOrders.TryGetValue(request.CourseId, out var orderId) ? orderId : null);

        var availability = PurchaseRules.Evaluate(facts, studentId);
        if (!availability.IsAvailable) return Result.Failure(CartErrors.Unavailable(availability));

        // Available guarantees a positive price. It is kept only to flag price changes later (O3).
        var item = new CartItem(request.CourseId, timeProvider.GetUtcNow(), facts.Course!.Price!.Value);

        // "Already in the cart" and the size limit are checked atomically by the store.
        return await cartStore.AddAsync(studentId, item, ct) switch
        {
            CartAddResult.AlreadyInCart => Result.Failure(CartErrors.AlreadyInCart),
            CartAddResult.CartFull => Result.Failure(CartErrors.CartFull),
            _ => Result.Success()
        };
    }
}
