using System;

namespace NamaTyping.NicoVideo;

public partial class LiveProgramClient : IDisposable
{
    public event EventHandler<MessageEventArgs> MessageReceived;
    public event EventHandler<EventArgs> ServerConnectionStateChanged;

    public event EventHandler<CommentReceivedEventArgs> CommentReceived;
    public event EventHandler<EventArgs> MessageServerConnectionStateChanged;

    private readonly string _accessToken;
    private readonly string _liveId;
    private readonly string _userId;


    public LiveProgramClient(string accessToken, string liveId, string userId)
    {
        _accessToken = accessToken;
        _liveId = liveId;
        _userId = userId;

        if (!_liveId.StartsWith("lv")) throw new ArgumentException(nameof(liveId));
    }


    public void Dispose()
    {
        _serverSocket?.Dispose();
        _serverCancellationTokenSource?.Dispose();
    }
}