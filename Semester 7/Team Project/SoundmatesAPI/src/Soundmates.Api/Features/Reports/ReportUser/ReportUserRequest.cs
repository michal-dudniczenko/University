namespace Soundmates.Api.Features.Reports.ReportUser;

internal sealed record ReportUserRequest(string ReportedUserId, string Reason, string Description);
