using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace heartBlade5X
{
    internal class text
    {
        public class fontMaker
        {
            public fontMaker() { }
            public Dictionary<string, Texture2D> ParseSpriteSheet(Texture2D spriteSheet, int blockSize)
            {
                var textures = new Dictionary<string, Texture2D>();
                var FinalTextures = new Dictionary<string, Texture2D>();
                int width = spriteSheet.Width;
                int height = spriteSheet.Height; // Assuming a single row
                int dataPerPart = blockSize * blockSize;
                
                for (int x = 0; x < width; x += blockSize)
                {
                    Rectangle sourceRectangle = new Rectangle(x, 0, blockSize, blockSize);
                    Texture2D newTexture = new Texture2D(spriteSheet.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);
                    newTexture.SetData(data);
                    textures.Add($"letter_{x / blockSize}", newTexture);
                }
                Debug.WriteLine(GetRightTransparentEdge(textures["letter_0"]));
                Debug.WriteLine(GetLeftTransparentEdge(textures["letter_0"]));
                foreach (var texture in textures)
                {
                    int leftOffset = GetLeftTransparentEdge(texture.Value);
                    int rightOffset = GetRightTransparentEdge(texture.Value);
                    //float LeftRatio = texture.Value.Width / (float)leftOffset;
                    //float RightRatio = texture.Value.Width / (float)rightOffset;
                    Rectangle source = new Rectangle(leftOffset, 0, (rightOffset - leftOffset), texture.Value.Height);
                    var tempTex = new Texture2D(spriteSheet.GraphicsDevice, source.Width, source.Height);
                    Color[] newData = new Color[source.Width * source.Height];
                    Color[] oldData = new Color[texture.Value.Width * texture.Value.Height];
                    texture.Value.GetData(0, new Rectangle(0, 0, texture.Value.Width, texture.Value.Height), oldData, 0, oldData.Length);
                    for (int y = 0; y < source.Height; y++)
                    {
                        for (int x = 0; x < source.Width; x++)
                        {
                            newData[y * source.Width + x] = oldData[((y * source.Width + x) + leftOffset + (((texture.Value.Width - rightOffset + leftOffset) * y)))];
                            //Debug.WriteLine(y * source.Width + x);
                        }
                    }
                    tempTex.SetData(newData);
                    FinalTextures.Add(texture.Key, tempTex);
                }
                return FinalTextures;
                //return textures;
            }
            public void DrawText(Dictionary<string, Texture2D> Font, string text, SpriteBatch _spriteBatch, Vector2 pos, int spacing, float scale)
            {
                int letterIndex = 0;
                int i = 0;
                int space = 0;
                float divScale = 0;
                //splitting texture 
                if(scale < 1)
                {
                    divScale = 1 / scale;
                    foreach (char c in text)
                    {
                        StringBuilder convertedText = new StringBuilder();
                        int newChar = c - 'a'; // Convert to 0-based index
                        if (newChar >= 0 && newChar < 26) // Check if within alphabet range
                        {
                            convertedText.Append($"letter_{newChar}");
                            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                            _spriteBatch.Draw(Font[convertedText.ToString()], new Rectangle(space + (int)pos.X, (int)pos.Y, (int)(Font[convertedText.ToString()].Width * scale), (int)(Font[convertedText.ToString()].Height * scale)), Color.White);
                            //32 by 32 is half the scale of what the letters are in the font
                            space += (int)((Font[convertedText.ToString()].Width + spacing)/divScale);
                            _spriteBatch.End();
                            letterIndex++;
                        }
                        else
                        {
                            letterIndex++;
                            space += (int)(spacing * 3/divScale);
                        }
                    }
                }
                if (scale > 1)
                {
                    divScale = 1 / scale;
                    foreach (char c in text)
                    {
                        StringBuilder convertedText = new StringBuilder();
                        int newChar = c - 'a'; // Convert to 0-based inde
                        if (newChar >= 0 && newChar < 26) // Check if within alphabet range
                        {
                            convertedText.Append($"letter_{newChar}");
                            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                            _spriteBatch.Draw(Font[convertedText.ToString()], new Rectangle(space + (int)pos.X, (int)pos.Y, (int)(Font[convertedText.ToString()].Width * scale), (int)(Font[convertedText.ToString()].Height * scale)), Color.White);
                            //32 by 32 is half the scale of what the letters are in the font
                            space += (int)((Font[convertedText.ToString()].Width + spacing) * scale);
                            _spriteBatch.End();
                            letterIndex++;
                        }
                        else
                        {
                            letterIndex++;
                            space += spacing * 3;
                        }
                    }
                }

            }

            public void DrawTextEffect(Dictionary<string, Texture2D> Font, string text, SpriteBatch _spriteBatch, Vector2 pos, int size, Effect _effect)
            {
                int[] spacing = new int[text.Length];
                int letterIndex = 0;
                int spacingOffset = 0;
                for (int x = 0; x < text.Length; x++)
                {
                    //first letter checks
                    if (text[x] == 'l')
                    {
                        spacingOffset = (-1 * size) / 4;
                    }
                    //second letter checks
                    if (x > 0)
                    {
                        if (text[x - 1] == 'l')
                        {
                            spacingOffset = (-1 * size) / 2;
                            Debug.WriteLine(text[x]);
                        }

                    }
                    spacing[x] = x * ((2 * size) / 3) + spacingOffset; //change number to change letter spacing \
                    Debug.WriteLine(spacing[x]);
                }
                //splitting texture 
                foreach (char c in text)
                {
                    StringBuilder convertedText = new StringBuilder();
                    int newChar = c - 'a'; // Convert to 0-based index
                    if (newChar >= 0 && newChar < 26) // Check if within alphabet range
                    {
                        convertedText.Append($"letter_{newChar}");
                        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _effect);
                        _spriteBatch.Draw(Font[convertedText.ToString()], new Rectangle(spacing[letterIndex] + (int)pos.X, (int)pos.Y, size, size), Color.White);
                        //32 by 32 is half the scale of what the letters are in the font
                        _spriteBatch.End();
                        letterIndex++;
                    }
                    else
                    {
                        letterIndex++;
                    }
                }
            }

            public int GetLeftTransparentEdge(Texture2D texture)
            {
                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(data);
                for (int x = 0; x < texture.Width; x++)
                {
                    for (int y = 0; y < texture.Height; y++)
                    {
                        if (data[y * texture.Width + x].A == 0)
                        {
                            //Debug.WriteLine("transparent");
                        }
                        else
                        {
                            return x;
                        }
                    }
                }
                return 0;
            }

            public int GetRightTransparentEdge(Texture2D texture)
            {
                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(data);

                for (int x = texture.Width - 1; x >= 0; x--)
                {
                    for (int y = texture.Height - 1; y >= 0; y--)
                    {
                        if (data[y * texture.Width + x].A == 0)
                        {
                            //Debug.WriteLine("transparent");
                        }
                        else
                        {
                            return x;
                        }
                    }
                }

                return 0; // No transparent pixels found, return width - 1
            }

        }
    }
}
