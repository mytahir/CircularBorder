using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;

namespace Testing
{
    public partial class ScrollableControl : ContentView
    {
        private double centerX, centerY;
        private double radius = 300; // Reduced radius for positioning
        private List<Border> borders = new List<Border>();
        private int currentBorderIndex = 0; // Keeps track of the next border's position
        private double totalRotation = 0; // Track total rotation angle for borders

        public ScrollableControl()
        {
            InitializeComponent();
            this.SizeChanged += OnSizeChanged;

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            this.GestureRecognizers.Add(panGesture);
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (absoluteLayout.Width > 0 && absoluteLayout.Height > 0)
            {
                centerX = absoluteLayout.Width / 2;
                centerY = absoluteLayout.Height / 2;
            }
        }

        private void OnCenterButtonClicked(object sender, EventArgs e)
        {
            if (currentBorderIndex < 10)
            {
                AddBorderAtCurrentPosition();
                currentBorderIndex++;
            }
        }

        private void AddBorderAtCurrentPosition()
        {
            double borderSize = (currentBorderIndex % 2 == 0) ? 120 : 150; // Even index = smaller, Odd index = larger

            var border = new Border
            {
                BackgroundColor = Colors.Black,
                StrokeShape = new RoundRectangle { CornerRadius = 75 }, // Increased corner radius
                WidthRequest = borderSize, // Adjust width based on pattern
                HeightRequest = borderSize, // Adjust height based on pattern
                Stroke = Colors.Transparent,
                StrokeThickness = 4,
                Content = new Label
                {
                    Text = (currentBorderIndex + 1).ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Colors.White
                }
            };

            borders.Add(border);
            absoluteLayout.Children.Add(border);

            UpdateBorderPosition(border, currentBorderIndex, 10);
        }

        private void UpdateBorderPosition(Border border, int index, int totalBorders)
        {
            double angleStep = 360.0 / totalBorders;
            double angle = 252 - (angleStep * index) + totalRotation; // Apply total rotation
            double radian = Math.PI * angle / 180.0;

            double x = centerX + radius * Math.Cos(radian) - border.WidthRequest / 2;
            double y = centerY + radius * Math.Sin(radian) - border.HeightRequest / 2;

            AbsoluteLayout.SetLayoutBounds(border, new Rect(x, y, border.WidthRequest, border.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(border, AbsoluteLayoutFlags.None);
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            // Only rotate when there are six or more borders
            if (borders.Count >= 6)
            {
                if (e.StatusType == GestureStatus.Running)
                {
                    // Calculate rotation based on horizontal panning distance
                    double rotationDelta = e.TotalX * 0.1; // Adjust sensitivity as needed
                    totalRotation -= rotationDelta;

                    // Update positions of each border according to the new rotation
                    for (int i = 0; i < borders.Count; i++)
                    {
                        UpdateBorderPosition(borders[i], i, borders.Count);
                    }
                }
            }
        }
    }
}
