using System.Net.Http.Headers;

namespace Soundmates.IntegrationTests.Common.Media;

/// <summary>Builds multipart/form-data bodies for the file-upload endpoints.</summary>
internal static class MultipartContentHelper
{
    public const string ValidMp3ContentType = "audio/mpeg";
    public const string ValidMp4ContentType = "video/mp4";
    // .jpg and .jpeg files both carry the image/jpeg media type ("image/jpg" is not a real MIME type).
    public const string ValidJpegContentType = "image/jpeg";

    /// <summary>
    /// Builds a single-file multipart body bound to the form field <paramref name="formFieldName"/>
    /// (the endpoints bind <c>[FromForm] IFormFile file</c>).
    /// </summary>
    public static MultipartFormDataContent BuildFileContent(
        byte[] bytes,
        string fileName,
        string contentType,
        string formFieldName = "file")
    {
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var form = new MultipartFormDataContent
        {
            { fileContent, formFieldName, fileName }
        };
        return form;
    }

    /// <summary>A file of an exact size filled with arbitrary bytes.</summary>
    public static MultipartFormDataContent BuildFileOfSize(
        int sizeInBytes, string fileName, string contentType, string formFieldName = "file")
    {
        var bytes = new byte[sizeInBytes];
        // Fill with a non-zero pattern so the payload is not trivially empty.
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(i % 251);
        }

        return BuildFileContent(bytes, fileName, contentType, formFieldName);
    }

    public static MultipartFormDataContent ValidMp3(int sizeInBytes = 1024) =>
        BuildFileOfSize(sizeInBytes, "sample.mp3", ValidMp3ContentType);

    public static MultipartFormDataContent ValidMp4(int sizeInBytes = 1024) =>
        BuildFileOfSize(sizeInBytes, "sample.mp4", ValidMp4ContentType);

    public static MultipartFormDataContent ValidJpeg(int sizeInBytes = 1024) =>
        BuildFileOfSize(sizeInBytes, "picture.jpeg", ValidJpegContentType);

    public static MultipartFormDataContent ValidJpg(int sizeInBytes = 1024) =>
        BuildFileOfSize(sizeInBytes, "picture.jpg", ValidJpegContentType);
}
