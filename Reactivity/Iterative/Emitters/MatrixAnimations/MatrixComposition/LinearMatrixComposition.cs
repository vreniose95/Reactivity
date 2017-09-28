using System;
using System.Collections.Generic;
using Core.Extensions;

namespace Reactivity.Iterative.Emitters.MatrixAnimations.MatrixComposition
{
	public class LinearMatrixComposition 
		: MatrixCompositionBase
	{
		private int length = 4;
		public int Length
		{
			get => length;
			set
			{
				if (length < 1)
					throw new ArgumentOutOfRangeException(nameof(Length));

				length = value;
			}
		}


		public override IEnumerable<IEnumerable<double>> ComposeMatrix(
			IEnumerable<double> source)
		{
			return source.Chunk(Length);
		}

		public override MatrixCoordinate GetMatrixCoordinate(
			int index, 
			int size)
		{

			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (size < 1)
				throw new ArgumentOutOfRangeException(nameof(size));

			if (index > size - 1)
				throw new IndexOutOfRangeException();

			var row = (int)Math.Floor((double)index / Length);
			var column = index - row * length;
			var rowCount = (int)Math.Floor((double)size / Length);

			return new MatrixCoordinate(row, column, rowCount, Length);
		}
	}
}
