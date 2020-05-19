using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Power_App.Helpers
{    
    internal static class AnimationHelper
    {
        internal static void ShowDoubleAnimation(string storyboardName, double animationTo, FrameworkElement animatedElement, Dispatcher dispatcher)
        {
            Task.Run(() => dispatcher.Invoke(() =>
            {
                Storyboard storyboard = Application.Current.TryFindResource(storyboardName) as Storyboard;
                DoubleAnimation animation = storyboard.Children.First() as DoubleAnimation;
                animation.To = animationTo;
                storyboard.Begin(animatedElement);
            }));            
        }
    }
}
