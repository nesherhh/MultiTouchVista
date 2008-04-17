using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Danilins.Multitouch.Controls
{
	public class MediaControl : Control
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(MediaControl),
			new PropertyMetadata(null, SourceChanged));

		private MediaElement mediaElement;
		private Slider timelineSlider;
		private MediaTimeline timeLine;
		private bool currentTimeInvalidated;

		static MediaControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaControl), new FrameworkPropertyMetadata(typeof(MediaControl)));
		}

		private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MediaControl)d).SourceChanged(e);
		}

		public MediaControl()
		{
			CommandBindings.Add(new CommandBinding(MediaCommands.TogglePlayPause, TogglePlayPause));
			CommandBindings.Add(new CommandBinding(MediaCommands.Stop, Stop));
		}

		public Uri Source
		{
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public override void OnApplyTemplate()
		{
			mediaElement = (MediaElement)GetTemplateChild("mediaElement");
			timelineSlider = (Slider)GetTemplateChild("timelineSlider");

			mediaElement.Clock = timeLine.CreateClock();
			mediaElement.Clock.Controller.Pause();

			mediaElement.MediaOpened += mediaElement_MediaOpened;
			mediaElement.MediaEnded += mediaElement_MediaEnded;
			timelineSlider.ValueChanged += timelineSlider_ValueChanged;
			base.OnApplyTemplate();
		}

		private void timelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!currentTimeInvalidated)
			{
				TimeSpan position = TimeSpan.FromMilliseconds(timelineSlider.Value);
				mediaElement.Clock.Controller.Seek(position, TimeSeekOrigin.BeginTime);
			}
		}

		private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
		{
			mediaElement.Clock.Controller.Stop();
		}

		private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
		{
			if (mediaElement.NaturalDuration.HasTimeSpan)
				timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
		}

		private void SourceChanged(DependencyPropertyChangedEventArgs e)
		{
			if (timeLine != null)
				timeLine.Source = Source;
			else
			{
				timeLine = new MediaTimeline(Source);
				timeLine.CurrentTimeInvalidated += timeLine_CurrentTimeInvalidated;
			}
		}

		private void timeLine_CurrentTimeInvalidated(object sender, EventArgs e)
		{
			currentTimeInvalidated = true;
			timelineSlider.Value = mediaElement.Position.TotalMilliseconds;
			currentTimeInvalidated = false;
		}

		private void Stop(object sender, ExecutedRoutedEventArgs e)
		{
			mediaElement.Clock.Controller.Stop();
		}

		private void TogglePlayPause(object sender, ExecutedRoutedEventArgs e)
		{
			if (mediaElement.Clock.IsPaused)
				mediaElement.Clock.Controller.Resume();
			else if (mediaElement.Clock.CurrentState == ClockState.Stopped)
				mediaElement.Clock.Controller.Begin();
			else
				mediaElement.Clock.Controller.Pause();
		}
	}
}