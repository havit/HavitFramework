using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Havit.Drawing
{
	/// <summary>
	/// Třída s metodami pro rychlou práci s obrázky.
	/// </summary>
	public static class BitmapExt
	{
		/// <summary>
		/// Metoda rotuje a/nebo překlápí obrázek.
		/// </summary>
		/// <param name="sourceFilename">Zdrojový obrázek.</param>
		/// <param name="destinationFilename">Cílový obrázek.</param>
		/// <param name="rotateFlipType"><see cref="System.Drawing.RotateFlipType"/> určující směr otočení a/nebo překlopení.</param>
		public static void RotateFlip(string sourceFilename, string destinationFilename, RotateFlipType rotateFlipType)
		{
			if (String.Compare(sourceFilename, destinationFilename, true) == 0)
			{
				// pro stejný zdroj a cíl musíme použít modifikovanou verzi
				RotateFlip(sourceFilename, rotateFlipType);
			}
			else
			{
				// pro různé názvy souborů je rychlejší toto
				using (Image image = Image.FromFile(sourceFilename))
				{
					image.RotateFlip(rotateFlipType);
					image.Save(destinationFilename);
				}
			}
		}

		/// <summary>
		/// Metoda rotuje a/nebo překlápí obrázek.
		/// </summary>
		/// <remarks>
		/// Tato varianta implementace obchází nemožnost zápisu do otevřeného obrázku jeho vykopírováním,
		/// uzavřením a uložením. Je proto o malinko pomalejší než vytvoření cílového obrázku do vedlejšího souboru.
		/// </remarks>
		/// <param name="filename">obrázek</param>
		/// <param name="rotateFlipType"><see cref="System.Drawing.RotateFlipType"/> určující směr otočení a/nebo překlopení.</param>
		public static void RotateFlip(string filename, RotateFlipType rotateFlipType)
		{
			Bitmap imageCopy;
			using (Image image = Image.FromFile(filename))
			{
				// zkopírujeme obrázek do nové bitmapy
				imageCopy = new Bitmap(image);
			}
			imageCopy.RotateFlip(rotateFlipType);
			imageCopy.Save(filename);
			imageCopy.Dispose();
		}

		/// <summary>
		/// Změní rozměry obrázku.
		/// </summary>
		/// <remarks>
		/// Jako vedlejší efekt lze změnit i formát obrázku, pokud zvolíme jinou cílovou příponu.
		/// </remarks>
		/// <param name="sourceFilename">zdrojový obrázek (název souboru včetně cesty)</param>
		/// <param name="destinationFilename">cílový obrázek (název souboru včetně cesty)</param>
		/// <param name="width">cílová šířka</param>
		/// <param name="height">cílová výška</param>
		/// <param name="resizeMode"><see cref="ResizeMode"/> možnost určující režim</param>
		/// <param name="quality">kvalita převodu a výsledného obrázku 0-100</param>
		/// <returns><see cref="Size"/> s rozměry výsledného obrázku</returns>
		public static Size Resize(
			string sourceFilename,
			string destinationFilename,
			int width,
			int height,
			ResizeMode resizeMode,
			int quality)
		{
			Bitmap originalBitmap;
			using (Image image = Image.FromFile(sourceFilename))
			{
				// načteme si obrázek do bitmapy, abychom mohli zavřít soubor
				originalBitmap = new Bitmap(image);
			}

			Size destinationSize = new Size(width, height);
			switch (resizeMode)
			{
				case ResizeMode.PreserveAspectRatioFitBox:
					if ((originalBitmap.Width / originalBitmap.Height) >= (width / height))
					{
						destinationSize.Height = (int)Math.Round(originalBitmap.Height * (width * 1.0 / originalBitmap.Width));
					}
					else
					{
						destinationSize.Width = (int)Math.Round(originalBitmap.Width * (height * 1.0 / originalBitmap.Height));
					}
					break;

				case ResizeMode.PreserveAspectRatioFitBoxReduceOnly:
					if ((originalBitmap.Width > width) || (originalBitmap.Height > height))
					{
						if ((originalBitmap.Width * 1.0 / originalBitmap.Height) >= (width * 1.0 / height))
						{
							destinationSize.Height = (int)Math.Round(originalBitmap.Height * (width * 1.0 / originalBitmap.Width));
						}
						else
						{
							destinationSize.Width = (int)Math.Round(originalBitmap.Width * (height * 1.0 / originalBitmap.Height));
						}
					}
					else
					{
						destinationSize.Width = originalBitmap.Width;
						destinationSize.Height = originalBitmap.Height;

						if (String.Compare(Path.GetFullPath(destinationFilename), Path.GetFullPath(sourceFilename), true) == 0)
						{
							// soubor je v pořádku
							return destinationSize;
						}
						else if (String.Compare(Path.GetExtension(sourceFilename), Path.GetExtension(destinationFilename), true) == 0)
						{
							// zrychlující zkratka - nic se nemění, typ souboru stejný, takže jenom zkopírujem
							File.Copy(sourceFilename, destinationFilename);
							return destinationSize;
						}
					}
					break;

				case ResizeMode.AdjustToBox:
					// nastavení vyhovuje
					break;

				default:
					throw new ArgumentException("Nerozpoznaný ResizeMode", "resizeMode");
			}

			using (Bitmap destinationBitmap = new Bitmap(destinationSize.Width, destinationSize.Height))
			{
				using (Graphics destinationBitmapGraphics = Graphics.FromImage(destinationBitmap))
				{
					if (quality >= 75)
					{
						destinationBitmapGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
						destinationBitmapGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
						destinationBitmapGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
						destinationBitmapGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
					}
					
					destinationBitmapGraphics.DrawImage(originalBitmap, 0, 0, destinationSize.Width, destinationSize.Height);
				}

				bool saved = false;
				if ((String.Compare(Path.GetExtension(destinationFilename), ".jpg", true) == 0)
					|| (String.Compare(Path.GetExtension(destinationFilename), ".jpeg", true) == 0))
				{
					ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
					ImageCodecInfo jpegEncoder = null;
					for (int i = 0; i < encoders.Length; i++)
					{
						if (encoders[i].FormatID.Equals(ImageFormat.Jpeg.Guid))
						{
							jpegEncoder = encoders[i];
							break;
						}
					}
					if (jpegEncoder != null)
					{
						EncoderParameters p = new EncoderParameters(1);
						p.Param[0] = new EncoderParameter(Encoder.Quality, quality);
						destinationBitmap.Save(destinationFilename, jpegEncoder, p);
						saved = true;
					}
				}

				if (!saved)
				{
					destinationBitmap.Save(destinationFilename);
				}
			}

			return destinationSize;
		}

		/// <summary>
		/// Změní rozměry obrázku.
		/// </summary>
		/// <remarks>
		/// Jako vedlejší efekt lze změnit i formát obrázku, pokud zvolíme jinou cílovou příponu.
		/// </remarks>
		/// <param name="filename">obrázek (název souboru včetně cesty)</param>
		/// <param name="width">cílová šířka</param>
		/// <param name="height">cílová výška</param>
		/// <param name="resizeMode"><see cref="ResizeMode"/> možnost určující režim</param>
		/// <param name="quality">kvalita převodu a výsledného obrázku 0-100</param>
		/// <returns><see cref="Size"/> s rozměry výsledného obrázku</returns>
		public static Size Resize(string filename, int width, int height, ResizeMode resizeMode, int quality)
		{
			return Resize(filename, filename, width, height, resizeMode, quality);
		}

		/// <summary>
		/// Změní rozměry obrázku.
		/// Pozor, používá default-quality 75.
		/// </summary>
		/// <param name="sourceFilename">zdrojový obrázek (název souboru včetně cesty)</param>
		/// <param name="destinationFilename">cílový obrázek (název souboru včetně cesty)</param>
		/// <param name="width">cílová šířka</param>
		/// <param name="height">cílová výška</param>
		/// <param name="resizeMode"><see cref="ResizeMode"/> možnost určující režim</param>
		/// <returns><see cref="Size"/> s rozměry výsledného obrázku</returns>
		public static Size Resize(string sourceFilename, string destinationFilename, int width, int height, ResizeMode resizeMode)
		{
			return Resize(sourceFilename, destinationFilename, width, height, resizeMode, 75);
		}

		/// <summary>
		/// Změní rozměry obrázku.
		/// Pozor, používá default-quality 75.
		/// </summary>
		/// <param name="filename">obrázek (název souboru včetně cesty)</param>
		/// <param name="width">cílová šířka</param>
		/// <param name="height">cílová výška</param>
		/// <param name="resizeMode"><see cref="ResizeMode"/> možnost určující režim</param>
		/// <returns><see cref="Size"/> s rozměry výsledného obrázku</returns>
		public static Size Resize(string filename, int width, int height, ResizeMode resizeMode)
		{
			return Resize(filename, filename, width, height, resizeMode, 75);
		}
	}
}
