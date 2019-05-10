using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DayVsNight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GaugeControl : ContentView
    {
        public GaugeControl()
        {
            InitializeComponent();
            Percent = 50;
        }

        public double Percent
        {
            get => percent;
            set
            {
                percent = value;
                TempGaugeCanvas.InvalidateSurface();
            }
        }

        SKPath clipPath = SKPath.ParseSvgPathData("M.021 28.481a25.933 25.933 0 0 0 8.824-2.112 27.72 27.72 0 0 0 7.391-5.581l19.08-17.045S39.879.5 44.516.5s9.352 3.243 9.352 3.243l20.74 18.628a30.266 30.266 0 0 0 4.525 3.545c3.318 2.263 11.011 2.564 11.011 2.564z");

        SKPaint redBrush = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.Red.ToSKColor(),
            StrokeWidth = 3
        };



        // background brush
        SKPaint backgroundBrush = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = Color.Red.ToSKColor()
        };
        private double percent;

        const float bottomPadding = 100f;

        private void TempGaugeCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;



            canvas.Clear();

            // get density
            float density = info.Size.Width / (float)this.Width;

            var scaledClipPath = new SKPath(clipPath);

            scaledClipPath.Transform(SKMatrix.MakeScale(density, density));
            scaledClipPath.GetTightBounds(out var tightBounds);

            // position it
            var xPos = info.Width * ((float)percent / 100);
            var translateX = (xPos - tightBounds.MidX);
            var translateY = info.Height - (tightBounds.Height + bottomPadding);

            using (new SKAutoCanvasRestore(canvas))
            {
                // apply translations to position the clip path
                canvas.Translate(translateX, translateY);
                canvas.ClipPath(scaledClipPath, SKClipOperation.Difference, true);
                canvas.Translate(-translateX, -translateY);

                // gradient backround
                backgroundBrush.Shader = SKShader.CreateLinearGradient(
                                              new SKPoint(0, 0),
                                              new SKPoint(info.Width, info.Height),
                                              new SKColor[] { Color.FromHex("98C1FF").ToSKColor(), Color.FromHex("FC7C7E").ToSKColor() },
                                              new float[] { 0, 1 },
                                              SKShaderTileMode.Clamp);
                SKRect backgroundBounds = new SKRect(0, 0, info.Width, info.Height - bottomPadding);
                canvas.DrawRoundRect(backgroundBounds, 20, 20, backgroundBrush);

                // draw tick marks
                info = DrawTicks(info, canvas);
                DrawSvgAtPoint(canvas, new SKPoint(100, (float)info.Height - (bottomPadding + 100)), (float)100, "DayVsNight.Images.Snowflake.svg");
                DrawSvgAtPoint(canvas, new SKPoint(info.Width - 100, (float)info.Height - (bottomPadding + 100)), (float)100, "DayVsNight.Images.Heat.svg");

            }
            // draw the thumb
            using (Stream stream = GetType().Assembly.GetManifestResourceStream("DayVsNight.Images.Thumb.png"))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);
                var imageHeight = bitmap.Height * .75;
                var imageWidth = bitmap.Width * .75;

                SKRect thumbRect = new SKRect(0, 0, (float)imageWidth, (float)imageHeight);
                thumbRect.Location = new SKPoint(xPos - ((float)imageWidth / 2), (float)info.Height -(bottomPadding + (float)(imageHeight / 2)));

                //SKPoint location = new SKPoint(xPos - (bitmap.Width / 2), info.Height - bitmap.Height);
                canvas.DrawBitmap(bitmap, thumbRect);
            }

        }

        private static SKImageInfo DrawTicks(SKImageInfo info, SKCanvas canvas)
        {
            var numTicks = 15;
            var distance = info.Width / numTicks;
            var tickHeight = 50;
            for (int i = 0; i < numTicks; i++)
            {
                var start = new SKPoint(i * distance, info.Height - bottomPadding);
                var end = new SKPoint(i * distance, info.Height - (tickHeight + bottomPadding));

                SKPaint tickBrush = new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                };
                tickBrush.Shader = SKShader.CreateLinearGradient(
                                          start,
                                          end,
                                          new SKColor[] { new SKColor(255, 255, 255, 200), new SKColor(255, 255, 255, 0) },
                                          new float[] { 0, 1 },
                                          SKShaderTileMode.Clamp);
                canvas.DrawLine(start, end, tickBrush);

            }

            return info;
        }

        private void DrawSvgAtPoint(SKCanvas canvas, SKPoint location, float size, string svgName)
        {
            // load up the SVG
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(svgName))
            {
                SkiaSharp.Extended.Svg.SKSvg svg = new SkiaSharp.Extended.Svg.SKSvg();
                svg.Load(stream);

                using (new SKAutoCanvasRestore(canvas))
                {
                    SKRect bounds = svg.ViewBox;

                    // work out how big we want it to be
                    float xRatio = size / bounds.Width;
                    float yRatio = size / bounds.Height;
                    float ratio = Math.Min(xRatio, yRatio);

                    canvas.Translate(location.X - bounds.MidX * ratio, location.Y - bounds.MidY * ratio);
                    var matrix = SKMatrix.MakeScale(ratio, ratio);

                    // redner the image
                    canvas.DrawPicture(svg.Picture, ref matrix);
                }
            }
        }

        private void TouchEffect_TouchAction(object sender, TouchEffect.TouchActionEventArgs args)
        {
            Percent = (args.Location.X / TempGaugeCanvas.Width) * 100;
        }
    }
}