using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Dto;

namespace LiveLearn.Ordering.Application.Queries;


// The current student's cart with each item re-checked against the O4 rules (O4, O11).
public sealed record GetCartQuery : IQuery<CartDto>;
