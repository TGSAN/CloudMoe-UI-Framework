using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace CloudMoeUI
{
    public class NoiseEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(NoiseEffect), 0);

        public static readonly DependencyProperty RandomInputProperty =
            RegisterPixelShaderSamplerProperty("RandomInput", typeof(NoiseEffect), 1);

        public static readonly DependencyProperty RatioProperty = DependencyProperty.Register("Ratio", typeof(double),
            typeof(NoiseEffect), new UIPropertyMetadata(0.5d, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty IsLightProperty = DependencyProperty.Register("IsLight", typeof(bool),
            typeof(NoiseEffect), new UIPropertyMetadata(true, PixelShaderConstantCallback(0)));

        public NoiseEffect()
        {
            var pixelShader = new PixelShader { UriSource = new Uri("/CloudMoeUI;component/Noise.ps", UriKind.Relative) };
            PixelShader = pixelShader;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            if (((bool)(GetValue(IsLightProperty))))
            {
                bitmap.UriSource = new Uri("pack://application:,,,/CloudMoeUI;component/Images/noise_light.png");
            }
            else
            {
                bitmap.UriSource = new Uri("pack://application:,,,/CloudMoeUI;component/Images/noise_dark.png");
            }
            bitmap.EndInit();
            RandomInput =
                new ImageBrush(bitmap)
                {
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(0, 0, 300, 300), // 300px*300px
                    ViewportUnits = BrushMappingMode.Absolute
                };

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(RandomInputProperty);
            UpdateShaderValue(RatioProperty);
        }

        public Brush Input
        {
            get => ((Brush)(GetValue(InputProperty)));
            set => SetValue(InputProperty, value);
        }

        /// <summary>The second input texture.</summary>
        public Brush RandomInput
        {
            get => ((Brush)(GetValue(RandomInputProperty)));
            set => SetValue(RandomInputProperty, value);
        }

        public double Ratio
        {
            get => ((double)(GetValue(RatioProperty)));
            set => SetValue(RatioProperty, value);
        }

        public bool IsLight
        {
            get => ((bool)(GetValue(IsLightProperty)));
            set => SetValue(IsLightProperty, value);
        }
    }
}