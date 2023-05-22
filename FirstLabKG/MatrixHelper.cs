namespace FirstLabKG;

public class MatrixHelper
{
    /// <summary>
    /// Матрица для перехода из rgb в lms
    /// </summary>
    public static readonly double[,] RgbToLms =
    {
        {0.3811, 0.5783, 0.0402},
        {0.1967, 0.7244, 0.0782},
        {0.0241, 0.1288, 0.8444}
    };
    
    /// <summary>
    /// Матрица для перехода из lms в rgb
    /// </summary>
    public static readonly double[,] LmsToRgb = 
    {
        {4.4679, -3.5873, 0.1193},
        {-1.2186, 2.3809, -0.1624},
        {0.0497, -0.2439, 1.2045}
    };  
    
    /// <summary>
    /// Диагональная матрица для перехода из lms в lab
    /// </summary>
    public static readonly double[,] LmsToLab1 = {
        {0.5774, 0.0, 0.0},
        {0.0, 0.4082, 0.0},
        {0.0, 0.0, 0.7071}
    };

    /// <summary>
    /// Матрица для перехода из lms в lab №2
    /// </summary>
    public static readonly double[,] LmsToLab2 = {
        {1.0000, 1.0000, 1.0000},
        {1.0000, 1.0000, -2.0000},
        {1.0000, -1.0000, 0.0000}   
    };
    
    /// <summary>
    /// Диагональная матрица для перехода из lab в lms
    /// </summary>
    public static readonly double[,] LabToLms1 = { 
        {0.5774, 0.0, 0.0},
        {0.0, 0.4082, 0.0},
        {0.0, 0.0, 0.7071}
    };
        
    /// <summary>
    /// Матрица для перехода из lab в lms №2
    /// </summary>
    public static readonly double[,] LabToLms2 = {
        {1.0000, 1.0000, 1.0000},
        {1.0000, 1.0000, -1.0000},
        {1.0000, -2.0000, 0.0000}
    };
}