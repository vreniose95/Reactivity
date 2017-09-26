namespace Reactivity.Iterative.Emitters.MatrixAnimations
{
	public class MatrixCoordinate
	{
		public int Row { get; }

		public int Column { get; }

		public int RowCount { get; }

		public int ColumnCount { get; }


		public MatrixCoordinate(int row, int column, int rowCount, int columnCount)
		{
			Row = row;
			Column = column;
			RowCount = rowCount;
			ColumnCount = columnCount;
		}
	}
}
