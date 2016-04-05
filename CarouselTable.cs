using System;
using Xamarin.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

namespace CarouseTables
{
    public class CarouselTable:ContentView
    {
        
        public PageGrid pageGrid{ get; set; }

        public StackLayout SelectorStack { get; set; }

        public bool IsBusy = false;

        public double _width = 0;
        public double _height = 0;
        public int StepValue = 0;

        public ActivityIndicator AI{ get; set; }

        public StackLayout TableStack{ get; set; }

        public ScrollView VerticalScroll{ get; set; }

        public List<CircleLabel>CircleLabels{ get; set; }

        public class CircleLabel:Label
        {
           
            public int Index = 0;

            public CircleLabel(int index)
            {
                Index = index;
                Text = @"●"; 
                VerticalTextAlignment = Device.OnPlatform(TextAlignment.Start, TextAlignment.Start, TextAlignment.Start);
                HorizontalTextAlignment = TextAlignment.Center;
                TextColor = (index == 0) ? Color.FromHex("0076FF") : Color.Gray;
                FontSize = Device.OnPlatform(20, 15, 20);
                VerticalOptions = LayoutOptions.FillAndExpand;
                HorizontalOptions = LayoutOptions.FillAndExpand;
            }
        }

        public CarouselTable(TableArray tableArray)
        {
            this.Padding = 10;
          
            TableStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Spacing = 0,
                BackgroundColor = Color.Gray.WithLuminosity(.9),
                Orientation = StackOrientation.Vertical,
            };
            var headerStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Horizontal,
                Padding = 1,
                Spacing = 4,
            };
            AI = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,

            };
            headerStack.Children.Add(AI);
            var headerLabel = new Label
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = tableArray.TableTitle,
                LineBreakMode = LineBreakMode.TailTruncation,
                FontSize = 14,
                HeightRequest = 20,
                TextColor = Color.Black.WithLuminosity(.25),
                FontAttributes = FontAttributes.Bold,
                   
            };
            headerStack.Children.Add(headerLabel);
            TableStack.Children.Add(headerStack);
            VerticalScroll = new ScrollView
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Orientation = ScrollOrientation.Vertical,
            };
            TableStack.Children.Add(VerticalScroll);
            var tableBorder = new StackLayout
            {
                BackgroundColor = Color.Gray.WithLuminosity(.6),
                Spacing = 0,
                Padding = .5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
             
            };
            tableBorder.Children.Add(TableStack);
            this.Content = tableBorder;
            pageGrid = new PageGrid(tableArray, this);

            pageGrid.tableScroll.Scrolled += ((object sender, ScrolledEventArgs e) =>
            {
                if (IsBusy)
                    return;
                if (Math.Abs(e.ScrollX) < .001)
                {
                    SelectCircle(CircleLabels[0]);   
                    return;
                }
              
                for (int i = 0; i < tableArray.TableData.GetUpperBound(0); i++)
                {
                    if (i % 2 == 0)
                    {
                        var l = pageGrid.tableGrid.Children[i].Bounds.Left;
                        var w = pageGrid.tableGrid.Children[i].Bounds.Right;
                        if (e.ScrollX >= l && e.ScrollX <= w)
                        {
                            SelectCircle(CircleLabels[i]);   
                            return;
                        }
                    }
                     
                }

            });
          
            SizeChanged += ((object sender, EventArgs e) =>
            {
                if ((sender as ContentView).Height > 0 && Math.Abs((sender as ContentView).Height - _height) > .001)
                {
                    this._width = (sender as ContentView).Width;
                    this._height = (sender as ContentView).Height; 
                    if (SelectorStack == null)
                    {
                        DrawSelector(tableArray);
                        
                        IsBusy = true;
                    }

                }
            });
        }

        public void SelectCircle(CircleLabel target = null)
        {
            foreach (CircleLabel clbl in CircleLabels)
            {
                if (clbl != target)
                {
                    clbl.TextColor = Color.Gray;
                }
                 
               
            }
            if (target != null)
            {
                target.TextColor = Color.FromHex("0076FF");
            }
        }

        public void DrawSelector(TableArray tableArray)
        {
            
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {

                    Device.BeginInvokeOnMainThread(() =>
                        {
                            
                            SelectorStack = new StackLayout
                            {
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                HeightRequest = 20,
                                Padding = 0,
                                Spacing = 4,
                                Orientation = StackOrientation.Horizontal,
                            }; 
                            if (TableStack.Children.Count == 2)
                            {
                                CircleLabels = new List<CircleLabel>();
                                for (int r = 0; r < tableArray.TableData.GetUpperBound(0); r++)
                                {
                                   
                                    var circleLabel = new CircleLabel(r);
                                    if (r % 2 == 0)
                                    {
                                        circleLabel.IsVisible = true;
                                    }
                                    else
                                    {
                                        circleLabel.IsVisible = false;
                                    }
                                    var tapGestureRecognizer = new TapGestureRecognizer();
                                    tapGestureRecognizer.Tapped += async(CircleLabel, e) =>
                                    {
                                        IsBusy = true;
                                        var cell = pageGrid.tableGrid.Children[(CircleLabel as CircleLabel).Index];
                                        
                                        SelectCircle((CircleLabel as CircleLabel));
                                        await pageGrid.tableScroll.ScrollToAsync(cell.Bounds.Left, 0, true);
                                        IsBusy = false; 
                                    };
                                    circleLabel.GestureRecognizers.Add(tapGestureRecognizer); 
                                    SelectorStack.Children.Add(circleLabel);
                                    CircleLabels.Add(circleLabel);
                                   
                                }
                            }
                           
                                
                      
                            if (TableStack.Children.Count == 2)
                            {
                                var selectorScroll = new ScrollView
                                {
                                    Orientation = ScrollOrientation.Horizontal,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.StartAndExpand,
                                    Padding = 0,
                                    Content = SelectorStack,
                                };
                                TableStack.Children.Add(selectorScroll);
                                AI.IsVisible = false;
                                AI.IsRunning = false;
                                VerticalScroll.Content = pageGrid;
                                Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
                                    {
                                        if (pageGrid.tableScroll.ContentSize.Width > pageGrid.tableScroll.Width)
                                        {
                                            SelectorStack.IsVisible = true;

                                        }
                                        else
                                        {
                                            SelectorStack.IsVisible = false;
                                        }
                                        return false;
                                    });

                                IsBusy = false;
                            }
                             
                           
                        });
                    return false;

                });
        }

        public class PageGrid:Grid
        {
            public ScrollView tableScroll{ get; set; }

            public Grid tableGrid{ get; set; }

            public Grid legendGrid{ get; set; }

            public double FontSize = 12;

            public PageGrid(TableArray tableArray, CarouselTable outerClass)
            {
                int rows = tableArray.TableData.GetUpperBound(0);
                int columns = tableArray.TableData.GetUpperBound(1);
                int rowHeight = 20;
                HorizontalOptions = LayoutOptions.FillAndExpand;
                VerticalOptions = LayoutOptions.StartAndExpand;
                //HeightRequest = rowHeight * 21;
                ColumnSpacing = .5;
                RowSpacing = .5;
                Padding = .5;
                BackgroundColor = Color.Gray.WithLuminosity(.9);
                ColumnDefinitions.Add(new ColumnDefinition{ Width = GridLength.Auto });
                ColumnDefinitions.Add(new ColumnDefinition());
             
                RowDefinitions.Add(new RowDefinition());
                legendGrid = new Grid
                {
                    ColumnSpacing = .5,
                    RowSpacing = .5,
                    Padding = .5,
                    BackgroundColor = Color.Gray.WithLuminosity(.9),
                    HeightRequest = this.HeightRequest,

                };
                legendGrid.ColumnDefinitions.Add(new ColumnDefinition{ Width = GridLength.Auto });
                for (int i = 0; i < columns + 1; i++)
                {
                    legendGrid.RowDefinitions.Add(new RowDefinition{ Height = rowHeight });
                    var legendStack = new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Padding = 2.5,
                        BackgroundColor = Color.Gray.WithLuminosity(.98),
                    };
                    var legendLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.End,
                        FontSize = FontSize,
                        Text = tableArray.TableData[0, i],
                        TextColor = Color.Black.WithLuminosity(.25),
                        FontAttributes = FontAttributes.Bold,
                    };
                    legendStack.Children.Add(legendLabel);
                    legendGrid.Children.Add(legendStack, 0, i);
                    this.Children.Add(legendGrid, 0, 0);
                  
                }
