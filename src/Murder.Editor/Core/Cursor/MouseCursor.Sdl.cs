using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Murder.Utilities;

namespace Murder.Editor.Core;

/// <summary>
/// Describes a mouse cursor.
/// </summary>
public partial class MouseCursor : IDisposable
{
    /// <summary>
    /// Gets the default arrow cursor.
    /// </summary>
    public static MouseCursor Arrow { get; private set; }

    /// <summary>
    /// Gets the cursor that appears when the mouse is over text editing regions.
    /// </summary>
    public static MouseCursor IBeam { get; private set; }

    /// <summary>
    /// Gets the waiting cursor that appears while the application/system is busy.
    /// </summary>
    public static MouseCursor Wait { get; private set; }

    /// <summary>
    /// Gets the crosshair ("+") cursor.
    /// </summary>
    public static MouseCursor Crosshair { get; private set; }

    /// <summary>
    /// Gets the cross between Arrow and Wait cursors.
    /// </summary>
    public static MouseCursor WaitArrow { get; private set; }

    /// <summary>
    /// Gets the northwest/southeast ("\") cursor.
    /// </summary>
    public static MouseCursor SizeNWSE { get; private set; }

    /// <summary>
    /// Gets the northeast/southwest ("/") cursor.
    /// </summary>
    public static MouseCursor SizeNESW { get; private set; }

    /// <summary>
    /// Gets the horizontal west/east ("-") cursor.
    /// </summary>
    public static MouseCursor SizeWE { get; private set; }

    /// <summary>
    /// Gets the vertical north/south ("|") cursor.
    /// </summary>
    public static MouseCursor SizeNS { get; private set; }

    /// <summary>
    /// Gets the size all cursor which points in all directions.
    /// </summary>
    public static MouseCursor SizeAll { get; private set; }

    /// <summary>
    /// Gets the cursor that points that something is invalid, usually a cross.
    /// </summary>
    public static MouseCursor No { get; private set; }

    /// <summary>
    /// Gets the hand cursor, usually used for web links.
    /// </summary>
    public static MouseCursor Hand { get; private set; }

    public IntPtr Handle { get; private set; }

    private bool _disposed;
    internal readonly Microsoft.Xna.Framework.Input.MouseCursor _mgCursor;

    static MouseCursor()
    {
        Arrow = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.Arrow);
        IBeam = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.IBeam);
        Wait = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.Wait);
        Crosshair = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.Crosshair);
        WaitArrow = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.WaitArrow);
        SizeNWSE = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.SizeNWSE);
        SizeNESW = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.SizeNESW);
        SizeWE = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.SizeWE);
        SizeNS = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.SizeNS);
        SizeAll = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.SizeAll);
        No = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.No);
        Hand = new MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor.Hand);
    }

    private MouseCursor(Microsoft.Xna.Framework.Input.MouseCursor mgCursor)
    {
        _mgCursor = mgCursor;
        Handle = mgCursor.Handle;
    }

    private MouseCursor(IntPtr handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Creates a mouse cursor from the specified texture.
    /// </summary>
    /// <param name="texture">Texture to use as the cursor image.</param>
    /// <param name="originx">X cordinate of the image that will be used for mouse position.</param>
    /// <param name="originy">Y cordinate of the image that will be used for mouse position.</param>
    public static MouseCursor FromTexture2D(Texture2D texture, int originx, int originy)
    {
        var mgCursor = Microsoft.Xna.Framework.Input.MouseCursor.FromTexture2D(texture, originx, originy);
        return new MouseCursor(mgCursor);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        PlatformDispose();
        _disposed = true;
    }

    private void PlatformDispose()
    {
        if (_mgCursor != null)
        {
            _mgCursor.Dispose();
        }
        Handle = IntPtr.Zero;
    }
}
