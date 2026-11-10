namespace LiveLearn.Ordering.Application.Services;

public enum CartAddResult
{
    Added,
    AlreadyInCart,
    // The cart already holds CartOptions.MaxItems items.
    CartFull
}
