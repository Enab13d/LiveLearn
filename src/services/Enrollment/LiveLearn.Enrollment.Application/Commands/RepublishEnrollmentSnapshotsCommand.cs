using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Application.Commands;

// Admin-only: publishes an EnrollmentSnapshotEvent per enrollment so a new consumer can backfill its projection.
// Returns the number of snapshots published.
public sealed record RepublishEnrollmentSnapshotsCommand : ICommand<int>;
