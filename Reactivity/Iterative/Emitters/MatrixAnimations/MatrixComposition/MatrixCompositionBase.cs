using System.Collections.Generic;

namespace Reactivity.Iterative.Emitters.MatrixAnimations.MatrixComposition
{
	public abstract class MatrixCompositionBase
	{
		public abstract IEnumerable<IEnumerable<double>> ComposeMatrix(IEnumerable<double> source);

		public abstract MatrixCoordinate GetMatrixCoordinate(int index, int size);
	}
}
