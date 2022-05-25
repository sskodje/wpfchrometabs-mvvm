using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChromeTabs
{
    public class TabShape : Shape
    {
        public TabShape()
        {
            Stretch = Stretch.Fill;
        }

        public Path TabShapePath
        {
            get { return (Path)GetValue(TabShapePathProperty); }
            set { SetValue(TabShapePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabShapePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabShapePathProperty =
            DependencyProperty.Register("TabShapePath", typeof(Path), typeof(TabShape), new PropertyMetadata(null));



        protected override Size MeasureOverride(Size constraint)
        {
            if (constraint.Width == double.PositiveInfinity || constraint.Height == double.PositiveInfinity)
            {
                return Size.Empty;
            }
            // we will size ourselves to fit the available space
            return constraint;
        }
        protected override Geometry DefiningGeometry => GetGeometry();

        private Geometry GetGeometry()
        {
            if(TabShapePath!= null)
            {
                return TabShapePath.Data;
            }
            double width = DesiredSize.Width - StrokeThickness;

            double height = 25;
            double x1 = width - 15;
            double x2 = width - 10;
            double x3 = width - 5;
            double x4 = width - 2.5;
            double x5 = width;

            return Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M0,{5} C2.5,{5} 5,0 10,0 15,0 {0},0 {1},0 {2},0 {3},{5} {4},{5}", x1, x2, x3, x4, x5, height));
        }
    }
}
