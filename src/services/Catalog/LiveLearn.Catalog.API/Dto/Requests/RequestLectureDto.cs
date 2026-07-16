
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.API.Dto.Requests;

public sealed record RequestLectureDto
(
    string Title,
    LectureType LectureType,
    string Description
);
