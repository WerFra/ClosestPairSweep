using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ClosestPairSweep.WPF {

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
            prv_Button.Click += ButtonHandler;

            void ButtonHandler(object aO, RoutedEventArgs aE) {
                prv_Button.Click -= ButtonHandler;
                aContinuation();
            };
        }
    }
}
