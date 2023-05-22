using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace FirstLabKG;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
class Program
{
    public static void Main(string[] args)
    {
        
        var bmpSource = new Bitmap("RedAlley.jpg");
        var bmpTarget = new Bitmap("BluePic.jpg");

        var mathExpSource = GetMathExpectation(bmpSource);
        var mathExpTarget = GetMathExpectation(bmpTarget);
        var varianceSource = GetStDev(bmpSource, mathExpSource);
        var varianceTarget = GetStDev(bmpTarget, mathExpTarget);
        
        Transform(bmpTarget,mathExpSource,mathExpTarget,varianceSource,varianceTarget);

    }


    public static void Transform(Bitmap bmpTarget,double[] mathExpSource, double[] mathExpTarget,double[] varianceSource, double[] varianceTarget)  
    {
        for (int i = 0; i < bmpTarget.Width; i++)
        {
            for (int j = 0; j < bmpTarget.Height; j++)
            {
                var color = bmpTarget.GetPixel(i, j);
                var R = (color.R / 255.0) * (235d/255);
                var G = (color.G / 255.0) * (235d/255);
                var B = (color.B / 255.0) * (235d/255);


                var lms = RgbToLms(R, G, B);
                var lab = LmsToLab(lms[0], lms[1], lms[2]);

                var cNewL = mathExpSource[0] + (lab[0] - mathExpTarget[0]) * (varianceSource[0] / varianceTarget[0]);
                var cNewA = mathExpSource[1] + (lab[1] - mathExpTarget[1]) * (varianceSource[1] / varianceTarget[1]);
                var cNewB = mathExpSource[2] + (lab[2] - mathExpTarget[2]) * (varianceSource[2] / varianceTarget[2]);

                var newLms = LabToLms(cNewL, cNewA, cNewB);
                var newRgb = LmsToRgb(newLms[0], newLms[1], newLms[2]);
                
                bmpTarget.SetPixel(i,j,Color.FromArgb(newRgb[0],newRgb[1],newRgb[2]));
            }
        }
        
        bmpTarget.Save("result.jpg");
    }
    
    public static double[] GetMathExpectation(Bitmap bmp)
    {
        double sumL = 0, sumA = 0, sumB = 0;
        var pixels = bmp.Width * bmp.Height;    
        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {

                var color = bmp.GetPixel(i, j);
                var R = (color.R / 255.0) * (235d/255);
                var G = (color.G / 255.0) * (235d/255);
                var B = (color.B / 255.0) * (235d/255);


                var lms = RgbToLms(R, G, B);
                var lab = LmsToLab(lms[0], lms[1], lms[2]);
    
                sumL += lab[0];
                sumA += lab[1];
                sumB += lab[2];

            }
        }
            
        var eL = sumL / pixels;
        var eA = sumA / pixels;
        var eB = sumB / pixels;

        double[] mathExp = {eL, eA, eB};

        return mathExp;
    }

    public static double[] GetStDev(Bitmap bmp,double[] mathExp) 
    {
        var pixels = bmp.Width * bmp.Height;
        double sumDiffL = 0, sumDiffA = 0, sumDiffB = 0;
        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {

                var color = bmp.GetPixel(i, j);
                var R = (color.R / 255.0) * (235d/255);
                var G = (color.G / 255.0) * (235d/255);
                var B = (color.B / 255.0) * (235d/255);


                var lms = RgbToLms(R, G, B);
                var lab = LmsToLab(lms[0], lms[1], lms[2]);
    
                double diffL = lab[0] - mathExp[0];
                double diffA = lab[1] - mathExp[1];
                double diffB = lab[2] - mathExp[2];
                sumDiffL += diffL * diffL;
                sumDiffA += diffA * diffA;
                sumDiffB += diffB * diffB;
            }
        }
        
        double varianceL = sumDiffL / pixels;
        double varianceA = sumDiffA / pixels;
        double varianceB = sumDiffB / pixels;
        
        var stdDevL = Math.Sqrt(varianceL);
        var stdDevA = Math.Sqrt(varianceA);
        var stdDevB = Math.Sqrt(varianceB);

        double[] variance = {stdDevL, stdDevA, stdDevB};

        return variance;
    }
    
    public static double[] RgbToLms(double R, double G, double B)
    {
        
        double[] RGB = { R, G, B };


        double[] LMS = new double[3];
        for (int i = 0; i < 3; i++)
        {
            double sum = 0;
            for (int j = 0; j < 3; j++)
            {
                sum += MatrixHelper.RgbToLms[i, j] * RGB[j];
            }
            LMS[i] = sum;
        }

        return LMS;
    }
    
    public static int[] LmsToRgb(double L, double M, double S)
    {

        double[] lms = { L,M,S };

        
        var rgb = new double[3];
        for (int i = 0; i < 3; i++)
        {
            double sum = 0;
            for (int j = 0; j < 3; j++)
            {
                sum += MatrixHelper.LmsToRgb[i, j] * lms[j];
            }
            rgb[i] = sum;
        }

        
        int[] result = new int[3];
        for (int i = 0; i < 3; i++)
        {
            result[i] = (int)Math.Round(rgb[i] * 255);
            result[i] = Math.Max(0, result[i]);
            result[i] = Math.Min(255, result[i]);
        }

        return result;
    }
    
    public static double[] LmsToLab(double L, double M, double S)
    {
        
        var minValue = 0.01176;

        
        L = Math.Max(L, minValue);
        M = Math.Max(M, minValue);
        S = Math.Max(S, minValue);
        
        double[] LMS = { Math.Log10(L), Math.Log10(M), Math.Log10(S) };
        var temp = new double[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                double sum = 0;
                for (int k = 0; k < 3; k++)
                {
                    sum += MatrixHelper.LmsToLab1[i, k] * MatrixHelper.LmsToLab2[k,j];
                }
                temp[i, j] = sum;
            }
        }


        double[] LAB = new double[3];
        for (int i = 0; i < 3; i++)
        {
            double sum = 0;
            for (int j = 0; j < 3; j++)
            {
                sum += temp[i, j] * LMS[j];
            }
            LAB[i] = sum;
        }

        return LAB;
    }

    public static double[] LabToLms(double L, double A, double B)
    {
        
        
        double[] LAB = { L,A,B };
        var temp = new double[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                double sum = 0;
                for (int k = 0; k < 3; k++)
                {
                    sum += MatrixHelper.LabToLms2[i, k] * MatrixHelper.LabToLms1[k,j];
                }
                temp[i, j] = sum;
            }
        }
        
        double[] LMS = new double[3];
        for (int i = 0; i < 3; i++)
        {
            double sum = 0;
            for (int j = 0; j < 3; j++)
            {
                sum += temp[i, j] * LAB[j];
            }
            LMS[i] = Math.Pow(10,sum);
        }

        return LMS;
    }
}