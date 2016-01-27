using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChromeTabs
{
    public class TabShape : Shape
    {
        public TabShape()
        {
            this.Stretch = Stretch.Fill;
        }



        protected override Size MeasureOverride(Size constraint)
        {
            if (constraint.Width == double.PositiveInfinity || constraint.Height == double.PositiveInfinity)
            {
                return Size.Empty;
            }
            // we will size ourselves to fit the available space
            return constraint;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                return GetGeometry();
            }
        }

        private Geometry GetGeometry()
        {
            double width = this.DesiredSize.Width - StrokeThickness;

            double height = 25;
            double x1 = width - 15;
            double x2 = width - 10;
            double x3 = width - 5;
            double x4 = width - 2.5;
            double x5 = width;

            return Geometry.Parse(String.Format(CultureInfo.InvariantCulture, "M0,{5} C2.5,{5} 5,0 10,0 15,0 {0},0 {1},0 {2},0 {3},{5} {4},{5}", x1, x2, x3, x4, x5, height));
        }
    }
}
