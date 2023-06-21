using System.IO;

namespace Havit.Services.FileStorage.Infrastructure;

internal class NonClosingWrappingStream : Stream
{
	private readonly Stream stream;

	public NonClosingWrappingStream(Stream stream)
	{
		this.stream = stream;
	}

	public override void Close()
	{
		// NOOP
	}

	public override bool CanRead
	{
		get { return stream.CanRead; }
	}

	public override bool CanSeek
	{
		get { return stream.CanSeek; }
	}

	public override bool CanWrite
	{
		get { return stream.CanWrite; }
	}

	public override long Length
	{
		get { return stream.Length; }
	}

	public override long Position
	{
		get { return stream.Position; }
		set { stream.Position = value; }
	}

	public override void Flush()
	{
		stream.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return stream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		stream.SetLength(value);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return stream.Read(buffer, offset, count);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		stream.Write(buffer, offset, count);
	}
}
