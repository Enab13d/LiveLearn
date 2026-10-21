namespace LiveLearn.Assessment.API.Dto.Requests;


public record struct SubmitHomeworkRequestDto(Guid CourseId, string Content);
