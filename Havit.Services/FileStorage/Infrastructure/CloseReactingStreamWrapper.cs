namespace Havit.Services.FileStorage.Infrastructure;

internal class CloseReactingStreamWrapper : Stream
{
	private readonly Stream wrappedStream;
	private readonly Action onCloseAction;

	public CloseReactingStreamWrapper(Stream wrappedStream, Action onCloseAction)
	{
		this.wrappedStream = wrappedStream;
		this.onCloseAction = onCloseAction;
	}

	public override bool CanRead => wrappedStream.CanRead;

	public override bool CanSeek => wrappedStream.CanSeek;

	public override bool CanWrite => wrappedStream.CanWrite;

	public override long Length => wrappedStream.Length;

	public override long Position { get => wrappedStream.Position; set => wrappedStream.Position=value; }

	public override void Flush()
	{
		wrappedStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return wrappedStream.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return wrappedStream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		wrappedStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		wrappedStream.Write(buffer, offset, count);
	}

	public override void Close()
	{
		base.Close();
		onCloseAction();
	}
}
