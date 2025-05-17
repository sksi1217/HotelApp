using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace HotelApp.Helpers.Animations
{
    class Animations
    {
        #region Все анимацию которые есть
        public static void AnimateWindowAppear(UIElement window)
        {
            // Создаем TranslateTransform для анимации
            TranslateTransform transform = new TranslateTransform();
            window.RenderTransform = transform;

            // Анимация прозрачности
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Анимация перемещения
            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                From = 50, // Небольшое смещение вниз
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Применяем анимации
            window.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
        }
        public static void AnimateWindowClose(UIElement window, Panel container, Action onAnimationCompleted = null)
        {
            if (window == null || container == null)
            {
                throw new ArgumentNullException("Window or container cannot be null.");
            }

            // Инициализация RenderTransform
            TranslateTransform transform = window.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                window.RenderTransform = transform;
                window.RenderTransformOrigin = new Point(0.5, 0.5); // Центральная точка трансформации
            }

            // Анимация прозрачности
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = window.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            // Анимация перемещения
            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                From = transform.Y,
                To = 50, // Небольшое смещение вниз
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            // После завершения анимации удаляем окно
            opacityAnimation.Completed += (s, e) =>
            {
                container.Children.Remove(window);
                onAnimationCompleted?.Invoke(); // Вызываем обработчик завершения
            };

            // Применяем анимации
            window.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
        }
        #endregion

    }
}