#region scrollingGrid
                tableGrid = new Grid
                {
                    ColumnSpacing = .5,
                    RowSpacing = .5,
                    Padding = .5,
                    BackgroundColor = Color.Gray.WithLuminosity(.7),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,

                };
                for (int i = 0; i < columns + 1; i++)
                {
                    tableGrid.RowDefinitions.Add(new RowDefinition{ Height = rowHeight });
                }
                for (int i = 0; i < rows + 1; i++)
                {
                    tableGrid.ColumnDefinitions.Add(new ColumnDefinition{ Width = GridLength.Auto });
                }
                tableScroll = new ScrollView
                {
                    Orientation = ScrollOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = 0,
                    BackgroundColor = Color.White,
                    
                };
                

                for (int r = 0; r <= columns; r++)
                {
                    for (int c = 0; c < rows; c++)
                    {
                        var cellStack = new StackLayout
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            Padding = 2.5,
                            BackgroundColor = (c % 2 == 0) ? Color.FromHex("#D1EEFC").WithLuminosity(.98) : Color.White,
                        };
                        var cellLabel = new Label
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Start,
                            FontSize = FontSize,
                            Text = tableArray.TableData[c + 1, r],
                            TextColor = Color.Black.WithLuminosity(.35),

                        };
                        cellStack.Children.Add(cellLabel);
                        tableGrid.Children.Add(cellStack, c, r);
                        Debug.WriteLine(tableArray.TableData[c + 1, r]);
                    }
                   
                }
               
                tableScroll.Content = tableGrid;
               
                this.Children.Add(tableScroll, 1, 0);
               
#endregion
            }
        }

      
        
    }

    public class TableArray
    {
        public string TableTitle{ get; set; }

        public string[,]TableData{ get; set; }
    }

}

