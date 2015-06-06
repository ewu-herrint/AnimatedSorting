using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;

// Tyler Herrin
// CSCD 371
// Final Project: Animated Sorting

/**
 * Extra/For Fun: Included Bogo Sort and a reset button as Bogo sort takes an indeterminate amount of time to sort... :P 
 *                Although, I believe it is technically Bozo Sort as I just swap two random indexes rather than shuffling
 *                the entire collection.
 * 
 * Shortcomings: My insertion sort animation isn't as pretty as the others because of all the extra "moves" required
 *               would of been painful to program in the same fashion as I did for the other sorts that just need swapping.
 */

namespace AnimatedSorting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Canvas[] collection;

        public MainWindow()
        {
            InitializeComponent();
            SortButton.IsEnabled = false;
            HideArray();
            collection = new Canvas[] {Array1, Array2, Array3, Array4, Array5, Array6};
        }

        private void DataButton_Click(object sender, RoutedEventArgs e)
        {
            if(RandomRadio.IsChecked.Value)
            {
                SortButton.IsEnabled = true;
                SetRandom();
                ShowArray();
            }
            else if(AscendingRadio.IsChecked.Value)
            {
                SortButton.IsEnabled = true;
                SetAscending();
                ShowArray();
            }
            else if(DescendingRadio.IsChecked.Value)
            {
                SortButton.IsEnabled = true;
                SetDescending();
                ShowArray();
            }
            else
            {
                MessageBox.Show("Select a data generation setting!");
            }
        }

        private void SetRandom()
        {
            Random rand = new Random();
            foreach (Canvas c in collection)
            {
                int x = rand.Next(1, 20);
                Label l = (Label)c.Children[0];
                l.Content = x;
            }
        }

        private void SetAscending()
        {
            for (int x = 0; x < 6; x++)
            {
                Label l = (Label)collection[x].Children[0];
                l.Content = x + 1;
            }
        }

        private void SetDescending()
        {
            for (int x = 5; x > -1; x--)
            {
                Label l = (Label)collection[5 - x].Children[0];
                l.Content = x + 1;
            }
        }

        private void HideArray()
        {
            Array1.Visibility = System.Windows.Visibility.Hidden;
            Array2.Visibility = System.Windows.Visibility.Hidden;
            Array3.Visibility = System.Windows.Visibility.Hidden;
            Array4.Visibility = System.Windows.Visibility.Hidden;
            Array5.Visibility = System.Windows.Visibility.Hidden;
            Array6.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ShowArray()
        {
            Array1.Visibility = System.Windows.Visibility.Visible;
            Array2.Visibility = System.Windows.Visibility.Visible;
            Array3.Visibility = System.Windows.Visibility.Visible;
            Array4.Visibility = System.Windows.Visibility.Visible;
            Array5.Visibility = System.Windows.Visibility.Visible;
            Array6.Visibility = System.Windows.Visibility.Visible;
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            DataButton.IsEnabled = false;
            SortButton.IsEnabled = false;
            BogoLabel.Visibility = System.Windows.Visibility.Hidden;
            BogoLabel.Content = "Sorted? No. :(";
            if(SelectionRadio.IsChecked.Value)
            {
                SelectionSort();
            }
            else if(InsertionRadio.IsChecked.Value)
            {
                InsertionSort();
            }
            else if(QuickRadio.IsChecked.Value)
            {
                QuickSort();
            }
            else if(BogoRadio.IsChecked.Value)
            {
                BogoSort();
            }
            else
            {
                MessageBox.Show("Select a sort setting!");
            }
        }

        private async void BogoSort()
        {
            Random rand = new Random();
            while(!IsArraySorted())
            {
                await Swap(rand.Next(0, 6), rand.Next(0, 6));
                BogoLabel.Visibility = System.Windows.Visibility.Visible;
            }
            DataButton.IsEnabled = true;
            SortButton.IsEnabled = true;
        }

        private Boolean IsArraySorted()
        {
            for (int i = 0, j = 1; j < 6; i++, j++)
            {
                if((int)((Label)collection[i].Children[0]).Content > (int)((Label)collection[j].Children[0]).Content)
                    return false;
            }
            BogoLabel.Visibility = System.Windows.Visibility.Visible;
            BogoLabel.Content = "Sorted? YES :D";
            return true;
        }

        private async void InsertionSort()
        {
            for(int i = 1; i < 6; i++)
            {
                Canvas x = collection[i];

                Thickness xThick = collection[i].Margin;
                Thickness jThick = xThick;
                collection[i].Margin = new Thickness(xThick.Left, xThick.Top -65 , xThick.Right, xThick.Bottom + 65);
                await Task.Delay(1000);

                int j = i;
                while(j > 0 && (int)((Label)collection[j-1].Children[0]).Content > (int)((Label)x.Children[0]).Content)
                {
                    jThick = collection[j - 1].Margin;
                    collection[j - 1].Margin = new Thickness(jThick.Left + 55, jThick.Top, jThick.Right - 55, jThick.Bottom);
                    await Task.Delay(750);

                    collection[j] = collection[j - 1];
                    j = j - 1;
                }

                x.Margin = jThick;
                await Task.Delay(1500);

                collection[j] = x;
            }

            DataButton.IsEnabled = true;
            SortButton.IsEnabled = true;
        }

        private async void QuickSort()
        {
            await QuickSort(collection, 0, 5);
            DataButton.IsEnabled = true;
            SortButton.IsEnabled = true;
        }

        private async Task QuickSort(Canvas[] array, int low, int high)
        {
            if(low < high)
            {
                int pivot = await partition(array, low, high);
                await QuickSort(array, low, pivot - 1);
                await QuickSort(array, pivot + 1, high);
            }

            return;
        }

        private async Task<int> partition(Canvas[] array, int low, int high)
        {
            int pivotIndex = (high + low) / 2; 
            int pivotVal = (int)((Label)array[pivotIndex].Children[0]).Content;

            await Swap(pivotIndex, high);
            int storeIndex = low;
            for (int i = low; i < high; i++)
            {
                if((int)((Label)array[i].Children[0]).Content < pivotVal)
                {
                    await Swap(i, storeIndex);
                    storeIndex++;
                }
            }
            await Swap(storeIndex, high);

            return storeIndex;
        }

        private async Task Swap(int i, int j)
        {
            await AnimateSwap(collection[i], collection[j]);

            Canvas temp = collection[i];
            collection[i] = collection[j];
            collection[j] = temp;

            return;
        }

        private async void SelectionSort()
        {
            int i,j;
            int min;
            for (i = 0; i < 6; i++)
            {
                min = i;
                for (j = i + 1; j < 6; j++)
                {
                    int realJ = (int)((Label)collection[j].Children[0]).Content;
                    int realMin = (int)((Label)collection[min].Children[0]).Content;
                    if (realJ < realMin)
                    {
                        min = j;
                    }
                }
                await Swap(i, min);
            }

            DataButton.IsEnabled = true;
            SortButton.IsEnabled = true;
        }

        private async Task AnimateSwap(Canvas c1, Canvas c2)
        {
            Random rand = new Random();

            int dis = (int)(c1.Margin.Left - c2.Margin.Left);

            TransformGroup transformGroup1 = new TransformGroup();
            TransformGroup transformGroup2 = new TransformGroup();

            // TRANSLATE UP
            TranslateTransform translationUp = new TranslateTransform();
            String transNameUp = "translateUp" + translationUp.GetHashCode() + rand.Next();
            RegisterName(transNameUp, translationUp);
            transformGroup1.Children.Add(translationUp);
            transformGroup2.Children.Add(translationUp);

            // TRANSLATE LEFT
            TranslateTransform translationLeft = new TranslateTransform();
            String transNameLeft = "translateLeft" + translationLeft.GetHashCode() + rand.Next();
            RegisterName(transNameLeft, translationLeft);
            transformGroup2.Children.Add(translationLeft);

            // TRANSLATE RIGHT
            TranslateTransform translationRight = new TranslateTransform();
            String transNameRight = "translateRight" + translationRight.GetHashCode() + rand.Next();
            RegisterName(transNameRight, translationRight);
            transformGroup1.Children.Add(translationRight);

            // TRANSLATE DOWN
            TranslateTransform translationDown = new TranslateTransform();
            String transNameDown = "translateDown" + translationDown.GetHashCode() + rand.Next();
            RegisterName(transNameDown, translationDown);
            transformGroup1.Children.Add(translationDown);
            transformGroup2.Children.Add(translationDown);

            c1.RenderTransform = transformGroup1;
            c2.RenderTransform = transformGroup2;

            DoubleAnimation slideUp = new DoubleAnimation { By = -65, Duration = TimeSpan.FromSeconds(1.0) };
            DoubleAnimation slideLeft = new DoubleAnimation { By = dis, Duration = TimeSpan.FromSeconds(1.0) };
            DoubleAnimation slideRight = new DoubleAnimation { By = -dis, Duration = TimeSpan.FromSeconds(1.0) };
            DoubleAnimation slideDown = new DoubleAnimation { By = 65, Duration = TimeSpan.FromSeconds(1.0) };

            // UP
            Storyboard sbSlideUp = new Storyboard();
            Storyboard.SetTargetName(sbSlideUp, transNameUp);
            Storyboard.SetTargetProperty(sbSlideUp, new PropertyPath(TranslateTransform.YProperty));
            String sbName = "sbUp" + sbSlideUp.GetHashCode() + rand.Next();
            Resources.Add(sbName, sbSlideUp);

            // LEFT
            Storyboard sbSlideLeft = new Storyboard();
            Storyboard.SetTargetName(sbSlideLeft, transNameLeft);
            Storyboard.SetTargetProperty(sbSlideLeft, new PropertyPath(TranslateTransform.XProperty));
            sbName = "sbLeft" + sbSlideLeft.GetHashCode() + rand.Next();
            Resources.Add(sbName, sbSlideLeft);

            // RIGHT
            Storyboard sbSlideRight = new Storyboard();
            Storyboard.SetTargetName(sbSlideRight, transNameRight);
            Storyboard.SetTargetProperty(sbSlideRight, new PropertyPath(TranslateTransform.XProperty));
            sbName = "sbRight" + sbSlideRight.GetHashCode() + rand.Next();
            Resources.Add(sbName, sbSlideRight);

            // DOWN
            Storyboard sbSlideDown = new Storyboard();
            Storyboard.SetTargetName(sbSlideDown, transNameDown);
            Storyboard.SetTargetProperty(sbSlideDown, new PropertyPath(TranslateTransform.YProperty));
            sbName = "sbDown" + sbSlideDown.GetHashCode() + rand.Next();
            Resources.Add(sbName, sbSlideDown);

            sbSlideUp.Children.Add(slideUp);
            sbSlideLeft.Children.Add(slideLeft);
            sbSlideRight.Children.Add(slideRight);
            sbSlideDown.Children.Add(slideDown);

            await RunStoryboard(sbSlideUp);
            await RunStoryboard(sbSlideLeft, sbSlideRight);
            await RunStoryboard(sbSlideDown);
            await Task.Delay(1000);

            sbSlideUp.Stop();
            sbSlideDown.Stop();
            sbSlideLeft.Stop();
            sbSlideRight.Stop();

            Thickness temp = c1.Margin;
            c1.Margin = c2.Margin;
            c2.Margin = temp;
            return;
        }

        private async Task RunStoryboard(Storyboard sb1, Storyboard sb2)
        {   
            sb1.Begin();
            sb2.Begin();
            await Task.Delay(1000);
            return;
        }

        private async Task RunStoryboard(Storyboard sb1)
        {      
            sb1.Begin();
            await Task.Delay(1000);
            return;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
