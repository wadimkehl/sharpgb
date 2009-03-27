using System;
using System.Collections.Generic;
using System.Text;

namespace sharpGB
{
    // This class will handle all drawing related stuff
    public class CVideo
    {
        CMemory Memory;                 // The main emulator memory object
        System.Drawing.Graphics Video;  // The object used for drawing
        System.Drawing.Color[] Shades;  // The 4 shades of the Gameboy
        System.Drawing.Bitmap Buffer;   // This represents the off-screen drawing object
        int[,] Pixels;                  // Temporary pixel buffer


        public CVideo(ref CMemory Mem)
        {
 
            // Create objects
            Memory = Mem;
            Pixels = new int[256,256];
            Buffer = new System.Drawing.Bitmap(256, 256);
            Shades = new System.Drawing.Color[4];

            // Some standard colors to begin with
            //Shades[0] = System.Drawing.ColorTranslator.FromHtml("#0B610B");
            Shades[0] = System.Drawing.ColorTranslator.FromHtml("#000000");
            Shades[1] = System.Drawing.ColorTranslator.FromHtml("#088A08");
            Shades[2] = System.Drawing.ColorTranslator.FromHtml("#04B404");
            Shades[3] = System.Drawing.ColorTranslator.FromHtml("#01DF01");
        }

        // Needs to be called once to know associated object
        public void SetDrawDestination(ref System.Windows.Forms.PictureBox obj)
        {
            Video = obj.CreateGraphics();
        }


        // Draws a frame, keeping in mind all constraints and cases
        unsafe public void DrawFrame()
        {
            // Only draw if LCD is enabled
            if ((Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x80) == 0)
                return;

            // Check what is to be drawn
            if ((Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x01) == 1) DrawBackground();
            if ((Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x20) == 1) DrawWindow();
            if ((Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x02) == 1) DrawObjects();

            // Convert the filled Pixels array to the buffer object and draw it to the screen
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                    Buffer.SetPixel(i, j, Shades[Pixels[i, j]]);

            Video.DrawImage(Buffer, 0, 0);
            

        }

        unsafe public void DrawBackground()
        {
            // Determine Tile Map and Tile Data addresses
            int TileDataAddress = (Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x10) == 0 ? 0x8800 : 0x8000;  
            int TileMapAddress  = (Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x08) == 0 ? 0x9800 : 0x9C00;
            int b1, b2, TileOffset, x=0, y=0, rely=0;


            for (int i = TileMapAddress; i < TileMapAddress + 0x400; i++)   // Run through tile map
            {
                TileOffset = Memory.Data[i];      // Get tile index
                b1 = Memory.Data[TileDataAddress + TileOffset];
                b2 = Memory.Data[TileDataAddress + TileOffset + 1];

                // This is the way the gameboy stores its tile data... not very intuitive
                Pixels[x,y+rely]        = (b1 & 0x80) + (b2 & 0x80) * 2;
                Pixels[x + 1, y + rely] = (b1 & 0x40) + (b2 & 0x40) * 2;
                Pixels[x + 2, y + rely] = (b1 & 0x20) + (b2 & 0x20) * 2;
                Pixels[x + 3, y + rely] = (b1 & 0x10) + (b2 & 0x10) * 2;
                Pixels[x + 4, y + rely] = (b1 & 0x08) + (b2 & 0x08) * 2;
                Pixels[x + 5, y + rely] = (b1 & 0x04) + (b2 & 0x04) * 2;
                Pixels[x + 6, y + rely] = (b1 & 0x02) + (b2 & 0x02) * 2;
                Pixels[x + 7, y + rely] = (b1 & 0x01) + (b2 & 0x01) * 2;

                // Adjusting variables
                rely++;  
                if (rely == 8)   // Check if tile is entirely "drawn" to Pixels
                {
                    rely = 0;
                    x += 8;
                    if (x == 256)   // Check if right border is reached
                    {
                        x = 0;
                        y += 8;
                    }
                }
            }

        }

        unsafe public void DrawWindow()
        {
            
        }

        unsafe public void DrawObjects()
        {

        }
    }
}
