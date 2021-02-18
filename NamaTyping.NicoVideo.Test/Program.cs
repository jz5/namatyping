using System;
using System.Threading.Tasks;

namespace NamaTyping.NicoVideo.Test
{
    /// <summary>
    /// NamaTyping.NicoVideo.LiveProgram 動作確認用コンソールアプリ
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // state
            var state = "xxx";

            // Auth.Client.OpenAuthPage(state) でアカウント連携後のテキストを text に設定。
            var text = "***";

            var result = Auth.Client.DecodeResult(text, state);
            var result2 = await Auth.Client.RefreshTokenAsync(result.RefreshToken);

            var liveId = "lv1";
            var client = new LiveProgramClient(result2.AccessToken, liveId, result.UserId);

            var endpoint = await client.GetWebSocketEndpointAsync();

            if (endpoint?.Meta.Status != 200)
            {
                Console.WriteLine(endpoint?.Meta.ErrorCode);
                return;
            }

            client.MessageReceived += async (o, e) =>
            {
                if (e.Message.Type == "room")
                {
                    await client.ConnectMessageServerAsync();
                }
            };

            client.CommentReceived += (o, e) =>
            {
                Console.WriteLine(e.Comment.Content);
            };

            client.ServerConnectionStateChanged += (o, e) =>
            {
                Console.WriteLine($"ServerConnectionStateChanged: {client.ServerSocketState}");
            };

            client.MessageServerConnectionStateChanged += (o, e) =>
            {
                Console.WriteLine($"MessageServerConnectionStateChanged: {client.MessageServerSocketState}");
            };

            await client.StartWatchingAsync();

            Console.ReadLine();
        }

    }
}
