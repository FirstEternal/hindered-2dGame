using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BitmapFont_equalHeight_dynamicWidth
{
    public readonly Texture2D fontTexture;
    private Dictionary<char, Character> characters_italic;
    private Dictionary<char, Character> characters_bold;
    private Dictionary<char, Character> characters_normal;
    public readonly float originalFontSize;
    private int spaceCharWidth = 20; // adjust to your needs

    private static readonly Dictionary<string, Color> NamedColors = new(StringComparer.OrdinalIgnoreCase)
{
    { "Red", Color.Red },
    { "Green", Color.Green },
    { "Blue", Color.Blue },
    { "White", Color.White },
    { "Black", Color.Black },
    { "Yellow", Color.Yellow },
    { "Cyan", Color.Cyan },
    { "Magenta", Color.Magenta },
    { "Gray", Color.Gray },
    { "Orange", new Color(255, 165, 0) },

    // Extended colors
    { "Purple", new Color(128, 0, 128) },
    { "Violet", new Color(238, 130, 238) },
    { "Pink", new Color(255, 192, 203) },
    { "Brown", new Color(139, 69, 19) },
    { "Lime", new Color(0, 255, 0) },
    { "Indigo", new Color(75, 0, 130) },
    { "Gold", new Color(255, 215, 0) },
    { "Silver", new Color(192, 192, 192) },
    { "Beige", new Color(245, 245, 220) },
    { "Olive", new Color(128, 128, 0) },
    { "Teal", new Color(0, 128, 128) },
    { "Navy", new Color(0, 0, 128) },
    { "Coral", new Color(255, 127, 80) },
    { "Salmon", new Color(250, 128, 114) },
    { "Turquoise", new Color(64, 224, 208) },
    { "Chocolate", new Color(210, 105, 30) },
    { "Crimson", new Color(220, 20, 60) },
    { "Lavender", new Color(230, 230, 250) },
    { "Khaki", new Color(240, 230, 140) },
};


    public BitmapFont_equalHeight_dynamicWidth(Texture2D fontTexture, Dictionary<char, Character> characters_normal, Dictionary<char, Character> characters_bold, Dictionary<char, Character> characters_italic, float originalFontSize)
    {
        this.fontTexture = fontTexture;
        this.originalFontSize = originalFontSize != 0 ? originalFontSize : 0.000001f;
        this.characters_normal = characters_normal;
        this.characters_bold = characters_bold;
        this.characters_italic = characters_italic;
    }
    public enum CenterY
    {
        Top,
        Middle,
        Bottom
    }
    public enum CenterX
    {
        Left,
        Middle,
        Right
    }

    public enum FontStyle
    {
        Normal,
        Bold,
        Italic
    }

    /*
    public void Draw(SpriteBatch spriteBatch, Rectangle drawSpace, CenterX centerX, CenterY centerY, FontStyle fontStyle, float fontSize, int spacingX, int spacingY, float rotation, string text, Color defaultColor, Vector2 objectScale)
    {
        if (string.IsNullOrEmpty(text)) return;

        // Parse the text and extract visible characters and their colors
        List<(char character, Color color)> parsedChars = new();
        Color currentColor = defaultColor;

        for (int i = 0; i < text.Length;)
        {
            if (text[i] == '<' && i + 1 < text.Length && text[i + 1] == '/')
            {
                int end = text.IndexOf('>', i + 2);
                if (end != -1)
                {
                    string colorTag = text.Substring(i + 2, end - (i + 2));

                    if (NamedColors.TryGetValue(colorTag, out Color namedColor))
                    {
                        currentColor = namedColor;
                        i = end + 1;
                        continue;
                    }
                }
            }

            parsedChars.Add((text[i], currentColor));
            i++;
        }

        Dictionary<int, Vector2> charIndexPositionsDictionary = new();
        float multiplier = fontSize / originalFontSize;

        Vector2 scale = new Vector2(multiplier, multiplier) * objectScale;

        Vector2 currPosition = new(drawSpace.Left, drawSpace.Top);

        Dictionary<char, Character> charDictionary = fontStyle switch
        {
            FontStyle.Bold => characters_bold,
            FontStyle.Italic => characters_italic,
            _ => characters_normal,
        };

        List<float> lineHeights = new();
        List<int> lineStartIndices = new();
        List<float> lineWidths = new();

        float currentLineHeight = 0;
        float currentLineWidth = 0;
        int currentLineStart = 0;

        for (int i = 0; i < parsedChars.Count; i++)
        {
            char c = parsedChars[i].character;

            if (c == '\n')
            {
                lineHeights.Add(currentLineHeight);
                lineStartIndices.Add(currentLineStart);
                lineWidths.Add(currentLineWidth);

                currPosition.X = drawSpace.Left;
                currPosition.Y += currentLineHeight + spacingY;

                currentLineStart = i + 1;
                currentLineHeight = 0;
                currentLineWidth = 0;
                continue;
            }

            if (charDictionary.TryGetValue(c, out var character))
            {
                int charWidth = (int)(character.sourceRectangle.Width * scale.X);
                int charHeight = (int)(character.sourceRectangle.Height * scale.Y);

                if (currPosition.X + charWidth > drawSpace.Right)
                {
                    lineHeights.Add(currentLineHeight);
                    lineStartIndices.Add(currentLineStart);
                    lineWidths.Add(currentLineWidth);

                    currPosition.X = drawSpace.Left;
                    currPosition.Y += currentLineHeight + spacingY;

                    currentLineStart = i;
                    currentLineHeight = 0;
                    currentLineWidth = 0;
                }

                charIndexPositionsDictionary[i] = currPosition;
                currPosition.X += charWidth + spacingX;
                currentLineHeight = Math.Max(currentLineHeight, charHeight);
                currentLineWidth = currPosition.X - drawSpace.Left;
            }
            else if (c == ' ')
            {
                currPosition.X += spaceCharWidth * scale.X;
                currentLineWidth = currPosition.X - drawSpace.Left;
            }
        }

        if (currentLineHeight > 0)
        {
            lineHeights.Add(currentLineHeight);
            lineStartIndices.Add(currentLineStart);
            lineWidths.Add(currentLineWidth);
        }

        float textHeight = lineHeights.Sum() + (lineHeights.Count - 1) * spacingY;

        float offsetY = centerY switch
        {
            CenterY.Top => 0,
            CenterY.Middle => (drawSpace.Height - textHeight) / 2f,
            CenterY.Bottom => (drawSpace.Height - textHeight),
            _ => 0,
        };

        for (int i = 0; i < parsedChars.Count; i++)
        {
            if (!charIndexPositionsDictionary.ContainsKey(i))
                continue;

            int lineIndex = 0;
            for (int j = 0; j < lineStartIndices.Count; j++)
            {
                if (i >= lineStartIndices[j])
                    lineIndex = j;
                else
                    break;
            }

            float lineOffsetY = lineHeights.Take(lineIndex).Sum() + lineIndex * spacingY;

            if (lineIndex >= 0 && lineIndex < lineWidths.Count)
                currentLineWidth = lineWidths[lineIndex];
            else
                currentLineWidth = 0;

            float lineOffsetX = centerX switch
            {
                CenterX.Left => 0,
                CenterX.Middle => (drawSpace.Width - currentLineWidth) / 2f,
                CenterX.Right => (drawSpace.Width - currentLineWidth),
                _ => 0,
            };

            Vector2 adjustedPosition = charIndexPositionsDictionary[i];
            adjustedPosition.X += lineOffsetX;
            adjustedPosition.Y = drawSpace.Top + lineOffsetY + offsetY;

            charDictionary[parsedChars[i].character].Draw(spriteBatch, parsedChars[i].color, adjustedPosition, scale, rotation);
        }
    }
    public (float width, float height) MeasureText(
        Rectangle drawSpace,
        FontStyle fontStyle,
        float fontSize,
        int spacingX,
        int spacingY,
        string text,
        Vector2 objectScale
    )
    {
        float multiplier = fontSize / originalFontSize;
        Vector2 scale = new Vector2(multiplier, multiplier) * objectScale;

        Dictionary<char, Character> charDictionary = fontStyle switch
        {
            FontStyle.Bold => characters_bold,
            FontStyle.Italic => characters_italic,
            _ => characters_normal,
        };

        List<float> lineHeights = new List<float>();
        List<float> lineWidths = new List<float>();

        float currentLineHeight = 0;
        float currentLineWidth = 0;

        float maxWidth = 0;

        for (int i = 0; i < text.Length; i++)
        {
            // Skip color tags like </Red>
            if (text[i] == '<' && i + 1 < text.Length && text[i + 1] == '/')
            {
                int end = text.IndexOf('>', i + 2);
                if (end != -1)
                {
                    i = end + 1;
                    continue;
                }
            }

            char c = text[i];

            if (c == '\n')
            {
                lineHeights.Add(currentLineHeight);
                lineWidths.Add(currentLineWidth);
                maxWidth = Math.Max(maxWidth, currentLineWidth);

                currentLineHeight = 0;
                currentLineWidth = 0;
                continue;
            }

            if (charDictionary.TryGetValue(c, out var character))
            {
                int charWidth = (int)(character.sourceRectangle.Width * scale.X);
                int charHeight = (int)(character.sourceRectangle.Height * scale.Y);

                if (drawSpace.Width > 0 && currentLineWidth + charWidth > drawSpace.Width)
                {
                    // Wrap line
                    lineHeights.Add(currentLineHeight);
                    lineWidths.Add(currentLineWidth);
                    maxWidth = Math.Max(maxWidth, currentLineWidth);

                    currentLineHeight = 0;
                    currentLineWidth = 0;
                }

                currentLineWidth += charWidth + spacingX;
                currentLineHeight = Math.Max(currentLineHeight, charHeight);
            }
            else if (c == ' ')
            {
                currentLineWidth += spaceCharWidth * scale.X;
            }
        }

        // Add last line
        if (currentLineHeight > 0)
        {
            lineHeights.Add(currentLineHeight);
            lineWidths.Add(currentLineWidth);
            maxWidth = Math.Max(maxWidth, currentLineWidth);
        }

        float totalHeight = lineHeights.Sum() + (lineHeights.Count - 1) * spacingY;
        return (maxWidth, totalHeight);
    }*/

    public void Draw(
    SpriteBatch spriteBatch,
    Rectangle drawSpace,
    CenterX centerX,
    CenterY centerY,
    FontStyle fontStyle,
    float fontSize,
    int spacingX,
    int spacingY,
    float rotation,
    string text,
    Color defaultColor,
    Vector2 objectScale,
    bool cutWordOnly 
)
    {
        if (string.IsNullOrEmpty(text)) return;

        // Parse text into characters with colors
        List<(char character, Color color)> parsedChars = new();
        Color currentColor = defaultColor;

        for (int i = 0; i < text.Length;)
        {
            if (text[i] == '<' && i + 1 < text.Length && text[i + 1] == '/')
            {
                int end = text.IndexOf('>', i + 2);
                if (end != -1)
                {
                    string colorTag = text.Substring(i + 2, end - (i + 2));
                    if (NamedColors.TryGetValue(colorTag, out Color namedColor))
                    {
                        currentColor = namedColor;
                        i = end + 1;
                        continue;
                    }
                }
            }

            parsedChars.Add((text[i], currentColor));
            i++;
        }

        Dictionary<int, Vector2> charIndexPositionsDictionary = new();
        float multiplier = fontSize / originalFontSize;
        Vector2 scale = new Vector2(multiplier, multiplier) * objectScale;
        Vector2 currPosition = new(drawSpace.Left, drawSpace.Top);

        Dictionary<char, Character> charDictionary = fontStyle switch
        {
            FontStyle.Bold => characters_bold,
            FontStyle.Italic => characters_italic,
            _ => characters_normal,
        };

        List<float> lineHeights = new();
        List<int> lineStartIndices = new();
        List<float> lineWidths = new();

        float currentLineHeight = 0;
        float currentLineWidth = 0;
        int currentLineStart = 0;

        int iChar = 0;
        while (iChar < parsedChars.Count)
        {
            char c = parsedChars[iChar].character;

            if (c == '\n')
            {
                lineHeights.Add(currentLineHeight);
                lineStartIndices.Add(currentLineStart);
                lineWidths.Add(currentLineWidth);

                currPosition.X = drawSpace.Left;
                currPosition.Y += currentLineHeight + spacingY;

                currentLineStart = iChar + 1;
                currentLineHeight = 0;
                currentLineWidth = 0;
                iChar++;
                continue;
            }

            if (cutWordOnly && !char.IsWhiteSpace(c))
            {
                // Look ahead to get the entire word
                int wordStart = iChar;
                int wordEnd = iChar;
                float wordWidth = 0;
                float wordHeight = 0;

                while (wordEnd < parsedChars.Count && !char.IsWhiteSpace(parsedChars[wordEnd].character) && parsedChars[wordEnd].character != '\n')
                {
                    char wc = parsedChars[wordEnd].character;
                    if (charDictionary.TryGetValue(wc, out var wChar))
                    {
                        int wWidth = (int)(wChar.sourceRectangle.Width * scale.X);
                        int wHeight = (int)(wChar.sourceRectangle.Height * scale.Y);
                        wordWidth += wWidth + spacingX;
                        wordHeight = Math.Max(wordHeight, wHeight);
                    }
                    else if (wc == ' ')
                    {
                        wordWidth += spaceCharWidth * scale.X;
                    }
                    wordEnd++;
                }

                if (currPosition.X + wordWidth > drawSpace.Right && wordWidth < drawSpace.Width)
                {
                    // Wrap the entire word
                    lineHeights.Add(currentLineHeight);
                    lineStartIndices.Add(currentLineStart);
                    lineWidths.Add(currentLineWidth);

                    currPosition.X = drawSpace.Left;
                    currPosition.Y += currentLineHeight + spacingY;

                    currentLineStart = iChar;
                    currentLineHeight = 0;
                    currentLineWidth = 0;
                }
            }

            // Process normally
            if (charDictionary.TryGetValue(c, out var character))
            {
                int charWidth = (int)(character.sourceRectangle.Width * scale.X);
                int charHeight = (int)(character.sourceRectangle.Height * scale.Y);

                if (!cutWordOnly && currPosition.X + charWidth > drawSpace.Right)
                {
                    // Char-based wrap
                    lineHeights.Add(currentLineHeight);
                    lineStartIndices.Add(currentLineStart);
                    lineWidths.Add(currentLineWidth);

                    currPosition.X = drawSpace.Left;
                    currPosition.Y += currentLineHeight + spacingY;

                    currentLineStart = iChar;
                    currentLineHeight = 0;
                    currentLineWidth = 0;
                }

                charIndexPositionsDictionary[iChar] = currPosition;
                currPosition.X += charWidth + spacingX;
                currentLineHeight = Math.Max(currentLineHeight, charHeight);
                currentLineWidth = currPosition.X - drawSpace.Left;
            }
            else if (c == ' ')
            {
                currPosition.X += spaceCharWidth * scale.X;
                currentLineWidth = currPosition.X - drawSpace.Left;
            }

            iChar++;
        }

        if (currentLineHeight > 0)
        {
            lineHeights.Add(currentLineHeight);
            lineStartIndices.Add(currentLineStart);
            lineWidths.Add(currentLineWidth);
        }

        float textHeight = lineHeights.Sum() + (lineHeights.Count - 1) * spacingY;

        float offsetY = centerY switch
        {
            CenterY.Top => 0,
            CenterY.Middle => (drawSpace.Height - textHeight) / 2f,
            CenterY.Bottom => (drawSpace.Height - textHeight),
            _ => 0,
        };

        for (int i = 0; i < parsedChars.Count; i++)
        {
            if (!charIndexPositionsDictionary.ContainsKey(i))
                continue;

            int lineIndex = 0;
            for (int j = 0; j < lineStartIndices.Count; j++)
            {
                if (i >= lineStartIndices[j])
                    lineIndex = j;
                else
                    break;
            }

            float lineOffsetY = lineHeights.Take(lineIndex).Sum() + lineIndex * spacingY;

            currentLineWidth = (lineIndex >= 0 && lineIndex < lineWidths.Count) ? lineWidths[lineIndex] : 0;

            float lineOffsetX = centerX switch
            {
                CenterX.Left => 0,
                CenterX.Middle => (drawSpace.Width - currentLineWidth) / 2f,
                CenterX.Right => (drawSpace.Width - currentLineWidth),
                _ => 0,
            };

            Vector2 adjustedPosition = charIndexPositionsDictionary[i];
            adjustedPosition.X += lineOffsetX;
            adjustedPosition.Y = drawSpace.Top + lineOffsetY + offsetY;

            charDictionary[parsedChars[i].character].Draw(spriteBatch, parsedChars[i].color, adjustedPosition, scale, rotation);
        }
    }


    public (float width, float height) MeasureText(
        Rectangle drawSpace,
        FontStyle fontStyle,
        float fontSize,
        int spacingX,
        int spacingY,
        string text,
        Vector2 objectScale,
        bool cutWordOnly 
    )
    {
        float multiplier = fontSize / originalFontSize;
        Vector2 scale = new Vector2(multiplier, multiplier) * objectScale;

        Dictionary<char, Character> charDictionary = fontStyle switch
        {
            FontStyle.Bold => characters_bold,
            FontStyle.Italic => characters_italic,
            _ => characters_normal,
        };

        List<float> lineHeights = new();
        List<float> lineWidths = new();

        float currentLineHeight = 0;
        float currentLineWidth = 0;
        float maxWidth = 0;

        int iChar = 0;
        while (iChar < text.Length)
        {
            if (text[iChar] == '<' && iChar + 1 < text.Length && text[iChar + 1] == '/')
            {
                int end = text.IndexOf('>', iChar + 2);
                if (end != -1)
                {
                    iChar = end + 1;
                    continue;
                }
            }

            char c = text[iChar];

            if (c == '\n')
            {
                lineHeights.Add(currentLineHeight);
                lineWidths.Add(currentLineWidth);
                maxWidth = Math.Max(maxWidth, currentLineWidth);

                currentLineHeight = 0;
                currentLineWidth = 0;
                iChar++;
                continue;
            }

            if (cutWordOnly && !char.IsWhiteSpace(c))
            {
                int wordStart = iChar;
                int wordEnd = iChar;
                float wordWidth = 0;
                float wordHeight = 0;

                while (wordEnd < text.Length && !char.IsWhiteSpace(text[wordEnd]) && text[wordEnd] != '\n')
                {
                    char wc = text[wordEnd];
                    if (charDictionary.TryGetValue(wc, out var wChar))
                    {
                        int wWidth = (int)(wChar.sourceRectangle.Width * scale.X);
                        int wHeight = (int)(wChar.sourceRectangle.Height * scale.Y);
                        wordWidth += wWidth + spacingX;
                        wordHeight = Math.Max(wordHeight, wHeight);
                    }
                    else if (wc == ' ')
                    {
                        wordWidth += spaceCharWidth * scale.X;
                    }
                    wordEnd++;
                }

                if (drawSpace.Width > 0 && currentLineWidth + wordWidth > drawSpace.Width && wordWidth < drawSpace.Width)
                {
                    lineHeights.Add(currentLineHeight);
                    lineWidths.Add(currentLineWidth);
                    maxWidth = Math.Max(maxWidth, currentLineWidth);

                    currentLineHeight = 0;
                    currentLineWidth = 0;
                }
            }

            if (charDictionary.TryGetValue(c, out var character))
            {
                int charWidth = (int)(character.sourceRectangle.Width * scale.X);
                int charHeight = (int)(character.sourceRectangle.Height * scale.Y);

                if (!cutWordOnly && drawSpace.Width > 0 && currentLineWidth + charWidth > drawSpace.Width)
                {
                    lineHeights.Add(currentLineHeight);
                    lineWidths.Add(currentLineWidth);
                    maxWidth = Math.Max(maxWidth, currentLineWidth);

                    currentLineHeight = 0;
                    currentLineWidth = 0;
                }

                currentLineWidth += charWidth + spacingX;
                currentLineHeight = Math.Max(currentLineHeight, charHeight);
            }
            else if (c == ' ')
            {
                currentLineWidth += spaceCharWidth * scale.X;
            }

            iChar++;
        }

        if (currentLineHeight > 0)
        {
            lineHeights.Add(currentLineHeight);
            lineWidths.Add(currentLineWidth);
            maxWidth = Math.Max(maxWidth, currentLineWidth);
        }

        float totalHeight = lineHeights.Sum() + (lineHeights.Count - 1) * spacingY;
        return (maxWidth, totalHeight);
    }


    public class Character
    {
        private Texture2D fontTexture;
        public Rectangle sourceRectangle { get; private set; }
        public Vector2 origin { get; private set; }

        public Character(Texture2D fontTexture, Rectangle sourceRectangle, Vector2 origin)
        {
            this.fontTexture = fontTexture;
            this.sourceRectangle = sourceRectangle;
            this.origin = origin;
        }

        public void Draw(SpriteBatch spriteBatch, Color color, Vector2 position, Vector2 scale, float rotation = 0)
        {
            spriteBatch.Draw(
                texture: fontTexture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }
    }
}
