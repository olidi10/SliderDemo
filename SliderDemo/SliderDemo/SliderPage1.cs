using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace SliderDemo
{
    public class SliderPage1 : ContentPage
    {
        // Properties
        private const int SIZE = 4;

        private AbsoluteLayout _absoluteLayout;
        private Dictionary<GridPosition, GridItem> _gridItems;

        // Constructor
        public SliderPage1()
        {
            //Defining parameters

            _gridItems = new Dictionary<GridPosition, GridItem>();
            _absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Color.Yellow,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center


            };

            var counter = 1;
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    GridItem item;
                    if (counter == 16)
                    {
                        item = new GridItem(new GridPosition(row, col), "EmptySpot");
                        // set property to hold onto value to show when puzzle is solved.
                        item.FinalLabel = "16";
                    }
                    else
                    {
                        item = new GridItem(new GridPosition(row, col), counter.ToString());
                    }
                    var tapRecognizer = new TapGestureRecognizer();
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer);


                    _gridItems.Add(item.CurrentPosition, item);
                    _absoluteLayout.Children.Add(item);


                    counter++;
                }
            }
            Shuffle();

            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout



            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
        }


        private void OnContentViewSizeChanged(Object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / SIZE;


            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    Rectangle rect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);
                    AbsoluteLayout.SetLayoutBounds(item, rect);

                }
            }
        }
        private void OnLabelTapped(Object sender, EventArgs args)
        {
            GridItem item = (GridItem)sender;


            //Did we click on empty? if so do nothing
            if (item.isEmptySpot() == true)
            {
                return;
            }

            // We know we didn't click on the empty spot

            // Checked up down, left, right untill we find empty
            var counter = 0;
            while (counter < 4)
            {
                GridPosition pos = null;
                if (counter == 0 && item.CurrentPosition.Row != 0)
                {
                    // Get position of square above current item
                    pos = new GridPosition(item.CurrentPosition.Row - 1, item.CurrentPosition.Column);
                }
                else if (counter == 1 && item.CurrentPosition.Column != SIZE - 1)
                {
                    //Get position of square to the right of current item
                    pos = new GridPosition(item.CurrentPosition.Row, item.CurrentPosition.Column + 1);
                }
                else if (counter == 2 && item.CurrentPosition.Row != SIZE - 1)
                {
                    //Get position of square below current item
                    pos = new GridPosition(item.CurrentPosition.Row + 1, item.CurrentPosition.Column);
                }
                else if (counter == 3 && item.CurrentPosition.Column != 0)
                {
                    //Get position of square below current item
                    pos = new GridPosition(item.CurrentPosition.Row, item.CurrentPosition.Column - 1);
                }

                if (pos != null)// dont have item to check because of edge
                {



                    GridItem swapWith = _gridItems[pos];
                    if (swapWith.isEmptySpot())
                    {
                        Swap(item, swapWith);
                        break; // if we  found the empty spot, break the loop, no need to check further

                    }
                   
                }
                counter = counter + 1;
            } OnContentViewSizeChanged(this.Content, null);

        }
        void Swap(GridItem item1, GridItem item2)
        {
            // First Swap positions
            GridPosition temp = item1.CurrentPosition;
            item1.CurrentPosition = item2.CurrentPosition;
            item2.CurrentPosition = temp;


            // Then update Dictionary too!
            _gridItems[item1.CurrentPosition] = item1;
            _gridItems[item2.CurrentPosition] = item2;
        }
        void Shuffle()
        {
            Random rand = new Random();
            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    int swapRow = rand.Next(0, 4);
                    int swapcol = rand.Next(0, 4);
                    GridItem swapItem = _gridItems[new GridPosition(swapRow, swapcol)];
                    Swap(item, swapItem);
                }
            }
        }
    }

    internal class GridItem : Image
    {
        public GridPosition CurrentPosition
        {
            get;
            set;

        }
        private GridPosition _finalPosition;
        private Boolean _isEmptySpot;

        public string FinalLabel
        {
            get; set;
        }

        public GridItem(GridPosition position, string text)
        {
            _finalPosition = position;
            CurrentPosition = position;
            

            if (text.Equals("EmptySpot"))
            {
                Source = ImageSource.FromResource("SliderDemo.EmptySpot.jpeg");
                _isEmptySpot = true;
            }
            else
            {
                Source = ImageSource.FromResource("SliderDemo.Image" + text + ".jpeg");
                _isEmptySpot = false;
            }
            //TextColor = Color.White;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
        }
            public Boolean isEmptySpot()
            {
           return _isEmptySpot;
            }
    public void showFinalLabel()
    {
        if (isEmptySpot())
        {
            Source  = ImageSource.FromResource("SliderDemo.Image16.jpeg"); 
        }
    }
    public Boolean isPositionCorrect()
    {
        return _finalPosition.Equals(CurrentPosition);
    }
}
        internal class GridPosition
        {
            public int Row
            {
                get; set;
            }
            public int Column
            {
                get; set;
            }
            public GridPosition(int row, int col)
            {
                Row = row;
                Column = col;
            }
            public override bool Equals(object obj)
            {
                GridPosition other = obj as GridPosition;
                if (other != null && this.Row == other.Row && this.Column == other.Column)
                {
                    return true;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return 17 * 23 + this.Row.GetHashCode() * 23 + this.Column.GetHashCode();
            }

        }
    
}

