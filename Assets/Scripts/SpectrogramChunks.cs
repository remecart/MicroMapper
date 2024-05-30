using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrogramChunks : MonoBehaviour
{
    public int textureWidth = 512; // Width of the texture (number of time frames)
    public int textureHeight = 256; // Height of the texture (number of frequency bins)

    public Texture2D GenerateSpectrogramTexture(AudioClip clip, float startTime, float duration)
    {
        // Get the sample data from the clip
        int sampleRate = clip.frequency;
        int startSample = Mathf.FloorToInt(startTime * sampleRate);
        int sampleCount = Mathf.FloorToInt(duration * sampleRate);
        float[] samples = new float[sampleCount];
        clip.GetData(samples, startSample);

        // Compute the STFT
        int windowSize = 1024; // Size of the FFT window
        int hopSize = windowSize / 2; // Hop size
        int numWindows = Mathf.Min(textureWidth, (sampleCount - windowSize) / hopSize + 1);
        float[][] spectrogram = new float[numWindows][];

        for (int i = 0; i < numWindows; i++)
        {
            int start = i * hopSize;
            float[] windowedSamples = new float[windowSize];
            Array.Copy(samples, start, windowedSamples, 0, windowSize);

            // Apply a Hamming window
            for (int j = 0; j < windowSize; j++)
            {
                windowedSamples[j] *= HammingWindow(j, windowSize);
            }

            // Perform FFT
            Complex[] spectrum = FFT(windowedSamples);
            float[] magnitudes = new float[spectrum.Length / 2];
            for (int j = 0; j < magnitudes.Length; j++)
            {
                magnitudes[j] = spectrum[j].Magnitude;
            }
            spectrogram[i] = magnitudes;
        }

        // Create the texture
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RFloat, false);

        for (int x = 0; x < numWindows; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                float magnitude = y < spectrogram[x].Length ? spectrogram[x][y] : 0;
                float intensity = Mathf.Log10(magnitude + 1) / 4; // Normalize to a range [0, 1]
                texture.SetPixel(x, y, new Color(intensity, intensity, intensity));
            }
        }

        texture.Apply();
        return texture;
    }

    private Complex[] FFT(float[] data)
    {
        int n = data.Length;
        int m = (int)Mathf.Log(n, 2);
        Complex[] result = new Complex[n];

        for (int i = 0; i < n; i++)
        {
            int j = BitReverse(i, m);
            result[j] = new Complex(data[i], 0);
        }

        for (int s = 1; s <= m; s++)
        {
            int m2 = 1 << s;
            Complex wm = Complex.Exp(-Complex.ImaginaryOne * (2 * Mathf.PI / m2));
            for (int k = 0; k < n; k += m2)
            {
                Complex w = Complex.One;
                for (int j = 0; j < m2 / 2; j++)
                {
                    Complex t = w * result[k + j + m2 / 2];
                    Complex u = result[k + j];
                    result[k + j] = u + t;
                    result[k + j + m2 / 2] = u - t;
                    w *= wm;
                }
            }
        }

        return result;
    }

    private int BitReverse(int x, int bits)
    {
        int y = 0;
        for (int i = 0; i < bits; i++)
        {
            y = (y << 1) | (x & 1);
            x >>= 1;
        }
        return y;
    }

    private float HammingWindow(int n, int N)
    {
        return 0.54f - 0.46f * Mathf.Cos((2 * Mathf.PI * n) / (N - 1));
    }

    public struct Complex
{
    public float Real;
    public float Imaginary;

    public Complex(float real, float imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }

    public float Magnitude
    {
        get { return Mathf.Sqrt(Real * Real + Imaginary * Imaginary); }
    }

    public static Complex Exp(Complex c)
    {
        float expReal = Mathf.Exp(c.Real);
        return new Complex(
            expReal * Mathf.Cos(c.Imaginary),
            expReal * Mathf.Sin(c.Imaginary)
        );
    }

    public static Complex operator +(Complex c1, Complex c2)
    {
        return new Complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
    }

    public static Complex operator -(Complex c1, Complex c2)
    {
        return new Complex(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
    }

    public static Complex operator *(Complex c1, Complex c2)
    {
        return new Complex(
            c1.Real * c2.Real - c1.Imaginary * c2.Imaginary,
            c1.Real * c2.Imaginary + c1.Imaginary * c2.Real
        );
    }

    public static Complex operator *(Complex c1, float f)
    {
        return new Complex(c1.Real * f, c1.Imaginary * f);
    }

    public static Complex operator *(float f, Complex c1)
    {
        return new Complex(c1.Real * f, c1.Imaginary * f);
    }

    public static Complex operator -(Complex c)
    {
        return new Complex(-c.Real, -c.Imaginary);
    }

    public static readonly Complex ImaginaryOne = new Complex(0, 1);
    public static readonly Complex One = new Complex(1, 0);
}

}
