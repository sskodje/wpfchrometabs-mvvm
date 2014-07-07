using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChromeTabs
{
    public class DropPreviewAdorner : Adorner
    {
        private ContentPresenter _presenter;
        private double _left = 0;
        private double _top = 0;

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                UpdatePosition();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                UpdatePosition();
            }
        }

        public DropPreviewAdorner(UIElement feedbackUI, UIElement adornedElt)
            : base(adornedElt)
        {
            _presenter = new ContentPresenter();
            _presenter.Content = feedbackUI;
            _presenter.IsHitTestVisible = false;
            this.Visibility = Visibility.Collapsed;
        }

        private void UpdatePosition()
        {
            AdornerLayer layer = this.Parent as AdornerLayer;
            if (layer != null)
            {
                layer.Update(AdornedElement);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _presenter.Measure(constraint);
            return _presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _presenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _presenter;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(Left, Top));
            if (this.Left > 0) this.Visibility = Visibility.Visible;
            result.Children.Add(base.GetDesiredTransform(transform));

            return result;
        }
    }
}
