using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Closest_Pair_Sweep {

    public static class AwaitButtonExtensions {
        public static AwaitButton GetAwaiter(this Button aButton) => new AwaitButton(aButton);
    }

    public class AwaitButton : INotifyCompletion {

        private Button prv_Button;

        public AwaitButton(Button aButton) {
            prv_Button = aButton;
        }

        public bool IsCompleted { get => false; }

        public void GetResult() { }

        public void OnCompleted(Action aContinuation) {
            RoutedEventHandler h = null;
            h = (o, e) => {
                prv_Button.Click -= h;
                aContinuation();
            };
            prv_Button.Click += h;
        }
    }
}
