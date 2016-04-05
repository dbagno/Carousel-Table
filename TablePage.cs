using System;

using Xamarin.Forms;
using System.Globalization;

namespace CarouseTables
{
    public class TablePage : ContentPage
    {
        public TablePage()
        {
            this.Title = "Carousel Table Demo";

            //created by davidbagno@dbagno@mac.com
            var pageStack = new StackLayout
            {
                Padding = 0,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            var pageScroll = new ScrollView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Padding = 0,
                Content = pageStack,
            };
            var d1 = new TableArray
            {
                TableTitle = DateTime.Now.Year.ToString() + " Year In Review",

                TableData = new string[31, 12]
            };
            for (int r = 0; r < 31; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    if (r == 0)
                    {
                        d1.TableData[r, c] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(c + 1);
                    }
                    else
                    {
                        d1.TableData[r, c] = String.Format("Total: {0:C}", App.GetRandomNumber(100, 100000));
                    }
                }
            }
            var d2 = new TableArray
            {
                TableTitle = DateTime.Now.AddYears(-1).Year.ToString() + " Year In Review",

                TableData = new string[31, 12]
            };
            for (int r = 0; r < 31; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    if (r == 0)
                    {
                        d2.TableData[r, c] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(c + 1);
                    }
                    else
                    {
                        d2.TableData[r, c] = String.Format("Total: {0:C}", App.GetRandomNumber(100, 100000));
                    }
                }
            }
            pageStack.Children.Add(new CarouselTable(d1));
            pageStack.Children.Add(new CarouselTable(d2));
            this.Content = pageScroll;
                
        }
    }
}


