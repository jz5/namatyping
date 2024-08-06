using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace NamaTyping.NicoVideo;

public class Retriever
{
    private readonly HttpClient _httpClient = new();

    public async IAsyncEnumerable<T> RetrieveAsync<T>(string uri, MessageParser<T> parser) where T : IMessage<T>
    {
        //Console.WriteLine($"Fetching messages... {uri}");
        HttpResponseMessage response;
        try
        {
            response = (await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                .EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Network error while fetching messages: {e.Message}");
            // 通信エラーの再試行などの処理をここに追加
            yield break;
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var unread = new List<byte>();
        var readBuffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length)) > 0)
        {
            unread.AddRange(readBuffer.Take(bytesRead));

            var memoryStream = new MemoryStream(unread.ToArray());
            while (true)
            {
                T message;
                try
                {
                    message = parser.ParseDelimitedFrom(memoryStream);
                }
                catch (InvalidProtocolBufferException)
                {
                    // 途中でちぎれていた場合、次回に読むための処理
                    //unread = memoryStream.ToArray()[(int)memoryStream.Position..(int)memoryStream.Length].ToList();
                    var byteArray = memoryStream.ToArray();
                    var start = (int)memoryStream.Position;
                    var length = (int)(memoryStream.Length - memoryStream.Position);
                    unread = byteArray.Skip(start).Take(length).ToList();
                    break;
                }
                yield return message;
            }
        }

        // 通信が途中で切れた場合に備えて未読データを記録しておく
        if (unread.Count > 0)
        {
            // 必要に応じて、`unread`リストのデータを保存や再試行処理などを行う
            Console.WriteLine($"Unprocessed data length: {unread.Count}");
        }
    }
}