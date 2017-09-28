using System.Windows;
using System.Windows.Media.Animation;
using Ccr.PresentationCore.Helpers.DependencyHelpers;

namespace Reactivity.Iterative.Emitters.AnimationMap
{
	//TODO use a DoubleAnimationUsingKeyFrames or similar
	public class LinearAnimationMap : AnimationMapBase
	{
		DoubleAnimationUsingKeyFrames
		public static readonly DependencyProperty StartPointProperty = DP.Register(
			new Meta<LinearAnimationMap, Point>(new Point(0.0, 0.0), onStartPointChanged));

		public static readonly DependencyProperty EndPointProperty = DP.Register(
			new Meta<LinearAnimationMap, Point>(new Point(1.0, 1.0), onEndPointChanged));

		public static readonly DependencyProperty MatrixGroupCountProperty = DP.Register(
			new Meta<LinearAnimationMap, int>(4));
		

		public Point StartPoint
		{
			get { return (Point)GetValue(StartPointProperty); }
			set { SetValue(StartPointProperty, value); }
		}
		public Point EndPoint
		{
			get { return (Point)GetValue(EndPointProperty); }
			set { SetValue(EndPointProperty, value); }
		}
		public int MatrixGroupCount
		{
			get { return (int) GetValue(MatrixGroupCountProperty); }
			set { SetValue(MatrixGroupCountProperty, value); }
		}
		
		private static void onStartPointChanged(LinearAnimationMap i, DPChangedEventArgs<Point> e)
		{
		}
		private static void onEndPointChanged(LinearAnimationMap i, DPChangedEventArgs<Point> e)
		{
		}

	}
}
