namespace SharpMath.FiniteElement.Materials.LambdaGamma;

public struct Material
{
    public double Gamma { get; set; }
    public double Lambda { get; set; }

    public Material()
    {
       Lambda = 1;
       Gamma = 1;
    }

    public Material(double lambda, double gamma)
    {
        Lambda = lambda;
        Gamma = gamma;
    }
};