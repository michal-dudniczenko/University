namespace Soundmates.Api.Features.Messages.SendMessage;

internal sealed record SendMessageRequest(string ReceiverId, string Content);
