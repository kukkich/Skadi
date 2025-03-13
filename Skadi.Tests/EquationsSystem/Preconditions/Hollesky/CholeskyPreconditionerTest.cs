using Skadi.EquationsSystem.Preconditions.Hollesky;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.Tests.EquationsSystem.Preconditions.Hollesky;

[TestFixture]
[TestOf(typeof(CholeskyPreconditioner))]
public class CholeskyPreconditionerTestTests
{
    [Test]
    public void Sparse()
    {
        var decomposedMatrix = new SymmetricRowSparseMatrix
        (
            [0, 0, 0, 1, 3, 5, 6, 10],
            [0, 0, 1, 1, 3, 0, 0, 3, 4, 5],
            [-1, -2, 1, 4, 5, 1, -1, -2, -3, 3],
            [5, 3, 4, 2, 1, 4, 7]
        );
        
        var preconditioner = new CholeskyPreconditioner(decomposedMatrix);
        // y = L * (L^T * x). We should prove that (L * L^T)^-1 = preconditioner * y = x
        var xExpected = Vector.Create(7, i => i + 1);
        var y = new Vector(-35, 90, 55, 82, 199, 173, 495);
        
        var xActual = preconditioner.MultiplyOn(y);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < xExpected.Length; i++)
            {
                Assert.That(xActual[i], Is.EqualTo(xExpected[i]).Within(1e-15));
            }
        });
    }

    [Test]
    public void Dense()
    {
        var decomposedMatrix = new SymmetricRowSparseMatrix
        (
            [0, 0, 1, 3, 6],
            [0, 0, 1, 0, 1, 2],
            [1, -1, -4, -2, 1, 3],
            [50, 30, 40, 20]
        );
        // y = L * (L^T * x). We should prove that (L * L^T)^-1 = preconditioner * y = x
        var preconditioner = new CholeskyPreconditioner(decomposedMatrix);
        
        var xExpected = Vector.Create(4, i => i + 1);
        var y = new Vector(2050, 1601, 5031, 1966);
        
        var xActual = preconditioner.MultiplyOn(y);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < xExpected.Length; i++)
            {
                Assert.That(xActual[i], Is.EqualTo(xExpected[i]).Within(1e-15));
            }
        });
    }
    
    [Test]
    public void ResultMemoryShouldBeFilled()
    {
        var decomposedMatrix = new SymmetricRowSparseMatrix
        (
            [0, 0, 0, 1, 3, 5, 6, 10],
            [0, 0, 1, 1, 3, 0, 0, 3, 4, 5],
            [-1, -2, 1, 4, 5, 1, -1, -2, -3, 3],
            [5, 3, 4, 2, 1, 4, 7]
        );
        
        var preconditioner = new CholeskyPreconditioner(decomposedMatrix);
        // y = L * (L^T * x). We should prove that (L * L^T)^-1 = preconditioner * y = x
        var xExpected = Vector.Create(7, i => i + 1);
        var y = new Vector(-35, 90, 55, 82, 199, 173, 495);
        
        var xActual = Vector.Create(7);
        preconditioner.MultiplyOn(y, xActual);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < xExpected.Length; i++)
            {
                Assert.That(xActual[i], Is.EqualTo(xExpected[i]).Within(1e-15));
            }
        });
    }
    
    [Test]
    public void ResultMemoryShouldBeFilledEvenIfFilledWithNonZeroes()
    {
        var decomposedMatrix = new SymmetricRowSparseMatrix
        (
            [0, 0, 0, 1, 3, 5, 6, 10],
            [0, 0, 1, 1, 3, 0, 0, 3, 4, 5],
            [-1, -2, 1, 4, 5, 1, -1, -2, -3, 3],
            [5, 3, 4, 2, 1, 4, 7]
        );
        
        var preconditioner = new CholeskyPreconditioner(decomposedMatrix);
        // y = L * (L^T * x). We should prove that (L * L^T)^-1 = preconditioner * y = x
        var xExpected = Vector.Create(7, i => i + 1);
        var y = new Vector(-35, 90, 55, 82, 199, 173, 495);


        var xActual = Vector.Create(7, RandomFactory);
        preconditioner.MultiplyOn(y, xActual);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < xExpected.Length; i++)
            {
                Assert.That(xActual[i], Is.EqualTo(xExpected[i]).Within(1e-15));
            }
        });
        return;

        double RandomFactory(int i) => i * i * i % 15;
    }
}