using Microsoft.Xna.Framework.Graphics;
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

    public Texture2D Texture { get; private set; }

    private bool _disposed;

    static MouseCursor()
    {
        // MonoGame doesn't expose system cursors directly; create a 1x1 pixel texture as placeholder
        // The actual cursor rendering is handled by CursorTextureManager
        Arrow = new MouseCursor(null);
        IBeam = new MouseCursor(null);
        Wait = new MouseCursor(null);
        Crosshair = new MouseCursor(null);
        WaitArrow = new MouseCursor(null);
        SizeNWSE = new MouseCursor(null);
        SizeNESW = new MouseCursor(null);
        SizeWE = new MouseCursor(null);
        SizeNS = new MouseCursor(null);
        SizeAll = new MouseCursor(null);
        No = new MouseCursor(null);
        Hand = new MouseCursor(null);
    }

    private MouseCursor(Texture2D texture)
    {
        Texture = texture;
    }

    /// <summary>
    /// Creates a mouse cursor from the specified texture.
    /// </summary>
    /// <param name="texture">Texture to use as the cursor image.</param>
    /// <param name="originx">X cordinate of the image that will be used for mouse position.</param>
    /// <param name="originy">Y cordinate of the image that will be used for mouse position.</param>
    public static MouseCursor FromTexture2D(Texture2D texture, int originx, int originy)
    {
        return new MouseCursor(texture);
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
        Texture?.Dispose();
        Texture = null;
    }
}
